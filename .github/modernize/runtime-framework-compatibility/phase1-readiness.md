# Phase 1 Findings: Readiness, Baseline and Compatibility (task 001)

Captured 2026-10-13. No repository source files were modified in Phase 1; builds ran
in a scratch copy under `%TEMP%\csg-baseline`.

## Environment

| Item | Result |
|---|---|
| .NET SDK | 10.0.302 and 10.0.401 installed (also 6, 7, 8, 9); runtimes 10.0.10 and 10.0.12 |
| Legacy tooling | VS 2022 Professional MSBuild and vstest available; no .NET 4.5 targeting pack installed (used `Microsoft.NETFramework.ReferenceAssemblies.net45` via `FrameworkPathOverride`) |
| NuGet | nuget.org reachable |
| SQL | `MSSQLLocalDB` available; no SQL Express, no `Security` database |
| IIS / Hosting Bundle | IIS (`W3SVC`) not installed, so no ASP.NET Core Module here |
| Node | Present (needed for `PdfCompare`) |

## Legacy baseline (.NET Framework 4.5, Release)

- Restore and build of all four projects succeed (2 warnings, both MSB3277 assembly-conflict).
- Placeholder empty `ClaimTemplate.docx` and `ClaimTemplateXB.docx` were required in the scratch copy (see blockers).
- Tests: Generator.Tests 123 passed, Web.Tests 8 passed, 0 failed/skipped.
  Names and outcomes are in [baseline-tests.txt](baseline-tests.txt).
- Existing NuGet advisories (for Phase 5): bootstrap 3.0.0, jQuery 1.10.2, jQuery.Validation 1.11.1, Newtonsoft.Json 6.0.4.

## Findings that change the plan

1. **Orphan source files.** Legacy csproj files list `Compile` items explicitly. These are on disk but not compiled:
   - Generator: 36 files in `Imports/` (all) and `Services/` (`BuildService`, `EmailService`, `IBuildService`, `IEmailService`, `IImportService`, `ILogService`, `ImportService`, `ITransformService`, `IValidationService`, `LogService`, `TransformService`, `ValidationService`). Live equivalents are under `Components/`.
   - Generator.Tests: `Services/{Generation,Import,Load,Log,Validation}ServiceTests.cs`.
   - Web: `Controllers/ARController.cs`; Web.Tests: `Controllers/ARControllerTests.cs`.
   SDK-style globbing would compile them (duplicate types, changed test counts). Keep the compiled set identical (123 + 8 tests).
   The assessment's "23 actions" includes `ARController.Import`; only 22 actions are compiled today.
2. **Word templates are not in the repo.** `*.docx` is git-ignored (`.gitignore` line 292); `ClaimTemplate.docx` and `ClaimTemplateXB.docx` are missing, and Web's build copies them as linked content.
3. **Role provider contract (`IDT.dll`).** The tracked copy (`Web/Assemblies/IDT.dll`, 11 KB, v1.0.0.0, built against .NET 4) is only referenced from `Web.config`; no C# uses it.
   `IDT.Web.Security.RoleProvider` derives from `System.Web.Security.SqlRoleProvider` (aspnet_* schema in the `Security` DB via `IDTSecurityConnection`, `applicationName="/"`).
   - `IsUserInRole`: first tries `CreateRole(roleName)` (errors swallowed), then base behaviour.
   - `GetRolesForUser`: effectively base behaviour.
   - Extras (`AddRolesToRoles`/`RemoveRolesFromRoles`, stored procs `dbo.aspnet_ChildRolesInParentRoles_*`) are not called by the web app.
   `System.Web` does not exist on .NET 10, so this assembly cannot load. With `roleManager enabled`, `User.IsInRole` and `[Authorize(Roles=...)]` use only this provider, not Windows groups.
   Roles used: Trigger Generation, View Claim Statements, Download Data, Manage Exceptions, Manage Suppressions, Manual Upload, Force Generation.
