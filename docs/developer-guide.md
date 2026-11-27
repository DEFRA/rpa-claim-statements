# Developer guide

For developers maintaining the Claim Statement Generator (CSG) after its move from .NET Framework to .NET 10. This
guide describes the checked-in implementation; it is not proof of production readiness.

Start with the README for [build and test commands](../README.md#build-and-test),
[configuration](../README.md#configuration) and [running locally](../README.md#run-locally). See the
[operator guide](./operator-guide.md) for Azure deployment and support.

## What changed

| Area | Current approach | What this means for maintenance |
|---|---|---|
| Runtime | .NET 10, Linux containers on the DEFRA Alpine parent image; also runs on Windows. | Deploy a rebuilt image, not files copied to a server. Do not rely on Windows services, the registry or a persistent local filesystem. |
| Web UI | ASP.NET Core Razor Pages with the original routes (`/`, `/Home`, `/Error`, `/Log`, `/Configuration`, `/Suppression`). | Change page models and `.cshtml` files together; do not add a parallel MVC workflow. |
| Configuration | Environment variables only. | No `App.config`, transforms or SlowCheetah. Add a setting as an environment variable with a validating class, as `MailSettings` does. |
| Data access | EF Core with migrations. | The schema is managed by migrations; see [Database schema and migrations](#database-schema-and-migrations). |
| Authentication | Entra ID through Microsoft.Identity.Web with an authentication cookie; app roles map to the original permission names. | The `Security` database and Windows authentication are gone. See [Authentication and roles](#authentication-and-roles). |
| PDF output | A Python script runs WeasyPrint 70.0 once per statement. | Replaces wkhtmltopdf and Chromium. See [The PDF pipeline](#the-pdf-pipeline). |
| Telemetry and keys | Azure Monitor OpenTelemetry and shared Data Protection keys, through an explicit user-assigned managed identity. | Neither falls back to developer credentials. |
| Tracking mail | Microsoft Graph `sendMail` with a raw `HttpClient` and the managed identity (Azure.Identity). | No Graph SDK and no SMTP. Off unless `Mail__Enabled=true`, so local runs send nothing. The sender and recipient are the `Email Account` value in `CS.Configuration`. See the [operator guide](./operator-guide.md#tracking-mail-platform-prerequisites). |

## Where to work

Paths are relative to `RPA.ClaimStatements`:

| Location | Responsibility |
|---|---|
| `RPA.ClaimStatements.Generator/Program.cs` | Command-line parsing, exit codes and the catch-all that reports failures. |
| `Generator/Services/StartupChecks.cs` | Checks that run before any work in both applications: connection string, working root, optional local migrations. |
| `Generator/GeneratorServices.cs` | The Unity registrations shared by the Generator and the Web application. |
| `Generator/Components/PowerPlant.cs` | The orchestration of one run: `Activate(statementType, maximumBatchSize)`. |
| `Generator/Components/PumpingStation` | Imports: managed FTP (`FTPService`), SITI, XB, DAX AP and AR files. |
| `Generator/Components/FuelTank` | Data retrieval for statements. |
| `Generator/Components/Turbine` | Statement generation: XML, XSLT, DOCX, HTML and PDF (`TransformService`), validation and logging. |
| `Generator/Components/TrackingStation` | The tracking report and its e-mail. |
| `Generator/Context` | `ClaimStatementsContext`, the EF mappings and `Migrations`. |
| `Generator/Services` | Configuration, folders, files, dates, currency, errors and the settings classes (`MailSettings`, `TelemetrySettings`, `ClaimStatementsConnectionString`). |
| `Generator/Rendering/render_pdf.py` | The WeasyPrint renderer. |
| `Generator/XSLT` | The XML to Word transformations, published next to the binaries. |
| `RPA.ClaimStatements.Web/Program.cs` | Startup and service registration. |
| `Web/Pages` | Page handlers and views. `OperatorPagesExtensions.cs` holds the routes and the middleware order. |
| `Web/Security` | Entra configuration, app role mapping and the Data Protection configuration. |
| `Web/Extensions` | Telemetry registration and the CSV export extension. |
| `Database/adopt-existing-database.sql` | One-time script for existing databases (see below). |

The Web project references the Generator project, so it runs the Generator in-process. Dependency injection is the
existing Unity container, adapted to the ASP.NET Core service provider.

## Startup behaviour

Both applications call `StartupChecks.EnsureReady()` before building any services and fail with a clear message if
the connection string or `CLAIMSTATEMENTS_ROOT` is missing or invalid, or (on a developer machine with
`CLAIMSTATEMENTS_APPLY_MIGRATIONS=true`) apply local migrations. The Generator's `PowerPlant.EnsureSchemaContract()`
then fails if the database is unreachable or migrations are pending, naming them. The Web application's middleware
order is: liveness probe, request localisation (`en-GB` only), authentication, a challenge for every unauthenticated
request (this replaces the old `<deny users="?" />`), static files, routing and authorization.

## Database schema and migrations

The schema is managed with EF Core migrations in `Generator/Context/Migrations`.

- **Baseline.** `InitialCreate` describes the schema the former EF6 migrations produced. The model keeps the EF6
  column types (`datetime`, not `datetime2`), constraint names and non-unique foreign-key indexes, so existing
  databases match it and later migrations can alter objects by name. The EF6 migrations are only in git history.
- **Existing databases (SIT, UAT, Production, local).** Do not run `InitialCreate` against them. First compare the
  database (tables, columns, types, constraint and index names) with `InitialCreate`, then run
  `Database/adopt-existing-database.sql` once. It only records `InitialCreate` as applied. Until then the application
  reports the pending migration and does no work.
- **Changing the schema.** Change the model, add a migration and commit it. The unit test `HasPendingModelChanges`
  fails when the model has changes no migration covers.

  ```powershell
  cd RPA.ClaimStatements
  dotnet tool restore
  $env:ConnectionStrings__ClaimStatementsContext = 'Server=localhost;Database=Design;Integrated Security=True;'  # design time only, never connected
  dotnet ef migrations add <Name> --project RPA.ClaimStatements.Generator --output-dir Context\Migrations
  ```

- **Applying migrations.** Deployed environments never migrate at startup, and the application's runtime identity
  stays low-privilege. Build the migration bundle and run it with an identity that has DDL rights:

  ```powershell
  dotnet ef migrations bundle --project RPA.ClaimStatements.Generator --self-contained -r linux-x64 --force -o efbundle
  ./efbundle --connection '<connection string>'
  ```

  Use `-r linux-musl-x64` when the bundle runs in the Alpine image. To review first, run
  `dotnet ef migrations script --idempotent --project RPA.ClaimStatements.Generator`.
- **Local development.** `CLAIMSTATEMENTS_APPLY_MIGRATIONS=true` creates and updates the local database at startup.
  It is ignored unless set to `true`; never set it in a deployed environment.
- **Not verified.** The model was compared with the EF6 end state from the former migration metadata, not with real
  databases. The database team's comparison before adopting each environment is required.

## The PDF pipeline

For each statement the Generator builds XML, transforms it with an XSLT from `XSLT` next to the binaries into a Word
document body, merges that into a copy of the template (`ClaimTemplate.docx` or `ClaimTemplateXB.docx` from
`$CLAIMSTATEMENTS_ROOT/Templates`), converts the document to HTML with Clippit, and renders the HTML to PDF with
`render_pdf.py`. Output names, the archive, publish and cleanup order are unchanged from the original application.

- **Process.** `TransformService.RenderPdfAsync` starts `python3` (`python` on Windows) in isolated mode with
  argument-list execution, once per statement, with a 60-second limit and process-tree termination on timeout. The PDF
  is written to a `.part` file, checked for a PDF signature and end marker, and renamed to the final name only after
  success. There is no retry or alternate renderer.
- **Layout.** A4 portrait, screen media, margins left 12.7, right 25, top 10 and bottom 10 mm, scale 1.18. The
  renderer lays out once and, only if content is wider than the printable width, reduces the scale just enough to fit;
  below 0.5 it fails instead of shrinking illegibly.
- **Word and Clippit output.** The renderer maps constructs WeasyPrint would otherwise drop or misplace: the CSS system
  colour `windowtext` (otherwise every table border is lost), the Symbol-font bullet, Clippit's tab emulation, and
  trailing empty paragraphs that would create a blank last page.
- **Images and resources.** CSS colours and embedded PNG, JPEG, GIF and TIFF images are converted to grayscale. Before
  conversion, `EnsureNoUnsupportedEmbeddedImages` fails a statement that embeds an image Clippit would silently drop
  (for example BMP or WMF). Missing, external or relative resources fail the statement; JavaScript never runs.
- **Fonts.** They come from the image (Liberation fonts; Arial resolves to Liberation Sans). Embedded font files are
  not admitted. If testing shows missing glyphs or unacceptable layout, choose and license the fonts, add them to the
  image through the Dockerfile and compare the output again. Do not copy Windows fonts.
- **Culture.** Date text depends on the process culture; the Generator must run with an English culture for English
  month names. The Web application uses `en-GB`.
- **Acceptance.** The synthetic tests prove the renderer works, not parity with the original output. Owner review of
  real statements for all supplied template variants is still required.

### Comparing PDF content between two versions

Use this after a change that should not alter PDF content. Run the setup SQL on UAT, replacing the placeholder:

```sql
create table InvoiceTestData
(
    InvoiceNumber nvarchar(max),
    apid uniqueidentifier
)

delete InvoiceTestData
insert into InvoiceTestData
select top <the number of statements required>  a.Invoice, a.APID
from dax.AP a
    inner join siti.sum s on s.FRN = a.Supplier and s.SchemeYear = a.MarketingYear
        and CONCAT('SITI', substring(a.Invoice, 2, 7)) = s.InvoiceNumber
where MarketingYear = '2017' and LastSettlementDate > '2017-12-19' --and a.LogID is not null
order by apid asc

delete cs.errors

update a
set LogID = null, Active = 1
from dax.ap a
    inner join InvoiceTestData t on t.apid = a.APID and t.invoicenumber = a.Invoice
```

1. Clear the PDF output folder (its path is in `CS.Folders`).
2. Run the CSG and check the PDFs were created, then move them to a new folder.
3. Install the new version, repeat the setup SQL and steps 1 and 2.
4. In `PdfCompare`, edit `index.js` to set `path1` and `path2` to the two folders, run `npm install` once, then
   `node .`. No output means no differences.

## Authentication and roles

The Web application is a confidential single-tenant client. Sign-in uses the authorization-code flow with PKCE and a
cookie session; the server redeems the code with the configured credential. The registration needs the Web redirect
URIs, not the implicit-grant ID tokens option. `AzureAdConfigurationValidation` stops startup for a missing or
multi-tenant `TenantId`.

- **Roles to permissions.** Entra app role claims (`roles`) map to the seven original permission names through a fixed
  table (`Security/AppRolePermissionMap.cs`), applied per request by `AppRolePermissionsClaimsTransformation`. Pages
  keep the original `[Authorize(Roles = "...")]` and `IsInRole(...)` checks. The table is in the
  [operator guide](./operator-guide.md#entra-setup-app-roles). Only an exact match on a known role value grants
  anything; groups, names and e-mail addresses are never used.
- **Paths.** `/signin-oidc` and `/signout-callback-oidc` are handled by the authentication middleware. Sign-out is
  `POST /Account/Logout` with an anti-forgery token and needs no role.
- **403, not a redirect.** A signed-in user without the needed role gets a plain HTTP 403; there is no access-denied
  page.
- **Every request is authenticated**, including static files, except the liveness probe `/health/live`.

## Tests and verification boundaries

- **Run:** `dotnet test RPA.ClaimStatements.sln`. The tests use mocks (including a mocked `ClaimStatementsContext` built
  with inert options so no connection string is read) and in-process hosting, not a real database.
- **PDF rendering tests** need Python and WeasyPrint 70.0 on the PATH; without them they are skipped
  (`RequiresWeasyPrint` attribute), not failed. A skipped run therefore says nothing about PDF output; run the tests in
  the container image or on a machine with WeasyPrint before changing the renderer.
- **Not covered by tests or verification:** a real SQL Server (collation, precision, concurrency), a real Entra
  sign-in, Azure resources, the Azure Monitor and Data Protection integrations against real services, FTP against the
  partner server, live Graph/Exchange mail delivery, and PDF parity with the original output.
- **Keep the suite small.** Test our own behaviour: business rules, the authorization matrix, binding and
  anti-forgery, startup validation, the PDF pipeline. Do not add tests for framework or library behaviour (path
  normalisation, static files, token validation), markup details, or counts of model members; the migrations are the
  source of truth for the schema, and `Model_Has_No_Changes_Missing_From_The_Migrations` catches model drift.
- **Choose the lowest useful layer** for a new test: direct service or handler tests before HTTP tests.

## Compatibility cautions

- Users who are signed in but lack a role now receive HTTP 403 (previously 401). A missing anti-forgery token gives
  400 (previously 500). Generation error messages that wrap several errors now include the inner message.
- The Generation Log page's manual-upload form posts to `Home/Upload`, and the Error page's Force Generation control
  targets `Home/ForceGeneration`. Neither action exists in the source, so both return 404. They were not invented
  during the migration.
- Plain FTP (binary, passive) is retained for SITI, matching the previous setting; TLS or SFTP was not added. Masks
  match file names case-insensitively with `*` and `?`; other WinSCP mask syntax is not supported. All control files are
  downloaded and removed remotely before any data file, and local files are replaced only after a complete download.
- Named mutexes coordinate runs per host only; do not run overlapping Generator jobs or replicas.
- Static asset URLs must match the case of the published file names (Linux file names are case-sensitive).
- Old libraries remain by decision: EPPlus 4.5.3.3 (the last LGPL release), CsvHelper 2.16.3, NUnit 3.14, the Unity
  container, jQuery 1.10.2 and Bootstrap 3.0.0 (end of life, with known medium-severity advisories). Larger refactors
  were intentionally not done: replacing Unity, separating the Web project's reference to the Generator executable, and
  upgrading the front-end assets.
- Keep the WeasyPrint, Python and Alpine packages patched in the image separately from NuGet auditing.
