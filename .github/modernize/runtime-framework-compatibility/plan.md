# Modernization Plan: Runtime and Framework Compatibility - .NET 10

**Project**: RPA Claim Statements  
**Target framework**: `net10.0` for all four solution projects  
**Status**: Phases 1-3 (tasks 001-003) accepted pending the open `Views/Log/Index.cshtml` item; Phase 4 (task 004) accepted; Phase 5 (task 005) accepted; Phase 6 (task 006) executed and awaiting review, **not signed off**: it depends on the owner decisions below. `Views/Log/Index.cshtml` was supplied, ported and is now tracked. Work stops for review after each phase.  
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

## Phase 1 Outcome and Approved Decisions

Full findings, evidence and probe results are in
[phase1-readiness.md](phase1-readiness.md); the legacy test baseline (123 + 8
passing tests) is in [baseline-tests.txt](baseline-tests.txt). Approved decisions
that shape the remaining phases:

| Topic | Decision | Applies to |
|---|---|---|
| Uncompiled files | The 36 Generator files in `Imports/` and parts of `Services/`, 5 Generator.Tests files, `ARController` and `ARControllerTests` stay out of the build, remain in git and are not modernized or modified. The compiled set must stay identical (22 live actions; 123 + 8 tests). | 002, 003, 004 |
| Role provider (`IDT.dll`) | Reimplement the current `IDT.Web.Security.RoleProvider` behaviour (source no longer exists) by calling the `Security` database `aspnet_UsersInRoles_IsUserInRole`, `aspnet_UsersInRoles_GetRolesForUser` and `aspnet_Roles_CreateRole` procedures, so database-side role inheritance is preserved. Keep the swallowed "create role if missing" call. Verified against a schema-only script on a local LocalDB copy. | 003, 006 |
| Word-to-HTML | Keep OpenXmlPowerTools 4.5.3.2 (restored through .NET Framework compatibility, NU1701) with `System.Drawing.Common`; revisit Clippit later if necessary. | 002 |
| NReco / WinSCP | Upgrade NReco.PdfGenerator to 1.2.1 and WinSCP to 6.5.7 (wrapper and `WinSCP.exe` must match). NReco licensing (single-server vs multi-server) still to be checked by the user; not a blocker. wkhtmltopdf 0.12.4 to 0.12.6 may change XB layout; compare real fixture PDFs. | 002, 006 |
| PDF baseline data | Fabricated fixtures in a LocalDB database; baseline PDFs come from the legacy build and are stored outside the repo. Word templates are supplied locally and are git-ignored (`*.docx`). | 002, 006 |
| Hosting validation | Kestrel with LocalDB for local validation. IIS-hosted behaviour (in-process hosting, IIS Windows authentication, app pool identity) is not verifiable locally and is reported as unverified in task 006 until checked on a real IIS host. | 003, 006 |
| Connection encryption | Microsoft.Data.SqlClient defaults to `Encrypt=True`; the current connection strings do not set it. EF6 6.5.2 uses System.Data.SqlClient 4.8.6 (defaults unchanged), so the Generator is not affected; the new role lookup in task 003 must choose its SQL client and encryption options explicitly (needs an approved value) for the `Security` database. | 003 |

## Implementation Phases and Task Breakdown

Tasks are persisted in [.metadata/tasks.json](.metadata/tasks.json).
All source paths below are relative to the repository root.

### Phase 1: Prerequisites, Baseline and Compatibility Decisions

- [x] **001-transform-upgrade-readiness**: Inspect
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

**Result (2026-10-14)**: Complete. Legacy build and tests baselined; package and
native-tool compatibility probed on .NET 10; `IDT.dll` role contract and the
`Security` schema verified; all nine decisions answered. Not captured: BPS/XB
baseline PDFs (needs fabricated fixtures; tracked for tasks 002/006).

### Phase 2: Generator and Persistence Migration

- [x] **002-transform-generator-net10**: Convert
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

