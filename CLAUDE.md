# IntelliMed — Notes for Claude Code

IntelliMed is a country-agnostic practice-management system boilerplate, targeting ASP.NET Core
Web API + Blazor (WASM web today, MAUI Blazor Hybrid for native later). Clean Architecture: `Core`
(entities/DTOs/interfaces, no EF) → `Infrastructure` (EF Core/SQLite, repositories, business-logic
services) → `Api` (controllers) / `Web` (Blazor WASM UI).

This repo is the starting point for country-specific forks — it deliberately has no
jurisdiction-specific claiming/billing logic (no automatic rebate calculation, no government payer
integrations). Invoicing is a plain, user-managed billing-item catalog and optional named price
lists (`BillingItem`/`FeeSchedule`); a country fork adds its own claiming rules on top of that
foundation rather than editing it in place.

Read these before making non-trivial changes — don't duplicate their content here, they're kept
up to date independently:

- `ARCHITECTURE.md` — system architecture, roles/permissions, deployment.
- `STRUCTURE.md` — project/folder layout and code patterns (repository pattern, DTO mapping).
- `REBUILD_PLAN.md` — modernization plan, MAUI setup gotchas, deployment environments.

## Build / run

- API + Blazor WASM client: `dotnet run` from `src/IntelliMed.Api` (client is served at `/`).
  Local API: `http://localhost:5284`. DB migrations run automatically on startup (SQLite,
  `intellimed.db`).
- Docker (matches the Render.com staging deploy): `docker build` from the repo root, see
  `Dockerfile` — `dotnet publish src/IntelliMed.Api -c Release`, entrypoint
  `dotnet IntelliMed.Api.dll`.
- Tests: `IntelliMed.Tests` project (`dotnet test`).

## Notes / planned features

- **IntelliSearch** — the floating search button (bottom-right) and `Ctrl+K` overlay for
  navigating the app and running quick actions. The action list is stored in the `SearchActions`
  table and editable from Administration → Command Palette Actions — extend it there rather than
  hardcoding new entries in the component.
