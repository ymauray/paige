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

- **`Epub.cs`** — the only non-trivial class. It builds an EPUB 3 ZIP archive directly using `System.IO.Compression.ZipArchive`. All content (metadata, chapters, nav) is currently hardcoded here.
- **`Program.cs`** — thin CLI wrapper using `System.CommandLine`. Reads `--project-root`, checks for `cover.jpg`, then delegates to `Epub.Write()`.

### The `.paige` DSL

`.paige` files (see `mybook.paige`) describe a book declaratively:

```
#metadata(identifier: …, title: "…", language: fr)

#manifest.add(id: "…", href: "…", mediaType: "…", spine: true)[
  <head>…</head>
  <body>…</body>
]
```

**The DSL parser does not exist yet.** Implementing it — so that `Epub` is driven by a parsed `.paige` file instead of hardcoded values — is the main pending work.

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
