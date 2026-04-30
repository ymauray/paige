# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build Paige/Paige.csproj

# Run (generate an EPUB from a project folder)
dotnet run --project Paige -- --project-root <dossier>

# Tests
dotnet test Paige.Tests/Paige.Tests.csproj

# Publish
dotnet publish Paige/Paige.csproj
```

The `<dossier>` must contain at least a `cover.jpg`. Optionally a `title-page.xhtml`.

## Architecture

Paige is a CLI EPUB 3 generator. The long-term goal is to compile `.paige` files (a custom DSL) into valid EPUB archives.

### Current state

- **`Token.cs`** — `TokenType` enum and `record Token(Type, Value, Line)`.
- **`Lexer.cs`** — `Lexer.Tokenize(string)` → `Token[]`. Handles all `.paige` token types.
- **`Est.cs`** — EST records: `EpubDocument`, `EpubMetadata`, `ManifestItem`.
- **`Parser.cs`** — `Parser.Parse(string)` → `EpubDocument`. Recursive descent, calls the Lexer internally.
- **`Epub.cs`** — builds an EPUB 3 ZIP archive using `System.IO.Compression.ZipArchive`. Content is still hardcoded; pending update to accept an `EpubDocument`.
- **`Program.cs`** — thin CLI wrapper using `System.CommandLine`. Pending update to read the `.paige` file and drive the full pipeline.

### The `.paige` DSL

`.paige` files (see `mybook.paige`) describe a book declaratively:

```
#metadata(identifier: …, title: "…", language: fr)

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
OEBPS/content.opf             (manifest + spine)
OEBPS/cover.xhtml
OEBPS/cover.jpg
OEBPS/title-page.xhtml        (optional, copied verbatim if present)
OEBPS/chapitre1.xhtml
OEBPS/nav.xhtml
```
