# Media & Static Assets Mapping Report

**Application:** RPA.ClaimStatements
**Scope:** `RPA.ClaimStatements.Web` (+ linked assets physically owned by `RPA.ClaimStatements.Generator`)
**Migration:** ASP.NET (legacy, historical `d2vmba001`/`d2vmbwa001` deployment snapshots) → .NET 10 / ASP.NET Core MVC, target host Azure Container Apps
**Owner:** Media/asset inventory only — **no file copies, no downloads, no migrated folder created**
**Generated:** 2026-10-16
**Companion document:** [Business Logic Mapping](Business-Logic-Mapping.md) synthesizes Generator document-generation behavior and the independent Web inventory (`W001`–`W025`). Asset status here is inventory only: nothing has been copied or verified in a migrated application.

> ⚠️ **Nothing in this report has been copied, moved, or re-hashed into a new location.** All paths below are the *current* repo paths. Target paths are **planned only** and require code-gate approval before any `RPA.ClaimStatements-Migrated` folder is created.

---

## 1. Methodology & Limitations

- Inventory built from `git ls-files`, `git check-ignore -v`, `Get-ChildItem`, and SHA-256 hashing of **first-party, non-sensitive, already-public** static assets (CSS/JS/fonts/icons) only.
- **No customer claim documents, generated DOCX/PDF/XML, or upload content were opened, hashed, or printed.** Those categories are described structurally (folder/pattern) only.
- **No binary or document content is reproduced in this report.** Only filenames, paths, sizes, tracked/ignored status, and (where safe) hashes are captured.
- Third-party library versions are read from filenames/comments only; no license files were found in-repo, so license provenance is marked **Unknown** pending owner confirmation — nothing was downloaded to verify.
- `ARController.cs` and `ARControllerTests.cs` are both excluded from their respective `.csproj` files (`<Compile Remove>`) — confirmed **consistent exclusion, not a packaging mismatch**. Not treated as a media/asset gap.
- Word template `.docx` files exist on local disk but are **git-ignored** (`.gitignore:293` → `*.docx`) — they are **not part of version control**, so "packaging" them into any migrated output is **blocked pending an approved owner/source/license decision** (per Decisions-Required). This report records their existence/filenames only.
- This agent did not investigate Generator **domain/business logic** (XSLT transform rules, DOCX field mapping, PDF conversion engine) — only the **filenames, packaging (csproj `Content Include`/`Link`), and git-tracking status** of the assets the Generator depends on, since the Web project physically links and ships copies of some Generator assets.

---

## 2. Executive Summary

| Category | Count | Tracked in Git | Status |
|---|---|---|---|
| First-party CSS (wwwroot) | 6 | 6 | Inventoried |
| First-party JS (wwwroot, shipped) | 14 | 14 | Inventoried |
| First-party JS (design-time only, NOT shipped) | 3 | 3 | Inventoried — dead weight candidate |
| Fonts (Glyphicons, Bootstrap-bundled) | 4 | 4 | Inventoried |
| Images/Icons | 4 | 4 | Inventoried |
| XSLT transform templates (linked from Generator) | 2 | 2 | Inventoried |
| DOCX source templates (linked from Generator) | 2 | 0 (git-ignored) | **Blocked** — not tracked, license/ownership unresolved |
| Working-folder placeholders (`Temp.txt`) | 6 | 6 | Inventoried |
| Historical deployment snapshots (`d2vm*`) | 116 (58 + 58) | 116 | Inventoried — excluded from build, not a source asset |
| CDN-hosted third-party assets (no local file) | 2 | n/a | Inventoried — external dependency |
| Build-tool asset references with missing source (`compilerconfig.json`) | 2 declared outputs | 0 source files found | **Blocked/Orphaned** — flagged for owner |
| Generated runtime output categories (DOCX/XML/HTML/PDF/Uploads) | 5 folders | n/a (empty by design) | Inventoried — structural only, no files to map |

**Total first-party static files inventoried and hashed:** 28 (all under `wwwroot/`)
**Total linked/shared asset files inventoried (no hash, by policy):** 4 (2 XSLT + 2 DOCX)
**Total historical/non-source files identified and excluded from scope:** 116

---

