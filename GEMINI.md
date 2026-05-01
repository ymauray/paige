# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build Paige/Paige.csproj

# Run (generate an EPUB from a project folder)
dotnet run --project Paige -- --project-root <dossier> [--output <path.epub>]

# Tests
dotnet test Paige.Tests/Paige.Tests.csproj

# Publish
dotnet publish Paige/Paige.csproj
```

The `<dossier>` must contain a `.paige` file. Defaults to the current directory if omitted.

## Architecture

Paige is a CLI EPUB 3 generator. The long-term goal is to compile `.paige` files (a custom DSL) into valid EPUB archives.

### Current state

- **`Token.cs`** — `TokenType` enum and `record Token(Type, Value, Line)`.
- **`Lexer.cs`** — `Lexer.Tokenize(string)` → `Token[]`. Handles all `.paige` token types.
- **`Est.cs`** — EST records: `EpubDocument`, `EpubMetadata`, `ManifestItem`.
- **`Parser.cs`** — `Parser.Parse(string, basePath)` → `EpubDocument`. Recursive descent, supports `#include` and relative path resolution.
- **`Epub.cs`** — `static Epub.Write(EpubDocument, basePath, outputPath)`. Builds an EPUB 3 ZIP archive driven by the EST. Auto-generates `cover.xhtml` (if a `cover-image` item exists) and `nav.xhtml` from spine items.
- **`Program.cs`** — CLI wrapper using `System.CommandLine`. Finds the `.paige` file in `--project-root`, parses it, calls `Epub.Write()`. Gère proprement les erreurs d'entrée/sortie et supporte l'option `--output`.

### The `.paige` DSL

`.paige` files (see `mybook.paige`) describe a book declaratively:

```
#metadata(...)
#include "chapters.paige"

#manifest.add(id: "…", href: "…", mediaType: "…", spine: true)[
  <head>…</head>
  <body>…</body>
]
```

See `SPECS.md` for the full grammar and EST definition.

### EPUB structure produced

```
mimetype                      (uncompressed, always first)
META-INF/container.xml
OEBPS/content.opf             (manifest + spine, generated from EST)
OEBPS/cover.xhtml             (auto-generated if a cover-image item exists)
OEBPS/{href}…                 (one entry per manifest item)
OEBPS/nav.xhtml               (auto-generated from spine items)
```