**Result (2026-10-14, awaiting review)**: The Generator is an SDK-style
`net10.0` project (`AppendTargetFrameworkToOutputPath=false`, so
`bin\<Configuration>\` is kept for `FolderService.Root()`), with all six
configurations (Debug, Release, Test, SIT, UAT, Production) building, publishing
and transforming `App.config` per configuration (SlowCheetah 4.0.50).
`packages.config` and the tracked `WinSCP.exe` (5.9.6) were removed; the WinSCP
6.5.7 package supplies the matching executable. The compiled source set is
identical to the legacy one (161 files) plus three new files; the uncompiled
files are excluded and untouched. The ten EF6 migration resources keep their
manifest names.

Code changes, only where .NET 10 required them:

- `Services/FolderService.cs`: `HttpRuntime.AppDomainAppId` replaced by
  `HostEnvironment.IsWebHosted` (new `Services/HostEnvironment.cs`, default
  `false`). **The web host must set it to `true` at startup (task 003).**
- `Components/TrackingStation/TrackingStation.cs` with new
  `ReportHtmlWriter.cs`: `System.Web.UI.HtmlTextWriter` replaced by a small
  writer that reproduces its output exactly.
- `Services/SmtpSettings.cs`, `Components/TrackingStation/EmailService.cs` and
  `App.config`: .NET ignores `<system.net><mailSettings>` and throws on the
  undeclared `system.net` section, so the section is declared in `App.config`
  and read explicitly; the existing per-environment transforms are unchanged.
- `RPA.ClaimStatements.Generator.csproj`: dropped the unused Office COM
  references and bootstrapper entries; `System.Management.Automation.dll`
  (transitive of OpenXmlPowerTools, unused) is excluded.
- `Components/Turbine/TransformService.cs`, `RPA.ClaimStatements.Generator.csproj`
  and `wkhtmltopdf/`: wkhtmltopdf is pinned to the legacy 0.12.4 binaries (the
  20 tracked files moved from `Web/App_Data/wkhtmltopdf` with `git mv`, plus the
  30 runtime files NReco 1.2.1 would otherwise extract at run time; 50 files,
  24 MB), copied to the output and passed to NReco as `PdfToolPath`. A missing
  `wkhtmltopdf.exe` throws instead of letting NReco silently extract 0.12.6.
- Packages: CsvHelper 2.16.3, DocumentFormat.OpenXml 2.7.2 and
  OpenXmlPowerTools 4.5.3.2 kept as they are (compatibility mode for the
  latter); EntityFramework 6.5.2, EPPlus 4.5.3.3, FastMember 1.5.0, NBuilder
  6.1.0, NReco.PdfGenerator 1.2.1, Unity 5.11.10, WinSCP 6.5.7,
  System.Drawing.Common and System.Configuration.ConfigurationManager 10.0.12.

Validation: legacy (.NET Framework 4.5 build) and new builds were run side by
side against isolated LocalDB databases with fabricated fixtures and compared.

| Check | Result |
|---|---|
| EF6 migrations on an empty database | 27 tables, 670 columns, 41 indexes, 14 foreign keys and 10 migration-history rows identical, except two default constraints (see risks) |
| Import mode (AP CSV, AR workbook, bulk copy, archive, monitors, errors) | Imported rows, archived files and console output identical |
| Tracking report email | Same headers, encoding and HTML as the legacy build (only timestamps differ) |
| CLI (`I`, `BPS 5`, invalid type) | Console output identical |
| Sample BPS and XB statements through XSLT, template, HTML and PDF | Word `document.xml` byte-identical; HTML identical except image-size number format; BPS and XB PDF text identical (pages and extracted text) with wkhtmltopdf pinned to 0.12.4 |
| FTP download through WinSCP (control file, data file, remove after download) | Files and hashes identical; remote cleanup identical |

Open points for review:

- Decided: wkhtmltopdf is pinned to 0.12.4 (applied above). Without pinning,
  NReco 1.2.1's bundled 0.12.6 changed the XB PDF text.
- Accepted and documented (no action): two default constraints on `SITI.GRPEN`
  (`CropDiversificationNumberAdditional`, `EFANumberAdditional`) are not created
  on a database built from scratch by EF 6.5.2, because its `AlterColumn` drops
  the default and does not recreate it. Existing databases already have their
  migration history and are unaffected. Confirmed against the real schema:
  see "Real database schema comparison" in the Phase 6 results.
- Not verified here (tracked in later tasks): generation against real-shaped
  SITI/XB inputs (only the sample-statement pipeline and AP/AR imports were
  fixtured), SITI/XB imports, the IIS path, and the Generator test suite
  (task 004).
- Build warnings: `System.Security.Cryptography.Pkcs` 9.0.18 (via EPPlus) has
  known vulnerabilities (task 005); two NU1701 compatibility-mode warnings;
  one obsolete serialization constructor (SYSLIB0051).

### Phase 3: ASP.NET Core MVC and Identity Migration

- [x] **003-transform-web-net10**: Convert
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

Carry-overs from Phase 2: set `HostEnvironment.IsWebHosted = true` at startup;
the Generator's configuration (connection strings, `entityFramework`, SMTP) is
read by `ConfigurationManager` from the host's `<assembly>.dll.config`, so the
web project needs an `App.config` that declares the `system.net` section as the
Generator's does; the Generator's `WinSCP.exe` and NReco's wkhtmltopdf are
copied or extracted into the host's output.

#### Phase 3 design decisions and findings (recorded before implementation)

- **Role semantics verified (net48 probe)**: legacy `RolePrincipal.IsInRole`
  calls only `RoleProvider.GetRolesForUser(Identity.Name)` (once per principal,
  case-insensitive match, no Windows-group lookup). `IsUserInRole` and
  `CreateRole` are never called by the application, so the legacy app performs
  **no role writes**. The new role lookup therefore implements only
  `GetRolesForUser` (via `aspnet_UsersInRoles_GetRolesForUser`, application
  `/`); decision 4 ("keep the swallowed create-role call") is moot and no
  database write is added.
- **Deployed configuration**: deployment copies show `compilation debug="true"`
  in every environment, so bundling/CDN script substitution was never active;
  the layout loads the local, unminified scripts and styles individually.
  Globalization is fixed `en-GB`; `customErrors RemoteOnly` maps to a global
  exception filter rendering `Shared/Error` for non-local requests.
- **Behaviour kept**: PascalCase JSON, `searchData` pager-link quirk, the dead
  `Home/ForceGeneration` URL in the Error view, CSV download encodings/headers,
  existing absence of antiforgery validation on the AJAX POST actions.
- **Missing source (resolved in Phase 6)**: `Views/Log/Index.cshtml` was not in
  git (ignored by the `[Ll]og/` rule), not in any deployment copy and not in
  history. The `LogController` was ported; the view was supplied later (see
  the Phase 6 section "Log view").
- **Dependency injection**: the existing `UnityConfig.RegisterTypes` is kept and
  plugged into the ASP.NET Core host via `Unity.Microsoft.DependencyInjection`.
- **Behaviour change**: authenticated-but-unauthorized requests now return 403
  (legacy MVC 5 returned 401, which makes IIS re-prompt for credentials).

#### Phase 3 result

- **Built**: `RPA.ClaimStatements.Web` is SDK-style `net10.0`
  (`Microsoft.NET.Sdk.Web`), all six configurations build; `Program.cs`
  replaces `Global.asax`/`App_Start`; static assets moved to `wwwroot/` with
  `git mv` (same URLs); `Web.config` became `App.config` plus SlowCheetah
  `App.<Configuration>.config` transforms (SIT/UAT/Production data sources and
  SMTP verified in each output `RPA.ClaimStatements.Web.dll.config`);
  `IDT.dll`, `packages.config`, bundles and the stale copied XSLT files were
  removed; templates/XSLT are linked from the Generator; `WinSCP.exe` and
  `wkhtmltopdf/` reach the Web output.
- **Code**: controllers/services ported (`IFormFile`, file results with the
  legacy headers/encodings); views ported (no `RenderAction`: the suppression
  page passes the existing-suppressions model to the partial);
  `Security/` holds the `GetRolesForUser` store, a `ClaimsPrincipal` that
  mirrors `RolePrincipal`, and the claims transformation; `HandleErrorFilter`;
  X.PagedList with the legacy pager labels. `HostEnvironment.IsWebHosted` is
  set and `APP_CONFIG_FILE` is pinned to `RPA.ClaimStatements.Web.dll.config`.
- **Validated side by side** (legacy MVC 5 app with the real `IDT.dll` under
  IIS Express vs the new app on Kestrel, same seeded LocalDB data, Windows
  authentication, Security database copy):
  all GET routes (HTML equal apart from the differences below; CSV bodies
  byte-equal; BPS and XB sample statements identical `document.xml`);
  POST/state actions (Trigger with antiforgery, Import, suppression create,
  remove and bulk add/remove from `.xlsx`, error resolve, configuration and
  scheme-year saves) leave the same database state;
  160 route/role combinations over 10 role sets classified identically, plus
  role inheritance, case-insensitive role names, unknown user and missing
  application.
- **Known differences (accepted, reported)**: authenticated-but-denied returns
  403 not 401; missing antiforgery token returns 400 not 500; a malformed `id`
  GUID binds to `Guid.Empty` (no-op) and `Log/Download` without `count` returns
  an empty CSV, where legacy returned 500; `Content("")` responses carry
  `text/plain`; downloads add `filename*`; `id` attributes on indexed inputs,
  antiforgery hidden field position and checkbox fallback hidden fields moved
  (still posted); client `data-val-number` messages are re-added explicitly;
  unknown URLs return a plain 404 instead of the legacy `customErrors`
  redirect to `/Error`; exceptions thrown while rendering a view (as opposed
  to in an action) are not turned into the `Error` page for remote users.
- **Not verified**: IIS-hosted (in-process) behaviour, including the server-type
  detection that selects IIS Windows authentication, the `APP_CONFIG_FILE` pin
  and SQL access under the app pool identity; data protection key storage under
  an app pool without a profile (antiforgery/TempData); the `Error` page for
  genuinely remote requests (exercised once with the local check forced off);
  `Views/Log/Index.cshtml` (missing at the time, so `/Log` returned 500; see
  the Phase 6 section "Log view"); the real Security database (the local copy needed `aspnet_SchemaVersions`
  rows for the legacy provider; the new lookup does not check them).
- **Open items**: `Views/Log/Index.cshtml` was missing (resolved in Phase 6).
  The Generator's `App.config`/`App.*.config` flow into the Web
  output and publish folder through the project reference (the legacy Web
  build did the same); Web.Tests conversion is task 004 and will need the
  `IFormFile` and `IActionResult` signature changes; README, .NET 10 Hosting
  Bundle and IIS settings (Windows authentication on, anonymous off) are
  task 006.

### Phase 4: Test Projects and Build Integration

- [x] **004-transform-tests-net10**: Convert
  `RPA.ClaimStatements/RPA.ClaimStatements.Generator.Tests/RPA.ClaimStatements.Generator.Tests.csproj`
  and
  `RPA.ClaimStatements/RPA.ClaimStatements.Web.Tests/RPA.ClaimStatements.Web.Tests.csproj`
  to SDK-style `net10.0`. Update test dependencies and adapt existing controller,
  HTTP-context and EF test doubles without weakening assertions or excluding
  tests to hide failures. Integrate all four projects and build configurations
  in `RPA.ClaimStatements/RPA.ClaimStatements.sln`.

#### Phase 4 result

- **Projects**: both test projects are SDK-style `net10.0` with the six
  configurations; `packages.config` and the two `App.config` files (legacy
  binding redirects only) were removed. Packages: NUnit 3.14.0 (kept on 3.x so
  the classic `Assert.AreEqual` assertions are unchanged), NUnit3TestAdapter
  5.0.0, Microsoft.NET.Test.Sdk 17.14.1, Moq 4.20.72,
  EntityFrameworkTesting.Moq 1.3.0 (Generator.Tests only; it works against
  EF 6.5.2 unchanged). The five orphan Generator test files and
  `ARControllerTests.cs` stay out of the build, untouched.
- **Generator.Tests**: no source changes needed; 123 of 123 pass.
- **Web.Tests**: only test doubles/result types adapted to ASP.NET Core
  (`RedirectToActionResult.ActionName` for `RedirectToRouteResult.RouteValues["action"]`,
  `ViewName` is `null` instead of `""` for the default view, and a
  `TempDataDictionary` is assigned because Core does not create one without an
  HTTP context). No assertion was removed or loosened; 8 of 8 pass.
- **Parity**: the 131 executed test names are identical to the legacy
  baseline in `baseline-tests.txt` (0 failed, 0 skipped).
- **Solution**: all four projects build for Debug, Release, SIT, UAT and
  Production through `RPA.ClaimStatements.sln` (0 errors; the only warnings
  are NU1701 for OpenXmlPowerTools and System.Management.Automation.dll).
  `dotnet test` on the solution in Release runs both suites green. The
  solution file itself needed no change; `Test` remains a project-level
  configuration only, as before.
- **Not covered**: the Web tests do not exercise the new `Security/`,
  `HandleErrorFilter` or `Program.cs` wiring (no equivalent legacy tests
  existed; Phase 3 validated them by side-by-side runs). Adding tests for
  them was not requested.

### Phase 5: Security/CVE Hygiene

- [x] **005-security-cve-remediation**: Scan the resulting .NET dependency graph,
  including transitive dependencies; apply compatible patched versions and
  rerun builds and tests. Retain the original plan's security scope. Document
  unresolved findings and block sign-off rather than reporting a clean result.

#### Phase 5 result

**Original findings (assessment `Report_20261005164238`, severity HIGH, scope direct):**

| Finding | Was | Now |
|---|---|---|
| CVE-2021-21252 jquery-validation ReDoS | `jQuery.Validation` 1.11.1 (packages.config), vendored as `Scripts/jquery.validate*.js` | **Fixed**: the vendored files are jquery-validation **1.21.0** (taken from the signed NuGet package); retire.js reports no finding for it (1.11.1 had four: CVE-2021-21252, CVE-2021-43306, CVE-2022-31147, CVE-2025-3573). |
| CVE-2024-21907 Newtonsoft.Json | 6.0.4 in Web.Tests | **Gone**: no project references it directly; the test SDK brings 13.0.3 (patched). |

- **Client validation behaviour checked**: the real create-suppression form
  markup with the old 1.11.1 and the new 1.21.0 library, in a browser, for
  five inputs (empty, non-numeric, below range, bad year, valid): identical
  validity and identical messages.
- **NuGet graph** (all four projects, direct and transitive, all severities):
  `dotnet list package --vulnerable --include-transitive` reports nothing and
  no NU1901-NU1904 warnings appear; the scan was confirmed to work by a control
  project that flags Newtonsoft.Json 6.0.4 (NU1903). The Phase 2 note about
  `System.Security.Cryptography.Pkcs` 9.0.18 no longer applies: it now
  resolves to 4.7.0 and has no advisory. Deprecated packages: only EPPlus
  4.5.3.3 ("Legacy").

**Unresolved / not fixable inside this upgrade (block unqualified sign-off):**

- **jQuery 1.10.2** (vendored): retire.js reports medium CVE-2015-9251,
  CVE-2019-11358, CVE-2020-11023, a parseHTML issue and end-of-life (jQuery
  1.x/2.x). Below the original HIGH threshold, so not changed; moving to
  jQuery 3.x touches every page script and the Bootstrap 3 plugins and is a
  frontend change the plan excludes. **Needs a decision.**
- **Bootstrap 3.0.0** (CSS and JS, vendored): not detected by retire.js, but
  Bootstrap 3 releases before 3.4.1 have published medium XSS advisories
  (for example CVE-2019-8331); this statement is from the advisories, not from
  a scanner run. Same decision as jQuery.
- **Other vendored scripts** (modernizr 2.6.2, respond.js,
  jquery.validate.unobtrusive, jquery.unobtrusive-ajax): no scanner finding.
- **wkhtmltopdf 0.12.4** (native binaries in `Generator/wkhtmltopdf`, pinned in
  Phase 2): all releases, including 0.12.6, are affected by CVE-2022-35583
  (SSRF when rendering attacker-influenced HTML); the project is archived, so
  there is no patched version. The Generator renders HTML produced by its own
  XSLT from imported data and passes no `--disable-*` options. Not changed;
  hardening options would change rendering behaviour and need approval.
- **EPPlus 4.5.3.3**: deprecated, no advisory. Later major versions use a
  commercial/non-commercial licence, so it stays (Phase 1 decision to keep
  the current licensing model).
- **Unmaintained, no advisory**: OpenXmlPowerTools 4.5.3.2 (NU1701, .NET
  Framework only, runtime-tested in Phase 2), DocumentFormat.OpenXml 2.7.2,
  CsvHelper 2.16.3.
- **Original CWE findings** (13 code-quality/concurrency/upload/XSS records)
  are code findings, not dependency vulnerabilities, and were not changed in
  this task. For CWE-434 (bulk upload of an arbitrary file type): the uploaded
  file is saved under `Uploads/` in the content root, which ASP.NET Core does
  not serve (not under `wwwroot`), so the legacy IIS handler-execution risk no
  longer applies; the file-type check itself is unchanged.
- **Scope of the scan**: NuGet advisories and retire.js only; native binaries
  (WinSCP.exe 6.5.7 from the NuGet package, wkhtmltopdf) were assessed by
  version, not by an independent scanner.

#### Open decisions for the application owner (not blocking, Phase 6 on hold)

These three findings are known, documented and deliberately unchanged. They need
an owner decision (fix in this upgrade, schedule separately, or accept the risk)
before final acceptance. Exposure statements below come from reading the views
and stylesheets, not from exploit testing.

| Finding | Why it is not fixed here | Implication if left as is | Options |
|---|---|---|---|
| **jQuery 1.10.2**: CVE-2015-9251 (cross-domain script evaluation), CVE-2019-11358 (prototype pollution via `$.extend(true, ...)`), CVE-2020-11023 (HTML with `<option>` from untrusted sources run by `.html()`/`.append()`), a `parseHTML` script issue; all medium. jQuery 1.x is end-of-life, so no further fixes will ever exist. | Below the original HIGH threshold; a move to 3.x changes every page script and may need jquery-migrate; the plan excludes frontend redesign. | Exposure looks low: the views only make same-origin calls (`$.ajax`, `.load`), never cross-domain or `dataType: script`; they do not use `$.extend` on untrusted data; the only `.html()` calls insert server-rendered partials from the same application or a fixed label (plus the user's own selected file name in the bulk-upload view, a self-inflicted case). Users are authenticated staff. The residual risk is that the library is unsupported, will keep appearing in security scans and audits, and any future page that handles untrusted HTML would inherit these flaws. | (a) Accept and record. (b) Upgrade to jQuery 3.7.x with jquery-migrate, then re-test every page (Home, Suppression incl. bulk, Errors, Configuration) against the Phase 3 side-by-side harness. Plugin versions (validate, unobtrusive) already work with jQuery 3. |
| **Bootstrap 3.0.0** (CSS and JS): Bootstrap 3 releases before 3.4.1 carry medium XSS advisories in the tooltip/popover/collapse/scrollspy data-attribute handling (for example CVE-2018-14040, -14041, -14042, CVE-2019-8331). Not reported by retire.js; stated from the published advisories. Bootstrap 3 itself is end-of-life. | Same reason as jQuery; 3.4.1 is a drop-in patch release but still a vendored-asset change in a frontend the plan excludes. | Exposure looks low: the only plugins used are the navbar collapse (`data-toggle`/`data-target` with fixed values) and the button loading text; there are no tooltips, popovers, scrollspy or user-controlled data attributes. The risk is again support status and scanner noise. | (a) Accept and record. (b) Replace with Bootstrap 3.4.1 (same major, same class names, expected visual parity) and re-check the pages; (c) a Bootstrap 4/5 move is a redesign and out of scope. |
| **wkhtmltopdf 0.12.4** (native, pinned in `Generator/wkhtmltopdf` in Phase 2 to keep PDFs identical to the legacy output): CVE-2022-35583, SSRF (reading internal URLs or local files) when the HTML it renders contains attacker-controlled markup. Every release including 0.12.6 is affected, the project is archived and no patched version exists. | There is nothing to upgrade to; the only mitigations are rendering options or replacing the PDF engine, both of which change the PDF output whose extracted text Phase 2 matched to the legacy output. | Impact depends on whether imported claim data can inject markup into the HTML that is rendered. The HTML comes from the Word template and claim data through OpenXmlPowerTools; the XSLT stylesheets use no `disable-output-escaping` and emit no `img`/`link`/`iframe`/`script` elements, which suggests text values are escaped, but the generated HTML was not inspected for injection. The generator runs under a service account on the application server (IIS app pool or console), so a successful SSRF could reach whatever that identity and server can reach, including internal HTTP endpoints. Input arrives from upstream finance feeds (DAX, SITI, XB), not from end users of the web UI. | (a) Accept with a compensating control (run the generator with least privilege and no route to sensitive internal endpoints). (b) Trial `--disable-local-file-access` or `--disable-javascript` style options and re-run the BPS/XB PDF comparison from Phase 2; if the output differs, the owner decides. (c) Replace the engine (for example a Chromium-based renderer): a separate project with new visual-parity testing. |

**Needed from the owner:** one decision per row (accept, fix now, or schedule),
and, for wkhtmltopdf, whether option (b) may be trialled against the PDF
baseline. Task 006 (acceptance, README, publish) is not started and will
carry whatever is decided; if all three are accepted, the README and the final
acceptance report must list them as accepted risks rather than as a clean
security result.

### Phase 6: Acceptance and Upgrade Documentation

- [x] **006-transform-net10-acceptance**: Restore, build, test and publish the
  migrated solution with .NET 10. Start the web host in the Windows validation
  environment and exercise the assessed operator flows. Verify CLI modes and
  imports/generation against isolated fixtures and compare BPS/XB statement
  output with the baseline. Update `README.md` with prerequisites,
  configuration, build/test/publish and IIS/console operation instructions.

Use an isolated database, test identities, controlled inbound/outbound folders
and test SMTP/FTP destinations. Do not run destructive UAT reset instructions
or generation/import flows against live systems. Existing tests and smoke/
behavioural checks are required; this plan does not mandate a new test suite.

#### Phase 6 result

All evidence was produced against isolated LocalDB databases, local folders,
a local FTP server and an SMTP pickup folder; nothing touched a live system.
The legacy side is the original .NET Framework 4.5 build (Generator console
and MVC 5 web with the real `IDT.dll`); the new side is the Release publish.

**1. Restore, build, test, publish (.NET SDK 10.0.401)**

- `dotnet restore`, `dotnet build -c Release` (0 errors; only NU1701 warnings
  for OpenXmlPowerTools and System.Management.Automation.dll), `dotnet test -c
  Release` (Web.Tests 8/8, Generator.Tests 123/123) and `dotnet publish` of the
  Generator (104 files) and the Web application (189 files) all succeed.
- A copy of exactly the files git would commit (tracked plus untracked, minus
  ignored) was built, tested and published in a scratch folder: it succeeds
  once the git-ignored Word templates are supplied; without them the build
  stops with MSB3030 for `ClaimTemplate.docx` and `ClaimTemplateXB.docx`
  (documented in the README; the legacy build needed them too).

**2. Generator, end to end on the same fixtures (legacy exe vs new exe)**

Fixtures: a SITI pipe-delimited file (4 claims) fetched over FTP with a
control file through WinSCP, an XB CSV, a DAX AP CSV and a DAX AR workbook,
plus seeded configuration, folders, rates, scheme years and a suppression.
Evidence compared per run: console output, stderr, files in every inbound,
archive, outbound and FTP folder, a database dump (AP, AR, SITI, XB, Log,
Errors, Monitor), the e-mail report (decoded, timestamps and durations
normalised) and the extracted text and page count of every PDF (pdf-parse,
the library `PdfCompare` uses).

| Run | Mode | Result |
|---|---|---|
| a1 | `ALL 10`, validation off: 4 imports, BPS and XB generation, 2 PDFs, archive, report | identical in every item; PDFs 4 and 9 pages, text identical |
| a2 | `ALL 10`, validation on: reconciliation failures logged instead of PDFs | identical (errors, report) |
| b3 / b4 | `BPS 5` / `XB 5` (working folders present) | identical, PDF text identical |
| a5 | `I` (imports only) | identical |
| a6 / a7 | invalid statement type / invalid batch size | identical message, nothing imported |
| a8 / a9 | interactive input (`bogus`, `ALL`, `3` / `I`) | identical, PDF text identical |

- Web-triggered generation: the Release web publish, with the same database,
  ran `Trigger` (ALL, 10) in-process: both PDFs and the report were produced
  in the web host's folders with native `wkhtmltopdf` and `WinSCP`; PDF text is
  identical to the legacy console output.
- This closes the Phase 2 gaps for SITI import, XB import and real-shaped
  generation.

**3. Web application, Release publish on Kestrel (Windows authentication)**

- All 22 actions re-run against the saved legacy captures: GET pages equal
  apart from the Phase 3 cosmetic differences, CSV bodies equal, BPS/XB sample
  `document.xml` identical; the state-changing action results classify as in
  Phase 3. Anonymous gets 401, a user without roles 403, a view-only user is
  allowed and denied as expected, an all-roles user is allowed.
- `dotnet run` from the project folder starts the host.

**Differences found in this phase (none changes results)**

- The text of an `AggregateException` now includes the inner message, so the
  error logged for a failed transform reads `... One or more errors occurred.
  (Failed to transform statement - Could not find a part of the path ...)`
  instead of ending after `One or more errors occurred.`.
- Pre-existing, unchanged: with `BPS` or `XB` the working folders
  (`XML`, `DOCX`, `HTML`, `PDF`) must already exist (only `ALL` creates them);
  EF6 creates an empty database when the configured one is missing; the SITI
  inbound path needs a trailing backslash.
- The Generator's working-folder root is still two levels above the binaries,
  so a published Generator must be deployed as `<root>\bin\<Configuration>\`
  with `Templates` and `XSLT` in `<root>` (README updated).

**Limits of this evidence**

- Data is fabricated: values are small sequential numbers, so statements do
  not reconcile (validation-on shows the failure path only) and the Word
  templates in use are the locally supplied ones, not production templates.
  PDF comparison is extracted text plus page count, not pixel comparison.
- The FTP server is a local test server; SMTP delivery was checked through a
  pickup folder only; AR workbooks, SITI files and XB files come from the
  application's own readers' formats, not from production feeds.
- IIS in-process hosting, IIS Windows authentication, application pool
  identity and data protection key storage remain unverified; the real
  `Security` database was not available (a schema-only copy was used).

**Real database schema comparison**

The schema-only script of the real `RPA.ClaimStatements` database (scripted
2026-09-15, no data, no indexes) was loaded into LocalDB and its structure
compared with databases built by the migrations (legacy EF 6.1.3 and new EF
6.5.2; both give the same result except for the GRPEN defaults).

| Area | Result |
|---|---|
| Tables and columns in the model | All 27 tables of the migration-built databases (including `__MigrationHistory`) and 670 columns match in type, length, precision, scale, nullability and identity |
| Column only in the real database | `SITI.SUM.PAFOrganisationName nvarchar(max) NULL`; not in the model or migrations (nullable, EF ignores it) |
| Objects only in the real database | `dbo.temp`, `FDMR.ExtractTest`, `FullExtract`, `PaidCurrency`, `SchemeYear`, `Stage_Net4`, `Stage_Sums`, view `FDMR.ExtractView`, procedures `FDMR.LoadExtract`, `PREPRODLoadExtract`, `Single_FRN_Investigation`, `Single_FRN_Investigation_FAST`, schemas `SC` and `YCP`; none used by the application code |
| Default constraints | Real database has 500 (almost every non-null numeric column, defined `((0))`; a few empty-GUID/1900-01-01), migration-built databases 12 (EF 6.5.2) or 14 (EF 6.1.3). The two `SITI.GRPEN` defaults are present in the real database, so the earlier note is confirmed: only a database built from scratch by EF 6.5.2 lacks them. The application writes every column, so the extra defaults do not change behaviour |
| Foreign keys | 13 of 14 match. The real database has `FK_SITI.YF_SITI.Claims_YFID` (`SITI.YF.YFID` -> `SITI.Claims.ClaimID`) instead of the model's `FK_SITI.Claims_SITI.YF_YFID` (`SITI.Claims.YFID` -> `SITI.YF.YFID`). Legacy and new builds share the model, so behaviour is unchanged; insert order of `YF` and `Claims` must be confirmed when running against this database |
| Primary key names | `PK_SC.Configuration` and `PK_CS.Directories` in the real database (old names from before a rename); cosmetic |
| Indexes | Not comparable: the script was generated without indexes. The 14 `IX_*` indexes of the model are therefore reported missing only because of the script options |
| `__MigrationHistory` | Table exists but no rows were supplied; the 10 expected migrations (`201802271513336_Initial` to `202108040919024_addPRTable`) cannot be confirmed from the script |

A database created from this script cannot be used by the Generator as is:
`DbMigrator.Update()` finds an empty history and tries to apply
`201802271513336_Initial` on existing tables. For a local run the history rows
must be loaded (from the real database, or copied from a migration-built one).

**Log view (`Views/Log/Index.cshtml`)**

The owner supplied the previously missing view. It is the original MVC 5
view: it matches `LogController` (model `PagedList<Log>`, `ViewBag.searchData`,
`ViewBag.uploadActive`, `Log/Download`), the `Log` entity fields and the
existing role names. Ported like the other views: `X.PagedList` usings and
`PagerOptions.Legacy`; nothing else changed.

- Verified on the Release publish (Kestrel, Windows authentication, seeded
  LocalDB, 25 log rows): `/Log` returns 200 with 15 rows and View links, the
  legacy pager and "Showing 1 to 15 of 25 Claim Statements"; search by FRN
  returns 1 row; page 2 shows 16 to 25; the manual-upload and download forms
  render for a user with the matching roles; `Log/Download` returns the CSV
  with the legacy headers. Not compared against a legacy render (the legacy
  app has no view to compare with), and the "Manual Claim Statement uploaded"
  row was not exercised (no seeded row has `Uploaded` set).
- **Finding**: the manual-upload form posts to `Home/Upload`, but no such
  action exists in the source (`HomeController`, any other controller or the
  git history). The form is shown only when the setting `Manual Upload` is
  `Active` and the user has the `Manual Upload` role, and submitting it would
  return 404. The view appears to come from a newer version than this source.
  Needs an owner decision (see the findings file).
- `.gitignore` keeps the `[Ll]og/` rule and adds an exception for
  `RPA.ClaimStatements.Web/Views/Log/`, so the view is tracked.

**Sign-off status**

Not signed off. Open: (1) the owner decisions on jQuery 1.10.2, Bootstrap 3.0.0
and wkhtmltopdf 0.12.4; (2) the IIS verification above; (3) the missing
`Home/Upload` action behind the Log page's manual-upload form. The README now
documents prerequisites, configuration, build/test/publish, console and IIS
operation, and lists the open items as known limitations and accepted risks.

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

- `IDT.dll` is an internal assembly that cannot load on .NET 10. Its behaviour
  is understood and verified (see Phase 1 outcome); the replacement is a
  reimplementation against the `Security` database procedures.
- EF6 migrations, document/PDF libraries, WinSCP and Windows-specific APIs were
  probed in Phase 1; runtime behaviour on .NET 10 is verified task by task.
- Windows baseline tooling and the .NET 10 SDK are available locally; IIS and
  the Hosting Bundle are not, so IIS-hosted validation remains outstanding.
- Phase 1 was investigation only; no source changes were made before Phase 2.