## 3. First-Party Static Assets (`wwwroot/`) — Hashed, Tracked, Low Risk

All files below are served from `RPA.ClaimStatements.Web/wwwroot` via `app.UseStaticFiles()`. **Important runtime note:** in `Program.cs`, the custom authentication-challenge middleware runs **before** `UseStaticFiles()`, so none of these assets are actually anonymously public — an unauthenticated request is challenged first (`<deny users="?" />` parity preserved). This is a behavioral carry-over, not a new finding to action, but it matters for any CDN/edge-caching decision later.

| # | Path (repo-relative) | Git Status | SHA-256 (first 16 hex) | Linux Case Risk |
|---|---|---|---|---|
| 1 | `wwwroot/Content/bootstrap-theme.css` | Tracked | `7A400CF0A1B0F827` | None |
| 2 | `wwwroot/Content/bootstrap-theme.min.css` | Tracked | `B100568F77C3473B` | None |
| 3 | `wwwroot/Content/bootstrap.css` | Tracked | `28735C36B12C5499` | None |
| 4 | `wwwroot/Content/bootstrap.min.css` | Tracked | `40B97C73687AF27B` | None |
| 5 | `wwwroot/Content/PagedList.css` | Tracked | `F805B4510D9013B2` | None |
| 6 | `wwwroot/Content/Site.css` | Tracked | `20128085FD6FFA45` | ⚠️ **Yes — see §6.1** |
| 7 | `wwwroot/fonts/glyphicons-halflings-regular.eot` | Tracked | `5FF7C239555476E9` | None |
| 8 | `wwwroot/fonts/glyphicons-halflings-regular.svg` | Tracked | `DB17872BAEAA24C1` | None |
| 9 | `wwwroot/fonts/glyphicons-halflings-regular.ttf` | Tracked | `CB18B8D11F9698FC` | None |
| 10 | `wwwroot/fonts/glyphicons-halflings-regular.woff` | Tracked | `71C12656535E9911` | None |
| 11 | `wwwroot/images/Generator.jpg` | Tracked | `A46609E0D15658CB` | None (correctly referenced, see §4) |
| 12 | `wwwroot/images/icon_inverted-6.png` | Tracked | `10463EEA1A5EAD88` | None |
| 13 | `wwwroot/images/Icon.ico` | Tracked | `92181ED0F0B91E92` | None — also used as `<ApplicationIcon>` in csproj |
| 14 | `wwwroot/images/Sheep.jpg` | Tracked | `93A4DD226A3D7A1B` | None |
| 15–28 | `wwwroot/Scripts/*.js` (14 files: bootstrap, jquery-1.10.2 + min + map, jquery.unobtrusive-ajax + min, jquery.validate + min, jquery.validate.unobtrusive + min, modernizr-2.6.2, respond + min) | All tracked | (hashed, omitted here for brevity — available on request) | None |

**Target path (planned, not created):** identical relative path under `RPA.ClaimStatements-Migrated/RPA.ClaimStatements.Web/wwwroot/...` — ASP.NET Core `wwwroot` convention is preserved 1:1, no restructuring required for this category.

---

## 4. Third-Party / Vendor Static Assets — Version & License Provenance

| Library | File(s) | Version (from filename) | License File in Repo? | Provenance |
|---|---|---|---|---|
| Bootstrap | `bootstrap.css`, `bootstrap.min.css`, `bootstrap-theme.css`, `bootstrap-theme.min.css`, `bootstrap.js`, `bootstrap.min.js` | Unversioned in filename (legacy MVC scaffolding bundle) | Not found | **Unknown** — needs owner confirmation (commonly MIT) |
| jQuery | `jquery-1.10.2.js` + `.min.js` + `.min.map` | 1.10.2 | Not found | **Unknown** (jQuery is MIT; not verified in-repo) |
| jQuery Validation | `jquery.validate.js` + `.min.js`, `jquery.validate.unobtrusive.js` + `.min.js` | Unversioned | Not found | **Unknown** |
| jQuery Unobtrusive Ajax | `jquery.unobtrusive-ajax.js` + `.min.js` | Unversioned | Not found | **Unknown** |
| Modernizr | `modernizr-2.6.2.js` | 2.6.2 | Not found | **Unknown** |
| Respond.js | `respond.js` + `.min.js` | Unversioned | Not found | **Unknown** |
| Glyphicons Halflings (Bootstrap font) | `.eot/.svg/.ttf/.woff` | Bundled with Bootstrap | Not found | **Unknown** |
| X.PagedList (CSS only, package is a NuGet dep) | `PagedList.css` | Matches `X.PagedList.Mvc.Core` NuGet `10.6.0` referenced in Web `.csproj` | N/A (NuGet license applies) | NuGet package license applies — not re-verified here |
| Font Awesome 4.7.0 | **CDN-only**, no local file (`_Layout.cshtml:8`, `cdnjs.cloudflare.com`) | 4.7.0 | N/A | External CDN dependency — not downloaded/mirrored; Azure egress/firewall rule needed if CDN access is restricted in target environment |
| Google Fonts "Hind" | **CDN-only**, no local file (`_Layout.cshtml:9`, `fonts.googleapis.com`) | N/A | N/A | External CDN dependency — same note as above |

