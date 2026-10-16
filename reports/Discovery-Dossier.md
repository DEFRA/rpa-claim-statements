# Discovery Dossier — RPA Claim Statements (Claim Statement Generator / CSG)

> Produced by: Discovery Engineer
> Date: 2026-10-16
> Status: Draft — Architect approval pending

> **Subsequent Phase 1 input (2026-10-16, superseded same-day):** The original discovery findings below remain a historical evidence snapshot. The user has since selected Entra ID and the GDPR/UK-GDPR compliance option, plus the preferences captured in [Decisions-Required.md](Decisions-Required.md). The user confirmed **code-only modernization**, with another team providing the target database schema and data. The target database engine was initially selected as PostgreSQL and has since been **superseded same-day to Azure SQL Database** ("It will be Azure SQL Database, not postgresql" — see [Decisions-Required.md](Decisions-Required.md) D-04 and [Decision-Log.md](Decision-Log.md)); schema/data migration, infrastructure, deployment and CI/CD changes remain outside this scope regardless of engine. Detailed role mapping, target database contracts and compliance obligations remain unverified. SQL Server remains the evidenced current datastore, and the current target preference (Azure SQL Database) shares the same engine family, which the accompanying reports now reflect for risk/retention-assumption purposes. IaC, region, budget, recovery, cutover and downtime have been deferred rather than declared unnecessary. See [Capability-Matrix.yaml](Capability-Matrix.yaml) for the updated planning context; no resource provisioning or deployment approval is implied.

## 1. Executive Summary (≤5 lines)

CSG generates per-claim BPS/Cross-Border PDF settlement statements for DEFRA RPA from imported DAX/SITI data, running today as an on-premises .NET console batch job plus an in-process-triggering ASP.NET Core MVC admin UI, both already upgraded to **.NET 10 / `net10.0`** (high-confidence code evidence). Primary risk is a set of **Windows-native runtime dependencies** (wkhtmltopdf.exe, WinSCP.exe, System.Drawing, Windows/Negotiate auth, a machine-scoped named mutex) that block as-is deployment to Linux-only Azure Container Apps, independent of the runtime upgrade already done. Recommended strategy is **Refactor** (⚠ Default guess — user/Architect owns final choice), with Azure Container Apps offered only as a **conditional** candidate pending removal of those native dependencies. Production topology, data volumes, compliance requirements, and network routes to Azure are all **unverified** — this dossier is a source-code snapshot, not a production-readiness sign-off.

## 2. Application Identity

| Field | Value |
|-------|-------|
| Name | RPA.ClaimStatements (Claim Statement Generator / CSG) |
| Business owner | Unknown — no named owner found in source; organisation context (DEFRA RPA / DDTS) is apparent from [`../README.md`](../README.md) and [`../modernization-findings.md`](../modernization-findings.md) |
| Purpose | Generates a PDF claim statement for every BPS claim on each change in settlement position, plus Cross-Border (XB) statements — [`../README.md`](../README.md) lines 1–2 |
| Current users | Unknown — not stated anywhere in source |
| Criticality | Unknown — not guessed; see Q10 |
| Business priority | Modernize first (per user) |

## 3. Source Profile

**Primary source adapter:** `source-on-premise`
**Access method:** `file-path` — local filesystem clone of the current repository only; **no production/deployment-host access was available or used**
**Evidence confidence:** `low` (for production deployment/topology — see Notes)

### Evidence

- Repository root listing: `README.md`, `modernization-findings.md`, `DAXAPFileTransfer.ps1`, `RPA.ClaimStatements/`, `PdfCompare/`, `docs/`, `reports/`
- [`../README.md`](../README.md) lines 6–12 — states the solution layout and prerequisites (Windows, SQL Server, IIS Hosting Bundle) as the *intended* deployment, not verified against a live host
- [`../modernization-findings.md`](../modernization-findings.md) — the application owner's own list of 16 open questions, including several about production environments and hosts that remain unanswered in-repo
- [`../docs/Report_20261005164238.md`](../docs/Report_20261005164238.md) — a prior automated Azure Container Apps readiness scan, generated **before** the current `net10.0` upgrade (confirmed stale — see §4 Notes)

### Notes