4. **Web-vs-console detection.** `HttpRuntime.AppDomainAppId` (`System.Web`) in `Services/FolderService.cs` (the compiled one; the other occurrence is in the uncompiled `Imports/FTPService.cs`) selects the base folder; needs a host-neutral replacement (done in Phase 2). `Using System.Web`/`System.Web.UI` directives appear throughout Generator models (unused; `System.Web` still resolves to `HttpUtility` on .NET).
5. **The web host runs the generator in-process.** `HomeController.Trigger` calls `IPowerPlant.Activate`, so PDF generation, wkhtmltopdf extraction and file/mutex permissions run under the web app's process identity.
6. **Startup DB migration.** `PowerPlant` runs EF6 `DbMigrator` with `Migrations.Configuration`; 10 migrations with `.resx` model snapshots must stay embedded with identical resource names.
7. **`SmtpClient` and `system.net/mailSettings`.** The code uses `new SmtpClient()` with `SpecifiedPickupDirectory` taken from config; .NET (Core) ignores `mailSettings`, so delivery settings must be set explicitly.
8. **`System.Management.Automation.dll` is not used by code**; it is a transitive dependency of OpenXmlPowerTools 4.5.3.2 and can be dropped.
9. **Bundled binaries tracked in git:** `Generator/WinSCP.exe` (5.9.6, content copy) and `Web/App_Data/wkhtmltopdf/*` (extracted by NReco).

## Package compatibility (probe: throwaway `net10.0` project, restore only)

| Package (current) | net10.0 candidate | Notes |
|---|---|---|
| EntityFramework 6.1.3 | 6.5.2 (netstandard2.1) | Keep EF6 and migrations; verify runtime `DbMigrator`/config on .NET 10 in Phase 2. SQL provider uses Microsoft.Data.SqlClient. |
| CsvHelper 2.16.3 | 33.1.0 | Large API break; the serializers/maps need porting. Alternative: none lower-risk. |
| DocumentFormat.OpenXml 2.7.2 | 3.x (or stay 2.x) | OpenXmlPowerTools 4.5.3.2 expects 2.x. |
| OpenXmlPowerTools 4.5.3.2 | restores only as .NET Framework (NU1701) | Option A: keep with System.Drawing.Common and test at runtime. Option B: `Clippit` 3.9.1 (maintained fork) but its `ImageInfo` uses `SKBitmap` (SkiaSharp), so the `ImageHandler` in `Components/Turbine/TransformService.cs` must be rewritten. **Decided: Option A (see Decisions).** |
| EPPlus 4.1.0 | 4.5.3.3 (netstandard) | Last LGPL line; 5+ is a commercial license, so avoid. |
| FastMember 1.1.0 | 1.5.0 | OK. |
| NBuilder 4.0.0 | 6.1.0 | OK (sample data services). |
| NReco.PdfGenerator 1.1.15 | 1.2.1 (netstandard2.0) | Embeds wkhtmltopdf (Windows only); version moves to 0.12.6, so PDF rendering may differ. **Decided: upgrade (decision 7); license check pending.** |
| Unity 5.5.6 | 5.11.10 | OK. `Unity.Mvc` is dropped. |
| WinSCP 5.9.6 | 6.5.7 (netstandard2.0) | `WinSCPnet.dll` and `WinSCP.exe` must match versions; replace the tracked exe. FTP protocol only. **Decided: upgrade (decision 7).** |
| System.Drawing | System.Drawing.Common 10.x | Windows-only API, which fits the Windows target. |
| ConfigurationManager (`System.Configuration`) | System.Configuration.ConfigurationManager 10.x | Needed for `ConnectionStrings` and EF6 config section. |
| Web: PagedList / PagedList.Mvc | X.PagedList.Mvc.Core 10.6.0 | Helper API differs. |
| Web: Unity.Mvc, WebActivatorEx, System.Web.Optimization/WebGrease, Antlr, Modernizr, Respond | none | Replace with ASP.NET Core DI, static files and a bundling/minification approach. |
| Tests: NUnit 3.6 / Adapter 3.10 / Moq 4.5.30 / Castle 3.3.3 / EntityFrameworkTesting.Moq 1.2.1 | NUnit 3.x or 4.x, Adapter 6.x, Moq 4.2x, EntityFrameworkTesting.Moq 1.3.0 | `Microsoft.VisualStudio.QualityTools.UnitTestFramework` references are replaced by the NUnit/test SDK packages. |

## Decisions (recorded 2026-10-14) - Phase 2 stays on hold until the user releases it

