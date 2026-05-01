# 📖 Paige — Moteur de création d'EPUB 3

[![Statut du Build](https://img.shields.io/badge/build-passing-brightgreen.svg?style=flat-square)](#)
[![Licence : MIT](https://img.shields.io/badge/Licence-MIT-yellow.svg?style=flat-square)](LICENSE)
[![.NET 10](https://img.shields.io/badge/.NET-10-blueviolet.svg?style=flat-square)](https://dotnet.microsoft.com/)
[![Conventional Commits](https://img.shields.io/badge/Conventional%20Commits-1.0.0-yellow.svg?style=flat-square)](https://conventionalcommits.org)

**Paige** est un outil CLI élégant et pensé pour les développeurs, conçu pour compiler des fichiers sources `.paige` modulaires en archives **EPUB 3** parfaitement valides. Arrêtez de vous battre avec les structures ZIP et le boilerplate XML — concentrez-vous sur l'écriture de votre livre. 🚀

---

## ✨ Fonctionnalités

- 🏗️ **DSL Déclaratif** : Définissez la structure de votre livre avec un langage simple et lisible par l'humain.
- 🧩 **Contenu Modulaire** : Utilisez `#include` pour découper votre livre en chapitres gérables.
- 🖼️ **Gestion des Assets** : Ajoutez facilement des images, polices et feuilles de style via le manifeste.
- 📝 **XHTML Inline** : Écrivez du contenu directement dans vos fichiers `.paige` ou pointez vers des sources externes.
- 🧭 **Génération Automatique** : 
  - `nav.xhtml` (Table des matières) généré automatiquement à partir de la "spine".
  - Génération intelligente de `cover.xhtml`.
  - Fichiers `content.opf` et `container.xml` valides.
- ✅ **Respect des Standards** : Produit des fichiers EPUB 3.0.1 prêts pour les liseuses.
- 🧪 **Robuste** : Développé avec un TDD strict (100%) et des tests unitaires complets.

---

## 🛠️ Installation

```bash
# Cloner le dépôt
git clone https://github.com/votre-nom/paige.git
cd paige

# Compiler le projet
dotnet build Paige/Paige.csproj

# (Optionnel) Installer comme outil global (bientôt disponible)
# dotnet tool install --global Paige
```

---

## 🚀 Démarrage Rapide

### 1. Créez votre fichier source (`livre.paige`)

```paige
#metadata(
    identifier: "978-3-16-148410-0",
    title: "Les Chroniques de Paige",
    language: fr
)

#manifest.add(
    id: "cover-image",
    href: "cover.jpg",
    mediaType: "image/jpeg",
    properties: "cover-image",
    source: "assets/cover.jpg"
)

#include "chapitres/01-introduction.paige"

#manifest.add(
    id: "outro",
    href: "outro.xhtml",
    mediaType: "application/xhtml+xml",
    spine: true,
    nav: "Le mot de la fin"
)[
  <body>
    <h1>Merci de votre lecture !</h1>
    <p>Généré avec amour par Paige.</p>
  </body>
]
```

### 2. Compilez en EPUB

```bash
dotnet run --project Paige -- --project-root . --output build/mon-livre.epub
```

---

## 💻 Utilisation CLI

| Option | Description | Défaut |
|:---|:---|:---|
| `--project-root` | Le dossier racine contenant vos fichiers `.paige`. | `.` |
| `--output` | Chemin de destination pour le fichier `.epub` généré. | `mybook.epub` |
| `--help` | Affiche les commandes et options disponibles. | - |

---

## 🏗️ Architecture

Paige utilise un pipeline de compilation moderne :

1. 🔍 **Lexer** : Analyse lexicale du source `.paige`.
2. 📐 **Parser** : Construit un **Epub Syntactic Tree (EST)** structuré.
3. 📦 **Générateur** : Pilote la `ZipArchive` pour produire le paquet EPUB final.

---

## 🧪 Développement & Méthodologie

Nous suivons une méthodologie **TDD (Test-Driven Development)** stricte. Chaque fonctionnalité est vérifiée par une suite de plus de 60 tests.

```bash
# Lancer la suite de tests
dotnet test Paige.Tests/Paige.Tests.csproj
```

---

## 📜 Licence

Distribué sous la licence **MIT**. Voir `LICENSE` pour plus d'informations.

---

## 🤝 Contribution

Les contributions sont les bienvenues ! Veuillez vous assurer de respecter la norme [Conventional Commits](https://conventionalcommits.org) et d'inclure des tests pour toute nouvelle fonctionnalité.

---

<p align="center">Fait avec ❤️ pour les auteurs et les développeurs.</p>
