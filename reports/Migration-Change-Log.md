# Migration Change Log

**Application:** RPA Claim Statements
**Started:** 2026-10-16
**Phase:** 2 - Code modernization
**Status:** Preservation inventories synthesized and structurally validated; implementation blocked by recorded prerequisites. Phase 2 is not complete.

**Updated 2026-10-19:** User reports blocker 1 is a temporary limitation of this machine, absent on a replacement machine expected soon. Build/test execution is deferred, not failed application verification and not a requirement to change dependencies. Continue independently verifiable work within the remaining gates; no feed-control bypass. Replacement-machine availability and successful restore/build/tests have not yet been verified.

## Scope and selected decisions

- Code-only modernization for Azure Container Apps; retain .NET 10, ASP.NET Core MVC, and the existing in-process Web-to-Generator call boundary.
- Azure SQL Database is the current D-04 target. The external team supplies schema, data, and service configuration; this engagement does not provision or convert databases or move data.
- Entra ID is selected, but identity/role mapping and the SQL authentication contract are not yet supplied.
- Keep the existing EF6/SQL Server access approach unless a verified incompatibility requires a separately approved change.
- Infrastructure, deployment, CI/CD changes, and live post-migration operations remain outside this pass.
- The user removed incremental verification pauses on 2026-10-16. This changes the interaction cadence, not architecture approval or implementation prerequisites.

## Changes in this pass

| Item | Change | Behavior impact | Verification |
|------|--------|-----------------|--------------|
| P2-001 | Synthesized independent Generator/business-rule and Web/asset preservation perspectives in [Business Logic Mapping](Business-Logic-Mapping.md) | None; analysis and documentation only | 54 Generator, 3 database-side and 25 Web primary inventory rows: 82 total, with no migrated/verified behavior claimed |
| P2-002 | Updated [status](Report-Status.md) to remove review pauses while retaining implementation gates | None | Required Phase 2 decisions checked; app-specific gates remain open |
| P2-003 | Produced [Media Assets Mapping](Media-Assets-Mapping.md), including controlled static assets, linked templates/XSLT, runtime-generated document categories, packaging and case-sensitive-path findings | None; no assets copied or replaced | File inventory and provenance limitations documented; no clean-checkout packaging or runtime asset-access proof |
| P2-004 | Expanded source mapping to cover payment-model families, comparer keys, serialization/import mapping and CrossBorder SQL structure | None; observed pre-existing behavior retained, including suspect calculations | Direct-source inventory only; characterization tests, approved golden fixtures and database-side formula verification remain outstanding |
| P2-005 | Traced database-host FDMR and CrossBorder exports to repository consumers; added owner discussion [item 17 and source trace](../modernization-findings.md#database-host-export-trace-2026-10-19) | None; no export removed/replaced and no SQL executed | FDMR external usage unknown; active XB importer confirmed, link to CrossBorder output inferred pending live folder/mask/scheduler evidence |

### Preservation validation

- Checked unique primary inventory IDs, eight behavior categories, report links, companion-document references, balanced code fences and whitespace.
- Excluded repeated IDs in risk/blocked/modified summaries from primary inventory counts.
- Preserved the per-endpoint CSV encoding matrix: suppression and log exports without BOM; error export with BOM.
- Recorded existing SQL-backed role lookup and open identity questions without selecting Entra group-to-role mapping.
- Existing calculation anomalies are observations requiring owner review and characterization, not bug fixes authorized by modernization.
- Coverage limits remain explicit. In particular, the CrossBorder stored procedure's 59 UPDATE sections were structurally inspected; this does not prove every embedded formula or scheme-year value matches external production requirements.

## Application change inventory

| Surface | Result |
|---------|--------|
| Source code | No changes |
| Runtime/framework | No change; already targets .NET 10 |
| Dependencies | No changes |
| Configuration keys externalized | None |
| Authentication/authorization | No changes; no role mapping assumed |
| Database provider, schema or data | No changes |
| Media/assets | No copying or replacement performed |
| Containerization | Not started; native Linux compatibility and reproducible asset packaging unresolved |
| Migrated project folder | Not created; implementation has not begun |
| Breaking changes | None introduced |

## Verification and blockers

**Database authentication confirmed (2026-10-19):** Managed identity will be used on Azure for both main and security databases. No SQL password path is selected. Identity type/identifiers and least-privilege grants are not specified; token-capable EF6/SqlClient connection integration and access tests remain implementation/acceptance work. User sign-in and mapping to existing SQL-backed roles are separate contracts.

**Migration-history clarification (2026-10-19):** User confirms the Azure SQL copy will include the existing EF6 `__MigrationHistory`. Schema coverage, freeze and history delivery are agreed inputs, not further questions for the user at this stage. Actual history alignment and absence of pending migrations remain acceptance checks in an approved test environment, not completed live validation. Existing startup migration behavior has not been changed.

The existing [Migration Plan](Migration-Plan.md#10-handoff-to-migration-orchestrator) requires contract and compatibility validation before implementation.

| Prerequisite | Current evidence | Required resolution |
|--------------|------------------|---------------------|
| Baseline build (temporarily deferred) | User reports on 2026-10-19 that the restriction is machine-specific and absent on an expected replacement machine. Previous current-machine attempts failed HTTP 400/403 for `System.Management.Automation.dll` 10.0.10586; SDK 10.0.401 / MSBuild 18.9.11; both builds zero warnings/five errors | Obtain replacement machine and record restore/build of unchanged source plus applicable test results. Continue independently verifiable work meanwhile; approved access remedy only if restriction persists, no policy bypass |
| Azure SQL contract (partially clarified) | User confirmed 2026-10-19 both main/security schemas retained and privately available, unsafe for Git. No EF6 migration or database schema changes during modernization/migration; future governance deferred beyond this scope. No schema inspected or copied | Confirm authentication/identity-to-role lookup, target migration-history alignment/no pending existing migrations, and database-host export handling. No schema-changing migration execution or removal of existing startup behavior implicitly approved; future schema ownership is not a planning prerequisite |
| Identity behavior | Entra ID selected; role/principal/delegation behavior unverified | Approved identity and authorization-parity mapping |
| Native Linux compatibility | Windows-bound PDF/drawing/WinSCP paths remain; no replacement proof recorded | Approved lowest-disruption choices and isolated PDF/transfer parity proofs |
| Reproducible assets | Required DOCX templates were previously found locally but ignored/untracked | Approved controlled source, provenance/licensing and packaging evidence |

No build/restore retry, application execution, test run, database migration, network endpoint probe, or Azure resource operation has been performed in this pass. Documentation validation is not compilation, behavioral parity, Linux readiness, or production approval.

## Handoff

Phase 2 is **not complete**. Preserve the original application unchanged until the dependent gates can be satisfied and the migrated workspace can be built and tested. Record each eventual source/configuration/dependency change and its validation here.

Do not start `/phase3-generate-infra` now: the current scope excludes infrastructure, the implementation is incomplete, and the deferred Phase 3 decisions remain binding.

## Related artifacts

- [Migration Plan](Migration-Plan.md)
- [Application Assessment Report](Application-Assessment-Report.md)
- [Decisions Required](Decisions-Required.md)
- [Capability Matrix](Capability-Matrix.yaml)
- [Migration Status](Report-Status.md)
- [Business Logic Mapping](Business-Logic-Mapping.md)
- [Media Assets Mapping](Media-Assets-Mapping.md)
