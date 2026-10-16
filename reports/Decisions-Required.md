# Decisions Required — RPA Claim Statements (Claim Statement Generator / CSG)

> **This file lists the major architecture decisions the Code Migration Modernization Agent needs from you before it can do its work.**
> Each section has options laid out with tradeoffs. Pick one, fill in your rationale, and the agent will execute against your choice.
> **The agent will NOT proceed past gates with PENDING decisions.** This is by design — see
> [`../.github/skills/migration-decisions/references/decision-hardstop.md`](../.github/skills/migration-decisions/references/decision-hardstop.md).
>
> This file was generated against the **closed, 18-item** decision catalog at
> [`../.github/skills/migration-decisions/references/decision-catalog.md`](../.github/skills/migration-decisions/references/decision-catalog.md).
> No decision IDs were added, removed, or renumbered. Every default guess below is grounded only in evidence already
> recorded in `reports/Capability-Matrix.yaml`, `reports/Discovery-Dossier.md`, and `reports/Decision-Log.md` — no
> production system, database, or Azure subscription was probed to produce this file.

**Generated:** 2026-10-16 by Phase 1 — Plan & Assess (Step A only — requirement gathering, not yet Step B analysis)
**Reconciled:** 2026-10-16 — user-edited checklist re-read and applied (see per-decision "Status note" entries below and `reports/Decision-Log.md`)
**Capability Matrix:** [`Capability-Matrix.yaml`](./Capability-Matrix.yaml)
**Discovery Dossier:** [`Discovery-Dossier.md`](./Discovery-Dossier.md)
**Decision Log:** [`Decision-Log.md`](./Decision-Log.md) (immutable history)
**Risk context:** [`../.github/skills/risk-low-evidence-confidence.md`](../.github/skills/risk-low-evidence-confidence.md)
 — this application carries the `low-evidence-confidence` risk flag; several guesses below are additionally qualified by that risk.

## ⏸ Deferral is permitted — read this before answering anything

You do **not** have to answer all 18 decisions in this turn, or even this session. You may explicitly say
**"fill this file in later"** (or equivalent) and the agent will leave every `⏸ PENDING` row exactly as-is and move on
to other non-gated work. **Invocation of `/phase1-plan` itself is not deferral, and silence is not deferral.** The
agent will not interpret "the user didn't object" as acceptance of any default guess — every guess below is marked
`⚠ Default guess` precisely because it is **not** a selection. **Update (2026-10-16):** the user has since edited
this file directly and ticked real selections for 12 of the 18 decisions (see the Status summary below); those
checkboxes are the user's own choices, not agent-inferred defaults, and the `⚠ Default guess` text is retained
underneath each decided item purely as historical, now-superseded, non-binding context. Decisions not yet answered
remain untouched and still carry no pre-ticked box. Phase 2, Phase 3, Phase 4, and database-migration work remain
blocked on the real items in the Status summary below until you decide, waive, or genuinely confirm N/A.

## Status summary

| # | ID | Decision | Status | Required for |
|---|----|----------|--------|---------------|
| 1 | D-01 | Target framework / runtime version | ✅ DECIDED 2026-10-16 | Phase 2 |
| 2 | D-02 | UI architecture | ✅ DECIDED 2026-10-16 | Phase 2 |
| 3 | D-03 | Backend / API style | 🚫 N/A — current scope (refactor/preserve) | Phase 2 (rearchitect/rebuild only) |
| 4 | D-04 | Database engine | ✅ DECIDED 2026-10-16 (superseded same-day — **Azure SQL Database**, not PostgreSQL) | Phase 2, Phase 3, DatabaseMigration |
| 5 | D-05 | Database migration tool | 🚫 N/A — current code-only scope (clarified) | DatabaseMigration |
| 6 | D-06 | Hosting platform | ✅ DECIDED 2026-10-16 | Phase 3 |
| 7 | D-07 | IaC tool | ⏸ PENDING — deferred outside code-only scope | Phase 3 |
| 8 | D-08 | Region & data residency | ⏸ PENDING — deferred outside code-only scope | Phase 3, Phase 4 |
| 9 | D-09 | Authentication | ✅ DECIDED 2026-10-16 | Phase 2, Phase 3 |
| 10 | D-10 | Multi-tenancy approach | ✅ DECIDED 2026-10-16 (scope-conditional) | Phase 3 |
| 11 | D-11 | Compliance scope | ✅ DECIDED 2026-10-16 | Phase 1 (constrains every later phase) |
| 12 | D-12 | Cost ceiling | ⏸ PENDING — deferred outside code-only scope | Phase 3, Phase 4 |
| 13 | D-13 | DR — RPO / RTO targets | ⏸ PENDING — deferred outside code-only scope | Phase 3, Phase 4 |
| 14 | D-14 | Cutover strategy | ⏸ PENDING — deferred outside code-only scope | Phase 4 |
| 15 | D-15 | Acceptable downtime | ⏸ PENDING — deferred outside code-only scope | Phase 4 |
| 16 | D-16 | CI/CD platform | ✅ DECIDED 2026-10-16 | Phase 5 |
| 17 | D-17 | Observability stack | ✅ DECIDED 2026-10-16 | Phase 6 |
| 18 | D-18 | Container registry | ✅ DECIDED 2026-10-16 | Phase 3 (container-based hosting only) |

**Count (as of 2026-10-16 supersession pass):** 10 `✅ DECIDED` (D-01, D-02, D-04, D-06, D-09, D-10, D-11, D-16,
D-17, D-18 — D-04's target database engine is **Azure SQL Database**, superseding an earlier same-day PostgreSQL
selection; see Decision 4's current status note), 2 `🚫 N/A` (D-03 — scope-conditional on refactor-vs-rearchitect;
D-05 — scope-conditional on code-only vs. data-migration-in-scope, unaffected by the D-04 engine supersession), 6
`⏸ PENDING` (D-07, D-08, D-12, D-13, D-14, D-15 — all explicitly deferred by the
user as outside the current code-only scope, not unanswered gaps and not production N/A determinations). This
deferral is what allows Phase 1 Step B assessment/build work to proceed on the code-only surface; it is **not** a
gate waiver for later Phase 3 (Generate Infra) or Phase 4 (Deploy to Azure) work, which remains blocked on these
six items regardless. See `reports/Decision-Log.md` for the immutable record of the reconciliation and supersession
passes.

---

## Decision 1: Target framework / runtime version

