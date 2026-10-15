# Modernization findings: topics for the application owner

Context: upgrade from .NET Framework 4.5 to .NET 10. Evidence is in `.github/modernize/runtime-framework-compatibility/plan.md`. The upgrade is not signed off until the items below are decided.

## Decisions needed

| # | Topic | Question | Options |
|---|---|---|---|
| 1 | NReco.PdfGenerator licence | The free licence covers a single production server only. The app has several environments and deployments. Does an existing licence cover 1.2.1? | Confirm the licence, buy a commercial licence, or replace the PDF engine |
| 2 | Missing `Home/Upload` action (manual statement upload on the Log page) | The supplied `Views/Log/Index.cshtml` has an upload form that posts to `Home/Upload`; no such action exists in the source. The form is shown only when the `Manual Upload` setting is `Active`. Is the feature in use, and is the supplied view from a newer version? | Supply the missing source, remove the form from the view, or keep the setting inactive |
| 3 | wkhtmltopdf 0.12.4 (CVE-2022-35583, SSRF) | No patched release exists; the project is archived. | Accept with least-privilege service account; trial hardening options (may change PDFs); replace the engine |
| 4 | jQuery 1.10.2 (end of life, medium CVEs) | Exposure looks low, but the library is unsupported. | Accept, or upgrade to 3.x with jquery-migrate and retest every page |
| 5 | Bootstrap 3.0.0 (medium XSS advisories) | Exposure looks low. | Accept, or move to 3.4.1 (same major) |
| 6 | Unmaintained libraries | OpenXmlPowerTools 4.5.3.2 runs in compatibility mode; EPPlus 4.5.3.3 is deprecated (newer versions need a commercial licence); CsvHelper 2.16.3 and DocumentFormat.OpenXml 2.7.2 are old. | Accept now, schedule replacements separately |
| 7 | Original security review findings (13 CWE items) | Code findings, not changed in this upgrade. | Fix now, schedule, or accept |
| 8 | `ARController` | Excluded from the build and left unchanged by decision, so 22 actions are live. | Keep excluded, or schedule modernization |
| 9 | Behaviour changes | Signed-in users without a role now get 403 (was 401); a missing antiforgery token gives 400 (was 500); some generation error messages now include the inner message. | Confirm acceptable |

## Information needed from the owner

| # | Item | Why |
|---|---|---|
| 10 | Production `SELECT MigrationId, ContextKey, ProductVersion FROM dbo.__MigrationHistory` | The supplied schema had no data; needed to confirm the 10 migrations and to run the Generator against a copy of the schema. |
| 11 | Schema scripted with indexes | The supplied script had none, so indexes could not be compared. |
| 12 | Production database drift | `SITI.SUM.PAFOrganisationName` is not in the model. The FK between `SITI.YF` and `SITI.Claims` points the other way than the model. Is this intentional? The `FDMR.*` objects and `dbo.temp` are not used by the app; who owns them? |
| 13 | `Security` database | Role names and the `/` application row are unconfirmed. `aspnet_Roles_CreateRole` fails on the supplied script (lost `OUTPUT` parameters); does production have the same problem? |
| 14 | Word templates | `ClaimTemplate.docx` and `ClaimTemplateXB.docx` are git-ignored, so a fresh clone and CI lack them. Where should the master copies live? |
| 15 | Real inputs | Generation was verified with fabricated fixtures, not production DAX, SITI or XB feeds or production templates. A run with representative data is needed. |
| 16 | Target environments | The new version needs the .NET 10 Hosting Bundle on IIS hosts. Which hosts and service accounts will run it? |

## Not yet verified

- IIS-hosted behaviour (in-process hosting, IIS Windows authentication, application pool identity, data protection keys). Verified on Kestrel only. Needs a smoke test on a real IIS host.
- Production-like PDF output: compared by extracted text and page count, not pixel by pixel.