| # | Topic | Status | Decision / note |
|---|---|---|---|
| 1 | Word templates | Answered | `ClaimTemplate.docx` and `ClaimTemplateXB.docx` are in `Generator/Templates/` (valid Word packages, 78,880 and 146,769 bytes). They are git-ignored (`*.docx`), so they exist only on this machine and are not committed. |
| 2 | PDF baseline data | Answered | Yes: fabricated fixtures in LocalDB. Baseline PDFs are generated from the legacy build and stored outside the repo. |
| 3 | Role provider | Answered | Reimplement the current `IDT.Web.Security.RoleProvider` behaviour (original source is no longer available; only the DLL) against the `Security` database `aspnet_*` schema. |
| 4 | `CreateRole` auto-create in `IsUserInRole` | Answered | Keep it, as in the current behaviour. |
| 5 | Test `Security` database | Answered | A schema-only script of the real `Security` database was provided and verified (see below). A local copy named `Security` was built in LocalDB from it for testing. Open gaps are listed below; they do not block the decision. |
| 6 | OpenXmlPowerTools (keep 4.5.3.2 vs Clippit) | Answered | Option A: keep OpenXmlPowerTools 4.5.3.2 for now; revisit a switch to Clippit later if necessary (separate change with its own PDF comparison). |
| 7 | NReco 1.1.15 to 1.2.1 and WinSCP 5.9.6 to 6.5.7 | Answered | Option 1: upgrade both. NReco.PdfGenerator license position still has to be checked by the user; not a blocker for now (tracked as a follow-up). |
| 8 | Uncompiled files and `ARController` | Answered | Exclude them from the build, leave them in git, do not modernize or modify them. `ARController` stays out, so 22 actions are live. |
| 9 | IIS validation approach | Answered | Use Kestrel for local validation for now. Getting IIS Express on the VDI does not look promising, and this is not a blocker for current work. IIS-hosted verification is deferred (see implications). |

Implications of the answers so far:
- The Web project's build copy of the templates and the baseline run are no longer blocked by missing files.
- Because the templates are git-ignored, any fresh clone (and CI) still lacks them; this should be documented in the README in task 006.
- Decisions 3 and 4 mean the replacement role lookup performs a role-create write against the `Security` database, so the account used needs write permission there.
- Decision 6 (Option A): `OpenXmlPowerTools` 4.5.3.2 stays, restored on net10.0 through .NET Framework compatibility mode (NU1701), with `System.Drawing.Common` for the image handler and its unused `System.Management.Automation.dll` dependency excluded. The `ImageHandler` and `WmlToHtmlConverter` code in `Components/Turbine/TransformService.cs` should need no rewrite.

Evidence for decision 6 (probe on the real templates, converting Word to HTML; throwaway projects in `%TEMP%`, not in the repo):
- Option A on .NET 10 ran correctly on both templates. Its HTML matches the legacy .NET Framework 4.8 output except for image size formatting (`1.770833in` vs `1.7708334in`). The embedded logo is byte-identical to legacy.
- Option B (Clippit 3.9.1) ran but differed more: 373 table-cell and 54 paragraph CSS class names, 46 `font-family` declarations (`'sans-serif'` vs `sans-serif`), and a re-encoded logo.
- Not verified: the final PDFs (NReco/wkhtmltopdf not yet run on these HTML files), and that `PdfCompare` hashes extracted PDF text only, so purely visual differences would not be caught by it.
- Risks accepted with Option A: unmaintained package running in compatibility mode, and it may be flagged in the Phase 5 dependency scan.

Decision 5 (`Security` database, verified 2026-10-14):
- The provided script is schema only (SQL Server 2017 default instance, compatibility level 100): 15 tables, 14 views, 76 stored procedures, no data. It is kept out of the repo because it contains the internal domain, account names and server file paths. A local copy was built in LocalDB (database `Security`, server-specific users and file paths skipped); it is empty and can be dropped at any time.
- It matches what the old `IDT.Web.Security.RoleProvider` called: `aspnet_UsersInRoles_IsUserInRole`, `aspnet_UsersInRoles_GetRolesForUser` and `aspnet_Roles_CreateRole`, each with the standard leading parameters plus an optional `@Recursive` (default on).
- Role inheritance is implemented in the database (`aspnet_ChildRolesInParentRoles`, recursive queries). The replacement lookup must therefore call these procedures and not query the tables, so the inheritance logic is preserved.
- Verified on the local copy with throwaway rows (rolled back): a directly held role returns 1; a role inherited two levels deep returns 1 (0 with `@Recursive=0`); a role not held returns 0; unknown user or application returns 2; unknown role returns 3; user names match case-insensitively; `GetRolesForUser` returns inherited roles.
- The application name is `/` (hardcoded in the admin procedures; matches `applicationName="/"` in `Web.config`). User names are stored as `DOMAIN\user`, the same form ASP.NET Core Windows authentication provides. Some admin procedures also handle bare staff numbers, so mixed formats may exist in the data.
- The app servers' machine accounts are `db_owner`, so the "create role if missing" write (decision 4) is permitted for them. That write only happens for a role that does not exist yet.
- Open gaps:
  - The script lost `OUTPUT` on some parameter declarations (for example in `aspnet_Applications_CreateApplication`), so `aspnet_Roles_CreateRole` fails on a database built from it. The lookup procedures are unaffected. The old provider swallowed errors on that call, so it is unknown whether production has the same problem; a script regenerated with SSMS "Script as > CREATE" would settle it.
  - The source environment is not stated in the script (it grants access to dev, pre-prod and production app-server accounts).
  - No data, so the `/` application row and the 7 role names are unconfirmed. Query to run on the source database: `SELECT a.ApplicationName, a.ApplicationId, r.RoleName FROM aspnet_Roles r JOIN aspnet_Applications a ON a.ApplicationId = r.ApplicationId WHERE r.RoleName LIKE 'Claim Statement Generator%'`.