- **Confidence split by axis, not a single blended number:** code-level access was complete (full repository present), which is why Stack and Workload confidence below are `high`. The `low` rating here specifically covers **production deployment facts** — actual hosting environment, IIS configuration state, network topology, and runtime behavior — which rest entirely on user statements and README-documented *intent*, not verified evidence. "On-prem today, connectivity to Azure available" was reported by the user; type of connectivity, routes, DNS, and reachability were **not independently verified** (see Q5).
- No production machine, IIS host, or live database was reached or queried in any part of this discovery.
- Word templates (`ClaimTemplate.docx`, `ClaimTemplateXB.docx`) are present on the local filesystem copy but are git-ignored (`.gitignore:293`, confirmed via `git check-ignore -v`) — a fresh clone/CI checkout would not have them. Existence was confirmed by file metadata only (size/timestamp); contents were not read.

## 4. Stack Profile

**Primary stack:** `dotnet`
**Primary framework:** ASP.NET Core MVC (Web) + .NET console (Generator), both targeting `net10.0`
**Build system:** `msbuild` (`dotnet build`/`dotnet publish`)
**Runtime version detected:** `net10.0` — confirmed directly in both project files: [`../RPA.ClaimStatements/RPA.ClaimStatements.Generator/RPA.ClaimStatements.Generator.csproj`](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/RPA.ClaimStatements.Generator.csproj) line 4 (`<TargetFramework>net10.0</TargetFramework>`), [`../RPA.ClaimStatements/RPA.ClaimStatements.Web/RPA.ClaimStatements.Web.csproj`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/RPA.ClaimStatements.Web.csproj) line 4
**Evidence confidence:** `high`

**Important correction to a prior stale assessment:** [`../docs/Report_20261005164238.md`](../docs/Report_20261005164238.md) describes the app as .NET Framework 4.5 with `packages.config` — this is a **pre-upgrade baseline scan**, superseded by the live `net10.0` source confirmed above. Treat that report's runtime findings as historical context only; its dependency/CVE tables for front-end libraries (jQuery, Bootstrap) remain currently accurate per direct `modernization-findings.md` cross-check.

### Secondary Stacks

| Stack | Share | Role |
|-------|-------|------|
| Node.js (PdfCompare utility) | Small (2 files, 2 npm deps) | Standalone, manually-run PDF-diff dev/test tool — [`../PdfCompare/index.js`](../PdfCompare/index.js), [`../PdfCompare/package.json`](../PdfCompare/package.json). **Not deployed** with the application; no production Node.js runtime exists. Production Node version is not applicable/known — this tooling is irrelevant to the runtime migration target (see Q12). |
| Browser JS (jQuery/Bootstrap-era) | 44 `.js` files | Client-side only, embedded in Web project views/wwwroot; no server-side Node role |

### Top External Dependencies

> Ranked by **architectural significance** (pin rationale, replacement blast-radius, licensing exposure) — **not** by measured call frequency. No call-frequency instrumentation or profiling was run; this is a declared limitation, not an omission.

