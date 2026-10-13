# Modernization Plan: Runtime and Framework Compatibility - .NET 10

**Project**: RPA Claim Statements  
**Target framework**: `net10.0` for all four solution projects  
**Status**: Planning only; implementation has not started  
**Source assessment**: `report-20261005162854`  
**Selected category**: Runtime Compatibility  
**Selected issue**: Old .NET Framework dependency detected (`Runtime.0003`)  
**Selected solution**: Runtime and Framework Compatibility (`kbId: runtime-compatibility-prompt`)

## Goal and Scope

Upgrade the web application, generator and both test projects from .NET
Framework 4.5 to .NET 10, maintaining existing business behaviour. The user's
.NET 10 target supersedes the original plan's .NET Framework 4.8 runtime check.
Running the old application on .NET Framework 4.8 is not the final outcome.

This is a framework migration, not simply a target-framework edit. ASP.NET MVC 5
and `System.Web` do not run on .NET 10. The web application must become an
ASP.NET Core MVC application, and dependencies and framework APIs must be
made compatible with modern .NET.

Include changes required for runtime compatibility, plus the existing
security/CVE hygiene phase. Exclude Azure deployment, cloud-service migrations,
database relocation, a new identity platform, frontend redesign, and unrelated
assessment findings. Preserve the standalone `PdfCompare` utility.

## Technical Context and Constraints

| Surface | Current implementation | Planned outcome |
|---|---|---|
| Generator | Legacy MSBuild console executable; .NET Framework 4.5 | SDK-style `Microsoft.NET.Sdk` executable targeting `net10.0`, still callable by the web application |
| Web | Legacy ASP.NET MVC 5.2.3, `Global.asax`, Unity MVC, `System.Web` | SDK-style `Microsoft.NET.Sdk.Web` ASP.NET Core MVC host targeting `net10.0` |
| Tests | Two legacy .NET Framework 4.5 projects; NUnit/Moq and legacy Visual Studio references | SDK-style `net10.0` test projects with compatible test SDK, framework, adapters and mocks |
| Persistence | Entity Framework 6.1.3, SQL Server, EF6 migrations and `SqlBulkCopy` | Compatible data-access packages; preserve schema, mappings, precision, migration history and bulk-import behaviour |
| Identity | IIS Windows authentication, role attributes, external security database and `IDT.dll` reference | Preserve Windows identity and current role decisions through ASP.NET Core authentication/authorization |
| Documents and files | Open XML, OpenXmlPowerTools, EPPlus, NReco PDF, WinSCP, CSV and filesystem integrations | Compatible packages/native tools with unchanged document, import and publication contracts |
| Configuration | `App.config`, `Web.config`, environment transforms and database settings | Equivalent configuration for both modern hosts; retain IIS settings where required and never embed secrets |

Keep Windows as the initial validation/hosting environment. `net10.0` does not
imply Linux support. Inventory actual Windows-only API usage, native tools,
permissions and process architecture before choosing package replacements.
If a Windows-specific TFM is genuinely required, obtain approval before
departing from the agreed `net10.0` target.

Prefer the smallest compatible changes. Evaluate a modern-.NET-compatible EF6
version before proposing EF Core: changing ORM is not automatically required.
Do not discard EF6 migrations or recreate the database. Keep compatible Unity
usage in the generator if practical; replace its MVC 5 integration at the web
boundary. Verify package support, licensing and native dependencies rather than
assuming the current packages or the latest releases are suitable.

## Evidence and Applied Migration Rules

Use these existing assessment artifacts as behavioural anchors:

- [Dependency map](../assessment/reports/report-20261005162854/facts/dependency-map.md)
- [API and service contracts](../assessment/reports/report-20261005162854/facts/api-service-contracts.md)
- [Business workflows](../assessment/reports/report-20261005162854/facts/business-workflows.md)
- [Data architecture](../assessment/reports/report-20261005162854/facts/data-architecture.md)
- [Configuration inventory](../assessment/reports/report-20261005162854/facts/configuration-inventory.md)

Replace framework-specific hosting, MVC, configuration and package mechanisms
with their modern equivalents. Preserve observable contracts, including
conventional routes, verbs, form binding, redirects, partial views, download
formats, JSON shapes, role restrictions, antiforgery protection and error
reporting. Flag intentional contract/security changes for approval rather than
silently bundling them into this upgrade.

## Implementation Phases and Task Breakdown

Tasks are persisted in [.metadata/tasks.json](.metadata/tasks.json).
All source paths below are relative to the repository root.

### Phase 1: Prerequisites, Baseline and Compatibility Decisions

- [ ] **001-transform-upgrade-readiness**: Inspect
  `RPA.ClaimStatements/RPA.ClaimStatements.sln` and all four projects. Verify the
  .NET 10 SDK, legacy build tools for baseline capture and the Windows test
  environment. Capture existing build/test results and representative BPS/XB
  outputs using sanitized fixtures. Inventory used package/framework APIs and
  choose compatible versions or approved replacements. Resolve the internal
  `IDT.dll` and external role-provider contract before proceeding.

**Gate**: Missing binary/source access, baseline failures, package/native-tool
incompatibility, or unresolved identity behaviour must be recorded explicitly.
Do not proceed with placeholder authorization or silently skipped validation.
Any dependency replacement that changes licensing or business behaviour
requires approval. The assessment is not proof of .NET 10 compatibility.

### Phase 2: Generator and Persistence Migration

- [ ] **002-transform-generator-net10**: Convert
  `RPA.ClaimStatements/RPA.ClaimStatements.Generator/RPA.ClaimStatements.Generator.csproj`
  to SDK-style `net10.0` and migrate `packages.config` to `PackageReference`.
  Update `Program.cs`, `Context/`, `Migrations/`, `Components/`, `Imports/` and
  `Services/` only as needed for the approved compatibility decisions. Preserve
  CLI modes and culture, dependency lifetimes, mutexes, import ordering,
  eligibility, batch caps, reconciliation, document templates, PDF publication,
  logs and email reporting. Retain the generator's use from the web host.

