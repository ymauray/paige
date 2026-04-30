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

Deux directives pour l'instant :

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
    spine: <bool>           // optionnel — false par défaut
)[
  <!-- contenu XHTML inline, optionnel -->
]
```

Le bloc `[…]` est le corps textuel de l'item (XHTML, Markdown, texte brut, YAML…). S'il est absent, `source` doit pointer vers un fichier existant.

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

## État d'avancement

| Composant | Fichier | État |
|---|---|---|
| Tokens | `Paige/Token.cs` | Fait — `TokenType` enum + `record Token` |
| Lexer | `Paige/Lexer.cs` | Fait — `Lexer.Tokenize(string)` → `Token[]` |
| EST | `Paige/Est.cs` | Fait — `EpubDocument`, `EpubMetadata`, `ManifestItem` |
| Parser | `Paige/Parser.cs` | Fait — `Parser.Parse(string)` → `EpubDocument` |
| `Epub.Write()` | `Paige/Epub.cs` | **À faire** — accepter un `EpubDocument`, supprimer le contenu hardcodé |
| `Program.cs` | `Paige/Program.cs` | **À faire** — lire le `.paige`, parser, passer l'EST à `Epub` |

### Tests

- `Paige.Tests/LexerTests.cs` — 19 tests (tous verts)
- `Paige.Tests/ParserTests.cs` — 22 tests (tous verts)
- `Paige.Tests/Fixtures/sample.paige` — fixture figée utilisée par les tests d'intégration
