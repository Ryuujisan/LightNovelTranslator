# LightNovelTranslator

Local AI-powered Light Novel translator built with .NET, Ollama and React.

LightNovelTranslator is a cross-platform application for translating Light Novels and other DOCX documents using locally hosted LLMs. The project supports batch processing, translation recovery, job management and real-time progress tracking.

---
## Live Demo

Frontend:
https://ashy-bush-04ce5a910.7.azurestaticapps.net/

# Features

## Translation

- Translate DOCX files using local LLMs
- Batch translation of multiple files
- Folder upload support
- Resume interrupted translations
- Retry failed chunks using a stronger model
- Multi-language translation support
- Local-first architecture (no cloud AI required)

## Document Preservation

- Preserve DOCX formatting
- Preserve images
- Preserve tables
- Preserve page breaks
- Preserve paragraph structure

## Job Management

- Job-based translation workflow
- Create and manage translation jobs
- Rename jobs
- Add files to existing jobs
- Remove files from jobs
- Resume existing jobs
- Real-time translation progress

## User Interface

- React Web UI
- Drag & Drop file upload
- SignalR real-time progress updates
- Cross-platform API backend
- Responsive Material UI interface

## Deployment

- GitHub Actions CI/CD
- Automated GitHub Releases
- Multi-platform builds:
    - Windows
    - Linux
    - macOS Intel
    - macOS Apple Silicon

---

# Architecture

```text
LightNovelTranslator
├── src
│
├── LightNovelTranslator.Core
│   ├── Translation Pipeline
│   ├── Job Processing
│   ├── Progress Tracking
│   └── Prompt Management
│
├── LightNovelTranslator.Docx
│   ├── DOCX Reader
│   ├── DOCX Writer
│   └── Formatting Preservation
│
├── LightNovelTranslator.Ollama
│   └── Ollama Integration
│
├── LightNovelTranslator.Cli
│   └── Command Line Interface
│
├── LightNovelTranslator.Api
│   ├── REST API
│   ├── SignalR Hub
│   ├── Job Queue
│   └── Background Services
│
├── frontend
│   ├── React
│   ├── Zustand
│   ├── Material UI
│   └── SignalR Client
│
└── LightNovelTranslator.App
    └── Avalonia UI (Work In Progress)
```

---

# Tech Stack

## Backend

- .NET 10
- ASP.NET Core
- SignalR
- OpenXML SDK
- Ollama

## Frontend

- React
- TypeScript
- Zustand
- Material UI
- Axios

## AI Models

### Recommended Models

#### Fast Translation
```bash
ollama pull qwen3.5:9b
```
Recommended for fast draft translations and testing.<br>

Fast generation speed<br>
Low VRAM requirements<br>
Good for initial translation passes<br>
Best choice for large translation batches
### Quality / Repair Models
```bash
ollama pull qwen3:32b
```

Alternative:

```bash
ollama pull qwen2.5:32b
```
```bash
ollama pull huihui_ai/Qwen3.6-abliterated:27b
```
Recommended for retry and correction passes.<br><br>

Higher translation quality<br>
Better context understanding<br>
Improved consistency<br>
Ideal for repairing failed chunks<br>
Recommended as Retry Model<br>

---

# Current Features

### CLI

- Batch translation
- Resume support
- Retry support
- DOCX preservation

### API

- Upload files
- Create translation jobs
- Resume existing jobs
- Real-time SignalR updates
- Job management

### Frontend

- Multi-file upload
- Folder upload
- Translation settings
- Job management
- Real-time progress tracking

---

# Running Locally

## Start Ollama

```bash
ollama serve
```

Install model:

```bash
ollama pull qwen3:32b
```
```bash
ollama pull qwen3.5:9b
```
```bash
ollama pull huihui_ai/Qwen3.6-abliterated:27b
```

---

## Run Backend

```bash
dotnet run --project src/LightNovelTranslator.Api
```

Default API:

```text
http://localhost:5156
```

---

## Run Frontend

```bash
cd frontend

npm install
npm run dev
```

Default frontend:

```text
http://localhost:5173
```

---

# CI/CD

GitHub Actions automatically:

- Build API
- Package releases
- Create GitHub Releases
- Publish platform-specific artifacts

Supported runtimes:

- win-x64
- linux-x64
- osx-x64
- osx-arm64

---

# Roadmap

## v0.1

- Job management
- Resume support
- Retry support
- React Web UI
- SignalR integration

## v0.2

- Avalonia Desktop UI
- Translation statistics
- Improved job monitoring
- Better retry workflow

## v0.3

- Translation Memory
- Custom Glossary
- User Profiles

## v0.4

- EPUB support
- PDF support
- OCR support

---

# Screenshots

## Web UI

- Translation dashboard
- Job management
- Real-time progress tracking

---

# Author

Created by Damian (Ryuujisan)

GitHub:
https://github.com/Ryuujisan

---

# License

MIT License