Preserve explicitly included resources/content, SQL and document templates,
copy-to-output behaviour, assembly metadata and environment-specific settings.
Remove obsolete package imports/compiler targets only after replacing their
required behaviour. Build and validate the generator independently.

### Phase 3: ASP.NET Core MVC and Identity Migration

- [ ] **003-transform-web-net10**: Convert
  `RPA.ClaimStatements/RPA.ClaimStatements.Web/RPA.ClaimStatements.Web.csproj`
  to SDK-style `net10.0`. Replace `Global.asax.cs` and applicable `App_Start/`
  setup with a new `Program.cs`; port `Controllers/`, `Views/`, filters,
  dependency registration, static assets and configuration to ASP.NET Core MVC.
  Replace MVC 5-only bundles/helpers, paging and Unity integration. Preserve
  all 23 assessed actions and existing Windows identity/role semantics.

Implement a compatible role lookup using the verified `IDT.dll` contract or
an approved equivalent; never bypass role checks. Preserve SQL integrated
security identity expectations when the IIS process model changes. Retain
IIS hosting configuration, document the required .NET 10 Hosting Bundle and
ensure no deployed runtime relies on the old `System.Web` host. Historical
deployment-copy directories are not additional application projects.

### Phase 4: Test Projects and Build Integration

- [ ] **004-transform-tests-net10**: Convert
  `RPA.ClaimStatements/RPA.ClaimStatements.Generator.Tests/RPA.ClaimStatements.Generator.Tests.csproj`
  and
  `RPA.ClaimStatements/RPA.ClaimStatements.Web.Tests/RPA.ClaimStatements.Web.Tests.csproj`
  to SDK-style `net10.0`. Update test dependencies and adapt existing controller,
  HTTP-context and EF test doubles without weakening assertions or excluding
  tests to hide failures. Integrate all four projects and build configurations
  in `RPA.ClaimStatements/RPA.ClaimStatements.sln`.

### Phase 5: Security/CVE Hygiene

- [ ] **005-security-cve-remediation**: Scan the resulting .NET dependency graph,
  including transitive dependencies; apply compatible patched versions and
  rerun builds and tests. Retain the original plan's security scope. Document
  unresolved findings and block sign-off rather than reporting a clean result.

### Phase 6: Acceptance and Upgrade Documentation

- [ ] **006-transform-net10-acceptance**: Restore, build, test and publish the
  migrated solution with .NET 10. Start the web host in the Windows validation
  environment and exercise the assessed operator flows. Verify CLI modes and
  imports/generation against isolated fixtures and compare BPS/XB statement
  output with the baseline. Update `README.md` with prerequisites,
  configuration, build/test/publish and IIS/console operation instructions.

Use an isolated database, test identities, controlled inbound/outbound folders
and test SMTP/FTP destinations. Do not run destructive UAT reset instructions
or generation/import flows against live systems. Existing tests and smoke/
behavioural checks are required; this plan does not mandate a new test suite.

## Requirements and Task Mapping

These requirements record the requested target and migration acceptance
constraints; they do not alter the source assessment.

| Requirement | Tasks | Completion evidence |
|---|---|---|
| All four solution projects target .NET 10, not merely Framework 4.8 | 001, 002, 003, 004, 006 | SDK-style project files targeting `net10.0`; restore/build/test/publish results |
| Web runs on ASP.NET Core without legacy hosting dependencies | 001, 003, 004, 006 | New web entry point, migrated MVC/views/configuration and operator-flow results |
| Preserve Windows authentication and existing authorization | 001, 003, 006 | Resolved IDT/provider decision; allowed/denied role checks with test identities |
| Preserve persistence, generator and document behaviour | 001, 002, 004, 006 | Existing schema/history retained; import, generation and BPS/XB comparison evidence |
| Preserve existing test intent and remediate dependency CVEs | 004, 005, 006 | Test discovery/count comparison, passing results and dependency scan |
| Document supported operation without cloud deployment | 003, 006 | Updated README and tested Windows/IIS/console publish artifacts |

## Acceptance Criteria

- All four projects explicitly target `net10.0`; no production path depends on
  a .NET Framework process, `System.Web` or Framework-only runtime assemblies.
- `dotnet restore`, `dotnet build -c Release`, `dotnet test -c Release` and
  publish of both applications succeed using the .NET 10 SDK.
- Existing tests remain discoverable and pass; any baseline failures or
  intentionally changed tests are explained, not silently suppressed.
- The web application starts with .NET 10 and preserves the assessed routes,
  responses and operator permissions, including denied/unauthenticated cases.
- Generator modes, imports, persistence, reconciliation and publication behave
  as before; representative BPS/XB output matches the approved baseline.
- Native document/transfer tools and Windows permissions work in the published
  applications, not just during compilation.
- No unresolved runtime-compatibility or security blocker is hidden by stubs,
  fallbacks, excluded tests or incomplete validation.

## Risks and Outstanding Execution Prerequisites

- `IDT.dll` comes from an internal network reference. Availability and modern
  .NET compatibility have not been established; the external security provider
  must be understood before porting authorization.
- Compatibility of EF6 migrations, document/PDF libraries, WinSCP and any used
  Windows-specific APIs must be verified during Phase 1.
- Windows baseline tooling, .NET 10 SDK/Hosting Bundle and isolated SQL Server,
  identity and integration fixtures must be available for acceptance.
- No application build, package compatibility proof or runtime validation has
  been performed as part of this plan revision.