| Library | Version | Notes |
|---------|---------|-------|
| NReco.PdfGenerator | 1.2.1 | Wraps the pinned native `wkhtmltopdf.exe` 0.12.4; licensing status open (`modernization-findings.md` #1) — [`../RPA.ClaimStatements/RPA.ClaimStatements.Generator/RPA.ClaimStatements.Generator.csproj`](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/RPA.ClaimStatements.Generator.csproj) |
| EntityFramework (classic EF6) | 6.5.2 | Not EF Core; needs explicit migration-path decision for Azure SQL target |
| WinSCP | 6.5.7 | Native Windows FTP client binary, shipped alongside the app — ACA Linux-container blocker |
| System.Drawing.Common | 10.0.12 | Explicitly pinned in csproj comment to avoid a vulnerable transitive version pulled in by EPPlus; Windows-only by design (csproj `CA1416` suppression comment) |
| EPPlus | 4.5.3.3 | Deprecated; newer major versions require a commercial licence |
| OpenXmlPowerTools | 4.5.3.2 | Runs in compatibility mode (owner-acknowledged in `modernization-findings.md` #6) |
| System.Data.SqlClient | 4.8.6 | Legacy SqlClient (Web project); modern `Microsoft.Data.SqlClient` migration not yet done |
| Microsoft.AspNetCore.Authentication.Negotiate | 10.0.13 | Windows/Negotiate auth — identity-model blocker for Linux ACA as-is |
| Unity / Unity.Microsoft.DependencyInjection | 5.11.10 / 5.11.5 | Legacy DI container bridged into ASP.NET Core DI |
| CsvHelper, DocumentFormat.OpenXml | 2.16.3 / 2.7.2 | Old versions, flagged for replacement by the owner (`modernization-findings.md` #6) |

## 5. Workload Profile

**Primary pattern:** `webapp` (ASP.NET Core MVC admin/trigger UI — the user-facing, continuously-running entry point)
**Secondary patterns:** `batch-job` (console Generator process — import/transform/PDF-generation batch run)

### Entry Points

- Web MVC entrypoint: [`../RPA.ClaimStatements/RPA.ClaimStatements.Web/Program.cs`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Program.cs) — minimal-hosting ASP.NET Core app, Negotiate/IIS Windows auth, pins `ConfigurationManager` to its own `.dll.config`
- Console batch entrypoint: [`../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Program.cs`](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Program.cs) lines 29–140 — CLI args `ALL|BPS|XB|I [maxBatchSize]`, interactive prompt when run with no args

### Runtime Topology

- **Single shared monolith, two process shapes.** The Web app sets `HostEnvironment.IsWebHosted = true` ([`../RPA.ClaimStatements/RPA.ClaimStatements.Web/Program.cs`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Program.cs) line 27) and **runs the Generator's orchestration in-process** rather than dispatching to a separate process, container, or message queue — there is no decoupled worker/queue boundary in the current design.
- Cross-process coordination uses a single machine-scoped named `Mutex` (`Global\CSG_{type}_{id}`) to prevent concurrent runs of the same statement type — [`../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Components/PowerPlant.cs`](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Components/PowerPlant.cs) lines 26, 52, 58, 60, 82. This is a single-machine concurrency primitive, not a distributed lock — it does not transparently support horizontally-scaled/multi-replica deployment.
- No containerization artifacts (Dockerfile, compose file, Bicep/Terraform, Container Apps manifest) exist anywhere in the scanned scope.

## 6. Data Profile

**Primary datastore:** `sql-server`
**Version:** Unknown — not present anywhere in source; the application owner itself asks for the production `dbo.__MigrationHistory` contents in [`../modernization-findings.md`](../modernization-findings.md) item 10
**Approximate size:** Unknown
**Data gravity:** `unknown` (not assumed — see §13 Assumptions for the impact of guessing wrong)
**Evidence confidence:** `low`

### Schema Highlights

- 26 EF6-mapped entities across logical schemas `SITI`, `DAX`, `XB`, `CS`, `Log`, `Monitor`, `FDMR` — [`../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Context/IClaimStatementsContext.cs`](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Context/IClaimStatementsContext.cs)
- 10 EF Code-First migrations, 2018-02 (`Initial`) through 2021-08 (`addPRTable`) — [`../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Migrations/`](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Migrations/)
- A second external `Security` database (ASP.NET membership/role tables) is accessed separately via [`../RPA.ClaimStatements/RPA.ClaimStatements.Web/Security/RoleStore.cs`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Security/RoleStore.cs) — its schema is not present in this repository.
- A reporting export path ([`../RPA.ClaimStatements/RPA.ClaimStatements.FDMR/LoadExtract.sql`](../RPA.ClaimStatements/RPA.ClaimStatements.FDMR/LoadExtract.sql), stored procedure `FDMR.LoadExtract`) invokes `EXEC xp_cmdshell @CMD` running `bcp` to export query results to a filesystem path. This is confirmed live T-SQL in the current script (not the stale docs report). **`xp_cmdshell` is disabled by default and not supported the same way on Azure SQL Database** — this cannot be assumed compatible with a PaaS SQL target without an explicit redesign decision (Database Specialist + Architect).
- The application owner's own notes flag unresolved production/model drift: `SITI.SUM.PAFOrganisationName` missing from the EF model, and a reversed FK direction between `SITI.YF` and `SITI.Claims` ([`../modernization-findings.md`](../modernization-findings.md) item 12) — unconfirmed against a real database in this pass.

### Data Migration Constraints

- Custom SQL-backed roles (`CS.Configuration`-driven feature switches, and the external `Security` database's custom role/role-inheritance logic) must have their **behavioral semantics preserved**, not just their schema — role inheritance resolution currently happens in SQL, not in application code (README: "the roles are named… role inheritance resolved by the database").
- `CS.Folders` is reported, **per the prior (stale) docs report only**, to hold FTP/SQL host/username/password values in-database. This was **not independently verified** in this pass (no database access was used) — it is carried forward as an unverified claim, not a confirmed finding of plaintext credential storage.
- FDMR export and Cross-Border scripts write to filesystem/other-database targets outside the EF-managed model — any lift of the primary database must account for these out-of-band dependents.

## 7. Integration Map

| Integration | Direction | Protocol | Criticality | Notes |
|-------------|-----------|----------|-------------|-------|
| SITI feed | In | FTP (WinSCP, `Protocol.Ftp`) | Unknown — not guessed (see Q10) | [`../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Components/PumpingStation/FTPService.cs`](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Components/PumpingStation/FTPService.cs) lines 20–24, 32–49 — this is the **compiled** FTP service; `Generator/Imports/FTPService.cs` is excluded from the build (`<Compile Remove="Imports\**" />` in the csproj) and is dead/duplicate source, not the live integration |
| DAX AP file drop | In | SMB/filesystem (scheduled copy script) | Unknown | [`../DAXAPFileTransfer.ps1`](../DAXAPFileTransfer.ps1) — polls a source folder for `DAXAP_BPS*_ClmStmt_*.csv`, waits for size stability, copies then deletes |
| PDF statement archive | Out | Filesystem/SMB share ("Finance archive directory") | Unknown | [`../README.md`](../README.md) Testing section; [`../RPA.ClaimStatements/RPA.ClaimStatements.Web/Views/Log/Index.cshtml`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Views/Log/Index.cshtml) |
| Tracking-report email | Out | SMTP | Unknown | [`../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Components/TrackingStation/EmailService.cs`](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Components/TrackingStation/EmailService.cs) lines 34–47 |
| FDMR reporting export | Out | T-SQL `xp_cmdshell` + `bcp` to filesystem | Unknown | [`../RPA.ClaimStatements/RPA.ClaimStatements.FDMR/LoadExtract.sql`](../RPA.ClaimStatements/RPA.ClaimStatements.FDMR/LoadExtract.sql) — see §6 for Azure SQL compatibility caveat |
| Cross-Border "Payment Filter" catalog | Out | DB-to-DB (T-SQL scripts) | Unknown | [`../RPA.ClaimStatements/RPA.ClaimStatements.CrossBorder/`](../RPA.ClaimStatements/RPA.ClaimStatements.CrossBorder/) |
| Security/roles database | In (auth) | SQL (ASP.NET membership/role schema) | Unknown | [`../RPA.ClaimStatements/RPA.ClaimStatements.Web/Security/RoleStore.cs`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Security/RoleStore.cs); [`../RPA.ClaimStatements/RPA.ClaimStatements.Web/Program.cs`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Program.cs) lines 53–63 |

### Identity Provider

- Current: Windows Integrated Authentication (Negotiate/IIS) with role-authorization resolved against a separate SQL `Security` database — [`../RPA.ClaimStatements/RPA.ClaimStatements.Web/Program.cs`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Program.cs) lines 44–63
- Target: **Entra ID reported available by the user** — availability is not the same as a design decision or user consent to a specific auth-replacement approach. No target identity design is made here; this stays open (Q3).

### Network Dependencies

- On-prem FTP host (SITI), SMTP relay/pickup share, "Finance archive" SMB share, external `Security` SQL Server, FDMR export filesystem path — all on-premises per current code/config.
- **User reports connectivity from on-prem to Azure is available ("yes")** — connection type (VPN/ExpressRoute), specific routes, DNS resolution, and firewall reachability were **not independently verified** in this discovery pass (no network probing was performed or permitted). Carried forward as an open item (Q5).

## 8. Risks & Compliance Flags

| Flag | Description | Implication |
|------|-------------|-------------|
| `windows-native-runtime-dependencies` | `wkhtmltopdf.exe` 0.12.4, `WinSCP.exe`, `System.Drawing` all confirmed Windows-only in csproj comments ([`.csproj`](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/RPA.ClaimStatements.Generator.csproj) `CA1416` suppression notes) | Blocks as-is deployment to Azure Container Apps, which per Microsoft's published documentation supports **linux/amd64 containers only** — no Windows-container option exists on that target. Azure Specialist + Coder required before ACA is viable. |
| `windows-auth-dependency` | Negotiate/IIS Windows Authentication is the only auth mode implemented | Needs an explicit identity redesign decision even though Entra ID is reported available (availability ≠ consent/design) — Security Auditor + Architect required |
| `machine-scoped-concurrency-lock` | `Global\` named `Mutex` in [`PowerPlant.cs`](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Components/PowerPlant.cs) lines 26, 52 | Blocks safe horizontal scale-out of the batch workload across multiple container replicas without a redesign |
| `vendor-licensed-runtime` | NReco.PdfGenerator/wkhtmltopdf licensing status unresolved (`modernization-findings.md` #1) | Cost Engineer input needed before relying on current PDF engine at new scale |
| `xp-cmdshell-export-dependency` | `FDMR.LoadExtract` stored procedure runs `xp_cmdshell`/`bcp` ([`LoadExtract.sql`](../RPA.ClaimStatements/RPA.ClaimStatements.FDMR/LoadExtract.sql)) | Cannot be assumed compatible with Azure SQL Database without a database-platform decision — Database Specialist required |
| `compliance-requirements-unknown` | Data protection, residency, retention, and audit/encryption requirements are reported by the user as **unknown** | Security Auditor must close this before any production data touches Azure; treated as open, not absent |
| `untracked-build-asset` | `ClaimTemplate.docx`/`ClaimTemplateXB.docx` present locally but git-ignored — not guaranteed present on a fresh checkout/CI | Build/CI pipeline needs an explicit template-custody decision (`modernization-findings.md` #14) |
| `production-topology-unverified` | No production host, database, or network access was used in this discovery | All production-specific facts below are user-reported only, not independently confirmed |
| `regulated-data-likely` | Prior (stale) docs report classifies several entities as PII/commercially-sensitive financial data; not independently re-verified here | Security Auditor should confirm classification before data-gravity/compliance decisions are finalized |

## 9. Migration Strategy Recommendation

**Recommended strategy:** `refactor` — ⚠ **Default guess.** This is the Discovery Engineer's structured recommendation per the decision tree below; the user/Architect owns the final choice.

### Decision Path (from `migration-strategy-decision-tree`)

1. Branch 1 (Retire/Retain): not triggered — app is active, business priority is "modernize," no sunset plan documented.
2. Branch 2 (No source code available): not triggered — full application source is present in the current repository.
3. Branch 3 (SaaS-embedded / unsupported source / unsupported runtime): not triggered — this is a standard .NET application, not a SaaS-embedded or mainframe/midrange system.
4. Branch 4 (speed-to-cloud): not triggered — business priority is "modernize," not "speed."
5. **Branch 5 (modernize-first): TRIGGERED.** Compiled first-party C# LOC is ~13,615 across 4 build-relevant projects plus their NUnit test projects (excluding `*.Designer.cs` and `<Compile Remove>`-excluded duplicate/dead source) — well under the tree's 50k-LOC "small codebase" threshold. The stack is already on a supported, current runtime (`net10.0`). NUnit test projects exist for both Generator and Web. → **Refactor** (in-place modernization to current LTS runtime + Azure PaaS). **Caveat: test execution was not performed in this discovery pass** — "tests exist" is a static-file fact (test projects and `[Test]`-attributed methods present), not evidence that the suite currently passes or provides adequate regression coverage. This caveat does not change the branch outcome but should be validated before relying on it as regression safety net (see Q12).
6. Branch 6 (cost-first): not applicable — priority is "modernize," not "cost."
7. Branch 7 (risk-first): not applicable — priority is "modernize," not "risk."
8. Branch 8 (workload override): workload = `webapp` (primary) + `batch-job` (secondary) — no override pattern (`data-pipeline`, `event-driven`, `desktop-client-server`, `packaged-app`) applies.
9. Branch 9 (data gravity override): data gravity is `unknown`, not `none`/`small` — treated conservatively as **at least medium** for specialist-staffing purposes only (Database Specialist required), without asserting an actual size.
10. Branch 10 (target Azure candidates): see §10 below — Refactor-path candidates, but gated by the Windows-native-dependency blocker in §8, so the Azure Container Apps candidate is marked **conditional**, not unconditional.
11. Branch 11 (alternatives): see below — at least two alternatives considered and rejected with reasons.
12. Branch 12 (required specialists): see §11.

### Alternatives Considered

| Alternative | Rejected because |
|-------------|------------------|
| Rehost (lift-and-shift, e.g. Azure VM) | Business priority is "modernize first" — a pure lift defers the Windows-native-dependency debt instead of paying it down, and does not advance the stated modernization goal. Also does not fit the requested Azure Container Apps target at all (VMs are a fallback only, not a modernization path). |
| Rebuild (greenfield rewrite) | The codebase is already on `net10.0` (a substantial, already-completed modernization investment), has existing NUnit test projects and a PDF-content regression-comparison tool (`PdfCompare`) representing retained regression-coverage assets; discarding these via a full rewrite is not justified by the evidence gathered (no "unmaintainable, no tests, original team gone" signal found). |
| Replatform | Considered as a lighter-touch alternative (minimal code change, move to managed PaaS as-is) — rejected as the *primary* recommendation because the Windows-native dependencies (wkhtmltopdf.exe, WinSCP.exe, System.Drawing, Windows auth) cannot be "replatformed" onto Linux-only Azure Container Apps without code-level replacement; a pure replatform would either fail the ACA target or require Windows VMs, which does not match the modernize-first priority. Replatform remains a **legitimate interim step** (e.g., App Service on Windows) if native-dependency removal is deferred — flagged as a secondary option for the Architect to weigh, not fully discarded. |

### Rationale (3–5 lines)

The application is small by LOC, already running on a current .NET runtime, and has existing automated tests — exactly the Branch 5 "modernize-first, small codebase, supported stack, tests exist" profile that the decision tree maps to Refactor rather than Rehost or Rebuild. Refactor preserves the recent `net10.0` investment and existing test assets while allowing the Windows-native dependencies, machine-scoped locking, and Windows-auth model to be replaced incrementally rather than thrown away wholesale. This recommendation is explicitly marked as a default guess because production criticality, data volume, compliance requirements, and network connectivity details remain unverified — the Architect should revisit it once §12's open questions are answered.

## 10. Target Azure Candidates

| Order | Service | Fit | Notes |
|-------|---------|-----|-------|
| 1 | Azure Container Apps | medium — **conditional** | Matches the user-requested target and the Refactor path's web+batch shape, but **only after** `wkhtmltopdf.exe`, `WinSCP.exe`, `System.Drawing`, and Windows/Negotiate auth are replaced with Linux/cross-platform equivalents — Azure Container Apps documentation requires linux/amd64 containers; no Windows-container option exists there. Not viable as a direct lift target today. |
| 2 | Azure Container Apps Jobs | low — **conditional** | Only applicable **if** an explicit topology decision splits the batch Generator from the Web app into an independently packaged unit (today the Web app runs the Generator in-process, not via queue/job trigger — §5). Also inherits the same native-dependency blocker as candidate 1 until resolved. |
| 3 | Azure Windows VMs | fallback only | **Not** "Windows containers on Azure Container Apps" (no such option exists) — a true VM-based fallback if native-dependency removal proves infeasible within the program's timeline. Lowest-fit relative to the stated modernize-first priority; listed only as a safety net. |

> No SKU, region, runtime, auth, or database design choices are made here — the Architect picks the final target, SKU, and detailed design once the native-dependency removal scope is decided.

## 11. Required Specialists (for execution)

| Specialist | Why required |
|------------|--------------|
| Architect | Always — owns strategy approval, target selection, and sequencing |
| Azure Specialist | Always — owns ACA-vs-fallback decision and platform fit given the native-dependency blockers in §8 |
| Tester | Always — must validate the existing NUnit/PDF-comparison regression baseline before/after any refactor (test execution was not performed in this discovery — Q12) |
| Scribe | Always — captures decisions and milestone context across the open questions in §12 |
| Coder | Any code change is in scope under Refactor (native-dependency replacement, auth model change, SqlClient modernization) |
| Database Specialist | Data gravity treated conservatively as at least medium (§9 Branch 9); EF6→target DB path, `xp_cmdshell`/`bcp` export dependency, and custom SQL role semantics all require DB-specialist review |
| Security Auditor | Compliance requirements (data protection/residency/retention/audit/encryption) are reported as unknown (§8 `compliance-requirements-unknown`); Windows-auth-to-Entra-ID transition needs identity/consent review; regulated-data classification needs confirmation |
| DevOps Engineer | New CI/CD will be needed for a containerized target, plus the untracked Word-template build dependency (§8) needs pipeline-level resolution |
| Observability Engineer | Recommended (not strictly mandatory by matrix signal) given the native-dependency remediation complexity and the need for cutover-time visibility once a target platform is chosen — scale in as the Architect's plan firms up |
| Cutover Commander | Recommended pending Q7/Q10 answers — production criticality, RTO/RPO, and whether a test environment exists are all currently unknown; add formally once those are confirmed |

## 12. Unresolved Questions

| # | Question | Why it matters | Recommended next probe |
|---|----------|----------------|------------------------|
| Q1 | What is the actual production deployment topology (hosts, IIS config, process layout), and what evidence supports the FDMR `xp_cmdshell`/`bcp` export actually running today? | All topology facts in this dossier are user-reported/README-stated, not independently verified; the export mechanism's real-world behavior (success rate, destination reachability) is unconfirmed | Architect/Azure Specialist to request a production architecture walkthrough or read-only access to deployment documentation from the application owner |
| Q2 | What SQL Server version(s), database size(s), and schema drift exist in production (including the `SITI.SUM.PAFOrganisationName` and `SITI.YF`↔`SITI.Claims` FK discrepancies the owner already flagged)? | Data gravity and migration-tooling choice (DMS vs. simple backup/restore) depend entirely on this; currently rated `unknown`/`low confidence` | Database Specialist to request `SELECT MigrationId, ContextKey, ProductVersion FROM dbo.__MigrationHistory` output and a scripted schema-with-indexes export, per `modernization-findings.md` item 10–11 |
| Q3 | What identity/SSO/role requirements actually apply, given Entra ID is reported available but no consent or design decision has been made? | Entra ID *availability* does not equal a decision to migrate the custom role-inheritance logic currently resolved in the external `Security` SQL database | Security Auditor + Architect to run an identity-mapping workshop with the app owner covering the 7 named CSG roles |
| Q4 | What data residency, retention, audit, and encryption requirements apply to this data (financial/PII-adjacent per the prior classification)? | Currently reported as entirely unknown by the user; blocks any compliant target-platform design | Security Auditor to run a data-classification and compliance-requirements interview with the data owner |
| Q5 | What is the actual on-prem-to-Azure connectivity type (VPN vs. ExpressRoute), routes, DNS resolution, and firewall reachability? | User reports connectivity "available," but type/routes/DNS/reachability were not verified in this discovery and no network probing was performed | Azure Specialist/DevOps Engineer to request a network diagram and run a connectivity test from a representative on-prem host |
| Q6 | What is the actual scheduler trigger mechanism, timezone, overlap handling, and typical run volumes for the Generator batch job? | The named-mutex concurrency guard (§5) implies overlap is a known risk, but the real trigger (Windows Task Scheduler? manual? something else) and volume profile are not in source | Coder/Architect to request the current production scheduling configuration and recent run-volume logs from the app owner |
| Q7 | What are the RTO/RPO, cutover window, and availability of a representative test environment? | Required to size the Refactor rollout plan and decide whether a Cutover Commander gate is mandatory; currently fully unknown | Cutover Commander (once engaged) to run a business-continuity requirements interview with the app owner |
| Q8 | Who owns/provides the master DOCX template files (`ClaimTemplate.docx`, `ClaimTemplateXB.docx`) — including fonts/images/licensing — given they are git-ignored? | Confirmed present locally but absent from a fresh clone/CI checkout (§3 Notes); build/CI cannot proceed reliably without a custody decision | DevOps Engineer to request template-custody and licensing confirmation per `modernization-findings.md` item 14 |
| Q9 | What native dependency replacements (PDF engine, FTP client, image handling) are acceptable, and what PDF-output parity/licensing constraints apply? | wkhtmltopdf.exe/WinSCP.exe/System.Drawing are confirmed Linux/ACA blockers (§8); replacement approach materially changes Refactor scope and Azure Container Apps viability | Architect + Coder to propose 2–3 replacement-library options and validate PDF-output parity against `PdfCompare`'s existing hash-comparison method |
| Q10 | What is this application's production criticality, named business owner, approximate user count, and business acceptance posture for this migration? | None of these could be found in source and none were guessed (§2); required for realistic specialist staffing (e.g., Cutover Commander) and risk framing | Scribe/Architect to request this directly from the organisation (apparent as DEFRA RPA/DDTS per README) in a stakeholder intake session |
| Q11 | What target Azure region, availability requirements, subscription quota, and budget constraints apply? | Cannot be queried without Azure subscription/consent context, which was not available in this discovery session | Azure Specialist to confirm subscription context and run quota/region checks once engaged |
| Q12 | Does the existing NUnit test suite and `PdfCompare` golden-file comparison actually pass today, and is the solution CI/restore-ready? (Node.js tooling version is a non-issue — no production Node role exists, per §4) | Test projects and the comparison tool exist in source (static fact), but none were executed in this discovery pass — "tests exist" is not the same as "tests pass" or "provide adequate coverage" | Tester to run `dotnet restore`/`dotnet build`/`dotnet test` against the solution and execute the README's manual PDF-comparison procedure, then report pass/fail baseline |

## 13. Assumptions

- Assumed the organisation context is DEFRA RPA/DDTS based on README wording ("DEFRA Digital, Data and Technology Services") and the repository's branding — **if wrong**, stakeholder engagement in Q10 would be misdirected; no named business owner is claimed, only this organisational inference.
- Assumed data gravity should be treated as *at least* medium for specialist-staffing purposes only, since actual size is unknown — **if wrong** (data is actually very-large), Branch 9's stronger "Replatform-with-DMS, forbid Rebuild" override would need to be reconsidered, and a Cutover Commander gate would become mandatory rather than recommended.
- Assumed the user's "connectivity to Azure: yes" statement reflects some existing network path, without assuming its adequacy, security posture, or bandwidth — **if wrong** (no usable path actually exists), Phase 3/4 infrastructure and cutover planning would stall until a network-connectivity project is scoped.
- Assumed "tests exist" (NUnit test projects + `[Test]` methods present in source) is sufficient static evidence to satisfy Branch 5's "tests exist" condition for recommending Refactor over Rebuild — **if wrong** (the suite does not currently pass, or coverage is negligible), the Refactor recommendation's regression-safety rationale weakens and Rebuild should be re-evaluated per Q12's findings.
- Assumed Entra ID "availability" is organisational infrastructure availability only, not a decision or consent to migrate the application's custom role-inheritance model to it — **if wrong** (consent/design already agreed elsewhere), Q3 can be closed faster than planned, but the dossier deliberately does not assume this to avoid overstating readiness.

## 14. Handoff Note for the Architect

This dossier recommends **Refactor** as a default-guess strategy under Branch 5 of the decision tree, based on a small (~13,615 LOC), already-`net10.0` codebase with existing tests — but it is explicitly not a final decision. Before proceeding, the Architect should: (1) confirm or override the Refactor recommendation given the native-dependency blockers in §8 and the ⚠ default-guess status in §9; (2) decide whether Azure Container Apps remains the target only after native-dependency removal, or whether an interim Replatform/Windows-VM step is acceptable first; (3) prioritize which of the twelve open questions in §12 must be closed before Phase 1 planning proceeds (Q1, Q2, Q4, and Q9 are the highest-impact gaps); and (4) note that six separate integration guides (`sql-server`, `winscp`, `ftp`, `smtp`, `file-share`, `active-directory`) are being authored independently under `.github/skills/` and should be consulted once available for the specific replacement/compatibility details behind each native dependency and integration flagged in §7–§8.

---

**Next commands:**
- `/phase1-plan` (primary path)
- `/build-migration-plan` (Architect-skill alternative, if preferred)
- `@agent open Migration-Orchestrator`
