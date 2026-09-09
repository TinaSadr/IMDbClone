# IMDbClone 🎬

A personal portfolio project inspired by IMDb, built with ASP.NET Core, microservices, SQLite, TMDB, and semantic search.

The goal of this project is to build a modern movie discovery platform with movie browsing, detailed movie information, semantic search, authentication, and user features.

---

## 🚀 Project Status

The project is currently under active development.

### Completed

- ASP.NET Core solution setup
- Microservice-based architecture
- MovieService
- SearchService foundation
- WebApp
- SQLite database
- Imported 4,803 movies from the TMDB 5000 Movie Dataset
- TMDB API integration
- Secure TMDB API token storage using .NET User Secrets
- TMDB movie poster enrichment
- Movie posters displayed on the homepage
- Movie posters displayed on movie detail pages
- Movie detail pages
- GitHub repository and version control

### In Progress

- Semantic movie search using OpenAI embeddings
- Connecting semantic search to the WebApp search bar

### Planned

- User authentication
- Registration and login
- JWT authentication
- User profiles
- Watchlists
- Movie ratings
- Improved movie discovery
- UI/UX improvements
- Automated tests
- Docker support
- Deployment
- Project documentation and architecture diagrams

---

# 🏗️ Architecture

The application is organized around separate services.

```text
                         ┌──────────────────┐
                         │      WebApp      │
                         │    ASP.NET MVC   │
                         │      :5270       │
                         └────────┬─────────┘
                                  │
                    ┌─────────────┴─────────────┐
                    │                           │
                    ▼                           ▼
          ┌──────────────────┐       ┌──────────────────┐
          │   MovieService   │       │  SearchService   │
          │   ASP.NET Core   │       │   ASP.NET Core   │
          │      :5132       │       │      :5112       │
          └────────┬─────────┘       └────────┬─────────┘
                   │                          │
                   ▼                          ▼
             ┌───────────┐              ┌─────────────┐
             │  SQLite   │              │    OpenAI   │
             │  Database │              │  Embeddings │
             └───────────┘              └─────────────┘
                   │
                   ▼
             ┌───────────┐
             │   TMDB    │
             │    API    │
             └───────────┘