- New risk for Phase 3: Microsoft.Data.SqlClient defaults to `Encrypt=True`, while the current connection strings do not set it. Correction from Phase 2: EF6 6.5.2 depends on System.Data.SqlClient 4.8.6, not Microsoft.Data.SqlClient, so the Generator's connections keep their current defaults. Only the new role lookup against the `Security` database must choose its SQL client and encryption options explicitly (connections to servers without a trusted certificate would fail with Microsoft.Data.SqlClient defaults).

Decision 7 (upgrade NReco.PdfGenerator to 1.2.1 and WinSCP to 6.5.7):
- Both upgrades are required in practice (probe on .NET 10, throwaway projects in `%TEMP%`):
  - NReco 1.1.15 crashes on .NET 10 (`TypeLoadException` for `System.Web.HttpContext` in the `HtmlToPdfConverter` constructor); 1.2.1 (first `netstandard2.0` build) works.
  - WinSCP 5.9.6 wrapper fails to load on .NET 10 (missing `System.Diagnostics.PerformanceCounter`); the 6.5.7 wrapper refuses a mismatched `WinSCP.exe` (5.9.6) but loads and starts with the matching 6.5.7 exe. Replace the tracked `Generator/WinSCP.exe` with the 6.5.7 one. The app uses plain FTP with binary `GetFiles` only.
- Rendering difference (wkhtmltopdf 0.12.4 to 0.12.6): on the real templates rendered from the same HTML, the BPS template gives identical extracted text (4 pages); the XB template gives 10 pages in both but 26 characters of text differ (line order near a table). `PdfCompare` would flag the XB difference. These were placeholder templates, not real data, so the fixture-based PDF comparison in Phase 2/6 must cover this. If the differences matter, an untested mitigation is to point NReco 1.2.1 at the old 0.12.4 binaries already tracked under `Web/App_Data/wkhtmltopdf`. **Update (Phase 2):** that mitigation was tested and applied; with wkhtmltopdf pinned to 0.12.4 the BPS and XB PDFs match the legacy output (the binaries now live in `Generator/wkhtmltopdf`).
- Follow-up (not a blocker): check the NReco.PdfGenerator license. Per nrecosite.com it is free for a single-server production deployment; multiple deployments or a multi-server production deployment need a commercial license. This app has several environments and deployment folders, and it is unknown whether a license already covers 1.1.15.

Decision 9 (Kestrel for local validation; IIS verification deferred):
- Local functional and authorization testing runs on Kestrel with LocalDB. Windows authentication on Kestrel uses the Negotiate handler, not IIS's own Windows authentication, so the Web host must configure authentication so that the IIS-hosted production path keeps working (retain the IIS hosting configuration per the plan). Settle this in task 003.
- Not covered by Kestrel: IIS in-process hosting and the ASP.NET Core Module, IIS-provided Windows authentication, the application pool identity (SQL integrated security, file shares, mutex/file permissions, wkhtmltopdf extraction), `web.config` hosting settings, and the .NET 10 Hosting Bundle install.
- Task 006 acceptance must report IIS-hosted behaviour as unverified, not passed, until someone runs a smoke check on a real IIS host (and the README documents that step). Revisit if IIS or IIS Express becomes available on the VDI; note IIS Express (with the ASP.NET Core Module V2) was found installed on the machine used for Phase 1.