**ID:** D-01
**Status:** ✅ DECIDED 2026-10-16
**Required for:** Phase 2 — Migrate Code
**Catalog reference:** [`decision-catalog.md#D-01-target-framework--runtime-version`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking

The catalog's generic `.NET` option table assumes a migration *from* .NET Framework 4.x. That assumption does not hold
here: `reports/Capability-Matrix.yaml#stack` records `runtime_version_detected: net10.0` at `high` evidence confidence
— both the Generator and Web projects already target `net10.0` directly in their `.csproj` files. Framing this
decision around "should we upgrade off Framework 4.x" would misrepresent the current state. The decision that
actually remains open is whether to **keep** the current `.NET 10` target or move to a **different** supported
runtime for a specific reason (team standardization, a dependency that lags on .NET 10, a deliberate choice to track
a different support lane). This decision is not settled by the fact that the source already targets .NET 10 — that
is a fact about the code, not an approval of it as the migration target.

### Your options

| # | Option | When to pick | Tradeoffs / risks |
|---|--------|--------------|-------------------|
| 1 | **Stay on .NET 10 (current, already targeted)** | No reason to move off the runtime already in source; minimizes additional change | Confirm .NET 10's own support lane (LTS/STS) and end-of-support date against Microsoft's published lifecycle before relying on it long-term — this file does not assert that date |
| 2 | **Move to .NET 8 LTS** | Team wants a widely-adopted LTS with a longer field track record | This is a **downgrade** from the current source; do not treat ".NET 8 is LTS" as proof it is still within active support — verify the current date against the lifecycle page before selecting |
| 3 | **Move to .NET 9** | Only if there is a specific, named reason (e.g., a dependency not yet certified on .NET 10) and a concrete follow-up upgrade plan | .NET 9 is a shorter-support-window release relative to LTS releases; do not select this as a "safer" default without a stated reason |
| 4 | **Other supported runtime the user specifies** | Team has a standardization requirement not listed above | Tell us in "Your rationale"; the agent will not substitute a different runtime without this being the selected option |

### Our default guess

⚠ **Default guess: Option 1 (Stay on .NET 10)**, based only on `stack.runtime_version_detected: net10.0` and
`stack.evidence_confidence: high` in the Capability Matrix — the current source already targets this runtime, so
remaining on it is the lowest-disruption path. This is **not** a claim that .NET 8 or .NET 9 are inferior, current,
or non-current — this file does not independently verify any .NET version's live support window, and no version in
this options table should be advertised as "currently supported" without that check. The right answer still depends
on team skills, support-window requirements, and dependency compatibility we cannot see from source alone.

### Your decision

- [X] Option 1 — Stay on .NET 10
- [ ] Option 2 — Move to .NET 8 LTS
- [ ] Option 3 — Move to .NET 9
- [ ] Other: ___________________

### Your rationale

<Not provided by user; choice selected in edited checklist (2026-10-16). An optional shared rationale can be added
at any time — none is assumed or invented here.>

### Dependencies

This decision constrains:
- Decision 6 (Hosting platform) — some Azure hosting options have runtime-version constraints.

This decision depends on:
- None.

---

## Decision 2: UI architecture

**ID:** D-02
**Status:** ✅ DECIDED 2026-10-16
**Required for:** Phase 2 — Migrate Code
**Catalog reference:** [`decision-catalog.md#D-02-ui-architecture`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking

`reports/Capability-Matrix.yaml#workload` records `primary_pattern: webapp` with an ASP.NET Core MVC administrative
UI (`RPA.ClaimStatements.Web`) at `high` evidence confidence. A UI rewrite is months of work and re-trains the team
on a new mental model; for a migration whose recommended strategy is `refactor` (⚠ Default guess, not yet approved —
see `migration_strategy` in the matrix), keeping the existing UI is usually the right call unless there is a
specific reason to modernize the front end.

### Your options

| # | Option | When to pick | Tradeoffs / risks |
|---|--------|--------------|-------------------|
| 1 | **Keep existing UI (ASP.NET Core MVC)** | Migration is infra/compatibility-focused; the current admin UI works | No frontend modernization; stays server-rendered |
| 2 | **Razor Pages (light modernization)** | Team is comfortable with Razor and wants a lighter pattern than full MVC | Still server-rendered; limited interactivity gain |
| 3 | **Blazor Server / WebAssembly** | Want a C#-everywhere SPA-like experience | Newer ecosystem; some library gaps |
| 4 | **React / Angular / Vue (full SPA rewrite)** | Team wants a modern SPA and has the frontend skills/budget | Full rewrite cost; not aligned with a `refactor`-scope migration unless the strategy itself changes |
| 5 | **No UI (API/batch-only)** | N/A for this app — a UI is in active use | Does not apply here; the Web project is a real administrative surface, not a stub |

### Our default guess

⚠ **Default guess: Option 1 (Keep existing UI)**, based on `workload.primary_pattern: webapp` plus the
`migration_strategy.recommendation: refactor` default guess already recorded in the matrix — refactor-scope work
targets compatibility and modernization of dependencies, not a UI rewrite. If the migration strategy itself changes
(rearchitect/rebuild), this guess should be revisited together with Decision 3.

### Your decision

- [X] Option 1 — Keep existing UI (ASP.NET Core MVC)
- [ ] Option 2 — Razor Pages
- [ ] Option 3 — Blazor Server / WebAssembly
- [ ] Option 4 — Full SPA rewrite (React/Angular/Vue)
- [ ] Other: ___________________

### Your rationale

<Not provided by user; choice selected in edited checklist (2026-10-16). An optional shared rationale can be added
at any time — none is assumed or invented here.>

### Dependencies

This decision constrains:
- Decision 18 (Container registry) — only if a frontend rewrite changes the containerization approach.

This decision depends on:
- Decision 1 (Target framework / runtime version).

---

## Decision 3: Backend / API style

**ID:** D-03
**Status:** 🚫 N/A — current refactor scope (not rearchitect/rebuild); re-open if scope changes
**Required for:** Phase 2 — Migrate Code (catalog scopes this decision to rearchitect/rebuild work only)
**Catalog reference:** [`decision-catalog.md#D-03-backend--api-style`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking (and why this is N/A right now)

The catalog itself scopes D-03 to **"only when rearchitect / rebuild."** The current `migration_strategy.recommendation`
in `reports/Capability-Matrix.yaml` is `refactor` (⚠ Default guess, not yet Architect/user-approved) — not
rearchitect or rebuild. Under the current scope, the Web project's in-process invocation of the Generator's shared
logic (`workload` notes: "Web runs Generator logic in-process, not through a queue or a separate deployed worker")
and the existing HTTP/MVC contract are both **preserved as-is**, not redesigned into a new API style (REST/OpenAPI,
GraphQL, gRPC, etc.). There is nothing for the user to decide here *under the current scope* — this is a scope-gated
non-applicability, not a resolved architecture choice.

### Scope condition that would re-open this decision

If the migration strategy recommendation changes from `refactor` to `rearchitect` or `rebuild` (a decision that
belongs to the user/Architect, not this file), **re-open Decision 3** and present the full catalog option table
(keep existing API surface / REST+OpenAPI / GraphQL / gRPC / Minimal API) at that time. Until that scope change is
made explicit and approved, this status remains `🚫 N/A`, not `⏸ PENDING` and not `✅ DECIDED`.

### Your options

Not presented — see scope condition above. If you want this decision opened now, say so explicitly and the full
catalog option table will be filled in on your next turn.

### Our default guess

Not applicable — no guess is offered for an N/A decision.

### Your decision

- [x] N/A — current refactor scope; backend/API surface preserved as-is; re-open only if migration strategy changes
- [ ] Reopen this decision now (request the full option table)

### Your rationale

<Not yet provided — this box only needs a rationale if you choose to reopen.>

### Dependencies

This decision constrains:
- None under the current N/A scope.

This decision depends on:
- The (separate, user/Architect-owned) migration strategy decision recorded in `migration_strategy.recommendation`.

---

## Decision 4: Database engine

**ID:** D-04
**Status:** ✅ DECIDED 2026-10-16 (superseded same-day — target is **Azure SQL Database**, not PostgreSQL; see the
"CURRENT, authoritative" status note below. The two notes above it are historical/superseded.)
**Required for:** Phase 2 — Migrate Code, Phase 3 — Generate Infra, DatabaseMigration
**Catalog reference:** [`decision-catalog.md#D-04-database-engine`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking

This is, per the catalog's own framing, the most consequential decision — it sticks for 5+ years and affects ops
cost, hiring, query rewrites, and tooling. `reports/Capability-Matrix.yaml#data` confirms `primary_datastore:
sql-server` at `high` confidence (EF6 context, confirmed engine), but `evidence_confidence: low` overall because
production version, size, backup state, and schema drift are unverified (Q2, blocking). Crucially, the matrix also
records that the FDMR export path **invokes `xp_cmdshell` and `BCP` directly on the database host** — this is a
host/filesystem-level dependency, not an ordinary query.

### Why this departs from the catalog's generic recommendation logic

The catalog's recommendation logic says: *"If `data.primary_engine == 'sql-server'` → suggest Option 2 (Azure SQL
Database)."* We are **not** following that generic suggestion here, because Azure SQL Database (and even Managed
Instance, in the general case) does not automatically grant `xp_cmdshell`/BCP-style host-level filesystem access the
way an on-prem or VM-hosted SQL Server instance can. **Do not infer that Managed Instance "solves" this by providing
host shell/filesystem access** — MI's `xp_cmdshell` support is deliberately constrained and sandboxed, not equivalent
to on-prem host access, and this has not been verified against the actual FDMR export requirement. Until the FDMR
export's real dependency on host-level file/OS access is understood (a database-owner-level question, not a code
question), the conservative guess is to **retain the existing SQL Server engine initially** rather than jump to a
PaaS engine recommendation that may silently break that export path.

### Your options

| # | Option | When to pick | Tradeoffs / risks |
|---|--------|--------------|-------------------|
| 1 | **Stay on current DBMS (SQL Server, rehost on Azure VM/on-prem)** | Pure lift; preserves `xp_cmdshell`/BCP host-level behavior without re-validating it | No PaaS benefits; ops burden remains; still needs host-level identity/patching |
| 2 | **Azure SQL Database** | Coming from SQL Server, once FDMR's host dependency is independently re-platformed or removed | Closest fidelity for T-SQL, but `xp_cmdshell`/BCP host-level behavior is not available the same way — this is a **blocking compatibility question**, not a minor tradeoff |
| 3 | **Azure SQL Managed Instance** | Coming from SQL Server with cross-DB or SQL Agent dependencies, if FDMR's host dependency is confirmed compatible with MI's constrained `xp_cmdshell` | Higher cost; MI's `xp_cmdshell` support is sandboxed and does **not** equal on-prem host/filesystem access — verify, do not assume |
| 4 | **Other engine (PostgreSQL / MySQL / Cosmos / multi-engine)** | Only if schema rewrite is already in scope | Not currently suggested; the source engine is confirmed SQL Server with no evidence of a planned engine change |

### Our default guess

⚠ **Default guess: Option 1 (Stay on current SQL Server, conservative)**, based on `data.primary_datastore:
sql-server` plus the explicit FDMR `xp_cmdshell`/BCP host-level evidence in `data.schema_highlights`. This
intentionally **departs** from the catalog's generic "SQL Server → Azure SQL Database" suggestion because that
generic logic does not account for the host-level export dependency found in this codebase. This guess is not a
recommendation to abandon PaaS — it is a statement that the FDMR export path must be independently assessed (by
Database Specialist, with the database owner) before a PaaS engine can be safely selected.

### Your decision

- [ ] Option 1 — Stay on current SQL Server
- [X] Option 2 — Azure SQL Database
- [ ] Option 3 — Azure SQL Managed Instance
- [ ] Other: Azure PostgreSQL

### Status note (2026-10-16 reconciliation) — **HISTORICAL / SUPERSEDED, see current note below**

> ⚠ **This note describes a prior PostgreSQL selection that the user has since reversed (see the authoritative
> current note below). Retained verbatim for audit history only — it does not describe the current decision.**

The user has selected "Other: Azure PostgreSQL" — a stated **target-engine preference**, departing from this file's
conservative stay-on-SQL-Server default guess above. This remains **⏸ PENDING**, not ✅ DECIDED, for two reasons:
(1) "Azure PostgreSQL" could refer to more than one distinct Azure service (e.g., Azure Database for PostgreSQL —
Flexible Server vs. Azure Cosmos DB for PostgreSQL / Citus) with different tradeoffs, and the exact service has not
been confirmed; (2) this is an **engine change**, not merely a hosting change, and it has not been silently approved
as "Azure Database for PostgreSQL — Flexible Server" on the user's behalf. **This decision does not overwrite the
Capability Matrix's recorded source-of-truth fact** — `data.primary_datastore: sql-server` remains accurate as the
*current, confirmed source engine*; this selection records a *target* preference only. The FDMR `xp_cmdshell`/BCP
host-level dependency and the separate SQL-role authorization database noted above have not been re-evaluated
against a PostgreSQL target and remain open questions (see Decision 5's status note).

### Status note (2026-10-16 clarification) — **HISTORICAL / SUPERSEDED, see current note below**

> ⚠ **This note also describes the prior PostgreSQL selection. Retained verbatim for audit history only — it does
> not describe the current decision.**

The user has since clarified: **"another team will provide this"** — i.e., the target PostgreSQL schema/data is an
externally-owned responsibility, not something this engagement's code-focused work must produce. On that basis this
decision moves to **✅ DECIDED**: the engine preference is PostgreSQL / Azure PostgreSQL, and the application's code
work will adapt against whatever interface contract the external team provides. This DECIDED status covers the
**engine choice only** — it does **not** silently select a specific SKU/tier, region, or other deployment/
configuration setting for the managed PostgreSQL service; those remain the external team's responsibility to
specify, and nothing here invents them. An interface contract (connection details, schema shape, auth model) is
still needed from that external team before Phase 2 code changes can be implemented, but **its absence today is not
a blocker to this code-focused assessment/build pass** — it is tracked as a dependency, not a gate on D-04 itself.
The original "exact service name unconfirmed" caveat above is retained as historical context; the engine-level
choice (PostgreSQL) is what is now decided, while finer-grained service selection remains the external team's call.

### Status note (2026-10-16 supersession — CURRENT, authoritative)

The user has since edited this file directly a further time and reversed the earlier selection: the checklist above
now shows **[X] Option 2 — Azure SQL Database** ticked, and **"Other: Azure PostgreSQL" unchecked**. The user
stated explicitly: **"It will be Azure SQL Database, not postgresql."** This is a genuine engine-choice supersession,
not a wording correction — the two PostgreSQL status notes above are **retained, unmodified, as historical record**
of what was decided and why between the first reconciliation pass and this supersession; they must not be read as
describing the current target. **D-04 is ✅ DECIDED: the target database engine is Azure SQL Database (Option 2 —
not Azure SQL Managed Instance, and not PostgreSQL).**

This supersession does **not** reopen or change the current code-only modernization scope confirmed earlier (D-05
remains 🚫 N/A — the external team still owns schema/data provisioning for whichever engine is targeted; only the
*engine* changed, not who supplies the schema/data or whether schema/data work is in this engagement's scope). It
also does **not** silently resolve the database-engine-specific findings carried in
`reports/Application-Assessment-Report.md` and `reports/Migration-Plan.md` — those documents are being updated in
this same pass to reflect Azure SQL Database as the target, including a reassessment of which SQL-Server-coupled
code paths can plausibly be **retained** (same engine family) versus which still require explicit validation:

- **Likely retainable, subject to validation (not an engine-rewrite requirement):** EF6 with its existing SQL Server
  provider, `System.Data.SqlClient` usage, T-SQL syntax (including the `[Extent1]`-alias `DeleteWhere<T>` pattern),
  `SqlParameter`/`SqlBulkCopy`-based ingestion, and EF6 code-based migrations' SQL generator — these do **not**
  inherently require an ORM/provider swap or a rewrite to `Microsoft.Data.SqlClient`/EF Core, because the engine
  family (SQL Server → Azure SQL Database) has not changed. Retention remains the default; migrating to EF Core or
  `Microsoft.Data.SqlClient` is a **separate, still-optional** modernization decision requiring its own approval and
  evidence, not an automatic consequence of this engine confirmation.
- **Still requires explicit compatibility validation against Azure SQL Database specifically (ordinary SQL Server
  compatibility is not blanket Azure SQL Database/PaaS compatibility):** cross-database queries, linked servers, the
  separate SQL-role "Security" database's stored procedures, SQL Agent-style scheduled jobs (if any), and any
  Windows/integrated-security-based connection pattern — Azure SQL Database does not support cross-database queries
  or SQL Server Agent the same way a full SQL Server instance does, and it does not support Windows-integrated
  ("Integrated Security=true" trusted-connection) authentication; it requires SQL authentication or Entra
  ID/managed-identity token-based authentication instead.
- **Cannot run as currently written — database-host-level dependency, not a client code-portability question:** the
  FDMR export's `xp_cmdshell` + `bcp` pattern (`LoadExtract.sql:125-127`) runs an OS-level command **on the database
  server host itself**. Azure SQL Database is a PaaS engine with no accessible host OS and does not support
  `xp_cmdshell` at all (not even in a constrained/sandboxed form — that distinction applies to Managed Instance, not
  Azure SQL Database, and D-04 explicitly selected Azure SQL Database, not Managed Instance). This is an
  architecture/ownership question for the external schema/data team: the export must be redesigned as a
  client-driven export (e.g., an application- or tooling-side process that runs `bcp`/equivalent against the
  database **as a client**, not a server-side `xp_cmdshell` invocation) or replaced with an Azure-native export
  mechanism. **Do not conflate a client-run `bcp`/export tool with the current server-host `xp_cmdshell` pattern —
  they are not equivalent and the current code does the latter.** This is not resolved by this supersession; it
  remains an explicit validation item (see updated Application-Assessment-Report backlog).
- **Authentication to the database remains gated, not decided by this supersession alone:** "integrated security"
  (Windows-integrated/trusted connection) as currently configured is not available against Azure SQL Database.
  Moving to Entra ID/managed-identity token-based SQL access is an approved **design direction** (consistent with
  D-09/D-10's Entra ID selection and this agent's managed-identity guardrails), but the specific token-acquisition
  and connection-string wiring is implementation work still gated by Phase 2, not retroactively decided here, and it
  is **not** a credential/connection-string change — it is a token-based access design that still needs its own
  validation against the external schema/data team's actual provisioning.
- **Not decided by this supersession (unchanged from before):** SKU/tier, service objective, region, network
  topology (private endpoint vs. public), storage size, and DR/backup configuration for the Azure SQL Database
  instance — those remain the external team's/Architect's responsibility and are not invented here; D-08, D-12,
  D-13, D-14, D-15 remain ⏸ PENDING as before, unaffected by this engine supersession.

### Your rationale

<Not provided by user; choice selected in edited checklist (2026-10-16, reversed from an earlier PostgreSQL selection
on the same date). The user's own words for the supersession: "It will be Azure SQL Database, not postgresql." No
further rationale is assumed or invented here.>

### Dependencies

This decision constrains:
- Decision 5 (Database migration tool) — unaffected: still 🚫 N/A under the current code-only scope regardless of
  engine, since the external team continues to own schema/data provisioning for whichever engine is targeted.
- Decision 6 (Hosting platform) — some hosting choices pair naturally with specific database engines; Azure SQL
  Database pairs naturally with the already-decided Azure Container Apps hosting target.

This decision depends on:
- Resolution of Q2 (production SQL Server facts) and Q9 (Linux-compatible native dependency paths, since FDMR's
  `xp_cmdshell`/BCP dependency sits adjacent to the same host-level portability question — now sharpened, not
  resolved, by the Azure SQL Database engine confirmation: Azure SQL Database has no accessible host OS at all).

---

## Decision 5: Database migration tool

**ID:** D-05
**Status:** 🚫 N/A — current code-only scope (clarified 2026-10-16); re-open if scope changes
**Required for:** DatabaseMigration
**Catalog reference:** [`decision-catalog.md#D-05-database-migration-tool`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking

The right migration tool depends entirely on Decision 4 (database engine) and on facts not yet available — production
database size, backup cadence, and acceptable downtime (Decision 15) all shape whether an online tool (Azure DMS) or
a manual/offline approach (scripts, BCP-based export/import) is appropriate.

### Your options

| # | Option | When to pick | Tradeoffs / risks |
|---|--------|--------------|-------------------|
| 1 | **Manual scripts + existing BCP-based export (stay as-is)** | Lowest disruption; the codebase already invokes BCP for FDMR exports, so a scripted approach extends an existing, understood pattern | Manual checkpointing; cutover risk; does not scale well to large datasets |
| 2 | **Azure DMS (Database Migration Service)** | If Decision 4 selects an Azure PaaS SQL engine and downtime tolerance (Decision 15) requires minimal-downtime migration | Not usable if Decision 4 stays on Option 1 (VM-hosted SQL Server) |
| 3 | **DMA (Data Migration Assistant)** | Useful as a pre-migration assessment step regardless of final engine choice | Assessment only, not a migration executor |
| 4 | **Database compare/sync tools (Red Gate, Flyway, etc.)** | If an incremental, CDC-based sync is needed during a long parallel-run cutover | Vendor/OSS license and ops overhead |

### Our default guess

⚠ **Default guess: Option 1 (Manual scripts / existing BCP-based pattern)**, based on the same FDMR BCP evidence used
in Decision 4 and the fact that Decision 4 itself defaults to "stay on current SQL Server." This is a placeholder
guess only — it should be revisited the moment Decision 4 is actually decided, since a different engine choice could
make Azure DMS (Option 2) the better fit.

### Your decision

- [ ] Option 1 — Manual scripts / existing BCP-based pattern
- [ ] Option 2 — Azure DMS
- [ ] Option 3 — DMA (assessment only)
- [ ] Option 4 — Database compare/sync tools
- [X] Other: No database migration, only code changes.

### Status note (2026-10-16 reconciliation) — **HISTORICAL, written when D-04 target was PostgreSQL; see current note below**

The user has selected "Other: No database migration, only code changes." This is recorded as a **scope statement**,
not a cancellation of data-migration responsibility, and remained **⏸ PENDING** until two things were clarified: (1)
who supplies the schema/data for the PostgreSQL target named in Decision 4's selection, if that engine change
proceeds — this file does not assume an external team or process already owns that; (2) what happens to the
separate SQL-role authorization database and the FDMR export's `xp_cmdshell`/BCP path (`reports/Capability-
Matrix.yaml#data`) if the primary engine changes — this selection does **not** state that those are also out of
scope, and this file does not assume they are cancelled.

### Status note (2026-10-16 clarification — resolves the above to N/A for current scope) — **HISTORICAL, written when D-04 target was PostgreSQL; see current note below**

The user has since clarified: **"another team will provide this"** (schema/data ownership) and confirmed this is a
**"code-only modernization."** On that basis, D-05 moves to **🚫 N/A for the current code-only scope** rather than
staying PENDING: no database schema/data provisioning, conversion, or movement work is in scope for this engagement
— the external team supplies the PostgreSQL schema/data, and this application's code adapts against whatever
contract that team provides. This is **scope-conditional, not a cancellation of the separate SQL-role authorization
database or FDMR `xp_cmdshell`/BCP boundary** — those boundaries still need to be planned jointly with the external
team providing the new schema/data; this file does not assume today that a compatible schema already exists, nor
does it assume the SQL-role database or FDMR export path have been addressed. If the scope later expands to include
actual data/schema migration work, re-open D-05 and select a real migration-tool option from the table above.

### Status note (2026-10-16 engine supersession — CURRENT, authoritative)

D-04 has since been superseded same-day: the target database engine is now **Azure SQL Database**, not PostgreSQL
(see Decision 4's "CURRENT, authoritative" status note). **D-05's status and scope statement are unaffected by this
engine change** — D-05 remains **🚫 N/A for the current code-only scope** regardless of which engine D-04 names: the
external team still owns schema/data provisioning for Azure SQL Database, exactly as it did for the earlier
PostgreSQL selection, and this engagement still performs no schema/data provisioning, conversion, or movement work.
The two notes above are retained as historical record of the reasoning at the time D-04 named PostgreSQL; read
"PostgreSQL" in those notes as describing that prior, now-superseded target, not the current one. The separate
SQL-role authorization database and the FDMR `xp_cmdshell`/BCP boundary still need to be planned jointly with the
external team — this is, if anything, a sharper question against Azure SQL Database than it was against PostgreSQL,
since Azure SQL Database has no accessible host OS at all (see Decision 4's current note).

### Your rationale

<Not provided by user; choice selected in edited checklist (2026-10-16). An optional shared rationale can be added
at any time — none is assumed or invented here.>

### Dependencies

This decision constrains:
- None directly.

This decision depends on:
- Decision 4 (Database engine) — the tool choice is meaningless until the target engine is fixed.
- Decision 15 (Acceptable downtime).

---

## Decision 6: Hosting platform

**ID:** D-06
**Status:** ✅ DECIDED 2026-10-16
**Required for:** Phase 3 — Generate Infra
**Catalog reference:** [`decision-catalog.md#D-06-hosting-platform`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking

`reports/Capability-Matrix.yaml#compatibility` records `requested_target: Azure Container Apps` with `ready_as_is:
false`. This was a **user-requested candidate during Discovery**, not an approved target — the matrix explicitly
lists this as "conditional on Linux/native/identity/state remediation and region/quota validation, not
deployment-ready," and `migration_strategy.target_azure_candidates` carries it at `fit: medium` alongside Azure
Container Apps Jobs and Azure Virtual Machines as a Windows-compatible fallback. Several native Windows dependencies
(`wkhtmltopdf.exe`, `WinSCP.exe`, `System.Drawing.Common`, Windows/Negotiate auth, a machine-scoped named mutex) are
confirmed incompatible with Container Apps' Linux-only container requirement as-is.

### Your options

| # | Option | When to pick | Tradeoffs / risks |
|---|--------|--------------|-------------------|
| 1 | **Azure Virtual Machines (Windows-compatible fallback, stay closest to current)** | If native Windows dependencies cannot be ported to Linux in acceptable time/risk | Highest ops burden; lowest modernization value; cheapest path to "it just runs" |
| 2 | **Azure Container Apps (user-requested candidate)** | Once native dependencies are remediated (Linux-compatible PDF/transfer/drawing paths — Q9), identity path is validated, and the machine-scoped mutex is replaced with cross-replica-safe coordination | Requires completing the remediation work the matrix flags as outstanding; not deployment-ready today |
| 3 | **Azure Container Apps Jobs** | Only if an explicitly approved, independently packaged batch boundary is carved out of the current in-process Web→Generator invocation | Current Web invokes generation in-process; this requires an architecture change, not just a hosting change |
| 4 | **Azure App Service** | If the team wants PaaS simplicity for the webapp half specifically, and native dependencies are resolved | Partial container support; some runtime-version constraints apply |

### Our default guess

⚠ **Default guess: Option 2 (Azure Container Apps)**, carried forward **because the user already requested it during
Discovery**, not because this file is introducing a new, unapproved hosting choice. This guess is explicitly
**conditional** — per the matrix, it requires remediation of the native Windows dependencies, a validated identity
path, and quota/region validation (Q9, Q11) before it is deployment-ready. If, after reviewing the remediation scope,
the user prefers to stay on Option 1 (Azure VMs) rather than invest in that remediation, that is equally valid and
does not need separate justification — Option 1 is the lowest-disruption path in this table.

### Your decision

- [ ] Option 1 — Azure Virtual Machines
- [X] Option 2 — Azure Container Apps (user-requested, conditional)
- [X] Option 3 — Azure Container Apps Jobs, but only if needed to make it run on azure as addition to Azure Container Apps
- [ ] Option 4 — Azure App Service
- [ ] Other: ___________________

### Status note (2026-10-16 reconciliation)

The user has checked **both** Option 2 (Azure Container Apps) and Option 3 (Azure Container Apps Jobs), with the
user's own qualifying text on the Option 3 line limiting it to "only if needed ... as addition to Azure Container
Apps." This is recorded as **primary = Azure Container Apps (Option 2)**, with Azure Container Apps Jobs (Option 3)
as a **conditional secondary** — to be used only if the native-dependency remediation work (wkhtmltopdf/WinSCP/
System.Drawing/mutex, Q9) actually demonstrates that part of the workload must be split into a separately-packaged
batch boundary. **This is not a blanket approval to decompose the existing in-process Web→Generator invocation.**
The current in-process contract is preserved until an explicit, separately validated proposal (reviewed against Q9
and the remediation scope) recommends otherwise. The remediation conditions noted in the default guess below still
apply before either option is deployment-ready.

### Status note (2026-10-16 clarification)

The user has since confirmed this engagement is a **"code-only modernization"**. This decision's DECIDED status
(primary Azure Container Apps, conditional Azure Container Apps Jobs) records a **target-compatibility choice for
code purposes only** — it does **not** authorize any actual Azure resource provisioning or an automatic topology
change. No Container Apps environment, job, registry, or networking resource is created as a result of this
decision; that remains Phase 3/4 work, gated separately by the still-PENDING items below (region, budget, DR,
cutover, downtime). The "no blanket worker-decomposition approval" caveat above still applies unchanged.

### Your rationale

<Not provided by user; choice selected in edited checklist (2026-10-16). An optional shared rationale can be added
at any time — none is assumed or invented here.>

### Dependencies

This decision constrains:
- Decision 7 (IaC tool) — every host has best-fit IaC modules.
- Decision 18 (Container registry) — only relevant if a container-based option is selected.

This decision depends on:
- Decision 1 (Target framework / runtime version).
- Resolution of Q9 (Linux-compatible native dependency paths) and Q11 (region/quota/budget) — **note (2026-10-16):**
  this decision is now ✅ DECIDED (the platform choice itself has been made by the user), but Q9/Q11 still gate
  whether the decided platform is actually *deployment-ready*; Phase 3/4 implementation work remains blocked on
  those answers regardless of this decision's status.

---

## Decision 7: IaC tool

**ID:** D-07
**Status:** ⏸ PENDING — explicitly deferred outside current code-only scope (see status notes below)
**Required for:** Phase 3 — Generate Infra
**Catalog reference:** [`decision-catalog.md#D-07-iac-tool`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking

No evidence of an existing infrastructure-as-code tool was found anywhere in this repository (no `.bicep`,
`.tf`, `template.json`, or Pulumi project files were part of the discovery evidence). The catalog's recommendation
logic says to suggest the team's existing IaC tool if there is one — there is none established here, so **no change
is assumed by default.**

### Your options

| # | Option | When to pick | Tradeoffs / risks |
|---|--------|--------------|-------------------|
| 1 | **No change / not yet established (stay-as-is)** | No IaC tool exists today; defer the choice rather than invent a prior standard | Phase 3 cannot generate infrastructure until a tool is picked — this is a placeholder, not a long-term answer |
| 2 | **Bicep** | Azure-only team, want Microsoft-native authoring | Multi-cloud impossible; this guess is offered only because Azure is the requested target, **not** an assumption that the team is Azure-only |
| 3 | **Azure Verified Modules (AVM) via Bicep** | Want Microsoft-supported, pre-built modules on top of Bicep | Subset of Bicep; same tradeoffs as Option 2 |
| 4 | **Terraform** | Multi-cloud estate or existing Terraform team/tooling elsewhere | No evidence either way in this repo — ask if this applies |
| 5 | **azd templates** | Want a one-command provision+deploy wrapper **on top of** whichever IaC engine is chosen | `azd` is a deployment convenience wrapper, **not a competing IaC engine** — it does not replace the Decision between Bicep/Terraform/AVM, it sits above it |

### Our default guess

⚠ **Default guess: Option 3 (Bicep + Azure Verified Modules), offered only as a candidate because Azure is the
requested target in this engagement** — this is **not** an assumption that the team is Azure-only or has any
existing IaC preference; no evidence of an established IaC tool exists in this repository today. Option 1 ("no
change / not yet established") remains the honest stay-as-is answer until the user actively picks an IaC tool.

### Your decision

- [X] Option 1 — No change / not yet established
- [ ] Option 2 — Bicep
- [ ] Option 3 — Azure Verified Modules (AVM) via Bicep
- [ ] Option 4 — Terraform
- [ ] Other: ___________________

### Status note (2026-10-16 reconciliation)

The user has selected Option 1 ("No change / not yet established"). This is captured as the current answer but
remains **⏸ PENDING**, not ✅ DECIDED, because selecting "no change" does not, by itself, give Phase 3 a tool to
generate infrastructure with — it confirms there is nothing existing to preserve, not which tool should be used
going forward. Clarification needed: does "no change" mean "defer the IaC-tool choice entirely for now" (infra
generation stays out of scope until a tool is picked), or "proceed using the candidate guess below once the
hosting platform's remediation work is further along"? Either answer is valid; this file does not assume either.

### Status note (2026-10-16 clarification)

The user has since confirmed this engagement is a **"code-only modernization."** On that basis, D-07's "no change"
selection is confirmed to mean: **no IaC tool has been chosen, and the choice remains deferred outside the current
code-only scope** — it is **not** permission to scaffold any Bicep/Terraform/AVM files or begin IaC authoring work
as part of this pass. This stays `⏸ PENDING` (not DECIDED, not N/A) because an IaC tool will still need to be
picked whenever Phase 3 (Generate Infra) work actually begins; this clarification only confirms that moment has not
arrived yet. This explicit deferral is what permits Phase 1 Step B assessment/build work to proceed on the
code-only surface — it is **not** a gate waiver for Phase 3/4 infra/deploy work, which remains blocked.

### Your rationale

<Not provided by user; choice selected in edited checklist (2026-10-16). An optional shared rationale can be added
at any time — none is assumed or invented here.>

### Dependencies

This decision constrains:
- None directly (consumed by Phase 3 tooling choices).

This decision depends on:
- Decision 6 (Hosting platform) — the IaC modules used depend on which host is selected.

---

## Decision 8: Region & data residency

**ID:** D-08
**Status:** ⏸ PENDING — explicitly deferred outside current code-only scope (see status notes below)
**Required for:** Phase 3 — Generate Infra, Phase 4 — Deploy to Azure
**Catalog reference:** [`decision-catalog.md#D-08-region--data-residency`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking

Data residency is a hard legal constraint, and region selection drives cost, latency, and DR options. **No default
region or paired-DR region is guessed here.** `reports/Capability-Matrix.yaml#network` records
`private_endpoints_needed: unknown`, `egress_constraints: unknown`, and `connection_type: unknown` despite
`azure_connectivity_reported: true` — connectivity being *reported available* does not establish a validated route,
DNS configuration, or reachable service endpoint (Q5, Q11). `compliance.residency_requirement` is separately
recorded as `unknown` (Q4) and must not be conflated with "no requirement" — see Decision 11.

### Required fields (no default value supplied for any of these)

- **Primary region:** _____________ (free-text Azure region — not guessed; depends on Decision 11's answer and the
  organisation's own policy)
- **Paired DR region:** _____________ (Microsoft publishes region pairs once a primary region is chosen — not
  assumed here)
- **Data residency constraints:** _____________ (e.g., "must stay in UK/EU," "no preference" — depends on Decision 11)

### Why no guess is offered

Unlike most other decisions in this file, this one intentionally carries **no `⚠ Default guess`** — region and DR
pairing defaults would be arbitrary without a resolved residency requirement (Decision 11) and a validated
connectivity/quota picture (Q5, Q11). Guessing a region here risks silently picking one that violates a residency
constraint the organisation has not yet stated.

### Your decision

- Primary region: ___________________
- Paired DR region: ___________________
- Data residency constraints: ___________________
- Other: Not relevant for this modernization/migration.

### Status note (2026-10-16 reconciliation)

The user has left the free-text region/DR/residency fields blank and added "Other: Not relevant for this
modernization/migration." This is recorded as an **explicit deferral under the current scope**, not a production
`🚫 N/A` determination — region selection may still become relevant if/when Phase 3/4 implementation actually
begins, so this stays `⏸ PENDING` rather than moving to N/A. **Decision 11's GDPR (or UK-GDPR equivalent) selection
does not, by itself, dictate a UK region** — residency and hosting-region are related but separate questions, and
no region is inferred from the compliance checkbox alone.

### Status note (2026-10-16 clarification)

The user has since confirmed this engagement is a **"code-only modernization."** This confirms D-08 remains
explicitly deferred **outside the current code-only scope** (still not `🚫 N/A` — region selection is a real future
need, just not part of this pass). This deferral is what allows Phase 1 Step B assessment/build work to proceed on
the code surface; it is **not** a gate waiver — Phase 3 (Generate Infra) and Phase 4 (Deploy to Azure) work remains
blocked on this decision regardless.

### Your rationale

<Not provided by user; choice selected in edited checklist (2026-10-16). An optional shared rationale can be added
at any time — none is assumed or invented here.>

### Dependencies

This decision constrains:
- Decision 6 (Hosting platform) — service availability varies by region.
- Decision 13 (DR — RPO/RTO targets).

This decision depends on:
- Decision 11 (Compliance scope) — residency requirements must be known before a region can be proposed at all.

---

## Decision 9: Authentication

**ID:** D-09
**Status:** ✅ DECIDED 2026-10-16
**Required for:** Phase 2 — Migrate Code, Phase 3 — Generate Infra
**Catalog reference:** [`decision-catalog.md#D-09-authentication`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking

`reports/Capability-Matrix.yaml#identity` records `current_provider: ad` (Windows Negotiate/IIS sign-in plus custom
SQL roles, `high` confidence) and `entra_id_available: true`, but `target_provider: unknown`. The matrix's own note
is explicit: *"Entra ID availability does not approve a replacement design."* **Entra being available is a
capacity fact, not a design decision** — it does not, by itself, settle whether Entra ID is the target
authentication provider, how principal-to-role mapping would carry over, or what Q3 (sign-in/SSO/delegation
behavior) resolves to.

### Your options

| # | Option | When to pick | Tradeoffs / risks |
|---|--------|--------------|-------------------|
| 1 | **Stay on existing IdP (Windows Negotiate/AD, custom SQL roles)** | Migration without auth modernization; lowest disruption to the current role-mapping behavior | Locked to current auth limitations; Negotiate/IIS-style auth needs a validated path on the eventual hosting platform |
| 2 | **Microsoft Entra ID (workforce)** | B2E app, and Entra is confirmed available — but still requires a validated principal-to-role mapping design | Available ≠ approved; requires Q3 resolution (role/delegation mapping) before implementation |
| 3 | **Federated to external IdP (Okta, Auth0, Ping)** | Already-standardized SSO elsewhere in the organisation | No evidence this applies here; ask if it does |
| 4 | **No auth required** | Internal, network-isolated service | Does not match current evidence — the app has an administrative UI with role-based access today |

### Our default guess

⚠ **Default guess: Option 1 (Stay on existing IdP / AD-based auth)** as the lowest-disruption path, explicitly
**not** Option 2, even though Entra ID is reported available — availability is a precondition for considering Option
2, not a reason to default to it. Moving to Option 2 requires the identity owner to confirm Q3 (principal-to-role
mapping, delegation, and sign-in behavior to be preserved) first.

### Your decision

- [ ] Option 1 — Stay on existing IdP (AD/Negotiate)
- [X] Option 2 — Microsoft Entra ID
- [ ] Option 3 — Federated external IdP
- [ ] Other: ___________________

### Status note (2026-10-16 reconciliation)

The user has selected Option 2 (Microsoft Entra ID). This is the user's own choice, not an agent inference from
Entra's mere availability — the matrix's "available ≠ approved" caveat is satisfied by this explicit selection.
Implementation still requires Q3 (principal-to-role mapping, delegation, and sign-in behavior to be preserved) to
be resolved before Phase 2 work proceeds; the decision itself (which provider) is settled, but the design detail
is not yet fully specified.

### Your rationale

<Not provided by user; choice selected in edited checklist (2026-10-16). An optional shared rationale can be added
at any time — none is assumed or invented here.>

### Dependencies

This decision constrains:
- Decision 6 (Hosting platform) — some auth patterns need specific hosting support (e.g., Negotiate/IIS behavior).

This decision depends on:
- Resolution of Q3 (sign-in/SSO/role-delegation behavior to be preserved).

---

## Decision 10: Multi-tenancy approach

**ID:** D-10
**Status:** ✅ DECIDED 2026-10-16 (scope-conditional — see status note below)
**Required for:** Phase 3 — Generate Infra (only if multi-tenant)
**Catalog reference:** [`decision-catalog.md#D-10-multi-tenancy-approach`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking

No evidence in `reports/Capability-Matrix.yaml` indicates a multi-tenant architecture (no tenant-column schema
pattern, no per-tenant routing, no tenant-scoped configuration was found). This looks like a single internal
application serving one organisation (DEFRA/RPA), but **this has not been explicitly confirmed by the application
owner**, and Q10 (accountable owner, criticality, user counts) remains open.

### Your options

| # | Option | When to pick | Tradeoffs / risks |
|---|--------|--------------|-------------------|
| 1 | **Single-tenant (no tenancy) — stay-as-is** | Internal app serving one organisation, as current evidence suggests | N/A — this is the simplest model and matches all evidence seen so far |
| 2 | **Pooled (shared DB + tenant column)** | Many small tenants sharing infrastructure | No evidence this applies; would require a schema change not currently indicated |
| 3 | **Silo per tenant** | Few large tenants with compliance separation needs | No evidence this applies |
| 4 | **Hybrid** | Tiered SaaS model | No evidence this applies |

### Our default guess

⚠ **Default guess: Option 1 (Single-tenant, existing architecture)**, based on the absence of any tenancy-related
schema, routing, or configuration evidence in the matrix. This is explicitly **not a confirmed fact** — Q10
(accountable owner/criticality/user population) has not been answered, and the application owner should confirm
there is in fact only one tenant before this is marked decided.

### Your decision

- [X] Option 1 — Single-tenant (existing)
- [ ] Option 2 — Pooled
- [ ] Option 3 — Silo per tenant
- [ ] Option 4 — Hybrid

### Status note (2026-10-16 reconciliation)

The user has selected Option 1 (Single-tenant). This decision is marked ✅ DECIDED but is **scope-conditional, not
an automatic architecture change approval** — it reflects the single-tenant model already evidenced in the current
codebase; it does not itself trigger any new work. If Q10 (accountable owner, criticality, user population) later
surfaces evidence of multiple tenants, this decision should be revisited rather than treated as permanently closed.

### Your rationale

<Not provided by user; choice selected in edited checklist (2026-10-16). An optional shared rationale can be added
at any time — none is assumed or invented here.>

### Dependencies

This decision constrains:
- Decision 4 (Database engine) — tenancy model affects schema design if it changes.

This decision depends on:
- Resolution of Q10 (accountable owner, criticality, user population).

---

## Decision 11: Compliance scope

**ID:** D-11
**Status:** ✅ DECIDED 2026-10-16
**Required for:** Phase 1 (constrains every later phase)
**Catalog reference:** [`decision-catalog.md#D-11-compliance-scope`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking

`reports/Capability-Matrix.yaml#compliance` records every field — `regulations`, `residency_requirement`,
`audit_logging_required`, `retention_requirement`, `encryption_requirement` — as `unknown`, with the explicit note:
*"User explicitly answered unknown. Do not infer no regulation or choose a region."* **`unknown` is not `none`.**
This is a UK public-sector application (DEFRA/RPA) handling claim-statement data; GDPR/UK-GDPR applicability is
plausible but has **not** been confirmed and must not be inferred either way.

### Required fields (multi-select — none pre-selected)

- [ ] None
- [ ] HIPAA
- [ ] PCI-DSS
- [ ] SOC 2 Type II
- [X] GDPR (or UK-GDPR equivalent)
- [ ] FedRAMP (High / Moderate / Low)
- [ ] ITAR / Export-controlled
- [ ] Industry-specific (e.g., public-sector data handling policy)
- [ ] Other: _____

### Why no guess is offered

No box above was pre-checked by the agent, including "None." Checking "None" would have been exactly the inference
the matrix explicitly warns against. This decision required the data owner / compliance function to make an
affirmative statement, not an agent guess, however plausible.

### Your decision

(User-checked: GDPR (or UK-GDPR equivalent). No other box is checked. This is the user's own affirmative statement,
not an agent-inferred default — the stale note that previously said "all boxes above are currently unchecked" no
longer applies and has been removed. This single selection does **not** by itself fully resolve
residency/retention/audit/encryption specifics beyond the GDPR/UK-GDPR applicability itself, and it does **not**
automatically dictate a hosting region — see Decision 8.)

### Your rationale

<Not provided by user; choice selected in edited checklist (2026-10-16). An optional shared rationale can be added
at any time — none is assumed or invented here.>

### Dependencies

This decision constrains:
- Decision 8 (Region & data residency).
- Decision 13 (DR — RPO/RTO targets) — retention/audit requirements can affect backup and recovery design.

This decision depends on:
- Resolution of Q4 (residency/retention/audit/encryption requirements).

---

## Decision 12: Cost ceiling

**ID:** D-12
**Status:** ⏸ PENDING — explicitly deferred outside current code-only scope (see status notes below)
**Required for:** Phase 3 — Generate Infra, Phase 4 — Deploy to Azure
**Catalog reference:** [`decision-catalog.md#D-12-cost-ceiling`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking

Cost ceiling drives SKU tier selection, reserved-capacity decisions, and alerting thresholds. No budget figure
appears anywhere in the discovery evidence, and **no amount is invented here.**

### Required fields (no value supplied)

- **Max monthly Azure spend:** _____________ (currency: USD per catalog default, but state your own currency — e.g.,
  GBP — if that is how your organisation budgets; no amount is assumed either way)
- **Tolerance for overage:** Hard cap / Soft cap / No cap — not selected
- **Reserved capacity / Savings Plans tolerance:** Yes / No / Need approval — not selected

### Your decision

- Max monthly Azure spend: ___________________
- Tolerance for overage: ___________________
- Reserved capacity tolerance: ___________________
- Other: Not relevant at the moment.

### Status note (2026-10-16 reconciliation)

The user has left all free-text budget fields blank and added "Other: Not relevant at the moment." This is an
**explicit deferral under the current scope**, not a production `🚫 N/A` determination — no budget amount is
invented, and this stays `⏸ PENDING` so it is re-raised once cost ceiling becomes relevant (Phase 3/4).

### Status note (2026-10-16 clarification)

The user has since confirmed this engagement is a **"code-only modernization."** This confirms D-12 remains
explicitly deferred **outside the current code-only scope** (still not `🚫 N/A`). This deferral is what allows
Phase 1 Step B assessment/build work to proceed on the code surface; it is **not** a gate waiver — Phase 3/4
infra/deploy work remains blocked on this decision regardless.

### Your rationale

<Not provided by user; choice selected in edited checklist (2026-10-16). An optional shared rationale can be added
at any time — none is assumed or invented here.>

### Dependencies

This decision constrains:
- Decision 6 (Hosting platform) — SKU tier availability within budget.
- Decision 8 (Region) — some regions carry different pricing.

This decision depends on:
- None.

---

## Decision 13: DR — RPO / RTO targets

**ID:** D-13
**Status:** ⏸ PENDING — explicitly deferred outside current code-only scope (see status notes below)
**Required for:** Phase 3 — Generate Infra, Phase 4 — Deploy to Azure
**Catalog reference:** [`decision-catalog.md#D-13-dr--rpo--rto-targets`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking

`reports/Capability-Matrix.yaml#operational` records `rto: unknown`, `rpo: unknown`, `cutover_window: unknown`, and
`current_sla: unknown`, all at `low` evidence confidence — Q7 is open and explicitly notes that recovery/rollout/
acceptance design cannot proceed safely without these being agreed with operations. **No RTO/RPO value is invented
here**, regardless of how typical a particular value might be for a similarly-sized application.

### Required fields (no value supplied)

- **RPO (max acceptable data loss):** _____________ (e.g., 0 / 15 min / 1 hr / 24 hr — not selected)
- **RTO (max acceptable downtime during failover):** _____________ (not selected)
- **DR strategy:** Active-active / Active-passive / Pilot light / Backup-only — not selected

### Your decision

- RPO: ___________________
- RTO: ___________________
- DR strategy: ___________________
- Other: Not relevant for now.

### Status note (2026-10-16 reconciliation)

The user has left RPO/RTO/DR-strategy blank and added "Other: Not relevant for now." This is an **explicit
deferral under the current scope**, not a production `🚫 N/A` determination — no RTO/RPO value is invented, and
this stays `⏸ PENDING` so it is re-raised once DR targets become relevant (Phase 3/4).

### Status note (2026-10-16 clarification)

The user has since confirmed this engagement is a **"code-only modernization."** This confirms D-13 remains
explicitly deferred **outside the current code-only scope** (still not `🚫 N/A`). This deferral is what allows
Phase 1 Step B assessment/build work to proceed on the code surface; it is **not** a gate waiver — Phase 3/4
infra/deploy work remains blocked on this decision regardless.

### Your rationale

<Not provided by user; choice selected in edited checklist (2026-10-16). An optional shared rationale can be added
at any time — none is assumed or invented here.>

### Dependencies

This decision constrains:
- Decision 8 (Region & DR) — geo-replication and secondary-region SKUs.
- Decision 14 (Cutover strategy).

This decision depends on:
- Resolution of Q7 (RTO/RPO/SLA/rehearsal-environment agreement with operations).

---

## Decision 14: Cutover strategy

**ID:** D-14
**Status:** ⏸ PENDING — explicitly deferred outside current code-only scope (see status notes below)
**Required for:** Phase 4 — Deploy to Azure
**Catalog reference:** [`decision-catalog.md#D-14-cutover-strategy`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking

The catalog's generic Option 1 is "Big-bang (cutover weekend)." We are **not** defaulting to that here, and we are
also **not** assuming we already know what the existing release procedure actually is — no deployment runbook,
release pipeline definition, or operational cutover history was found in this repository's evidence (Q6, Q7 remain
open). The lowest-disruption path is to **retain whatever release/cutover approach is already in use today**,
whatever that turns out to be, rather than introduce Big-bang, blue-green, or any other named strategy as a new
default.

### Your options

| # | Option | When to pick | Tradeoffs / risks |
|---|--------|--------------|-------------------|
| 1 | **Retain the existing release/cutover approach (exact mechanics to be confirmed)** | Lowest disruption; do not introduce a new cutover pattern until the current one is understood | The current approach's specifics are unconfirmed (Q6, Q7) — this option is a placeholder for "whatever already works," not a specific named pattern |
| 2 | **Big-bang (defined maintenance window)** | Small app, acceptable all-or-nothing risk, confirmed maintenance window | All-or-nothing risk; requires Decision 15 (acceptable downtime) to be answered first |
| 3 | **Blue-green** | Need instant rollback capability | Conditional on **verified database compatibility and concurrency behavior** — the machine-scoped named mutex noted in `workload` evidence does not coordinate across replicas, and must be resolved before blue-green (which implies running two live environments) can be considered safe |
| 4 | **Phased by feature / Canary / Strangler fig** | Larger decomposition efforts | Not aligned with current refactor-scope recommendation; revisit only if migration strategy changes |

### Our default guess

⚠ **Default guess: Option 1 (Retain existing release/cutover approach)**, explicitly **not** an assumption that we
know what that approach is today — only that preserving it, whatever it is, is lower-risk than introducing a new
named cutover pattern before Q6/Q7 are resolved. Option 3 (Blue-green) is noted as a real candidate but is marked
conditional on independently verifying the database's compatibility with two concurrently-running environments and
resolving the machine-scoped concurrency lock noted elsewhere in the matrix.

### Your decision

- [ ] Option 1 — Retain existing release/cutover approach
- [ ] Option 2 — Big-bang
- [ ] Option 3 — Blue-green (conditional on DB compatibility/concurrency verification)
- [X] Other: Not relevant for now.

### Status note (2026-10-16 reconciliation)

The user has selected "Other: Not relevant for now," none of the named options. This is an **explicit deferral
under the current scope**, not a production `🚫 N/A` determination — this stays `⏸ PENDING` so it is re-raised once
cutover strategy becomes relevant (Phase 4).

### Status note (2026-10-16 clarification)

The user has since confirmed this engagement is a **"code-only modernization."** This confirms D-14 remains
explicitly deferred **outside the current code-only scope** (still not `🚫 N/A`). This deferral is what allows
Phase 1 Step B assessment/build work to proceed on the code surface; it is **not** a gate waiver — Phase 4
deploy work remains blocked on this decision regardless.

### Your rationale

<Not provided by user; choice selected in edited checklist (2026-10-16). An optional shared rationale can be added
at any time — none is assumed or invented here.>

### Dependencies

This decision constrains:
- Decision 15 (Acceptable downtime).

This decision depends on:
- Decision 4 (Database engine) and Decision 13 (DR targets) — particularly for Option 3.
- Resolution of Q6 (schedules/run volumes/concurrency) and Q7 (RTO/RPO/rehearsal environment).

---

## Decision 15: Acceptable downtime

**ID:** D-15
**Status:** ⏸ PENDING — explicitly deferred outside current code-only scope (see status notes below)
**Required for:** Phase 4 — Deploy to Azure
**Catalog reference:** [`decision-catalog.md#D-15-acceptable-downtime`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking

This constrains both Decision 14 (cutover strategy) and Decision 4/5 (online vs. offline database migration
approach). `reports/Capability-Matrix.yaml#operational` records `cutover_window: unknown`. **No downtime value is
invented here.**

### Required field (no value supplied)

- **Max acceptable downtime for cutover:** _____________ (Zero / Seconds / Minutes / Hours / Maintenance window of X
  — not selected)

### Your decision

- Max acceptable downtime: ___________________
- Other: Not relevant for now.

### Status note (2026-10-16 reconciliation)

The user has left the downtime field blank and added "Other: Not relevant for now." This is an **explicit
deferral under the current scope**, not a production `🚫 N/A` determination — no downtime value is invented, and
this stays `⏸ PENDING` so it is re-raised once acceptable downtime becomes relevant (Phase 4).

### Status note (2026-10-16 clarification)

The user has since confirmed this engagement is a **"code-only modernization."** This confirms D-15 remains
explicitly deferred **outside the current code-only scope** (still not `🚫 N/A`). This deferral is what allows
Phase 1 Step B assessment/build work to proceed on the code surface; it is **not** a gate waiver — Phase 4
deploy work remains blocked on this decision regardless.

### Your rationale

<Not provided by user; choice selected in edited checklist (2026-10-16). An optional shared rationale can be added
at any time — none is assumed or invented here.>

### Dependencies

This decision constrains:
- Decision 14 (Cutover strategy).
- Decision 4 / Decision 5 (database migration approach — online vs. offline).

This decision depends on:
- Resolution of Q7 (operational SLA/RTO agreement).

---

## Decision 16: CI/CD platform

**ID:** D-16
**Status:** ✅ DECIDED 2026-10-16
**Required for:** Phase 5 — Setup CI/CD
**Catalog reference:** [`decision-catalog.md#D-16-cicd-platform`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking

The repository is hosted on GitHub (`DEFRA/rpa-claim-statements`), which makes GitHub Actions a natural **candidate**
— but no existing CI/CD pipeline definition (no `.github/workflows/*.yml` tied to build/deploy, no Azure DevOps
pipeline YAML, no Jenkinsfile) was found as part of discovery evidence. **The existing CI/CD setup, if any, is
unknown**, so the stay-as-is option is "keep whatever currently exists," not a named tool.

### Your options

| # | Option | When to pick | Tradeoffs / risks |
|---|--------|--------------|-------------------|
| 1 | **Keep existing CI/CD (unspecified / to be confirmed)** | No CI/CD pipeline was found in repository evidence; do not assume one exists or name one that wasn't found | If no existing pipeline truly exists, this option effectively means "build one," which should be a deliberate choice, not a default |
| 2 | **GitHub Actions** | Code is already in GitHub and the team wants native integration | Cost scales with usage on private repos; no evidence yet on repo visibility/usage tier |
| 3 | **Azure DevOps Pipelines** | Enterprise Azure governance requirements apply | Introduces a second system if GitHub remains the source of truth |
| 4 | **GitLab CI** | N/A — code is not hosted on GitLab | Does not apply given current repository evidence |

### Our default guess

⚠ **Default guess: Option 2 (GitHub Actions)**, based only on the repository already being hosted on GitHub — this
is a **candidate based on hosting location**, not a confirmed statement that an existing CI/CD pipeline is absent or
that GitHub Actions is already configured. If an existing pipeline (Azure DevOps, Jenkins, etc.) is in use elsewhere
and simply wasn't captured in this repository's evidence, Option 1 (keep existing, unspecified) is the more accurate
starting point.

### Your decision

- [X] Option 1 — Keep existing CI/CD (unspecified)
- [ ] Option 2 — GitHub Actions
- [ ] Option 3 — Azure DevOps Pipelines
- [ ] Other: ___________________

### Status note (2026-10-16 reconciliation)

The user has selected Option 1 (Keep existing CI/CD, unspecified). This means **keep whatever currently exists** —
it is **not** a selection of GitHub Actions, Azure DevOps Pipelines, or any specific pipeline configuration, even
though the default guess above flagged GitHub Actions as a hosting-location-based candidate. Phase 5 will still
need to confirm what the "existing" pipeline actually is, since no pipeline definition was found in this
repository's discovery evidence — this decision settles the *approach* (preserve, don't replace), not the *tool*.

### Status note (2026-10-16 clarification)

The user has since confirmed this engagement is a **"code-only modernization."** This reinforces that D-16 means
**retain the current CI as-is — no pipeline modifications** are made as part of this pass. No `.github/workflows`,
Azure DevOps YAML, or other pipeline file is authored, changed, or scaffolded as a result of this decision.

### Your rationale

<Not provided by user; choice selected in edited checklist (2026-10-16). An optional shared rationale can be added
at any time — none is assumed or invented here.>

### Dependencies

This decision constrains:
- None directly.

This decision depends on:
- Decision 6 (Hosting platform) and Decision 18 (Container registry) — the pipeline's deploy step depends on both.

---

## Decision 17: Observability stack

**ID:** D-17
**Status:** ✅ DECIDED 2026-10-16
**Required for:** Phase 6 — Post-Migration Ops
**Catalog reference:** [`decision-catalog.md#D-17-observability-stack`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking

No existing observability/monitoring tool (Splunk, Datadog, etc.) was found in discovery evidence. Azure Monitor +
Application Insights is the natural Azure-native candidate for a workload targeting Azure, but this file does not
select it on your behalf.

### Your options

| # | Option | When to pick | Tradeoffs / risks |
|---|--------|--------------|-------------------|
| 1 | **Keep existing (unspecified, if one exists)** | An existing observability tool is already standardized elsewhere in the organisation | Not confirmed either way by current evidence |
| 2 | **Azure Monitor + Application Insights** | Want Microsoft-native, end-to-end observability for an Azure-hosted workload | Less customization than a dedicated APM vendor |
| 3 | **Azure + Grafana managed** | Want dashboards-as-code | Additional operator overhead |
| 4 | **Datadog / New Relic / Dynatrace** | Already a customer elsewhere, or multi-cloud requirement | License cost; no evidence this applies here |

### Our default guess

⚠ **Default guess: Option 2 (Azure Monitor + Application Insights)**, offered as a candidate only because the
requested target platform is Azure — this is **not a selection**, only the Microsoft-native default that would apply
if no existing observability standard needs to be preserved.

### Your decision

- [ ] Option 1 — Keep existing (unspecified)
- [X] Option 2 — Azure Monitor + Application Insights
- [ ] Option 3 — Azure + Grafana managed
- [ ] Other: ___________________

### Status note (2026-10-16 reconciliation)

The user has selected Option 2 (Azure Monitor + Application Insights). This is the user's own choice.

### Your rationale

<Not provided by user; choice selected in edited checklist (2026-10-16). An optional shared rationale can be added
at any time — none is assumed or invented here.>

### Dependencies

This decision constrains:
- None directly.

This decision depends on:
- Decision 6 (Hosting platform).

---

## Decision 18: Container registry

**ID:** D-18
**Status:** ✅ DECIDED 2026-10-16
**Required for:** Phase 3 — Generate Infra (only if Decision 6 selects container-based hosting)
**Catalog reference:** [`decision-catalog.md#D-18-container-registry-if-d-06-is-container-based`](../.github/skills/migration-decisions/references/decision-catalog.md)

### Why we're asking

This decision only materially applies if Decision 6 (Hosting platform) selects Container Apps, AKS, or Functions
Premium with custom containers. No existing container registry was found in discovery evidence, so the stay-as-is
answer is "preserve whatever exists today, if anything" rather than assuming none exists or inventing one.

### Your options

| # | Option | When to pick | Tradeoffs / risks |
|---|--------|--------------|-------------------|
| 1 | **Preserve existing registry if any (unknown today) — stay-as-is** | No registry use was found in discovery evidence; do not assume none exists elsewhere in the organisation | If genuinely none exists, this effectively defers to "create one," which should be a deliberate choice |
| 2 | **Azure Container Registry (ACR)** | Only once Decision 6 confirms a container-based hosting choice (e.g., Azure Container Apps) | Candidate only — not selected, since Decision 6 itself is still pending |
| 3 | **GitHub Container Registry (GHCR)** | Code is in GitHub and the team wants a unified registry/code location | Less Azure-native networking integration |
| 4 | **Docker Hub / Multi-registry** | Already publishing there, or need to mirror across providers | No evidence this applies here |

### Our default guess

⚠ **Default guess: Option 1 (Preserve existing / unknown) as the base state, with Option 2 (Azure Container
Registry) flagged as the natural candidate only if and when Decision 6 confirms Azure Container Apps.** No registry
choice is finalized while Decision 6 remains `⏸ PENDING`.

### Your decision

- [ ] Option 1 — Preserve existing / unknown
- [X] Option 2 — Azure Container Registry (candidate if ACA confirmed)
- [ ] Option 3 — GitHub Container Registry
- [ ] Other: ___________________

### Status note (2026-10-16 reconciliation)

The user has selected Option 2 (Azure Container Registry). Decision 6 now confirms Azure Container Apps as the
primary hosting platform, so the condition flagged in the default guess above ("only if and when Decision 6
confirms Azure Container Apps") is satisfied, and this choice is DECIDED.

### Your rationale

<Not provided by user; choice selected in edited checklist (2026-10-16). An optional shared rationale can be added
at any time — none is assumed or invented here.>

### Dependencies

This decision constrains:
- None directly (consumed by Phase 3/5 pipeline wiring).

This decision depends on:
- Decision 6 (Hosting platform) — only applicable if a container-based host is selected.

---

## Catalog options intentionally omitted from this file

Per the catalog's own notes, retired or no-longer-recommended service options are not presented as live choices in
this file: **Azure Database for MariaDB** (catalog notes EOL announced) and **Azure Spring Apps** (not applicable —
this is a .NET, not a Spring/Java, workload) are omitted from Decision 4 and Decision 6 respectively rather than
listed as viable options. This does not change the catalog itself — it reflects which generic options are
non-applicable to this specific application.

## How to answer

You do not need to answer all 18 at once. For any decision:
1. Tick the option box that matches your choice (or fill the free-text fields for Decisions 8, 11, 12, 13, 15).
2. Fill in "Your rationale."
3. Tell the agent which Decision number(s) you've answered, and it will update this file's Status line, the Status
   summary table, and append an entry to `reports/Decision-Log.md`.

Or say **"fill this in later"** and no further action is needed from you right now — every `⏸ PENDING` row above
stays exactly as it is until you choose to return to it.
