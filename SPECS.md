# SPECS.md — Paige : pipeline de compilation

## Vue d'ensemble

```
mybook.paige
    │
    ▼
[ Lexer ]  →  tokens
    │
    ▼
[ Parser ] →  Epub Syntactic Tree (EST)
    │
    ▼
[ Epub.Write(est) ] →  mybook.epub
```

Le `.paige` est le source. L'**EST** est la représentation intermédiaire structurée (l'équivalent du CIL de .NET ou du bytecode Java) : un graphe d'objets typés qui décrit complètement le livre, indépendamment du format de sortie.

---

## Le format `.paige`

Trois directives pour l'instant :

### `#metadata`
```
#metadata(
    identifier: <int>,
    title: <string>,
    language: fr | en
)
```

### `#manifest.add`
```
#manifest.add(
    id: <string>,
    href: <string>,
    mediaType: <string>,
    properties: <string>,   // optionnel
    source: <string>,       // optionnel — fichier source à copier dans l'EPUB
    nav: <string>,          // optionnel — texte à afficher dans la table des matières
    spine: <bool>           // optionnel — false par défaut
)[
  <!-- contenu XHTML inline, optionnel -->
]
```

Le bloc `[…]` est le corps textuel de l'item (XHTML, Markdown, texte brut, YAML…). S'il est présent, il a priorité sur `source`.

### `#include`
```
#include "<chemin_relatif>"
```
Permet d'inclure le contenu d'un autre fichier `.paige`. Le chemin est résolu relativement au fichier appelant. Les items définis dans le fichier inclus sont ajoutés au manifeste du document parent.

---

## Epub Syntactic Tree (EST)

```csharp
record EpubDocument(
    EpubMetadata Metadata,
    IReadOnlyList<ManifestItem> Manifest
);

record EpubMetadata(
    string Identifier,
    string Title,
    string Language
);

record ManifestItem(
    string Id,
    string Href,
    string MediaType,
    string? Properties,   // ex: "cover-image", "nav"
    string? Source,       // chemin vers un fichier externe à copier
    string? InlineContent,// contenu XHTML brut (bloc [...])
    string? Nav,          // label pour la navigation
    bool InSpine
);
```

Invariants :
- Un `ManifestItem` a soit `Source` soit `InlineContent`, jamais les deux.
- `Source` est un chemin vers un fichier **extérieur** au projet ; `Epub.Write()` le copie tel quel dans `OEBPS/`.
- `InlineContent` est du texte brut (XHTML, Markdown, etc.) ; c'est `Epub.Write()` qui détermine comment le traiter selon le `MediaType` de l'item.

### Navigation (`nav.xhtml`)

La nav est **générée automatiquement** par `Epub.Write()` à partir de la liste ordonnée des items dont `InSpine == true`. Elle n'est pas déclarée dans le `.paige`.

---

## Méthodologie

**TDD strict** : les tests sont écrits avant toute implémentation. Aucune fonction n'est intégrée dans le projet avant d'avoir des tests qui passent. L'ordre systématique pour chaque composant : tests rouges → implémentation → tests verts → intégration.

**CONTRÔLE DES COMMITS (PRIMORDIAL)** : L'agent ne doit **JAMAIS** effectuer de `git commit` de sa propre initiative. Chaque commit doit faire l'objet d'une demande explicite et distincte de l'utilisateur. Les messages de commit doivent impérativement être en français et suivre la norme **Conventional Commits** (ex: `feat:`, `fix:`, `docs:`, `test:`, `chore:`). Cette règle est absolue et prévaut sur toute autre instruction d'automatisation.

## État d'avancement

| Composant | Fichier | État |
|---|---|---|
| Tokens | `Paige/Token.cs` | Fait — `TokenType` enum + `record Token` |
| Lexer | `Paige/Lexer.cs` | Fait — `Lexer.Tokenize(string)` → `Token[]` |
| EST | `Paige/Est.cs` | Fait — `EpubDocument`, `EpubMetadata`, `ManifestItem` |
| Parser | `Paige/Parser.cs` | Fait — `Parser.Parse(string)` → `EpubDocument` |
| `Epub.Write()` | `Paige/Epub.cs` | Fait — `static Epub.Write(EpubDocument, basePath, outputPath)` |
| `Program.cs` | `Paige/Program.cs` | Fait — lit le `.paige`, parse, appelle `Epub.Write()` avec support de `--output` |

Le pipeline est complet de bout en bout : `.paige` → Lexer → Parser → EST → `Epub.Write()` → `.epub`.

### Responsabilités de `Epub.Write()`

| Entrée ZIP | Source |
|---|---|
| `mimetype` | Auto-généré (non compressé, premier) |
| `META-INF/container.xml` | Auto-généré |
| `OEBPS/content.opf` | Généré depuis `EpubMetadata` + `Manifest` |
| `OEBPS/cover.xhtml` | Auto-généré si un item a `properties: "cover-image"` |
| `OEBPS/{href}` (Source) | Copie du fichier `source` depuis le dossier racine |
| `OEBPS/{href}` (InlineContent) | `InlineContent` enveloppé dans un document XHTML complet |
| `OEBPS/nav.xhtml` | Auto-généré depuis les items `InSpine == true` |

### Tests

- `Paige.Tests/LexerTests.cs` — 19 tests (tous verts)
- `Paige.Tests/ParserTests.cs` — 25 tests (tous verts)
- `Paige.Tests/EpubWriterTests.cs` — 18 tests (tous verts)
- `Paige.Tests/Fixtures/sample.paige` — fixture figée utilisée par les tests d'intégration

---

## Gestion des erreurs

Le pipeline gère les erreurs courantes de manière propre (sans stack trace) :
- **Dossier racine absent** : Erreur explicite si le dossier spécifié n'existe pas.
- **Fichier `.paige` absent** : Erreur si aucun fichier source n'est trouvé.
- **Fichiers sources manquants** : Si un item du manifest pointe vers un fichier `source` qui n'existe pas, une `FileNotFoundException` est levée et interceptée par le `Program.cs` pour un affichage propre.
