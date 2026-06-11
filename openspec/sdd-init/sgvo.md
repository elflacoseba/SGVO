# SDD Init — SGVO

**Executed:** 2026-06-10
**Artifact store:** openspec
**Execution mode:** interactive
**Review budget:** 400 lines

---

## Status: `success`

## Stack Detected

| Technology | Version | Notes |
|------------|---------|-------|
| .NET | 10.0 | |
| EF Core | 9.0.5 | Pomelo.EntityFrameworkCore.MySql |
| MySQL | 9.6.0 | Socket: `/tmp/mysql.sock` |
| xUnit | 2.9.3 | |
| FluentAssertions | 7.2.0 | |
| Moq | 4.20.72 | |
| Microsoft.NET.Test.Sdk | 17.14.1 | |
| coverlet.collector | 6.0.4 | Coverage |
| Microsoft.AspNetCore.Mvc.Testing | 10.0.8 | Integration tests |
| Microsoft.EntityFrameworkCore.InMemory | 9.0.5 / 9.0.0 | |

**Architecture:** Clean Architecture (Domain / Application / Infrastructure / Api / Shared)

---

## Testing Capability

### Test Runner
- **Command:** `dotnet test`
- **Framework:** xUnit v2.9.3

### Test Layers

| Layer | Available | Tool |
|-------|-----------|------|
| Unit | ✅ | xUnit + Moq + FluentAssertions + EF InMemory |
| Integration | ✅ | xUnit + Microsoft.AspNetCore.Mvc.Testing (WebApplicationFactory) |
| E2E | ❌ | Not configured |

### Coverage
- **Available:** ✅
- **Command:** `dotnet test --collect:"XPlat Code Coverage"`

### Quality Tools

| Tool | Available | Command |
|------|-----------|---------|
| Linter | ✅ (via TreatWarningsAsErrors) | `dotnet build` |
| Type checker | ✅ (Nullable enabled) | `dotnet build` |
| Formatter | ❌ | Not configured |

---

## Strict TDD

**Status:** `false` — No TDD enforcement marker found. Test projects exist with proper isolation (Moq, InMemory DB), but there is no enforced RED-GREEN-REFACTOR workflow or TDD-specific CI gate.

**Fallback applied:** `strict_tdd: false` — test runner exists, no strict mode enforced.

---

## Conventions Found

### Database (from sql/ and AGENTS.md)
- Table names: PascalCase, plural, Spanish singular (`Personas`, `Ocupaciones`)
- Column names: PascalCase, no prefixes
- PKs: `Id` (BIGINT UNSIGNED → `long` in C#)
- FKs: `[Entity]Id` pattern (`PersonaId`, `PuestoId`)
- Timestamps: `CreadoEn`, `ModificadoEn`
- Soft delete: `Activo` (boolean)
- Enums: mapped to `VARCHAR` in MySQL

### Known Decisions
- Auditoría: single table with JSON columns, implemented via `SaveChangesInterceptor`
- Skill match: `PuntajeMatch` stored in `Postulaciones`, recalculated at evaluation
- Postulantes: `PersonaId` nullable for external candidates
- Hierarchy: self-referencing FKs on `UnidadesOrganizativas` (PadreId) and `Puestos` (SuperiorId)
- Auth: SHA-256 hashed passwords (dev), M:N with Roles via UsuarioRoles

### Project Structure
```
src/
  SGVO.Api/        — Controllers, middleware, extensions
  SGVO.Application/ — Commands, queries, validators, behaviors
  SGVO.Domain/     — Entities, interfaces, events, value objects
  SGVO.Infrastructure/ — EF Core, repositories, services
  SGVO.Shared/    — Common types, Result pattern
tests/
  SGVO.UnitTests/      — 17 test files
  SGVO.IntegrationTests/ — Api tests, extension tests
```

---

## OpenSpec Artifacts

| Path | Status |
|------|--------|
| `openspec/config.yaml` | **Created** |
| `openspec/specs/` | Exists (empty) |
| `openspec/changes/` | Exists with `jwt-auth-refresh-bcrypt` |
| `openspec/changes/archive/` | Exists (empty) |

---

## Skill Resolution

### Project Skills (from `.agents/skills/`)
| Skill | Path |
|-------|------|
| `database-designer` | `.agents/skills/database-designer/SKILL.md` |
| `dotnet-api` | `.agents/skills/dotnet-api/SKILL.md` |
| `dotnet-backend-patterns` | `.agents/skills/dotnet-backend-patterns/SKILL.md` |
| `dotnet-best-practices` | `.agents/skills/dotnet-best-practices/SKILL.md` |
| `dotnet-csharp` | `.agents/skills/dotnet-csharp/SKILL.md` |
| `dotnet-xunit` | `.agents/skills/dotnet-xunit/SKILL.md` |
| `github-issues` | `.agents/skills/github-issues/SKILL.md` |
| `mysql` | `.agents/skills/mysql/SKILL.md` |
| `pr-review-dotnet` | `.agents/skills/pr-review-dotnet/SKILL.md` |

### User Skills (from skill-registry)
| Skill | Path |
|-------|------|
| `branch-pr` | `/Users/elflacoseba/.config/opencode/skills/branch-pr/SKILL.md` |
| `chained-pr` | `/Users/elflacoseba/.config/opencode/skills/chained-pr/SKILL.md` |
| `cognitive-doc-design` | `/Users/elflacoseba/.config/opencode/skills/cognitive-doc-design/SKILL.md` |
| `comment-writer` | `/Users/elflacoseba/.config/opencode/skills/comment-writer/SKILL.md` |
| `go-testing` | `/Users/elflacoseba/.config/opencode/skills/go-testing/SKILL.md` |
| `issue-creation` | `/Users/elflacoseba/.config/opencode/skills/issue-creation/SKILL.md` |
| `judgment-day` | `/Users/elflacoseba/.config/opencode/skills/judgment-day/SKILL.md` |
| `skill-creator` | `/Users/elflacoseba/.config/opencode/skills/skill-creator/SKILL.md` |
| `skill-improver` | `/Users/elflacoseba/.config/opencode/skills/skill-improver/SKILL.md` |
| `work-unit-commits` | `/Users/elflacoseba/.config/opencode/skills/work-unit-commits/SKILL.md` |

**Skill registry:** `.atl/skill-registry.md` (already exists, up to date)

---

## Next Recommended

1. **Orchestrator** → Launch `/sdd-explore` to begin a new change or continue existing ones
2. **Or** → Use `/sdd-propose` to create a formal proposal for a change
3. Existing change `jwt-auth-refresh-bcrypt` can be resumed via `sdd-verify` or `sdd-archive`

---

## Risks

| Risk | Severity | Mitigation |
|------|----------|------------|
| No CI/GitHub Actions configured | Medium | Consider adding `dotnet test` + coverage to CI |
| No formatter (dotnet format) configured | Low | Add `.editorconfig` and `dotnet format` to workflow |
| Strict TDD not enforced | Low | Project has test isolation (Moq + InMemory); can enable if requested |
| No opencode.json workspace config | Low | Optional; .NET projects work without it |