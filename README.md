<div align="center">

# novini4ka *(новини4ка)*

### An automated news aggregation platform built with ASP.NET Core

**Pulls, processes, and serves news from multiple sources — automatically.**

<br/>

[![C#](https://img.shields.io/badge/C%23-80%25-239120?style=flat-square&logo=csharp&logoColor=white)](https://github.com/georgidelchev/novini4ka/search?l=c%23)
[![HTML](https://img.shields.io/badge/HTML-16%25-E34F26?style=flat-square&logo=html5&logoColor=white)](https://github.com/georgidelchev/novini4ka/search?l=html)
[![CSS](https://img.shields.io/badge/CSS-3%25-1572B6?style=flat-square&logo=css3&logoColor=white)](https://github.com/georgidelchev/novini4ka/search?l=css)
[![Build](https://img.shields.io/github/actions/workflow/status/georgidelchev/novini4ka/dotnet.yml?style=flat-square&label=CI&logo=github-actions&logoColor=white)](https://github.com/georgidelchev/novini4ka/actions)
[![License](https://img.shields.io/badge/License-MIT-6366F1?style=flat-square)](LICENSE)
[![Stars](https://img.shields.io/github/stars/georgidelchev/novini4ka?style=flat-square&color=F59E0B)](https://github.com/georgidelchev/novini4ka/stargazers)

<br/>

*The name is Bulgarian — новини (novini) means "news", and the "-4ka" suffix gives it*
*a playful, familiar feel. Small name, serious architecture.*

<br/>

</div>

---

## 📰 About

**novini4ka** is a full-stack ASP.NET Core news aggregation platform that automatically collects articles from multiple news sources on a scheduled basis, stores them, and presents them in a clean browsable interface — no manual curation needed.

At its core is a pipeline: scheduled cron jobs trigger news source adapters, which fetch and normalise articles into a unified data model, persisted via Entity Framework Core. The web layer then serves that content to users in real time.

The project follows the same layered, production-grade architecture as [AYN](https://github.com/georgidelchev/AYN-) — complete with StyleCop code quality enforcement and a GitHub Actions CI pipeline.

---

## ✨ Features

### 🤖 Automated News Pipeline
- **Cron job scheduler** — background jobs run on a configurable schedule to fetch fresh news automatically
- **Multiple news source adapters** — the `NewsSources` service layer abstracts each external source behind a common interface, making it trivial to add more
- **Normalised data model** — articles from different sources are mapped into a consistent structure before storage

### 📋 Content & Browsing
- Browse aggregated articles by source, category, or date
- Clean, readable article listings with metadata (title, source, date, summary)
- Paginated results for navigating large article sets

### 👤 User System
- ASP.NET Core Identity — registration, login, role management
- User preferences and saved articles

### 🛠️ Code Quality
- **StyleCop** with custom ruleset — consistent, enforceable code style across the entire solution
- **Unit tests** — service layer covered by an isolated test suite
- **GitHub Actions CI** — build and test pipeline runs on every push to `main`

---

## 🏗️ Architecture

novini4ka is built on a clean layered architecture mirroring industry patterns:

```
novini4ka/
├── .github/workflows/              # CI/CD — GitHub Actions pipeline
├── Novinichka.Common/              # Shared constants and utility helpers
├── Novinichka.Services.CronJobs/   # Scheduled background jobs (news fetching)
├── Novinichka.Services.NewsSources/ # News source adapters & normalisation logic
├── Data/
│   ├── Novinichka.Data/            # DbContext, EF Core configuration & migrations
│   └── Novinichka.Data.Models/     # Domain entities (Article, Source, Category…)
├── Services/                       # Core business logic services
├── Tests/                          # Unit tests for the service layer
└── Web/                            # ASP.NET Core MVC — controllers, views, areas
```

The `NewsSources` and `CronJobs` layers are deliberately separated — sources define *how* to fetch and normalise data; jobs define *when* to trigger that process. This makes the system easy to extend with new sources without touching scheduling logic.

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core MVC |
| Language | C# (.NET) |
| ORM | Entity Framework Core |
| Auth | ASP.NET Core Identity |
| Background Jobs | Cron-based scheduler |
| Database | MS SQL Server |
| Code Quality | StyleCop + custom ruleset |
| CI/CD | GitHub Actions |
| IDE | Visual Studio |

---

## 🚀 Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (local or remote)
- Visual Studio or JetBrains Rider

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/georgidelchev/novini4ka.git
   cd novini4ka
   ```

2. **Configure the connection string** in `Web/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=Novinichka;Trusted_Connection=True;"
     }
   }
   ```

3. **Apply migrations and run**
   ```bash
   cd Web
   dotnet ef database update
   dotnet run
   ```

4. The cron jobs will begin fetching news automatically on their configured schedule. Open `https://localhost:5001` to browse articles.

---

## 👤 Author

**Georgi Delchev**

[![GitHub](https://img.shields.io/badge/GitHub-georgidelchev-181717?style=flat-square&logo=github)](https://github.com/georgidelchev)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-delchevgeorgi-0A66C2?style=flat-square&logo=linkedin)](https://www.linkedin.com/in/delchevgeorgi/)
[![Facebook](https://img.shields.io/badge/Facebook-georgi.d99-1877F2?style=flat-square&logo=facebook)](https://www.facebook.com/georgi.d99/)

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

<div align="center">
<sub>Made with ☕ and a passion for clean architecture · новини4ка</sub>
</div>