**None of these files were downloaded, re-licensed, or copied.** They are grouped here purely from what is already checked into this repository. License confirmation is a Decisions-Required item, not something this agent can resolve unilaterally.

---

## 5. Linked/Shared Assets — Generator ↔ Web (filenames/packaging only, no domain logic reviewed)

The Web project does **not** own these files; it links to the Generator project's copies via MSBuild `<Content Include ... Link="...">`, so they are packaged into the Web project's output under the paths shown. This section only reports **packaging mechanics and tracking status** — the Generator's XSLT/DOCX field-mapping logic itself is explicitly out of this agent's scope (owned by the parallel Generator-focused agent).

| Asset | Generator Source Path | Web Project Path (via `Link`) | Git Tracked | CopyToOutput / CopyToPublish | Notes |
|---|---|---|---|---|---|
| `ClaimXSLT.xslt` | `RPA.ClaimStatements.Generator/XSLT/ClaimXSLT.xslt` | `Web/XSLT/ClaimXSLT.xslt` (link) | ✅ Tracked | `PreserveNewest` / `PreserveNewest` | Referenced by `TransformService.cs:91,281` via `Path.Combine(root, "XSLT", xslt)` |
| `ClaimXSLTXB.xslt` | `RPA.ClaimStatements.Generator/XSLT/ClaimXSLTXB.xslt` | `Web/XSLT/ClaimXSLTXB.xslt` (link) | ✅ Tracked | `PreserveNewest` / `PreserveNewest` | Referenced by `SampleXBService.cs:109` |
| `ClaimTemplate.docx` | `RPA.ClaimStatements.Generator/Templates/ClaimTemplate.docx` | `Web/Templates/ClaimTemplate.docx` (link) | 🚫 **Not tracked** (`.gitignore:293` → `*.docx`) | `PreserveNewest` / `PreserveNewest` (declared in csproj, but file only exists if present on the build machine's disk) | **Blocked** — present locally, absent from a clean checkout. Packaging/CI/CD builds will **fail silently to include it** unless sourced from elsewhere (e.g., a build artifact store or Key Vault/Blob-backed asset drop). Needs an approved owner + source + license decision before any migrated packaging step. |
| `ClaimTemplateXB.docx` | `RPA.ClaimStatements.Generator/Templates/ClaimTemplateXB.docx` | `Web/Templates/ClaimTemplateXB.docx` (link) | 🚫 **Not tracked** | Same as above | Same blocker as `ClaimTemplate.docx` |

**Clean-checkout risk (confirmed, not hypothetical):** a fresh `git clone` + build of this repo will **not** produce `Templates/ClaimTemplate.docx` or `Templates/ClaimTemplateXB.docx` in the output directory, because the source files are git-ignored and only exist on this particular working copy. `HomeController.Sample()` (calls `ISampleService.Sample()` / `ISampleXBService.Sample()`, which depend on `TransformService.CopyTemplate` → these DOCX files) **will fail at runtime on a clean environment** until this is resolved. This is recorded as a packaging/CI gap for the Decisions-Required log — not fixed here.

---

## 6. Case-Sensitivity / Path-Portability Risks (Windows → Linux container)

Azure Container Apps runs Linux containers by default; the current app is built/tested on Windows (case-insensitive filesystem), so any path/casing mismatch that is silently tolerated today will **break on Linux** unless corrected during code migration (owned by the code-migration phase, flagged here as an asset-mapping finding).

### 6.1 Confirmed mismatch: `Site.css`
- **Actual file on disk / in git:** `wwwroot/Content/Site.css` (capital **S**)
- **Reference in `Views/Shared/_Layout.cshtml:22`:** `<link href="~/Content/site.css" rel="stylesheet" />` (lowercase **s**)
- **Impact:** Works today on Windows/IIS (case-insensitive). **Will 404 on Linux** (Azure Container Apps default) unless the reference or the filename is normalized.
- **Recommendation (decision, not applied by this agent):** normalize the `<link>` reference to `Site.css` to match the tracked filename, OR rename the file — either requires a code change in Phase 2, not this report.

### 6.2 No other filename/reference casing mismatches found
Checked `_Layout.cshtml` for all `~/Content`, `~/Scripts`, `~/images` references — all other paths (`Icon.ico`, `icon_inverted-6.png`, `bootstrap.css`, `bootstrap-theme.css`, `modernizr-2.6.2.js`, `jquery-1.10.2.js`, `jquery.unobtrusive-ajax.js`, `jquery.validate.js`, `jquery.validate.unobtrusive.js`, `bootstrap.js`, `respond.js`) match tracked filenames exactly, including case.

### 6.3 Folder-name case consistency
Working folders (`DOCX`, `HTML`, `XML`, `XSLT`, `Templates`, `Uploads`) are referenced consistently in uppercase/PascalCase in both `FolderService.cs` and the Web `.csproj`'s `Content Include` entries. No mismatch found.

---

## 7. Working-Folder Placeholders (`Temp.txt`) — Structural, Not Content Assets

These zero-byte-equivalent placeholder files exist **solely** so that an otherwise-empty folder survives `git` (which does not track empty directories) and so the folder is present at first run before `FolderService.CreateDirectory` executes. They are **not** themselves business assets.

| Folder | Placeholder Tracked | Purpose | Target |
|---|---|---|---|
| `Web/DOCX/Temp.txt` | ✅ | Runtime-generated DOCX staging before conversion to PDF (`TransformService.cs:216`) | Same relative path, planned only |
| `Web/HTML/Temp.txt` | ✅ | Runtime-generated HTML intermediate | Same relative path, planned only |
| `Web/XML/Temp.txt` | ✅ | Runtime-generated XML intermediate (`TransformService.cs:188,198`) | Same relative path, planned only |
| `Web/XSLT/Temp.txt` | ✅ | Co-located with the linked `.xslt` files (see §5) | Same relative path, planned only |
| `Web/Templates/Temp.txt` | ✅ | Co-located with the linked `.docx` templates (see §5) | Same relative path, planned only |
| `Web/Uploads/Temp.txt` | ✅ | Manual bulk-suppression upload staging (`SuppressionController._BulkSuppression`) | Same relative path, planned only |

**Observation (not a defect):** `FolderService.cs` also calls `Directory.CreateDirectory(Root(), "PDF")`, but there is **no** `Web/PDF/Temp.txt` placeholder and no `PDF` folder currently exists on disk or in git for the Web project. This is asymmetric with the other five folders but not necessarily broken, since `Directory.CreateDirectory` is idempotent and will self-create `PDF` at first run. Flagged for awareness only — not treated as Blocked.

**⚠️ Runtime-generated content under these folders (actual DOCX/XML/PDF files produced at runtime) must never be mapped to a public `wwwroot` path.** None currently are — all five working folders sit beside the Web project root, outside `wwwroot`, and are not served by `UseStaticFiles()`. The eventual durable target for this runtime output (Azure Files share / Blob container / ephemeral container filesystem) is a **Decisions-Required** item, not resolved by this report.

---

## 8. User-Facing Generated/Uploaded Documents — NOT Ingested, Structural Only

Per scope, no user-generated or customer claim documents were opened, listed by content, counted, or hashed. Only the code paths that produce/consume them are noted (filenames/patterns only):

| Flow | Controller / Service | Output Filename Pattern | Storage Location (today) | Notes |
|---|---|---|---|---|
| Sample claim statement download | `HomeController.Sample()` → `ISampleService` / `ISampleXBService` | `Sample Claim Statement - {statementType}.docx` | Generated to a temp DOCX path, streamed to the browser, then `File.Delete()`d immediately — **not persisted** | No test coverage found (`HomeControllerTests.cs` only covers `Index`/`Trigger`, not `Sample`) |
| Full claim statement batch generation | `IPowerPlant.Activate()` (Generator, out of scope) → `TransformService` | `{claimId}{dateTimeString}.docx`, `RPA_OUT_BPS{schemeYear}_ClmStmt_{frn}_{sbi}_{dateTimeString}.pdf` | `Web/DOCX/`, `Web/PDF/` (runtime-created) | Domain logic of generation is explicitly out of this agent's scope |
| Bulk suppression upload | `SuppressionController._BulkSuppression` | User-supplied `.xlsx` via `IFormFile bulkSource` | Not written to disk directly in the controller (passed to `ISuppressionService.BulkUpdate`) — storage behavior inside that service was **not** inspected (service internals = business logic, out of this report's scope) | Declared expectation in code: "ten digit FRNs ... .xslx format" |
| Suppression/Log/Error CSV export | `SuppressionController.Download`, `LogController.Download`, `ErrorController.Download` | `Suppressions.csv`, `Log.csv`, `Errors.csv` | Streamed directly from a DB query, never written to disk | Encoding behavior preserved intentionally per in-code comments (UTF-8 no-BOM vs UTF-8 BOM) — noted for awareness, not a media-asset action item |

No files in this section were counted, hashed, or enumerated beyond the controller/service code paths above.

---

## 9. Historical Deployment Snapshots (`d2vmba001`, `d2vmbwa001`) — Excluded, Not Source Assets

- Both folders are **tracked in git** (58 files each, 116 total) but are **excluded from the active build** via `RPA.ClaimStatements.Web.csproj`:
  `<DefaultItemExcludes>$(DefaultItemExcludes);d2vmba001\**;d2vmbwa001\**</DefaultItemExcludes>`
- Contents mirror a prior IIS deployment's `wwwroot/rpa.claimstatements/` output (duplicate copies of `Content/*.css`, `Scripts/*.js`, `Global.asax`, an `IDT.dll` assembly, and empty-folder placeholders for `DOCX`, `HTML`, `PDF`).
- **Classification: historical deployment snapshot, not a required source asset.** No migration action needed; do not copy into `RPA.ClaimStatements-Migrated`. Retained here only as an inventory note in case cleanup/removal is considered later (a decision for the repo owner, not actioned by this report).

---

## 10. Design-Time-Only Script Helpers — Not Shipped

| File | Tracked | Shipped to `wwwroot`? | Notes |
|---|---|---|---|
| `Web/Scripts/_references.js` | ✅ | ❌ No | Legacy Visual Studio IntelliSense reference aggregator — no runtime effect |
| `Web/Scripts/jquery-1.10.2.intellisense.js` | ✅ | ❌ No | VS IntelliSense annotations for jQuery 1.10.2 |
| `Web/Scripts/jquery.validate-vsdoc.js` | ✅ | ❌ No | VS IntelliSense doc-comments for jQuery Validate |

These sit in `Web/Scripts/` (not `wwwroot/Scripts/`) and are not referenced by any `Content Include` in the `.csproj`, so they are **not part of the deployable output**. Candidate for removal during modernization cleanup, but that is a code-change decision, not performed here.

---

## 11. Build-Tooling Orphan Reference — `compilerconfig.json`

```json
[
  { "outputFile": "Content/_accessibility.css", "inputFile": "Content/_accessibility.scss", ... },
  { "outputFile": "Content/govuk-template.css", "inputFile": "Content/govuk-template.scss", ... }
]
```

- Neither `Content/_accessibility.scss` nor `Content/govuk-template.scss` exists anywhere in the repository (confirmed via recursive search for `*.scss`, `_accessibility*`, `govuk-template*`).
- Neither compiled output (`_accessibility.css`, `govuk-template.css`) exists either.
- **Classification: orphaned/stale build-tool configuration.** Not referenced by `.csproj` as a `Content`/`None` item, so it does not affect the current build — but it indicates a GOV.UK Design System integration that was either removed or never completed. Flagged for the repo owner; no action taken.

---

## 12. Target Mapping Principle (Planned Only — Nothing Created)

For every first-party asset in §3 and the linked assets in §5 that are resolved (owner/license confirmed), the planned target is the **identical relative path** under the future `RPA.ClaimStatements-Migrated/RPA.ClaimStatements.Web/...` tree:

```
wwwroot/Content/*        → wwwroot/Content/*        (unchanged, after Site.css casing fix)
wwwroot/fonts/*           → wwwroot/fonts/*           (unchanged)
wwwroot/images/*          → wwwroot/images/*          (unchanged)
wwwroot/Scripts/*         → wwwroot/Scripts/*         (unchanged)
XSLT/ClaimXSLT*.xslt      → XSLT/ClaimXSLT*.xslt      (unchanged, same Link mechanism)
Templates/ClaimTemplate*.docx → Templates/ClaimTemplate*.docx  (BLOCKED — see §5)
DOCX/, HTML/, XML/, Uploads/, PDF/ → same relative paths, outside wwwroot (BLOCKED pending durable-storage decision — see §7)
```

**No `RPA.ClaimStatements-Migrated` folder exists yet.** This mapping is documentation for the future infrastructure/code-migration phases, gated on the Decisions-Required items below.

---

## 13. Decisions Required (flagged here, not resolved by this report)

1. **DOCX template ownership/source/license** — `ClaimTemplate.docx` / `ClaimTemplateXB.docx` are git-ignored and must come from an approved source (repo owner, secure blob, or re-added to version control with sign-off) before any clean-checkout build or migrated packaging can succeed.
2. **Third-party static asset license confirmation** — Bootstrap, jQuery, jQuery Validation, Modernizr, Respond.js, Glyphicons: no license files in-repo; assumed common OSS licenses but not verified here.
3. **CDN dependency allow-listing** — Font Awesome (`cdnjs.cloudflare.com`) and Google Fonts (`fonts.googleapis.com`) are loaded directly from the browser; confirm these origins are reachable/allowed from the target network policy (Entra-protected app, possible restricted egress).
4. **Durable storage target for runtime-generated DOCX/XML/HTML/PDF/Uploads** — currently local-disk folders beside the binaries; Azure Container Apps' ephemeral filesystem means these need an explicit target (Azure Files share, Blob Storage, or redesigned pass-through) — not decided here.
5. **`Site.css` casing fix** — cosmetic but will 404 on Linux; low-risk code change recommended in Phase 2.
6. **`d2vmba001`/`d2vmbwa001` historical snapshots** — repo owner decision on whether to retain, archive, or delete; not required for migration to proceed.
7. **`compilerconfig.json` orphaned GOV.UK Design System reference** — confirm whether this integration was abandoned intentionally or needs to be completed/removed.

---

## 14. Acceptance Checklist

- [x] All first-party `wwwroot` assets enumerated with tracked status and SHA-256 (where safe)
- [x] All third-party/vendor assets grouped with version (license marked Unknown where unverifiable in-repo)
- [x] Linked Generator assets (XSLT/DOCX) inventoried via packaging mechanics only, no domain logic reviewed
- [x] DOCX templates confirmed present-but-ignored; clean-checkout packaging blocker documented
- [x] Case-sensitivity risk identified and documented (`Site.css` vs `site.css`) with Linux impact explained
- [x] Working-folder placeholders enumerated; distinguished from actual generated content
- [x] Runtime-generated/user-document categories described structurally only — **no content printed, no PII, no hashing of customer docs**
- [x] Historical deployment snapshots (`d2vm*`) identified and excluded from source-asset scope
- [x] Design-time-only script helpers identified as not-shipped
- [x] Orphaned build-tool reference (`compilerconfig.json`) flagged
- [x] No files copied, downloaded, or migrated-folder created
- [x] No edits made to `Business-Logic-Mapping.md`, `Report-Status.md`, `Application-Assessment-Report.md`, or any infra/pipeline file
- [x] Internal links/headings verified, Markdown fences balanced

---

*This report is additive inventory only. All "target path" entries are planned and require the Decisions-Required items above to be resolved before any code or infrastructure migration step acts on them.*
