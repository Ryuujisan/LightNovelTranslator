# LightNovelTranslator

Local AI-powered DOCX light novel translator built with .NET and Ollama.

## Features

- Translate DOCX files using local LLMs
- Ollama integration
- Qwen3 / Qwen2.5 support
- Batch translation
- Resume interrupted translations
- Preserve DOCX formatting
- Preserve images
- Preserve tables
- Preserve page breaks
- Cross-platform (Windows / Linux / macOS)
- CLI mode
- Avalonia UI (planned)

## Tech Stack

- .NET 10
- C#
- Ollama
- Qwen3
- OpenXML SDK
- Spectre.Console
- Avalonia UI

## Project Structure

```text
LightNovelTranslator
├── src
│   ├── LightNovelTranslator.Core
│   ├── LightNovelTranslator.Ollama
│   ├── LightNovelTranslator.Docx
│   ├── LightNovelTranslator.Cli
│   └── LightNovelTranslator.App
└── translated
```
## Roadmap
# Version 1
DOCX loading
Ollama integration
Single file translation
Batch translation
Progress tracking
# Version 2
Translation memory
Custom glossary
Resume after interruption
Parallel processing
# Version 3
Avalonia UI
Drag & Drop support
Translation statistics
# Version 4
EPUB support
PDF support
Multi-model support
## Requirements
Ollama
Qwen3:32b (recommended)<br>
Qwen2.5:32b (optional)

```text
ollama pull qwen3:32b
```
