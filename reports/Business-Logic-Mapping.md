# Business Logic Mapping Report — RPA Claim Statements (Generator + Web + DB-side + Media/Assets, synthesized)

**Application:** RPA Claim Statements — `RPA.ClaimStatements.Generator` (+ `.Generator.Tests`), `RPA.ClaimStatements.Web`
(+ `.Web.Tests`), `RPA.ClaimStatements.FDMR`, `RPA.ClaimStatements.CrossBorder` (DB-side SQL assets)
**Generated:** 2026-10-16 (Phase 2 preflight — mapping only, no source edits performed); synthesized/expanded across
three passes: (1) Generator-scope direct inspection, (2) an independent Web-perspective agent's source-verified
inventory incorporated by synthesis, (3) a source-gap-closure pass (this revision) resolving previously-unopened
comparer/model/Maps/Serializer/BulkInsert files and a bounded read of both CrossBorder SQL assets.
**Migration:** .NET 10 (current) → .NET 10 on Azure Container Apps + Entra ID (code-only modernization; target DB
Azure SQL Database, externally owned schema/data)
**Owner of this file:** Phase 2 Coder pass — covers the full synthesized Generator + Web + DB-side + Media/Assets
business-logic inventory (not Generator-only; see history above)
**Related reports:** [Migration-Plan.md](Migration-Plan.md) · [Application-Assessment-Report.md](Application-Assessment-Report.md) · [Decisions-Required.md](Decisions-Required.md) · [Report-Status.md](Report-Status.md) (owned by parent workstream) · [Media-Assets-Mapping.md](Media-Assets-Mapping.md) (owned by the Web-perspective pass; cross-linked, not edited here)

> **Scope — read before using this file.** This report now covers, by synthesis of multiple passes: (1) direct
> inspection of the `RPA.ClaimStatements.Generator` project, its compiled test project, and the DB-side SQL assets
> (`RPA.ClaimStatements.FDMR\LoadExtract.sql`, both `RPA.ClaimStatements.CrossBorder\*.sql` files — the second now
> read structurally in full, see § Database Business Logic); (2) the `RPA.ClaimStatements.Web` /
> `RPA.ClaimStatements.Web.Tests` controllers and `Security/` authorization contracts, incorporated verbatim from an
> independent Web-perspective agent's completed, source-verified pass (§ Media and Web Business Inventory, W001–W025);
> static/media asset paths remain owned by **[Media-Assets-Mapping.md](Media-Assets-Mapping.md)**, cross-linked but
> not edited by this file. **Coverage is still not asserted as complete** — see § Coverage Gaps and Limitations for
> what remains unread or unconfirmed even after this expansion.
>
> **No source, test, infrastructure, pipeline, or dependency changes were made to produce this report.** No live Azure
> resource was queried. No credentials, connection strings, hostnames, or PII appear below — `App.config`/`App.*.config`
> values, any literal secrets/hosts, and sample claim data are deliberately not reproduced; only business-rule
> *shapes* and file paths/line numbers are captured.
>
> **Gates already recorded in `Migration-Plan.md` are not bypassed by this file.** In particular: the external Azure
> SQL Database schema/authentication contract (gate before C01/C02/C04/C20), the Linux-native-dependency proof (gate
> before Phase 3 container build), and the Mutex/concurrency decision C13 remain exactly as blocked/pending as before
> this mapping pass. This file is an input to those gates, not a substitute for them.
>
> **Status key used below:** all items are **Inventoried** (⬜, discovered/located/described), **Blocked** (🚫, gated
> by an open Migration-Plan decision/contract — e.g. C03/C08/C13), or **Pending** (❓, discovery still incomplete for
> that item — see Coverage Gaps). **No item is "Migrated" or "Verified"** — zero source files were changed and the
> build/test run documented in `Migration-Plan.md`/`Application-Assessment-Report.md`
> (`System.Management.Automation.dll` 10.0.10586 feed HTTP 400/403) has not been re-attempted or resolved by this pass.
>
> **Catalog-ID discipline:** `BL-G001`–`BL-G044`+ and `DB-G001`–`DB-G003` are this report's own open-ended IDs
> (Generator + DB-side scope). `W001`–`W025` are the independent Web-perspective agent's own inventory-local row
> numbers, incorporated as-is in this synthesis pass — no renumbering into `BL-*` was performed here. This is a
> statement of what happened in this pass only, **not** a permanent rule against a future `BL-W` or renumbered series;
> that is a future-maintainer decision, not one this report makes. This `BL-*`/`W-*`/`DB-*` inventory scheme is
> entirely separate from, and must not be confused with, the closed, fixed 18-item **`D-01`–`D-18`** architecture
> **decision** catalog in `Migration-Plan.md` — the two ID families track different things (open business-logic
> inventory rows vs. closed architecture decisions) and neither numbering space constrains the other.

---

## Executive Summary

**Primary, non-double-counted row counts** (each item/row counted exactly once; cross-references to the same ID from
the Modified Logic / Blocked Items / Risk Synthesis sections are *references*, not additional rows):

| Metric | Count |
|---|---|
| **BL-G primary catalog rows** (`BL-G001`–`BL-G054`, all 8 skill categories, Generator/compiled-C# scope) | **54** |
| **DB-G primary catalog rows** (`DB-G001`–`DB-G003`, DB-side SQL assets — separate catalog, not part of the 54 above) | **3** |
| **W primary rows** (`W001`–`W025`, independent Web-perspective agent, incorporated by synthesis — separate catalog, not part of the 54 above) | **25** |
| — of which BL-G/DB-G flagged **🚫 Blocked** (gated by an open Migration-Plan decision/contract) | **8** (BL-G022 "Blocked-adjacent", BL-G023, BL-G024, BL-G027, BL-G029, BL-G036, BL-G038, DB-G001) |
| — of which DB-G flagged **❓ Pending** (discovery still incomplete after this pass's bounded SQL read) | **0 after this revision** — DB-G002/DB-G003 structural read completed this pass (see § Database Business Logic); both remain **not** cross-checked against a live schema (no live DB queried, per instruction) and are tagged accordingly, not "Pending" in the discovery sense |
| — of which comment-evidenced "intentional" in the Web rows (cited to an explicit code comment, not asserted) | 5 (W002, W009, W010, W015, W019) |
| — of which raised as open questions (Q3.1–Q3.11) rather than asserted intent | 7 Web rows → 11 subquestions |
| Active (compiled) Generator test files inspected | 10 files / **123** `[Test]` methods |
| Excluded (non-compiled) Generator test files inspected | 5 files / **111** `[Test]` methods (stale, target namespaces that no longer exist) |
| Compiled source files directly read in full or by targeted range (Generator scope, cumulative across both passes) | **48** (29 in the first pass + 19 newly opened this revision: 3 comparers, 12 `*Map`/`*FileMap` files, 2 BulkInsert files, 2 Serializer files — see list in § Coverage Gaps) |
| Source exclusions confirmed (csproj `<Compile Remove>`) | Generator: `Imports\**` (24 files) + 12 named files under `Services\`; Web: `ARController.cs`/`ARControllerTests.cs` (symmetric exclusion, confirmed consistent — W022) |
| **Combined total primary catalogued rows (BL-G + DB-G + W, no double-count)** | **54 + 3 + 25 = 82** |

> **Catalog-ID discipline (clarified this revision):** `BL-G001`–`BL-G054` and `DB-G001`–`DB-G003` are this report's
> own open-ended IDs (Generator + DB-side scope). `W001`–`W025` are the independent Web-perspective agent's own
> inventory-local row numbers, incorporated as-is in this synthesis — no renumbering into `BL-*` was performed here.
> That is a statement of what happened in **this** pass only, not a rule that a future `BL-W` series or renumbering
> could never happen — any such change is a future-maintainer decision, out of scope for this report to pre-decide.
> Separately, and not to be confused with any of the above: `Migration-Plan.md` maintains its own closed, fixed
> **18-item `D-01`–`D-18` architecture-decision catalog** — that is a different numbering space for a different kind
> of artifact (closed architecture decisions, not open business-logic inventory rows) and is not extended, closed,
> or referenced by ID equivalence anywhere in this file.

**This mapping is a discovery artifact, not a preservation proof.** No equivalence test, golden-fixture comparison,
or build/test run was performed as part of producing this file — see § Coverage Gaps and § Acceptance Criteria for
what would still be required before any item here could be marked Migrated/Verified. **This revision closes several
concrete previously-unopened-source gaps (comparers, Maps/Serializer/BulkInsert, 16 payment/XB models, both
CrossBorder SQL files structurally) but does not claim total/exhaustive coverage** — see § Coverage Gaps for what
still remains unconfirmed.

---

## Source Exclusion Reconciliation (csproj `<Compile Remove>`)

| Exclusion | Project | What it is | Verified disposition |
|---|---|---|---|
| `Imports\**` (24 files: `APImportService.cs`, `ARImportService.cs`, `FTPService.cs`, `IFTPService.cs`, `SITIImportService.cs`, `XBImportService.cs`, + `BulkInsert\`, `Maps\`, `Serializer\` subfolders) | `RPA.ClaimStatements.Generator.csproj` | A second, non-compiled copy of the import pipeline, pre-dating the active `Components\PumpingStation\*` implementations. csproj comment: *"exist on disk but were never part of the legacy build; keep them out, unchanged."* | Confirmed legacy/dead code, not a hidden active path. No behavior to map from this folder. |
| `Services\BuildService.cs`, `IBuildService.cs` | `RPA.ClaimStatements.Generator.csproj` | Duplicate of active `Components\Turbine\BuildService.cs` | Active implementation is `Components\Turbine\BuildService.cs` — mapped below (BL-G008). |
| `Services\EmailService.cs`, `IEmailService.cs` | `RPA.ClaimStatements.Generator.csproj` | Duplicate of active `Components\TrackingStation\EmailService.cs` | Active implementation mapped below (BL-G039). |
| `Services\ImportService.cs`, `IImportService.cs` | `RPA.ClaimStatements.Generator.csproj` | Older, unified import service — superseded by the four split services (`SITI`/`XB`/`AP`/`AR`ImportService) in `Components\PumpingStation\` | Active implementations mapped below (BL-G019–BL-G021, BL-G035, BL-G037). |
| `Services\LogService.cs`, `ILogService.cs` | `RPA.ClaimStatements.Generator.csproj` | Duplicate of active `Components\Turbine\LogService.cs` | Active implementation mapped below (BL-G040 is ErrorService; LogService is BL-G025 context, file `Components\Turbine\LogService.cs`). |
| `Services\TransformService.cs`, `ITransformService.cs` | `RPA.ClaimStatements.Generator.csproj` | Duplicate of active `Components\Turbine\TransformService.cs` | Active implementation mapped below (BL-G028–BL-G031). |
| `Services\ValidationService.cs`, `IValidationService.cs` | `RPA.ClaimStatements.Generator.csproj` | Duplicate of active `Components\Turbine\ValidationService.cs` | Active implementation mapped below (BL-G014–BL-G018). |
| `Services\GenerationServiceTests.cs`, `ImportServiceTests.cs`, `LoadServiceTests.cs`, `LogServiceTests.cs`, `ValidationServiceTests.cs` (5 files, 111 `[Test]` methods total) | `RPA.ClaimStatements.Generator.Tests.csproj` | Excluded test files that **reference namespaces not present anywhere in the current repository** (`RPA.ClaimStatements.Data.UnitOfWork`, `RPA.ClaimStatements.Data.Context`, `RPA.ClaimStatements.Data.Entities.*`, a monolithic `ImportService` class, a `FakeUnitOfWork` test double) | Confirmed stale — these do **not** describe the current `Components\*` code shape and cannot be treated as evidence for it. Counted separately in § Coverage Gaps; **not** cited as current-behavior tests anywhere below. |

**Pattern confirmed:** for every excluded main-source duplicate under `Services\`, the matching excluded test file exists
under the same old name (`ImportService.cs` ↔ `ImportServiceTests.cs`, `LogService.cs` ↔ `LogServiceTests.cs`,
`ValidationService.cs` ↔ `ValidationServiceTests.cs`); the two exceptions are `GenerationServiceTests.cs` and
`LoadServiceTests.cs`, which are excluded test files with **no matching excluded main-source file of that literal
name** anywhere in `Services\` — they appear to target a pre-split `GenerationService`/`LoadService` shape that no
longer exists under either name. This is noted as-is; no inference is drawn about what that prior shape contained.

---

## Business Logic Inventory

Status legend: ⬜ Inventoried (discovered/located/described; not migrated, not verified) · 🚫 Blocked (gated by an
open Migration-Plan decision) · ❓ Pending (discovery incomplete, see Coverage Gaps).

### 📊 Calculations

| ID | Name | Description | Source Location | Target Location | Status | Tested? |
|----|------|-------------|-----------------|-----------------|--------|---------|
| BL-G001 | Euro→Sterling conversion | `Math.Round(value * rate, 2)` — the single conversion routine used everywhere a Euro amount is shown in Sterling | `Services/CurrencyService.cs:24-27` | Not created; retain existing relative path pending gates | ⬜ | Yes — `Services/CurrencyServiceTests.cs` (active, 1 of 5 tests: `Test_Covert_Converts_By_Rate`) |
| BL-G002 | Tolerance check | `bool CheckTolerance(claimValue, transactionValue, tolerance = 0.05M)` — `Math.Abs(claimValue-transactionValue) <= tolerance && >= -tolerance` | `Services/CurrencyService.cs:13-22` | Not created; retain existing relative path pending gates | ⬜ | Yes — 4 of 5 tests in `Services/CurrencyServiceTests.cs` (positive/negative, within/outside) |
| BL-G003 | Part C sterling-tolerance substitution | For each Sterling invoice, if `CheckTolerance(ClaimValueSterling, TransactionValue)` passes, overwrite `ClaimValueSterling` with `TransactionValue`; then sort invoices by `DateDate` ascending; `TotalPayments = Sum(TransactionValue)` (not `ClaimValueSterling`) | `Models/ClaimStatements/PartC.cs:27-45` | Not created; retain existing relative path pending gates | ⬜ | No direct unit test found for `PartC.Build` in the active test tree |
| BL-G004 | Claim-statement-level sterling reconciliation | `ClaimStatement.Build()`: `Currency` = most-recent invoice's currency (`OrderByDescending(DateDate)`); if Sterling and `CheckTolerance(PartA.TotalSterling, PartC.TotalPayments)` passes, **overwrite** `PartA.TotalSterling` with `PartC.TotalPayments` before validation runs | `Models/ClaimStatements/ClaimStatement.cs:55-67` | Not created; retain existing relative path pending gates | ⬜ | No direct unit test found in the active test tree |
| BL-G005 | Part A totals derivation | `CrossComplianceTotal = SubTotal - CrossComplianceReduction`; `ProgressiveReductionTotal = CrossComplianceTotal - ProgressiveReduction` **only when `claim.PR != null`**; `TotalSterling = CurrencyService.Convert(TotalEuro, conversionRate)` (BL-G001) | `Models/ClaimStatements/PartA.cs:64-86` | Not created; retain existing relative path pending gates | ⬜ | No direct unit test found in the active test tree |
| BL-G006 | BPS payment totals & penalty-precedence cascade | `TotalNumber`/`TotalTotal` region sums; `OverDeclarationTotal = BPSGross - OverDeclarationReduction - AdditionalOverDeclarationReduction`, clamped to 0 if negative (line 258-260); `LastPenalty` is a **last-write-wins cascade** through 7 penalty checks in fixed order (`OverDeclarationReduction` → `LateClaimSubmissionReduction` → `LateEntitlementsApplicationReduction` → `LateEvidenceReduction` → `LateEntitlementsAmendmentReduction` → `NonDeclarationReduction` → `LateChangePenaltyReduction`) | `Models/ClaimStatements/BPSPayment.cs:198-263` (method starts 198; `TotalNumber`/`TotalTotal` 253-254; `OverDeclarationTotal` 256-260) | Not created; retain existing relative path pending gates | ⬜ | Exercised indirectly via `Components/Turbine/ValidationServiceTests.cs` (28 tests) and `Components/FuelTank/FuelTankTests.cs` (71 tests) — no test asserts `BPSPayment.Calculate()` output directly by name |
| BL-G007 | Greening payment totals & dual penalty-precedence cascade | Region count (same NonSDA/SDA/Moorland>0 pattern as BL-G006); **two independent** last-write-wins cascades: `LastPenalty` (Crop Diversification → Permanent Grassland → EFA) and `LastPenalty2` (Administrative → Late Application → Late Evidence, continues past the viewed range) | `Models/ClaimStatements/GreeningPayment.cs:194-230+` | Not created; retain existing relative path pending gates | ⬜ | Same indirect coverage as BL-G006; no direct assertion by name found |
| BL-G008 | Build-service composition rule | `BuildService.Build()` composes `PartB` from 4 mutually exclusive shapes based on `SUM2.YoungFarmerValue > 0` and `PR?.PRBTotalResult > 0`: both → `PartB(bps, greening, PR, YF)`; YF only → `PartB(bps, greening, YF)`; PR only → `PartB(bps, greening, PR)`; neither → `PartB(bps, greening)` | `Components/Turbine/BuildService.cs:13-65` | Not created; retain existing relative path pending gates | ⬜ | No direct unit test of `BuildService.Build`/`BuildXB` found in the active test tree |
| BL-G009 | Progressive Reductions "grossed-up" amount | For 4 bands: `PRBxAmount = (PRBxResult / PRBxPercent) * 100` — **divides by a stored percent with no guard**; a `0` percent with non-zero result throws `DivideByZeroException`-equivalent (decimal division by zero throws) | `Models/ClaimStatements/ProgressiveReductionsReduction.cs:40-56` | Not created; retain existing relative path pending gates | ⬜ | No test found — **flagged as an untested edge case**, see Risks |
| BL-G010 | Young Change Protection (YCP) rate rule | `YCPService.GetRate(schemeYear, overDeclarationPercent, hectares, previousYearFlag)`: only applies for `schemeYear >= 2016`; triggers when `overDeclarationPercent > 3 OR hectares > 2`; then `"Standard Rate"` if `previous == true OR overDeclarationPercent > 10`, else `"Reduced Rate"`; otherwise `"No Reduction"` | `Services/YCPService.cs:14-26` | Not created; retain existing relative path pending gates | ⬜ | No test found for `YCPService` in the active test tree |
| BL-G011 | Invoice-number cross-system prefix transform | `ConversionService.TransformInvoice(invoice, to)`: `to="SITI"` → prefix `"SITI"`; `to="XB"` → prefix `"PFSY"`; new value = `prefix + invoice.Substring(1,7)`, **only when `invoice.Length >= 8`** (else returned unchanged) | `Services/ConversionService.cs:11-29` | Not created; retain existing relative path pending gates | ⬜ | Yes — `Services/ConversionServiceTests.cs` (active, both branches: SITI and XB) |
| BL-G012 | Batch-size refinement rule | `RefineMaximumBatchSize(requested, outstandingCount)`: clamps `requested` to the DB-configured `"Maximum Batch Size"` setting if smaller, then clamps further to `outstandingCount` if that is smaller still | `Components/FuelTank/FuelTank.cs:165-181` | Not created; retain existing relative path pending gates | ⬜ | Exercised indirectly via `Components/FuelTank/FuelTankTests.cs` (71 tests, not individually itemized by this pass) |
| BL-G013 | FDMR Net4 (net-of-reductions) value computation [DB-side] | `ClaimNet4 = ROUND(BPS_Net4 + GR_Net4 + YF_Net4, 2)`; `BPS_Net4`/`GR_Net4` each independently clamp to 0 if `(Gross - Reductions) < 0`; `YF_Net4 = TotalYF + FDMReduction` (no clamp) | `RPA.ClaimStatements.FDMR\LoadExtract.sql` (stored procedure `FDMR.LoadExtract`, full file read — 90 lines) | N/A — DB-side stored procedure, external schema ownership per Migration-Plan | ⬜ | No code-level test possible (lives in the externally-owned database); **separate category from compiled C# source** |
| BL-G048 | **Greening-family `LateChangePenaltyTotal` self-referential bug (observed, not asserted intentional)** | Base class (`GreeningPayment.cs:300`) correctly computes `LateChangePenaltyTotal = NonDeclarationTotal - LateChangePenaltyReduction;`. Three per-nation **override** subclasses instead read `LateChangePenaltyTotal = NonDeclarationTotal - LateChangePenaltyTotal;` — i.e. the destination variable appears on both sides. Since `LateChangePenaltyTotal` is still its decimal default (`0`) at that line, this collapses to `NonDeclarationTotal - 0`, silently skipping the late-change-penalty deduction, even though `LateChangePenaltyReduction` **is** correctly populated earlier in the same `Build()` method (e.g. from `xbData.NIGreeningLateAmendmentReduction`) and the "LastPenalty2" label-selection logic still checks `if (LateChangePenaltyReduction > 0)` — so the penalty's **name** can be reported even though its **amount** was never subtracted. **Flagged as an observed defect requiring business confirmation — not silently fixed or assumed intentional**, consistent with the code-only/no-silent-decisions constraint. | `Models/ClaimStatements/NIGreeningPayment.cs:155`, `Models/ClaimStatements/ScotlandGreeningPayment.cs:193`, `Models/ClaimStatements/WalesGreeningPayment.cs:155` (base-class correct reference: `Models/ClaimStatements/GreeningPayment.cs:300`) | Not created; retain existing relative path pending gates | ⬜ | No test found exercising any of the 3 override subclasses' `Calculate()` by name |
| BL-G049 | **YoungFarmer-family `LateChangePenaltyTotal` omitted-assignment bug (observed, not asserted intentional)** | Base class (`YoungFarmerPayment.cs:244`) and `WalesRedistributivePayment.Calculate()` both correctly compute `LateChangePenaltyTotal = NonDeclarationTotal - LateChangePenaltyReduction;` before the `FDMTotal = LateChangePenaltyTotal - FDMReduction` step. The 3 per-nation **YoungFarmer override** subclasses have **no such assignment line at all** before their `if (LateChangePenaltyTotal < 0)` clamp — the field is never set away from its decimal default (`0`), so `FDMTotal` is computed as `0 - FDMReduction` for every claim, regardless of the actual `NonDeclarationTotal`. This is a stronger effect than BL-G048 (not just "one reduction ignored" but "the entire upstream cascade value is discarded for this specific payment family/nation combination"). **Flagged as an observed defect requiring business confirmation.** | `Models/ClaimStatements/NIYoungFarmerPayment.cs` (no assignment before the clamp near line 134), `Models/ClaimStatements/ScotlandYoungFarmerPayment.cs` (same, near line 133), `Models/ClaimStatements/WalesYoungFarmerPayment.cs` (same); correct base-class reference: `Models/ClaimStatements/YoungFarmerPayment.cs:244`; correct sibling reference: `Models/ClaimStatements/WalesRedistributivePayment.cs` | Not created; retain existing relative path pending gates | ⬜ | No test found exercising any of the 3 override subclasses' `Calculate()` by name |
| BL-G050 | **`ScotlandGreeningPayment` Region3Total copy-paste bug (observed, not asserted intentional)** | `Region3Total = xbData.ScotlandGreeningRegion1Total;` — reuses the **Region1** source field for the Region3 destination property, instead of a `ScotlandGreeningRegion3Total`-shaped field (which does not appear to be read anywhere in this method). `Region2Rate`/`Region3Rate` are also both hardcoded to `0` in the same method (rate fields, not totals — may be intentional display-only zeroing, not confirmed either way). | `Models/ClaimStatements/ScotlandGreeningPayment.cs:48` (Region3Total bug); same file, `Region2Rate`/`Region3Rate` zeroing nearby (exact lines not separately re-confirmed this pass) | Not created; retain existing relative path pending gates | ⬜ | No test found |
| BL-G051 | **Cross-nation `ScotlandYFPreviousYearOverDeclaration` field-reference bug (observed, not asserted intentional)** | `NIYoungFarmerPayment.cs` (~line 64) calls `ycpService.GetRate(..., xbData.ScotlandYFPreviousYearOverDeclaration)` when computing its own YCP status — i.e. the **NI** payment class reads a **Scotland**-prefixed XBData field for its "previous year over-declaration" flag. `ScotlandYoungFarmerPayment.cs` makes the identical call (correctly, for Scotland) — the NI file appears to be a copy-paste of the Scotland file with this one field reference left unchanged. | `Models/ClaimStatements/NIYoungFarmerPayment.cs` (~line 64); cross-referenced against the (correct) `Models/ClaimStatements/ScotlandYoungFarmerPayment.cs` call to the same field name | Not created; retain existing relative path pending gates | ⬜ | No test found |
| BL-G052 | **PartD hectare-unit-conversion asymmetry + `AreaIneligible`/`AreaEligible` naming question** | `PartD.Build(cld)` multiplies every CLD-sourced area/entitlement field by `0.0001M` (unit conversion, presumably from square metres or a smaller CLD-native unit to hectares) — `PartDXB.Build(xbData)` applies **no such conversion** on the equivalent XBData-sourced fields (XBData values are already in hectares). This is an intentional, source-system-dependent asymmetry that **must be preserved exactly, not normalized** if the two Build paths are ever refactored together. Separately: in `PartD.Build(cld)`, the destination properties `NonSDAAreaIneligible`/`SDAAreaIneligible`/`MoorlandAreaIneligible` are assigned from CLD source fields literally named `...AreaEligible` (not `...AreaIneligible`) — consistent across all 3 regions. By contrast, `PartDXB.Build(xbData)` assigns the same-named `...AreaIneligible` destination fields from XBData source fields **already literally named** `...AreaIneligible` — no inversion on that path. **This naming mismatch on the CLD path is an open question, not confirmed as a bug or an intentional relabeling — flagged for business confirmation, not resolved by this pass.** | `Models/ClaimStatements/PartD.cs` (both the `0.0001M` scaling and the `AreaEligible`→`AreaIneligible` assignments; exact line numbers not individually re-cited beyond the file-level read already performed for this family); `Models/ClaimStatements/PartDXB.cs` (comparison path, no scaling, no naming mismatch) | Not created; retain existing relative path pending gates | ⬜ | No direct unit test found for `PartD.Build`/`PartDXB.Build` in the active test tree |
| BL-G053 | Invoice currency bucketing rule | `Invoice` constructor: any `transactionCurrency != "EUR"` (case-sensitive exact string compare) is bucketed to the literal string `"Sterling"`; only an exact `"EUR"` match yields `"Euros"` — a strict two-bucket rule, not a general currency-code pass-through or case-insensitive match | `Models/ClaimStatements/Invoice.cs` (constructor; exact line not individually re-cited beyond the file-level read already performed) | Not created; retain existing relative path pending gates | ⬜ | Not confirmed against a located active test for `Invoice`'s constructor specifically |
| BL-G054 | `WalesRedistributivePayment` 54-hectare hard cap | `LandUsedToClaim = xbData.WalesRedLandUsedToClaim > 54 ? 54 : xbData.WalesRedLandUsedToClaim;` — a hardcoded 54-hectare cap not present in any other payment class read in this pass; `WalesRedistributivePayment` does not inherit from any shared `BPSPayment`/`GreeningPayment`/`YoungFarmerPayment` base class (stands alone, unlike the other 9 per-nation classes) | `Models/ClaimStatements/WalesRedistributivePayment.cs` (`Build()` method; exact line not individually re-cited beyond the file-level read already performed) | Not created; retain existing relative path pending gates | ⬜ | No test found |

### ✔️ Validations

| ID | Name | Description | Source Location | Target Location | Status | Tested? |
|----|------|-------------|-----------------|-----------------|--------|---------|
| BL-G014 | Validation orchestration order | `Validate()` runs, in fixed order: `ValidateCurrency` → `ValidateInvoices` → `ValidateClaimHistory` → `ValidateReconciliation`; only invoked when `configurationService.IsActive("Validation")` (checked once at `Turbine` construction, not per-request) | `Components/Turbine/ValidationService.cs:13-19`; gate at `Components/Turbine/Turbine.cs` constructor (`validate = configurationService.IsActive("Validation")`) | Not created; retain existing relative path pending gates | ⬜ | Yes — `Components/Turbine/ValidationServiceTests.cs` (active, 28 tests) |
| BL-G015 | Currency populated check | Throws `ClaimStatementValidationException` if `statement.Currency == null` | `Components/Turbine/ValidationService.cs:47-53` | Not created; retain existing relative path pending gates | ⬜ | Yes (within the 28 `ValidationServiceTests.cs` tests) |
| BL-G016 | Invoices-present check | Throws if `PartC.Invoices` is null or `Count == 0` | `Components/Turbine/ValidationService.cs:55-61` | Not created; retain existing relative path pending gates | ⬜ | Yes (within the 28 tests) |
| BL-G017 | Claim-history reconciliation | `lastSettlement` = most recent invoice (`OrderByDescending(DateDate).ThenByDescending(Reference)`, first) `ClaimValueEuro`; BPS: must equal `PartA.TotalEuro`; XB: must equal `PartAXB.TotalTotalEuro` — else throws with a distinct message per statement type | `Components/Turbine/ValidationService.cs:21-45` | Not created; retain existing relative path pending gates | ⬜ | Yes (within the 28 tests) |
| BL-G018 | Part A/Part C reconciliation | BPS: if `Currency=="Euros"` compare `PartA.TotalEuro` vs `PartC.TotalPayments`, else compare `PartA.TotalSterling` vs `PartC.TotalPayments`; XB: equivalent comparison against `PartAXB.TotalTotalEuro`/`TotalTotalSterling` — **note BL-G004 pre-adjusts `TotalSterling` to match `TotalPayments` within tolerance before this check runs for the Sterling/BPS path**, which is why this check commonly passes for small rounding differences | `Components/Turbine/ValidationService.cs:63-95` | Not created; retain existing relative path pending gates | ⬜ | Yes (within the 28 tests) |
| BL-G019 | AP import scheme filter | Imported AP rows are kept only if `bpsCodes.Exists(b => b == x.Scheme)`, where `bpsCodes` = all `SchemeCodeName` values from `db.SchemeCodes` | `Components/PumpingStation/APImportService.cs:99` | Not created; retain existing relative path pending gates | ⬜ | No test found for `APImportService` in the active test tree |
| BL-G020 | AR import scheme filter | Each parsed Excel row is only passed to `CreateAR` if `bpsCodes.Contains(row[5])` (same `SchemeCodes` source as BL-G019, column 5 = `H_Scheme`) | `Components/PumpingStation/ARImportService.cs:133-135` | Not created; retain existing relative path pending gates | ⬜ | No test found for the active `ARImportService` (see Coverage Gaps — only the stale excluded `ImportServiceTests.cs` exercises a differently-named `ImportService.CreateAR`) |
| BL-G021 | AR row parse/dedup gate | `CreateAR` only proceeds if `Int64.TryParse(supplier)`, `DateTime.TryParse(invoiceDate)`, and `int.TryParse(lineNumber)` all succeed; then skips if an AR row with the same `(H_InvoiceAccount, InvoiceNumber, L_LineNumber)` already exists; `L_LineAmount` is **negated** (`* -1`) on insert; several optional fields (`H_MarketingYear`, `L_MarketingYear`, `OriginalClaimSettlementDate`, `OpenBalance`) are individually `TryParse`-guarded and silently left unset/`null` on failure | `Components/PumpingStation/ARImportService.cs:152-230` (approx., continues past viewed range) | Not created; retain existing relative path pending gates | ⬜ | No test found for the active `ARImportService.CreateAR` |
| BL-G046 | **`Claim.IsValid` omits the `YF` (YoungFarmer) null-check (observed, not asserted intentional)** | `Claim.IsValid` (used as the add-to-output-list gate in BL-G045's `PTD`-trailer step) checks that `SUM`, `SUM2`, `BPS`, `BPSPEN`, `GR`, `GRPEN`, `CLD`, `PR` are all non-null — **but does not check `YF`**. A SITI import file whose `PTBYF` record-type line is missing or malformed would still produce a Claim that passes `IsValid` and is added to the output with `YF == null`. **Not independently confirmed** whether any downstream code (e.g. `BuildService.Build()`, BL-G008, which branches on `SUM2.YoungFarmerValue`, a different object) ever dereferences `claim.YF` directly in a way this would break — flagged as a plausible risk, not a confirmed defect. | `Models/Entities/SITI/Claim.cs` (`IsValid` property) | Not created; retain existing relative path pending gates | ⬜ | No test found asserting `Claim.IsValid`'s field list (positive or negative) |

### 🔄 Workflows

| ID | Name | Description | Source Location | Target Location | Status | Tested? |
|----|------|-------------|-----------------|-----------------|--------|---------|
| BL-G022 | Top-level activation sequence | `PowerPlant.Activate(statementType, maxBatchSize)`: `RunMigrations()` → `pumpingStation.Activate()` (imports) → **conditionally** (see BL-G023) generation → `trackingStation.Report()` (notification) — always runs in this fixed order regardless of `statementType` | `Components/PowerPlant.cs:40-90` | Not created; retain existing relative path pending gates | 🚫 Blocked-adjacent (see C13 in Migration-Plan) | No direct unit test of `PowerPlant.Activate` found in the active test tree |
| BL-G023 | Generation gate + named Mutex | Generation (fuel-fill + turbine) only runs when `maximumBatchSize > 0 AND configurationService.IsActive("Generation")`; guarded by a **global, machine-scoped named `Mutex`**: `new Mutex(false, $@"Global\CSG_{statementType}_{csgMutexId}")` with `csgMutexId = "2F56AE60-350C-4AA2-B69D-9C661B04AE1F"`; `AbandonedMutexException` is swallowed (treated as acquired) | `Components/PowerPlant.cs:26,40,52-82` | Not created; retain existing relative path pending gates | 🚫 **Blocked** — this is exactly the single-replica concurrency finding gated behind decision **C13** in `Migration-Plan.md §6`; the `"Global\"` named-mutex namespace is itself a Windows-only kernel-object concept with no Linux equivalent, which is a second, more basic compatibility question than just "which replica count" | No direct test |
| BL-G024 | EF6 migrations applied at every activation | `RunMigrations()` constructs `new Migrations.Configuration()` + `DbMigrator(configuration).Update()` **on every `PowerPlant.Activate()` call**, not just at deploy time | `Components/PowerPlant.cs:93-99` | Not created; retain existing relative path pending gates | 🚫 **Blocked** — this is the runtime-applied-EF6-migrations pattern referenced by Migration-Plan assumption A1/finding C03 (external-team schema-ownership contract); cannot be assessed for Azure SQL Database compatibility until that contract is received | No direct test |
| BL-G025 | Per-request error isolation + parallel transform | `Turbine.Generate()`: builds/validates each request in a sequential loop, catching `ClaimStatementValidationException` and generic `Exception` separately (both logged via `IErrorService`, never rethrown — a failed request is simply excluded from `requestStatements`); successfully built+validated requests are then transformed via `Parallel.ForEach(..., MaxDegreeOfParallelism = 16, ...)`, with `AggregateException` unwrapped afterwards (`ClaimStatementGenerationException` branch logs with FRN/SchemeYear; any other inner exception logs with a literal `0,0` "unknown statement" placeholder) | `Components/Turbine/Turbine.cs:38-107` (loop 54-78; parallel block 83-99; `MaxDegreeOfParallelism = 16` at line 83; `const byte statementUnknown = 0` at line 99) | Not created; retain existing relative path pending gates | ⬜ | Yes — `Components/Turbine/TurbineTests.cs` (active, 6 tests: no-claims/null-request/validation-active/validation-inactive/multiple-claims) |
| BL-G026 | Fixed import sequencing | `PumpingStation.Activate()` always imports in the order **SITI → XB → AP → AR**, regardless of which statement type was requested | `Components/PumpingStation/PumpingStation.cs:25-34` | Not created; retain existing relative path pending gates | ⬜ | No direct test of `PumpingStation.Activate` ordering found |
| BL-G027 | Per-import-service named Mutex (4 distinct GUIDs) | Each of `SITIImportService`, `APImportService`, `ARImportService`, `XBImportService` wraps its own `Import()` in a **separate** `Global\CSG_{guid}` named Mutex: SITI=`5C45109B-...`, AP=`676DFF3F-...`, AR=`E1F813C0-...`, XB uses a constant **misleadingly named `apMutexId`** = `FD4F74C2-...` (not the same GUID as the real AP service, but the field name is copy-pasted from it) — same `AbandonedMutexException`-swallow pattern as BL-G023 | `Components/PumpingStation/SITIImportService.cs:33`, `APImportService.cs:30`, `ARImportService.cs:25`, `XBImportService.cs:31` | Not created; retain existing relative path pending gates | 🚫 Same underlying Linux-named-mutex concern as BL-G023 | No direct test of mutex behavior |

### 🔀 Transformations

| ID | Name | Description | Source Location | Target Location | Status | Tested? |
|----|------|-------------|-----------------|-----------------|--------|---------|
| BL-G028 | Document pipeline | `TransformService.Transform()`: XML-serialize statement → XSLT transform → copy DOCX template → merge HTML into DOCX via `WmlToHtmlConverter` → convert to PDF → delete intermediate XML/DOCX/HTML → optional archive → `LogService.Log` (under a static lock) → `Publish`; any stage failure is wrapped into `ClaimStatementGenerationException` carrying `FRN`/`SchemeYear` | `Components/Turbine/TransformService.cs:59-128` | Not created; retain existing relative path pending gates | ⬜ | No direct unit test of `TransformService.Transform` found (file I/O and Office interop heavy; would need a fixture-based/golden-document test — see Coverage Gaps) |
| BL-G029 | PDF filename + rendering parameters | Output name: `RPA_OUT_BPS{schemeYear}_ClmStmt_{frn}_{sbi}_{DateService.CurrentDateTimeString()}.pdf`; `Zoom = 1.18F`; margins `Left=12.7f, Right=25f`; `Grayscale = true`; pinned to the shipped `wkhtmltopdf.exe` 0.12.4 — throws `FileNotFoundException` rather than letting NReco silently extract its own 0.12.6 | `Components/Turbine/TransformService.cs:130-156` | Not created; retain existing relative path pending gates | 🚫 Gated by the Linux-compatibility/PDF-parity proof required before Phase 3 container build (Migration-Plan §6 app-specific gate) | No test (would require a golden-PDF comparison per the same gate) |
| BL-G030 | HTML filename pattern (matches PDF pattern) | `RPA_OUT_BPS{schemeYear}_ClmStmt_{frn}_{sbi}_{DateService.CurrentDateTimeString()}.html` | `Components/Turbine/TransformService.cs:207` | Not created; retain existing relative path pending gates | ⬜ | No direct test |
| BL-G031 | DOCX/HTML conversion image-format mapping | `WmlToHtmlConverterSettings`: fixed CSS (`table{width:700px}`, page-break-avoid); `tiff` images are re-mapped to `gif`, `x-wmf` re-mapped to `wmf` (not passed through as-is); image counter increments per embedded image | `Components/Turbine/TransformService.cs:205-260+` | Not created; retain existing relative path pending gates | ⬜ | No direct test |
| BL-G032 | Timestamp-suffix format used in every generated filename | `CurrentDateTimeString()` → `_{day}{month}{year}{hour}{minute}{second}` — **no zero-padding**, so e.g. 3 Jan 09:05:07 → `_312095 7`-shaped non-fixed-width string; used by BL-G029/BL-G030 and the XML/DOCX temp-file names | `Services/DateService.cs:11-16` | Not created; retain existing relative path pending gates | ⬜ | Yes — `Services/DateServiceTests.cs` (active, 1 test — only asserts the string contains `"_"`, does **not** assert the non-padded format, so the exact shape is not test-locked) |
| BL-G033 | Outstanding-trigger dedup + ordering | `FuelTank.Fill()`: `GroupBy(FRN, MarketingYear).Select(First())` dedups outstanding DAX triggers before batching; if the deduped count exceeds 1, `OrderByDescending(Invoice)`; **then** `OrderDAX()` re-sorts by `SettlementDate` ascending, but **only when `daxOutstandingFinal.Count > maximumBatchSize`** (i.e., ordering strategy changes depending on whether a batch cap is actually in effect) | `Components/FuelTank/FuelTank.cs:46-53` (Fill), `159` (OrderByDescending(Invoice)), `184-191` (OrderDAX) | Not created; retain existing relative path pending gates | ⬜ | Exercised indirectly via `Components/FuelTank/FuelTankTests.cs` (71 tests) — not individually attributed by this pass |
| BL-G034 | XB import reconcile-by-diff | `XBImportService`: `InactivateDefunct` = `currentXB.Except(newXB, new XBDataComparer())` → marks matching `XB` rows inactive; `AddNew` = `newXB.Except(currentXB, new XBDataComparer())` → bulk-inserts only the new rows; equality (gap closed this revision) is defined by `XBDataComparer.Equals`/`GetHashCode` on **`FRN + InvoiceNumber + CalculationDate + SchemeYear`** (all 4 fields must match for two rows to be considered the same) | `Components/PumpingStation/XBImportService.cs:107-148`; comparer at `Utilities/XBDataComparer.cs` (full file read — **note: corrected location; previously mis-cited as `Extensions/`**) | Not created; retain existing relative path pending gates | ⬜ | No direct test of `XBImportService.Import`/`InactivateDefunct`/`AddNew` or of `XBDataComparer` itself found |
| BL-G035 | AP import reconcile-by-diff (same pattern as BL-G034, using `APComparer`) | `InactivateDefunct`/`AddNew` over `APComparer`-defined equality, after the BL-G019 scheme filter is already applied; equality (gap closed this revision) is defined by `APComparer.Equals`/`GetHashCode` on **`Supplier + Invoice + LastSettlementDate + Active`** | `Components/PumpingStation/APImportService.cs:112-130`; comparer at `Utilities/APComparer.cs` (full file read — **note: corrected location; previously mis-cited as `Extensions/`**) | Not created; retain existing relative path pending gates | ⬜ | No direct test found |
| BL-G045 | **SITI fixed-record-type state-machine parser (major gap closed this revision — previously entirely unmapped)** | `SITISerializer.DeSerialize` (overrides the generic `Serializer<T>.DeSerialize`) parses pipe-delimited (`\|`) SITI import files as a sequence of fixed record-type tags, requiring each tag's **immediately preceding line** to equal its defined predecessor before the corresponding fields are read: `PTA1`→`PTA2`→`PTBE`→`PTBPEN`→`PTBG`→`PTBGPEN`→`PTBYF`→`PTBPR`→`PTD`, building up one composite `Claim` object across the whole sequence. Unrecognized tags are **silently ignored** (`default: break` — no error, no log). All numeric fields are read **positionally** (`csv.GetField<decimal>(n)`, not by column name) with a repeated `!string.IsNullOrEmpty(field) ? value : 0` null/empty-to-zero coercion; all dates use `DateTime.ParseExact(field, "dd-MMM-yy HH.mm.ss", CultureInfo.InvariantCulture)` (exact format, not culture-flexible). A completed `Claim` is appended to the output list **only** at the `PTD` tag, gated by `claim.IsValid` (BL-G046) **and** `lastLine != "PTD"` (an anti-duplicate-trailer guard preventing two consecutive `PTD` lines from adding the same claim twice). | `Components/PumpingStation/Serializer/SITISerializer.cs` (full file read, 304 lines) | Not created; retain existing relative path pending gates | ⬜ | No test found for `SITISerializer.DeSerialize` in the active test tree — **this is a previously entirely-unmapped, non-trivial parser with no confirmed test coverage; see Risk Synthesis** |

### 🔌 Integrations

| ID | Name | Description | Source Location | Target Location | Status | Tested? |
|----|------|-------------|-----------------|-----------------|--------|---------|
| BL-G036 | SITI FTP download (control-file pattern) | `FTPService.Download(from, to, mask, controlPrefix)`: downloads `{from}{controlPrefix}{mask}` control files first, then for each control file transferred, downloads the corresponding data file (control-prefix stripped from the name); binary transfer mode; WinSCP session per call | `Components/PumpingStation/FTPService.cs:19-50` | Not created; retain existing relative path pending gates | 🚫 Gated by the Linux-compatibility proof (WinSCP is a Windows-native wrapper) before Phase 3 container build | No test found for `FTPService` |
| BL-G037 | SITI control-prefix constant + archive/cleanup | `controlPrefix = "CTL_"`; after import, source file archived via `fileService.Archive(...)` and the companion control file explicitly deleted | `Components/PumpingStation/SITIImportService.cs:34,114-115` | Not created; retain existing relative path pending gates | ⬜ | No test found |
| BL-G038 | FDMR CSV export via `xp_cmdshell`/`bcp` [DB-side] | Builds a `bcp "<select *>" queryout "<UNC path>" -T -c -t^| -S D1VMGEN001` command string and executes it via `EXEC xp_cmdshell`, gated by `WHERE sent_to_fdmr IS NULL`; marks exported rows with `sent_to_fdmr = SYSDATETIME()` after export; hardcoded target path `I:\RPA.Finance.FDMR\DEV\Import\ClaimStatement_Extract_<timestamp>.csv` and SQL Server instance name `D1VMGEN001` | `RPA.ClaimStatements.FDMR\LoadExtract.sql` (full file, 90 lines) | N/A — DB-side; external schema/data ownership per Migration-Plan | 🚫 **Blocked** — this is precisely the "no `xp_cmdshell`/host-level export" Azure SQL Database PaaS constraint called out as finding **C08** in `Migration-Plan.md`; no equivalent exists on Azure SQL Database and a client-driven redesign is required before any data-layer code change | N/A (DB-side) |
| BL-G047 | **Maps/Serializer/BulkInsert subfolders confirmed as no-hidden-transformation plumbing (gap closed this revision)** | All 12 `Components/PumpingStation/Maps/*SQLMap.cs` files (`APSQLMap`, `BPSSQLMap`, `BPSPENSQLMap`, `ClaimSQLMap`, `CLDSQLMap`, `GRSQLMap`, `GRPENSQLMap`, `PRSQLMap`, `SUMSQLMap`, `SUM2SQLMap`, `XBSQLMap`, `XBDataSQLMap`, `YFSQLMap`) are confirmed **pure 1:1 column-name passthrough** (`"SourceColumn,DestColumn"` static string-pair lists) with no renaming/transformation logic — `XBDataSQLMap.cs` is 426 lines purely because it mirrors the full per-UK-nation XBData field set, not because of extra logic. `APFileMap.cs` (CsvHelper `ClassMap`) is likewise a straight column→property map. `BulkInsert/SQLBulkInsert.cs` is a generic `SqlBulkCopy` wrapper (`BatchSize=10000`, `BulkCopyTimeout=60`, optional column mappings, no business rules); `BulkInsert/SITIBulkInsertContainer.cs` is a pure DI container of 10 typed `IBulkInsert<T>` instances (no logic). `Serializer/Serializer.cs` is a generic CsvHelper-based comma-delimited `Serialize`/`DeSerialize` (`WillThrowOnMissingField=false`, `TrimFields=true`) — its SITI-specific override is the major parser documented separately as BL-G045. **Separately noted:** `Utilities/SITIComparer.cs` (equality on `SUM?.FRN + SUM?.InvoiceNumber + SUM?.SchemeYear`, null-safe on the nested `SUM` reference) is defined and actively compiled, but **has no confirmed call site anywhere else in the repository** (grepped repo-wide) — unlike `XBDataComparer`/`APComparer` (BL-G034/BL-G035), it does not appear to be wired into any `Except(...)`-based reconcile path today; flagged as "present but currently inert," not asserted dead, since no exhaustive reflection/DI-wiring search was performed. | `Components/PumpingStation/Maps/*.cs` (12 files), `APFileMap.cs`, `BulkInsert/SQLBulkInsert.cs`, `BulkInsert/SITIBulkInsertContainer.cs`, `Serializer/Serializer.cs`, `Utilities/SITIComparer.cs` (all read in full this revision) | Not created; retain existing relative path pending gates | ⬜ | No tests found for any of these files individually; confirmed-no-hidden-logic is based on direct code read, not test assertions |

### 🔐 Authorization

No in-scope authorization/role logic was found inside `RPA.ClaimStatements.Generator` itself (it is a console batch
process with no end-user session). Role-based/sign-in authorization lives in the `RPA.ClaimStatements.Web` project
and the Entra ID target design — this **is** covered in this synthesized report, incorporated from the independent
Web-perspective agent's completed pass; see **§ Media and Web Business Inventory** below (rows W001, W004, W006,
W014, W017, W020-adjacent, W023, W024, and the "Existing Roles Model" subsection) for the full authorization
inventory. It is not repeated under this `BL-G*` heading because it was not independently re-derived by this
Generator-scope reading pass — this is a synthesis/cross-reference note, not a gap.

### 📧 Notifications

| ID | Name | Description | Source Location | Target Location | Status | Tested? |
|----|------|-------------|-----------------|-----------------|--------|---------|
| BL-G039 | Conditional tracking-report email | `TrackingStation.Report()` only calls `emailService.Send(...)` if at least one `Monitor` row recorded since the run's `start` timestamp has a `Reference` that does **not** start with `"Load Claim Statement Data"` — i.e., import-only runs with no other monitored activity send no email | `Components/TrackingStation/TrackingStation.cs:30-48` | Not created; retain existing relative path pending gates | ⬜ | No test found for `TrackingStation.Report`/`Build` |
| BL-G040 | Single-recipient report email | `EmailService.Send()`: `From` and `To` are **both** the same configured `"Email Account"` value (self-addressed report), `IsBodyHtml = true`, subject fixed `"Claim Statement Generation Report"`, sent via `SmtpSettingsSection.CreateClient()` | `Components/TrackingStation/EmailService.cs:34-49` | Not created; retain existing relative path pending gates | ⬜ | No test found |
| BL-G041 | Report HTML composition (already-modernized helper) | `TrackingStation.Build()` renders an HTML report (title, per-monitor start/end/duration, generated-statement count for `"Batch Generation"` monitors, per-run error listing) using a project-local `ReportHtmlWriter` class whose code comment states it **replaces** `System.Web.UI.HtmlTextWriter` (not available on .NET 10) and claims **byte-identical output** for the 4 tags it implements (`Div`/`H2`/`H4`/`P`) | `Components/TrackingStation/TrackingStation.cs:55-120`; replacement helper at `Components/TrackingStation/ReportHtmlWriter.cs` (full file, ~100 lines) | Already present at this relative path — **this substitution predates this mapping pass**; flagged here as a "Modified Logic" item per the skill's tracking protocol, not introduced by this report | ⬜ (pre-existing; not verified by this pass — the "byte-identical" claim in the code comment has not been independently confirmed) | No test found for `ReportHtmlWriter` or for a before/after HTML byte-comparison |
| BL-G042 | Persistent error log (not just console) | `ErrorService.Log()` writes every logged error to `db.Errors` (`Error` entity) **and** the console — errors are a durable business record, not a transient trace | `Services/ErrorService.cs:19-28` | Not created; retain existing relative path pending gates | ⬜ | Yes — `Services/ErrorServiceTests.cs` (active, 1 test: `Test_Log_Creates_Log_Without_FilePath`) |

### ⏰ Scheduling

| ID | Name | Description | Source Location | Target Location | Status | Tested? |
|----|------|-------------|-----------------|-----------------|--------|---------|
| BL-G043 | Invocation surface + statement-type/batch-size gate | `Program.Main`: accepts `args[0]` (statement type) / `args[1]` (batch size) non-interactively, or falls back to an interactive console prompt loop; valid statement types are exactly `BPS`, `XB`, `ALL`, `I` (imports-only, no generation/mutex path); batch size must `int.TryParse`; `"I"` calls `Activate(statementType)` with no batch size (skips the whole generation/mutex branch, BL-G023) | `Program.cs:26-145` | Not created; retain existing relative path pending gates | ⬜ | No test found (console entry point; not unit-testable as written) |
| BL-G044 | Fixed culture for the whole process | `CultureInfo.DefaultThreadCurrentCulture`/`DefaultThreadCurrentUICulture` forced to `en-GB` at startup — affects every string-based `DateTime.Parse`/`decimal.Parse`/`ToString("dd MMMM yyyy")` call elsewhere in this inventory (e.g., BL-G021's `DateTime.TryParse`, BL-G005's `DateOfCalculation` formatting) | `Program.cs:144-148` | Not created; retain existing relative path pending gates | ⬜ | No test found |

---

## Database Business Logic (DB-side, separate from compiled C# source)

| ID | Name | Type | Database/Schema | Status | Migration Approach | Notes |
|----|------|------|------------------|--------|--------------------|-------|
| DB-G001 | `FDMR.LoadExtract` | Stored Procedure | `RPA.ClaimStatements` DB, `FDMR` schema | 🚫 Blocked (see BL-G038/BL-G013) | Not decided — external schema/data ownership; `xp_cmdshell`/`bcp` pattern has no direct Azure SQL Database equivalent | Full file read (90 lines). Contains BL-G013 (Net4 calc) and BL-G038 (xp_cmdshell export) |
| DB-G002 | `dbo.claim_statement_data` table definition | Table DDL | `RPA.Finance.PaymentFilter.Integration (PreProd)` DB (per `USE` statement at top of file) | ⬜ Structurally read this revision (not Migrated/Verified) | Not decided | **Gap closed this revision**: full 1,146-line file read. Single `CREATE TABLE [dbo].[claim_statement_data]` statement with **402 columns** (lines ~11–413), overwhelmingly `decimal(18,2)`/`decimal(18,4)` fields following the same per-UK-nation (`England`/`Wales`/`Scotland`/`NI`) × per-payment-type (`BPS`/`Greening`/`YoungFarmer`/`Redistributive`) shape confirmed in BL-G006/BL-G007/BL-G009/BL-G048–BL-G054 — this table is the DB-side mirror of the `XBData` entity (same field-for-field shape as `XBDataSQLMap.cs`, BL-G047). One `PRIMARY KEY CLUSTERED` constraint named `[PK_XB.XBStage]` on `[XBDataID]` (line ~414). The remainder of the file (lines ~418–1146, ~730 lines) is a long sequence of individual `ALTER TABLE ... ADD CONSTRAINT [DF_...] DEFAULT ((0)) FOR [Column]` statements — one default-constraint per nullable numeric column, all defaulting to `0`. No other indexes, no other constraints (no foreign keys, no check constraints) were found in the file. **Not cross-checked against a live schema** (no live DB queried, per instruction) — this is a static-file read only. |
| DB-G003 | `dbo.claim_statement_data_gen` | Stored Procedure | Same DB as DB-G002 (UTF-16-encoded `.sql` file, `ALTER PROCEDURE [dbo].[claim_statement_data_gen]`) | ⬜ Structurally read this revision (not Migrated/Verified) | Not decided | **Gap closed this revision**: full 2,001-line file read structurally (not every one of its ~60 near-identical `UPDATE` blocks individually line-cited — see note below). Confirmed structure, in execution order: **(1)** a `@Rates TABLE` populated via **6** `INSERT INTO @Rates` blocks, one per scheme year **2015–2020** (lines 16–199), each supplying ~23 hardcoded rate constants per year (England/Wales/Scotland/NI BPS, FDM, over-150k, and Greening-region rates — e.g. 2015 England BPS Non-SDA Rate `171.83`, 2020 England BPS Non-SDA Rate confirmed present but not individually re-stated here to avoid redundant constant transcription; see file for exact per-year table); **(2)** `TRUNCATE TABLE [dbo].[claim_statement_data]` (line ~206) followed by a base-row `INSERT` (lines ~210–241) that joins an **external, separately-owned "Payment Engine" database's** `instances`/`claim_number_conversion`/`working_header`/`messages`/`versioned_land` tables (cross-database linked-server query; exact bracketed database identifier intentionally not repeated here beyond this one structural mention, per no-hosts-in-report instruction) and extracts the invoice number via an XML `.value()` XQuery against a `message_in` XML column; **(3)** a sequential pipeline of **59 distinct, individually `RAISERROR`-labeled** `UPDATE ... FROM claim_statement_data data LEFT JOIN <external calculation/land/entitlement staging tables> ON ...` blocks (lines ~262–1904), covering, per nation, BPS/Greening/YoungFarmer sub-totals, gross amounts, FDM & capping, over-declaration, late-claim, non-declaration, administrative-data, and crop-diversification — each block follows a uniform `COALESCE(source.field, 0)` additive pattern against `net5_*`/`net6_*` staging tables joined by `InstanceID`/`FRN`/`SchemeYear`/`country_name`/`region`; **one labeling anomaly observed**: the RAISERROR label `'INSERTING Scotland LATE CLAIM REDUCTION'` (mixed case) appears as a distinct section from the otherwise-consistent `'INSERTING SCOTLAND LATE CLAIM REDUCTION'` pattern used for the other 3 nations — not confirmed whether this is a true duplicate/copy-paste section or a deliberately separate later-added block, flagged not resolved; **(4)** a final `'EXPORTING DATA'` section (lines ~1942–1997) that uses **`EXEC master..xp_cmdshell`** to shell out to `bcp.exe` (exporting a column-header row and a data row via two separate `bcp queryout` calls to a `PreProd`-environment UNC file path, then `copy /b` concatenating them, then deleting the two temp files) — a **second, independent `xp_cmdshell`-based export pattern alongside BL-G038/DB-G001**, with a parallel, fully commented-out `Production`-environment variant of the same 5-step sequence directly below it in the file. **UNC host/path and server name intentionally not transcribed here** (no-hosts-in-report instruction) beyond noting the pattern exists and is environment-parameterized. **Rate constants not cross-checked against the C# model defaults** in `Models/ClaimStatements/*` (those classes read their numbers from DB-sourced entities at runtime, not from C# literals, so no direct code-vs-constant conflict was found, but exhaustive per-year numeric cross-checking was not performed). **Not every one of the ~59 `UPDATE` blocks was individually line-cited** — they were confirmed structurally uniform (same `COALESCE`-additive join pattern, varying only by nation/payment-type/staging-table names) via their RAISERROR labels (full list of 59 distinct labels extracted and reviewed) rather than transcribed block-by-block, consistent with the instruction to group identical formula/model families rather than reproduce line-for-line duplicates. **Not cross-checked against a live schema or live data** (no live DB queried). |

**Both CrossBorder SQL files have now been read structurally in full** (not every line individually cited, per the
note above for DB-G003's repetitive `UPDATE` blocks) — this closes the previous "only first 40/50 lines read" gap.
Remaining limitation: numeric rate-constant values for scheme years beyond what is cited above, and the exact column
set referenced inside each of the 59 per-nation/per-type `UPDATE` blocks, were not individually transcribed line-by-line
(confirmed structurally uniform instead) — see § Coverage Gaps.

---

## Media and Web Business Inventory

**Synthesized from an independent Web-perspective agent's completed pass** (`RPA.ClaimStatements.Web` /
`RPA.ClaimStatements.Web.Tests` controllers + `Security/` authorization contracts). **This Generator-scope pass did
not scan Web source itself** — the rows below are incorporated verbatim from that agent's returned, source-verified
inventory for same-task synthesis, not re-derived or re-analyzed here. Static/media asset paths remain owned by
**[Media-Assets-Mapping.md](Media-Assets-Mapping.md)** (already exists in `reports/`); the Web-perspective agent
re-checked that file during its pass and found no inaccuracies requiring a fix — no edit to it was made by either
agent.

> **Catalog-ID note:** rows are labeled `W001`–`W025` exactly as returned by the Web-perspective agent. This
> synthesis pass did not renumber them into `BL-*` — that is a statement of what happened here, not a permanent rule
> against ever minting a `BL-W` series or renumbering in a future pass; that remains a future-maintainer decision.
> Separately, `W001`–`W025` and `BL-G001`–`BL-G054`/`DB-G001`–`DB-G003` are both **open, arbitrary inventory ID
> series local to this report** — neither should be confused with the closed, fixed **18-item `D-01`–`D-18`**
> architecture-decision catalog in `Migration-Plan.md`, which tracks a different kind of artifact entirely (closed
> architecture decisions, not open business-logic inventory rows). Target-column convention for every row below
> (carried over unchanged): *Not created; planned same relative path; retain EF6 unless approved.* No ORM/EF Core
> migration and no Entra group→role mapping is prescribed by any row — both remain open decisions (see Q3.1/Q3.9).

### Normalized Web Inventory (25 rows)

| # | Category | Source (relative from this file) | Behavior (source-verified by the Web-perspective agent) | Target | Status | Verification expectation | Test coverage |
|---|---|---|---|---|---|---|---|
| W001 | Authorization | [`HomeController.cs:13`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/HomeController.cs#L13) | Class-level `[Authorize(Roles="Claim Statement Generator: Trigger Generation, Claim Statement Generator: Manage Exceptions, Claim Statement Generator: Manage Suppressions, Claim Statement Generator: View Claim Statements, Claim Statement Generator: Manual Upload")]` — ASP.NET Core `Roles=` is OR-semantics (any one role satisfies); `Trigger` (line 37) adds a further single-role requirement "Trigger Generation" | Not created; planned same relative path; retain EF6 unless approved | Inventoried | Role-claim strings must be emitted verbatim by whatever replaces `RoleManagerClaimsTransformation` post-cutover (see W024) | `HomeControllerTests.cs`: Index, Trigger (view-return only, not auth) |
| W002 | Workflow | [`HomeController.cs:37-48`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/HomeController.cs#L37-L48) | `Trigger(StatementTypes="ALL", maximumBatchSize=10)`: `if (maximumBatchSize > 0) powerPlant.Activate(...)`, then unconditionally sets `TempData["Message"]="Generation complete"` and redirects to Index — message/redirect fire regardless of whether the guard passed | Not created; planned same relative path; retain EF6 unless approved | Inventoried | Must preserve the always-redirect/always-message behavior, not just the success path | Tested (view-return only) |
| W003 | Transformation/UX | [`HomeController.cs:51-60`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/HomeController.cs#L51-L60) | `Sample(statementType)`: branches to `sampleXBService.Sample()` only when `statementType=="XB"` else `sampleService.Sample()`; builds filename `"Sample Claim Statement - {statementType}.docx"`; reads bytes then `File.Delete(filePath)` immediately; streams with `MediaTypeNames.Application.Octet` | Not created; planned same relative path; retain EF6 unless approved | Inventoried | Temp file must not persist post-download; content-type stays octet-stream, not the real docx mime | **No test** — gap |
| W004 | Authorization | [`ImportController.cs:12`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/ImportController.cs#L12) | Class-level `[Authorize(Roles="Claim Statement Generator: Trigger Generation")]` — single role | Not created; planned same relative path; retain EF6 unless approved | Inventoried | — | `ImportControllerTests.cs`: Import (view only) |
| W005 | Workflow | [`ImportController.cs:20-26`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/ImportController.cs#L20-L26) | `Import()` unconditionally calls `pumpingStation.Activate()`, sets `TempData["Message"]="Import run complete"`, redirects to `Home/Index` — no branching on outcome visible at this layer | Not created; planned same relative path; retain EF6 unless approved | Inventoried | Upstream failure handling inside `pumpingStation.Activate()` is Generator-internal — see BL-G026/BL-G027 above for that side | Tested (view only) |
| W006 | Authorization | [`ConfigurationController.cs:13`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/ConfigurationController.cs#L13) | Class-level `[Authorize(Roles="Claim Statement Generator: Trigger Generation")]` applies to both `Index` and `SchemeYear` actions — same role gates both config screens, no per-action override | Not created; planned same relative path; retain EF6 unless approved | Inventoried | **Q3.2** — Observed behavior — no comment evidence of intent: is reuse of "Trigger Generation" role (rather than a "Manage Configuration" role) deliberate or a legacy artifact? | Tested (POST only, not GET) |
| W007 | Workflow | [`ConfigurationController.cs:30-38,48-56`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/ConfigurationController.cs#L30-L56) | POST `Index`/`SchemeYear`: iterate list, `db.SetModified(x)` per item (bulk attach-as-modified, no per-row diff), single `SaveChanges()`; **no `ModelState.IsValid` check before save in either action** | Not created; planned same relative path; retain EF6 unless approved | Inventoried | **Q3.3** — Observed behavior — no comment evidence of intent: absence of validation gate before save — confirm whether this is accepted as minimum-viable carry-over or needs a Decisions-Required entry | `ConfigurationControllerTests.cs` POST tests exist (view-return assertions only, not save-semantics) |
| W008 | Query semantics | [`SuppressionController.cs:39-55`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/SuppressionController.cs#L39-L55) | `ExistingSuppressions(searchString,page,pageSize)`: search branch (line 45) `x.SuppressionEnd==null && x.FRN.ToString()==searchString.Trim()` (exact match, trimmed); no-search branch (line 49) lists all open suppressions | Not created; planned same relative path; retain EF6 unless approved | Inventoried | **Q3.4** — Observed behavior — no comment evidence of intent: is exact-match (not partial) FRN search the desired UX, or should partial-match be introduced (a behavior change requiring sign-off)? | Tested (list-return assertions) |
| W009 | Workflow/Routing | [`SuppressionController.cs:28-36`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/SuppressionController.cs#L28-L36) | `_Existing(searchString=null,page=1,pageSize=15)` partial action. **Explicit in-code comment (line 30): "The existing-suppressions panel was a child action (RenderAction(\"_Existing\")) with default arguments."** — documents this as an already-completed, intentional MVC-version adaptation | Not created; planned same relative path; retain EF6 unless approved | Inventoried | Confirm default paging (pageSize=15) is preserved exactly; comment confirms intent, so no re-adaptation should occur in Phase 2 | Not directly tested |
| W010 | Transformation | [`SuppressionController.cs:58-69`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/SuppressionController.cs#L58-L69) | `Download()`: 4-column projection, `SuppressionEnd==null` filter, desc order, CSV via `new UTF8Encoding(false).GetBytes(...)` (**no BOM**), filename `Suppressions.csv`, mime `application/text`. **Explicit in-code comment (line 68): "UTF-8 without a byte order mark and without a charset parameter, as before."** | Not created; planned same relative path; retain EF6 unless approved | Inventoried | No-BOM encoding is comment-evidenced intentional. Part of the 3-way BOM matrix (see below): Suppression=no-BOM, Log=no-BOM, Error=BOM | **No test** — gap (also requires "Download Data" role) |
| W011 | Validation/Workflow | [`SuppressionController.cs:86-103`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/SuppressionController.cs#L86-L103) | POST `_CreateSuppression(Suppression)`: `ModelState.IsValid` gate (line 88); dedupe query (line 90, verified exact predicate): `db.Suppressions.AsNoTracking().Where(x => x.FRN == suppression.FRN && x.SchemeYear == suppression.SchemeYear && x.SuppressionEnd != null).FirstOrDefault()` — **the guard checks for an already-ended suppression** (`SuppressionEnd != null`, i.e. a previously-ended record) for the same FRN+SchemeYear, not a currently-open one; if found, the new suppression is **not** added/saved (lines 92-97 skipped); in both the found and not-found cases the action returns `Json(new { Ok = true })` (line 99) | Not created; planned same relative path; retain EF6 unless approved | Inventoried | **Q3.5** — Observed behavior — no comment evidence of intent: silently skipping persistence while still returning `Ok:true` when a prior ended suppression exists for the same FRN+SchemeYear — confirm this is the desired response contract (client cannot distinguish "saved" from "skipped-as-duplicate"). **Preserve the observed predicate (`SuppressionEnd != null`) exactly — do not assume a different intent (e.g. "no open suppression") without a decision record.** | Tested (`_CreateSuppression` view-return test exists, not the dedupe/no-op branch) — **partial gap** |
| W012 | Validation | [`SuppressionController.cs:114-128`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/SuppressionController.cs#L114-L128) | POST `_BulkSuppression`: try/catch wraps `suppressionService.BulkUpdate(...)`; catch (line 124) rethrows a **new** `Exception` with fixed message "Unable to bulk update suppressions. Please ensure that supplied file contains only ten digit FRNs and is in .xslx format." wrapping the original as `InnerException` — original exception detail is not surfaced at this layer | Not created; planned same relative path; retain EF6 unless approved | Inventoried | **Q3.6** — confirm whether/how this rethrown exception currently reaches the end user (no visible catch at the view/filter layer in this controller) | Tested (view-return only, not exception path) — **gap** |
| W013 | Workflow | [`SuppressionController.cs:131-143`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/SuppressionController.cs#L131-L143) | `RemoveSuppression(Guid id)`: null-safe `Find`; on hit calls `.End()`, `db.SetModified()`, `SaveChanges()`; returns `Content("")` (HTTP 200, empty body) **both on hit and on miss** — no 404/error signal when `id` doesn't match | Not created; planned same relative path; retain EF6 unless approved | Inventoried | **Q3.7** — Observed behavior — no comment evidence of intent: silent no-op on unknown id — same pattern recurs at W020; confirm whether systemic and intended or an oversight | Not tested |
| W014 | Authorization | [`LogController.cs:14-17`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/LogController.cs#L14-L17) | File declares a **nested namespace** `namespace RPA.ClaimStatements.Web.Controllers { namespace RPA.ClaimStatements.Controllers { ... } }` (lines 12-14) — class `LogController` ends up in namespace `RPA.ClaimStatements.Web.Controllers.RPA.ClaimStatements.Controllers`. Class-level `[Authorize(Roles="Claim Statement Generator: View Claim Statements")]` (line 16); `Download` further requires "Download Data" (line 28) | Not created; planned same relative path; retain EF6 unless approved | Inventoried | **Q3.8** — the double-namespace is a structural oddity worth flagging to Phase 2 code review; not a behavior change, but confirm whether the fully-qualified nested-namespace type name is referenced anywhere (DI registration, reflection) before refactoring it away | **No LogController tests found at all** — full gap |
| W015 | Transformation | [`LogController.cs:29-40`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/LogController.cs#L29-L40) | `Download(count)`: 5-column projection incl. `PDFFile`; `Take(count)` with **no upper-bound validation on `count`**; CSV via `new UTF8Encoding(false).GetBytes(...)` (**no BOM**), filename `Log.csv`. **Explicit in-code comment (line 38): "UTF-8 without a byte order mark and without a charset parameter, as before."** — same comment wording as W010 | Not created; planned same relative path; retain EF6 unless approved | Inventoried | No-BOM encoding is comment-evidenced intentional (matches W010). Unbounded `count` parameter is a separate, uncommented hardening note only — no change applied | Not tested |
| W016 | Query semantics | [`LogController.cs:42-60`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/LogController.cs#L42-L60) | `Index(searchString,page,pageSize=15)`: search branch (line 52) matches **`FRN.ToString()==trim` OR `SBI.ToString()==trim`** (OR of two exact-match fields, single search box); also reads `configurationService.IsActive("Manual Upload")` into `ViewBag.uploadActive` (line 57) to drive UI | Not created; planned same relative path; retain EF6 unless approved | Inventoried | **Q3.10** — Observed behavior — no comment evidence of intent: OR-of-two-fields with no indication to the user which field matched — confirm desired before altering | Not tested |
| W017 | Authorization | [`ErrorController.cs:29,60,76`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/ErrorController.cs#L29) | Class-level `[Authorize(Roles="Claim Statement Generator: View Claim Statements")]` (line 29); `Download` → "Download Data" (line 60); `ResolveError` → "Manage Exceptions" (line 76) | Not created; planned same relative path; retain EF6 unless approved | Inventoried | — | **No ErrorController tests found** — full gap |
| W018 | Query semantics | [`ErrorController.cs:41-58`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/ErrorController.cs#L41-L58) | `Index(searchString,page,pageSize=10)`: both branches always filter `!x.Resolved`; search branch (line 51) additionally requires `FRN.ToString()==trim` — **AND** semantics (contrast with W016's OR) — resolved errors are never shown in this view, even when searching; also surfaces `ViewBag.ForceGeneration` from `configurationService.IsActive("Force Generation")` (line 54) | Not created; planned same relative path; retain EF6 unless approved | Inventoried | **Q3.11** — Observed behavior — no comment evidence of intent: confirm there is no other path to view `Resolved=true` errors — is a permanently-filtered-out resolved-errors view intentional? | Not tested |
| W019 | Transformation | [`ErrorController.cs:61-73`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/ErrorController.cs#L61-L73) | `Download()`: CSV via `StreamExtensions.ToBytes(errors.ToCsv(), Encoding.UTF8)` (line 72) — `Encoding.UTF8` **includes** the BOM. **Explicit in-code comment (line 71): "The UTF-8 byte order mark is part of the file, as before."** — different wording/intent from W010/W015's explicit no-BOM comments | Not created; planned same relative path; retain EF6 unless approved | Inventoried | BOM-inclusion is comment-evidenced intentional for **this export only**; the 3-way matrix (Suppression=no-BOM, Log=no-BOM, Error=BOM) must be preserved per-endpoint, not normalized to one convention | Not tested |
| W020 | Workflow | [`ErrorController.cs:77-88`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Controllers/ErrorController.cs#L77-L88) | `ResolveError(Guid id)`: null-safe `Find` (line 79); sets `Resolved=true`, `SaveChanges()` on hit; returns `Content("")` **both on hit and on miss** — same silent-no-op-on-miss pattern as W013 | Not created; planned same relative path; retain EF6 unless approved | Inventoried | **Q3.7 (same question as W013)** — recurrence across two controllers suggests a systemic pattern; confirm scope (preserve as-is vs. flag) | Not tested |
| W021 | Integration/Hosting | [`Program.cs:40-63,73-86`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Program.cs#L40-L63) | Host composition governing every controller above: `AddAuthentication("WindowsOrNegotiate")` with a policy scheme (lines 42-49) selecting `IISServerDefaults` vs `NegotiateDefaults` by server type at runtime; `AddAuthorization()` (line 51); custom role-store/claims-transformation registration (lines 53-63, see W024); a custom middleware runs a `ChallengeAsync()` for any unauthenticated request **before** `UseStaticFiles()` is reached — meaning static assets also require authentication today; fixed `en-GB` request culture | Not created; planned same relative path; retain EF6 unless approved — **Entra ID / OIDC replacement is a Decisions-Required item; no target mapping (including group→role mapping) is prescribed here** | Inventoried | **Q3.1** — the single highest-impact open question for the Entra ID migration: should "static assets require authentication" be preserved, and what replaces the Windows-Negotiate→SQL-role-lookup chain (see W023-W025)? Not decided; must not be silently resolved in Phase 2 | N/A (host-level, no unit test) |
| W022 | Packaging/Non-mismatch | [`RPA.ClaimStatements.Web.csproj`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/RPA.ClaimStatements.Web.csproj) + [`RPA.ClaimStatements.Web.Tests.csproj`](../RPA.ClaimStatements/RPA.ClaimStatements.Web.Tests/RPA.ClaimStatements.Web.Tests.csproj) | Both csproj files exclude `ARController.cs` / `ARControllerTests.cs` respectively via `<Compile Remove>`, each with an explicit comment that it was never part of the legacy build and stays in git unchanged | N/A | Inventoried (informational — confirmed consistent, not a mismatch) | — | File exists on disk but excluded from compilation in both the controller and its test project — symmetric, matches the same exclusion pattern already confirmed on the Generator side (§ Source Exclusion Reconciliation above) |
| W023 | Authorization contract | [`Security/RoleStore.cs:10-14,19-24,27-50`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Security/RoleStore.cs#L10-L50) | `IRoleStore.GetRolesForUser(string userName)` contract; `SecurityDatabaseRoleStore` implementation (lines 19-50) queries a **SQL Server stored procedure `dbo.aspnet_UsersInRoles_GetRolesForUser`** (ASP.NET legacy membership/role-provider schema) via `IDTSecurityConnection` connection string + `IDTSecurityApplicationName` app setting, keyed by `userName` — **not** Windows group membership, **not** Entra ID groups/app roles. Explicit in-code doc-comment: "Reads roles from the Security database (ASP.NET membership schema) ... which is the call the former IDT role provider made. Role inheritance is resolved by the database." | Not created; planned same relative path; retain EF6 unless approved — **no Entra group→role mapping is prescribed; this remains a SQL-backed stored-procedure role lookup until an explicit decision replaces it** | Inventoried | **Q3.1 / Q3.9** — confirm target: keep SQL-backed role lookup as-is (rehost), or replace with Entra ID group claims/App Roles (requires schema/ownership decision from the external security-database-owning team — out of this engagement's authority per the code-only/external-schema-ownership scope lock already recorded in Migration-Plan.md) | No dedicated unit tests found for `RoleStore.cs` |
| W024 | Authorization contract | [`Security/RoleManagerPrincipal.cs:10-15,19-34`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Security/RoleManagerPrincipal.cs#L10-L34) | `RoleManagerPrincipal : ClaimsPrincipal` wraps the authenticated principal; roles are lazily loaded once per request from `IRoleStore.GetRolesForUser(principal.Identity.Name)` into a `HashSet<string>` compared with `StringComparer.OrdinalIgnoreCase` (line 24); `IsInRole(role)` (lines 27-34) is the sole role-check entry point consumed by every `[Authorize(Roles=...)]` attribute across W001-W020. **Explicit doc-comment: "Mirrors the ASP.NET `RolePrincipal` used by the legacy roleManager: roles come only from the role store for `Identity.Name` (Windows group membership is not used), are loaded once, on first use, and are compared case-insensitively."** | Not created; planned same relative path; retain EF6 unless approved | Inventoried | Case-insensitive role-name comparison is comment-evidenced intentional and must be preserved by any replacement; confirms Windows group membership is explicitly **not** used today — an Entra "group→role" mapping would be a **new** mechanism, not a like-for-like port | No dedicated unit tests found |
| W025 | Authorization contract | [`Security/RoleManagerPrincipal.cs:36-53`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Security/RoleManagerPrincipal.cs#L36-L53) | `RoleManagerClaimsTransformation : IClaimsTransformation` — registered at [`Program.cs:63`](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Program.cs#L63) via `AddTransient<IClaimsTransformation, RoleManagerClaimsTransformation>()`; `TransformAsync` (lines 46-53) short-circuits (returns principal unchanged) when not authenticated or already a `RoleManagerPrincipal` (avoids double-wrapping), otherwise wraps into a new `RoleManagerPrincipal` | Not created; planned same relative path; retain EF6 unless approved | Inventoried | Double-wrap guard is a framework-integration detail — preserve the guard if the role-resolution mechanism changes | No dedicated unit tests found |

### CSV BOM Matrix (confirmed, cross-controller)

| Export | Encoding | Comment-evidenced? |
|---|---|---|
| `SuppressionController.Download` (W010) | `UTF8Encoding(false)` — **no BOM** | Yes — "as before" |
| `LogController.Download` (W015) | `UTF8Encoding(false)` — **no BOM** | Yes — "as before" (same wording as W010) |
| `ErrorController.Download` (W019) | `Encoding.UTF8` — **with BOM** | Yes — "as before" (different wording from W010/W015) |

**This 2-no-BOM / 1-BOM matrix must be preserved per-endpoint during migration, not normalized to a single convention.**

### Existing Roles Model (confirmed, applies to W001/W004/W006/W014/W017/W020-adjacent authorization rows)

Roles are **not** Windows groups and **not** Entra ID groups/App Roles today — `SecurityDatabaseRoleStore` (W023) calls
a SQL Server stored procedure (`dbo.aspnet_UsersInRoles_GetRolesForUser`, ASP.NET legacy membership schema) against an
externally-owned Security database. **No Entra group→role mapping has been selected** — Q3.1/Q3.9 remain open, and
per the code-only/external-schema-ownership scope lock already recorded in `Migration-Plan.md`, resolving the target
mapping requires the external security-database-owning team, not this engagement unilaterally.

### Open Questions Register (Q3.1–Q3.11)

| ID | Question | Raised at row(s) | Needs input from |
|---|---|---|---|
| Q3.1 | Should "static assets require authentication" (no anonymous CSS/JS/image access) be preserved under Entra ID, and what replaces the Windows-Negotiate → SQL-role-lookup chain end-to-end? | W021, W023 | Repo owner / security decision |
| Q3.2 | Is reusing "Trigger Generation" role to gate the Configuration screens deliberate, or a legacy artifact that should get its own role? | W006 | Repo owner |
| Q3.3 | Is the absence of `ModelState.IsValid` validation before bulk-save in `ConfigurationController` POST actions acceptable as minimum-viable carry-over? | W007 | Repo owner |
| Q3.4 | Is exact-match (not partial) FRN search in `SuppressionController` the desired UX, or should it become partial-match during migration (would be a behavior change requiring sign-off)? | W008 | Repo owner |
| Q3.5 | Is it acceptable that `_CreateSuppression` returns `Ok:true` even when the record was silently skipped due to an existing ended-suppression match? | W011 | Repo owner / business |
| Q3.6 | Does the rethrown bulk-suppression exception message currently reach the end user, and via what mechanism (global error page, AJAX error handler)? | W012 | Repo owner / QA |
| Q3.7 | Is silent no-op-on-missing-id (no 404/error) in `RemoveSuppression` and `ResolveError` an accepted systemic pattern, or should these surface errors post-migration? | W013, W020 | Repo owner |
| Q3.8 | Is the nested-namespace declaration in `LogController.cs` referenced anywhere by fully-qualified name (DI, reflection) that would break if "cleaned up" in Phase 2? | W014 | Code-migration phase, verify before refactor |
| Q3.9 | Should the SQL-backed role store (`IDTSecurityConnection` / `aspnet_UsersInRoles_GetRolesForUser`) be replaced by Entra ID group/App Role claims, kept as-is behind Entra authentication, or bridged — and who owns the Security database schema/data for that decision? | W023, W024 | External security-database-owning team (code-only scope boundary noted) |
| Q3.10 | Is OR-of-FRN-or-SBI exact-match search in `LogController.Index` the desired UX? | W016 | Repo owner / business |
| Q3.11 | Confirm there is no other path to view `Resolved=true` errors — is a permanently-filtered-out resolved-errors view intentional? | W018 | Repo owner / business |

### Web Inventory Counts (as returned by the Web-perspective agent)

- Total inventory rows: **25** (W001–W022 normalized + W023–W025 new for `Security/RoleStore.cs` + `RoleManagerPrincipal.cs`)
- Comment-evidenced "intentional" rows (explicit code comment, not asserted): **5** (W002, W009, W010, W015, W019)
- Rows downgraded from "intentional" to "Observed behavior — no comment evidence of intent" + Q3 question: **7** (W006, W007, W008, W011, W013, W016, W018, W020 — W013/W020 share Q3.7)
- Open subquestions recorded: **11** (Q3.1–Q3.11)
- New catalog IDs minted during Web inventory or this synthesis: **0**
- `Media-Assets-Mapping.md` changes made by the Web-perspective agent: **none** — re-checked, no inaccuracies found

---

## Modified Logic (requires documentation — found, not introduced, by this pass)

| ID | Original Logic | New Logic | Reason for Change | Approved By |
|----|----------------|-----------|---------------------|-------------|
| BL-G041 | `System.Web.UI.HtmlTextWriter` (not available on .NET 10) | Project-local `ReportHtmlWriter` class implementing the same 4 tags (`Div`/`H2`/`H4`/`P`) with a code comment claiming byte-identical output | Required for .NET 10 compatibility (`System.Web.UI` namespace removed) | **Unknown — the substitution pre-dates this mapping pass; no Decision-Log entry for it was located.** Flagged for the parent workstream to confirm when/by whom this was approved, since it is a functional change to notification-email HTML generation that has not been independently verified against the original output |

---

## Blocked Items

| ID | Name | Blocker Description | Required Decision | Owner | Due |
|----|------|---------------------|--------------------|-------|-----|
| BL-G023 / BL-G027 | Single-replica named Mutex (generation + all 4 import services) | `Global\`-namespace `Mutex` provides no cross-replica exclusion and is a Windows-only kernel-object concept; container-replica count and OS base image are both still open | Migration-Plan decision **C13** (Mutex/concurrency) — not yet recorded | Architect / Database Specialist, per Migration-Plan §6 | Before any Phase 3 replica-count configuration |
| BL-G024 | Runtime-applied EF6 migrations (`DbMigrator.Update()` on every activation) | No external-team schema-ownership/migration-execution contract received yet | Migration-Plan finding **C03** | Database Specialist | Before any Phase 2 data-access-layer code change |
| BL-G038 / DB-G001 | FDMR export via `xp_cmdshell`/`bcp` to a UNC path on a named SQL Server instance | No `xp_cmdshell` or host-level file export exists on Azure SQL Database PaaS | Migration-Plan finding **C08** | Database Specialist, Architect | Before any Phase 2 data-access-layer code change touching FDMR export |

---

## Verification Test Coverage

| Category | Items inventoried | Items with a located active test | Coverage (by item count, not assertion depth) |
|----------|-------------------|-----------------------------------|------------------------------------------------|
| Calculations | 20 (incl. 1 DB-side; +7 new this revision: BL-G048–BL-G054) | 3 (BL-G001, BL-G002, BL-G011) | 15% |
| Validations | 9 (+1 new: BL-G046) | 4 (BL-G014–BL-G018 share the same 28-test file) | 44% (by item; all through one shared test file) |
| Workflows | 6 | 1 (BL-G025) | 17% |
| Transformations | 9 (+1 new: BL-G045) | 1 fully, 1 partially (BL-G032 asserts shape loosely; BL-G033 only indirectly via FuelTankTests) | ~11–22% depending on how "tested" is counted |
| Integrations | 4 (incl. 1 DB-side; +1 new: BL-G047) | 0 | 0% |
| Notifications | 4 | 1 (BL-G042) | 25% |
| Scheduling | 2 | 0 | 0% |
| **Total** | **54** (3 BL-G DB-side-tagged; **+10 new rows this revision: BL-G045–BL-G054**) | **10 distinct active test files covering a subset of the above (unchanged — none of the 10 new rows have a located test)** | **No single aggregate percentage is asserted — item-level counts above are the only claim made** |

**This table counts "a test file exists that exercises the class" — it does not claim every business rule inside that
file is independently asserted.** For example, `ValidationServiceTests.cs` (28 tests) covers all four methods in
BL-G014–BL-G018, but no attempt was made in this pass to map every one of the 28 individual test methods to a specific
rule variant (e.g., Euro vs. Sterling branch, BPS vs. XB branch). **None of the 10 newly-added rows this revision
(the SITI parser, the `Claim.IsValid` gap, the Maps/Serializer/BulkInsert confirmation, and the 7 payment-model
calculation defects/rules) have a located active test — this is itself the most actionable finding of this revision,
see Risk Synthesis items 1–3 below.**

---

## Risk Synthesis (prioritized, not a security audit)

1. **Highest — `LateChangePenaltyTotal` is silently never computed for NI/Scotland/Wales Young Farmer payments**
   (BL-G049). All 3 per-nation `YoungFarmerPayment` override subclasses omit the `LateChangePenaltyTotal =
   NonDeclarationTotal - LateChangePenaltyReduction;` assignment entirely, so `FDMTotal = LateChangePenaltyTotal -
   FDMReduction` always evaluates from the decimal default `0` — i.e. `FDMTotal` for every NI/Scotland/Wales Young
   Farmer claim is computed independently of the claim's actual `NonDeclarationTotal`. This is a pre-existing source
   defect (not introduced by this pass), has **no located test coverage**, and — unlike the platform-compatibility
   risks below — is a potential **financial correctness** issue in the current production system, not only a
   migration-compatibility one. **Flagged for business/finance confirmation before any Phase 2 code touches this
   calculation family — not silently "fixed" or assumed intentional.**
2. **Highest — `LateChangePenaltyTotal` self-referential no-op bug in 3 Greening override subclasses** (BL-G048).
   `NIGreeningPayment.cs:155`, `ScotlandGreeningPayment.cs:193`, `WalesGreeningPayment.cs:155` all read
   `LateChangePenaltyTotal = NonDeclarationTotal - LateChangePenaltyTotal;` (destination variable on both sides,
   defaulting to `0`), silently discarding the `LateChangePenaltyReduction` deduction even though that field is
   correctly populated and still influences the "LastPenalty2" **label** selection — i.e. the penalty name can be
   reported while its amount is never subtracted. Same financial-correctness framing as item 1; same "flag, don't
   silently fix" instruction applies.
3. **Highest — Mutex-based single-writer guarantee does not survive the target platform as written** (BL-G023,
   BL-G027). A `Global\`-prefixed named `Mutex` has no meaning outside a single Windows machine; Azure Container Apps
   is Linux-only per the already-recorded Migration-Plan finding. This affects **5 independent mutexes** (generation +
   4 import services), not just the one already flagged as C13 — the import-side mutexes do not appear to have been
   individually called out before this pass.
4. **High — FDMR export has no direct Azure SQL Database equivalent** (BL-G038/DB-G001, already gated as C08). Any
   code-layer change touching this path before a redesign is agreed would be a silent architecture decision, which
   the user has explicitly required to be surfaced, not assumed. **Compounding finding this revision (DB-G003):** the
   separate `claim_statement_data_gen` stored procedure contains a **second, independent `xp_cmdshell`/`bcp`-based
   export pattern** (environment-parameterized header+data CSV export to a UNC path) — this is not a one-off; the
   `xp_cmdshell`-dependent export pattern recurs in at least two DB-side assets and should be treated as a family of
   gated items, not a single isolated one, when C08 is actually resolved.
5. **High — runtime-applied EF6 migrations on every process start** (BL-G024, already gated as C03) compound with the
   external-schema-ownership scope lock: if the external team's provisioning process also applies migrations (or
   expects to own schema state exclusively), two independent migration appliers could race or conflict. This is a
   sharper framing of C03 than "schema shape must match" — it is specifically about **who is allowed to run
   `DbMigrator.Update()`** against the externally-owned database.
6. **High — authorization role source is a SQL-backed stored procedure with no Entra mapping selected** (W023/W024,
   Q3.1/Q3.9). `SecurityDatabaseRoleStore` calls `dbo.aspnet_UsersInRoles_GetRolesForUser` against an **externally
   owned** Security database — role-source migration therefore sits behind the same external-schema-ownership
   scope lock already recorded for the Generator's data access (C01-adjacent). Windows group membership is
   explicitly **not** used today (confirmed by an in-code comment on `RoleManagerPrincipal`), so an Entra
   group→role mapping would be a **new** mechanism, not a like-for-like port — must not be silently assumed or
   selected without the external team and a Decisions-Required entry.
7. **Medium–High — a previously entirely-unmapped SITI import parser has zero test coverage** (BL-G045). The
   `SITISerializer` pipe-delimited, strict-predecessor-tag state machine (9-step record-type sequence) is a
   non-trivial, silently-permissive parser (unrecognized tags are dropped with no error/log) with **no located active
   test anywhere in the repository**. Combined with the BL-G046 `Claim.IsValid` gap (next item), this is the least
   test-protected import path in the whole Generator, yet structurally the most complex one found.
8. **Medium — `Claim.IsValid` does not check `YF` for null** (BL-G046). A SITI claim missing or malforming its
   `PTBYF` record-type line still passes validation and is added to the output with `YF == null`; whether any
   downstream code path dereferences `claim.YF` (as opposed to the separate `SUM2.YoungFarmerValue`, BL-G008) was
   **not independently confirmed** this pass — flagged as a plausible, not confirmed, null-reference risk.
9. **Medium — two field-reference/copy-paste bugs found in per-nation payment subclasses this revision**
   (BL-G050 `ScotlandGreeningPayment.cs:48` Region3Total reading a Region1 field; BL-G051
   `NIYoungFarmerPayment.cs`'s YCP-rate lookup reading a Scotland-prefixed XBData field). Both are consistent with a
   "copy a sibling-nation file, change most but not all field references" authoring pattern already observed
   elsewhere in this codebase (e.g. the pre-existing `apMutexId`-named-but-XB-guarding field, item 12 below) —
   suggests this authoring pattern may be systemic across the 10 per-nation payment subclasses, not isolated to the
   2 instances confirmed here. **Not exhaustively re-verified across every per-nation field in every subclass** —
   see Coverage Gaps.
10. **Medium — untested division-by-zero surface** (BL-G009, Progressive Reductions `PRBxAmount =
    (Result/Percent)*100` for 4 bands). No test exercises a zero-percent, non-zero-result combination; current
    behavior on such input is an unhandled exception, not a validated business rule.
11. **Medium — un-tested document/transformation pipeline** (BL-G028–BL-G031): no unit test exists for
    `TransformService.Transform` or its PDF/HTML filename and rendering-parameter rules; this is also exactly the area
    gated behind the Linux-compatibility + golden-document-comparison requirement in Migration-Plan §6, so the lack of
    existing tests means that gate has **no existing baseline to diff against** — a new golden-fixture baseline would
    need to be captured from the current Windows-hosted behavior before any Linux-path change, not derived after.
12. **Medium — CSV BOM convention differs per export and is comment-evidenced intentional, not a bug** (W010/W015/W019):
    Suppression and Log exports omit the BOM; Error export includes it. A well-intentioned "normalize all CSV
    encoding" cleanup during Phase 2 would silently break two of the three endpoints' comment-stated contract
    (`"...as before"`) — this matrix must be preserved per-endpoint, verbatim.
13. **Medium — silent no-op-on-missing-id is a recurring, uncommented pattern across two controllers** (W013/W020,
    Q3.7): `RemoveSuppression` and `ResolveError` both return HTTP 200/empty body whether or not the given `id`
    matched a record, with no comment establishing this as intended API contract. Combined with W011's
    silent-skip-but-`Ok:true` dedupe response, this suggests a systemic "never surface a miss to the caller" pattern
    across the Suppression/Error write paths that should be confirmed with the business owner before Phase 2 either
    preserves or "improves" it.
14. **Low–Medium — PartD area-field naming mismatch, not confirmed as bug or relabeling** (BL-G052): CLD-sourced
    `...AreaIneligible` destination properties are assigned from CLD source fields literally named `...AreaEligible`,
    while the equivalent XBData path (`PartDXB`) has no such mismatch. Flagged as an open question for business
    confirmation, not asserted either way.
15. **Low–Medium — unverified "byte-identical" claim on an already-made modernization substitution** (BL-G041,
    `ReportHtmlWriter`). The code comment's claim has not been independently tested; flagged in Modified Logic above
    for the parent workstream to decide whether it needs retroactive verification.
16. **Low — two independently-named but functionally-overlapping mutex GUID fields** (BL-G027): the `XBImportService`
    constant is literally named `apMutexId` (copy-paste residue from `APImportService`) but holds a different GUID and
    guards `XB` import. This is a latent correctness risk for anyone extending the import pipeline who trusts the field
    name over its value — worth a naming fix flagged for the Phase 2 backlog, **not** performed by this mapping pass.
17. **Low — a repeated, functionally-harmless self-assignment typo recurs across 4 per-nation payment subclasses**:
    `YCPStatus = YCPStatus = ycpService.GetRate(...)` appears identically in `NIYoungFarmerPayment.cs`,
    `ScotlandYoungFarmerPayment.cs`, `WalesYoungFarmerPayment.cs`, and `WalesRedistributivePayment.cs`. Functionally a
    no-op (assigns the same value twice), but a clear copy-paste artifact — noted once here rather than as a separate
    catalog row, since it has no behavioral effect to preserve or migrate.
18. **Low — stale/duplicate test and source trees add discovery noise, not behavior risk**: the `Imports\**` folder (24
    files) and the 5 `Services\` legacy duplicate source files + their matching excluded test files are confirmed dead
    code, but their continued presence on disk means every future business-logic search in this repository must
    re-confirm the exclusion before trusting a match (already done once in this pass; worth automating as a repo
    lint/guard if Phase 2 tooling allows). **Also noted this revision**: `Utilities/SITIComparer.cs` (BL-G047) is
    actively compiled but has no confirmed call site anywhere in the repository — not dead by exclusion, but
    currently inert; worth confirming whether it is reserved for a not-yet-active SITI reconcile-by-diff path before
    assuming it is safe to leave unmigrated.

---


## Acceptance Criteria (representative sample — not exhaustive for all 54 BL-G/3 DB-G/25 W items)

These are the kinds of checks that would need to pass **before** any corresponding item above could be marked
Migrated/Verified. None of these have been executed by this pass.

- **BL-G001/BL-G002 (currency conversion/tolerance):** For a representative matrix of `(value, rate)` and
  `(claimValue, transactionValue, tolerance)` pairs drawn from the existing 5+1 active tests plus at least one
  boundary case per branch (exactly at tolerance, negative values), the migrated code must produce bit-identical
  `decimal` results to the current `Math.Round(value*rate,2)` / `Math.Abs(...)<=tolerance` logic.
- **BL-G006/BL-G007 (penalty-precedence cascades):** For every permutation where more than one penalty-reduction field
  is simultaneously non-zero, the migrated `LastPenalty`/`LastPenalty2` value must match the exact fixed-order
  last-write-wins outcome of the current code — a reordered `if` chain would silently change which penalty name is
  reported even if all underlying totals are correct.
- **BL-G013/BL-G038 (FDMR Net4 + export):** Once a client-driven export replacement is designed (post-C08 decision),
  the replacement must reproduce the same `ROUND(...,2)` and zero-clamping semantics for BPS/GR, and the same
  unmodified `YF_Net4` semantics, against a reference dataset — before this item can move past Blocked.
- **BL-G028–BL-G032 (document pipeline/filenames):** A golden-document comparison (binary PDF diff with tolerance for
  non-deterministic metadata, or page-rendered-image diff) against current Windows-hosted output is required per the
  already-recorded Migration-Plan gate; filename patterns (BL-G029/BL-G030) must be asserted exactly, including the
  non-zero-padded timestamp format in BL-G032.
- **BL-G023/BL-G027 (mutex-based exclusivity):** Whatever concurrency mechanism replaces the named `Mutex` (per the
  pending C13 decision) must be proven to serialize generation/import work **across all replicas**, not just within
  one process — a single-process-only test would not be sufficient evidence.
- **BL-G048/BL-G049 (`LateChangePenaltyTotal` defects, new this revision):** Before any Phase 2 code change touches
  `GreeningPayment`/`YoungFarmerPayment` or any of their 6 per-nation override subclasses, the business owner must
  confirm whether the current self-referential-no-op (Greening) and omitted-assignment (YoungFarmer) behaviors are
  **intentional** (in which case they must be preserved byte-for-byte) or **defects** (in which case a fix is a
  business-approved behavior change, not a silent migration side-effect, and needs a Decisions-Required entry before
  Phase 2 proceeds on this calculation family).
- **BL-G045/BL-G046 (SITI parser + `Claim.IsValid`):** A fixture-based test corpus covering every one of the 9
  record-type tags in correct order, at least one out-of-order/missing-predecessor case (to confirm the silent-skip
  behavior), and a `PTBYF`-omitted case (to confirm or refute the suspected YF-null downstream risk) would need to
  exist and pass before this parser could be marked Migrated/Verified — none of this exists today.
- **BL-G050/BL-G051 (per-nation field-reference bugs):** Same confirm-before-fix framing as BL-G048/BL-G049 — the
  business owner must state whether `ScotlandGreeningPayment`'s Region3Total and `NIYoungFarmerPayment`'s YCP-rate
  field reference are intended cross-nation reuse or copy-paste defects before either is touched in Phase 2.

---

## Coverage Gaps and Limitations (explicit, not implied)

This mapping does **not** constitute proof that all business logic has been found or that any item is preservation-ready.
**Several gaps flagged in the prior revision of this file are closed as of this revision** (see "Closed this revision"
below); the remaining open gaps follow.

### Closed this revision

- **`dbo.claim_statement_data` table DDL (DB-G002)** — now read in full (1,146 lines; 402 columns, 1 clustered PK, ~730
  default-constraint statements structurally confirmed). See § Database Business Logic.
- **`dbo.claim_statement_data_gen` stored procedure (DB-G003)** — now read structurally in full (2,001 lines; 6
  scheme-year rate blocks, base-data insert, 59 labeled `UPDATE` sections, final `xp_cmdshell`/`bcp` export). See §
  Database Business Logic. **Caveat**: not every one of the 59 `UPDATE` blocks was individually line-cited — confirmed
  structurally uniform via their distinct RAISERROR labels rather than transcribed block-by-block.
- **`XBDataComparer`, `APComparer`, `SITIComparer`** — all 3 read in full; equality-key fields captured exactly
  (BL-G034, BL-G035); location citation corrected from the previously-wrong `Extensions/` to the actual `Utilities/`.
  `SITIComparer` is additionally noted as having no confirmed call site (BL-G047).
- **16 named per-nation/cross-border payment model files** (`PartB`, `PartD`, `PartAXB`, `PartBXB`, `PartDXB`,
  `Invoice`, `Rate`, and the 10 per-nation BPS/Greening/YoungFarmer/Redistributive classes) — all 17 files (16 named
  + the already-base-class-verified comparison point) read in full; findings captured as BL-G048–BL-G054.
- **`Components/PumpingStation/Maps/`, `Serializer/`, `BulkInsert/` subfolders** — all 12 `*SQLMap` files,
  `APFileMap.cs`, both `BulkInsert/*.cs` files, and both `Serializer/*.cs` files read in full; captured as BL-G045
  (SITI parser) and BL-G047 (confirmed no-hidden-logic for the rest).

### Still open

1. **DB-G003's 59 `UPDATE` blocks were not each individually line-cited.** They were confirmed structurally uniform
   (same `COALESCE`-additive join pattern per nation/payment-type) via a full extraction of their distinct RAISERROR
   labels, but a line-by-line transcription of every join condition and column in every block was not performed —
   if a specific nation/payment-type combination's exact DB-side formula is needed later, that specific block should
   be opened individually rather than assumed from this summary.
2. **DB-G002/DB-G003 were not cross-checked against a live schema or live data.** No live Azure/SQL Server resource
   was queried (per instruction) — both remain static-file reads only; actual deployed schema/procedure state may
   differ from what is in these `.sql` files in the repository.
3. **BL-G050/BL-G051's field-reference-bug pattern was not exhaustively re-verified across every field in every
   per-nation subclass.** Only the 2 specific instances found during this pass's read-through are confirmed; the risk
   synthesis (item 9) explicitly flags that this authoring pattern (copy sibling-nation file, change most but not all
   references) may recur elsewhere in the 10 per-nation classes without having been individually re-grepped field by
   field for every one of their ~15–30 XBData field references each.
4. **No test was run.** The build/test blocker already recorded in `Migration-Plan.md`/`Application-Assessment-Report.md`
   (`System.Management.Automation.dll` 10.0.10586 feed HTTP 400/403) was **not retried or bypassed** by this pass, per
   explicit instruction. All "Tested?" column entries above describe what exists in source, not a passing/failing run.
5. **Namespace-stale excluded tests are not usable as evidence.** The 111 `[Test]` methods in the 5 excluded test files
   reference a `RPA.ClaimStatements.Data.*` namespace and a monolithic `ImportService`/`FakeUnitOfWork` shape that is
   **not present anywhere in the current repository** — they describe a prior, differently-structured codebase, not
   the current `Components/*` split. They are listed for completeness (§ Source Exclusion Reconciliation) but not
   cited as coverage for any BL-G item above.
6. **No code-change, no "Migrated" or "Verified" status anywhere in this file.** Every BL-G/DB-G item is, at best,
   Inventoried. 8 items are flagged **🚫 Blocked** by already-open Migration-Plan decisions (C13, C03, C08 — see §
   Executive Summary for the exact count/list); none are flagged Pending as of this revision (DB-G002/DB-G003 moved
   from Pending to structurally-read-but-not-live-verified, a different and more specific caveat, not a discovery gap).
7. **W001–W025 (Web business inventory) are incorporated by synthesis, not independently re-derived by this
   Generator-scope pass.** This report did not scan `RPA.ClaimStatements.Web`/`RPA.ClaimStatements.Web.Tests` or
   `Security/` source itself; the Web rows, BOM matrix, roles-model summary, and Q3 register above are carried over
   verbatim from the independent Web-perspective agent's completed, source-verified pass. None of the **11 open Q3
   subquestions** (Q3.1–Q3.11) have been resolved by either agent — they remain open decisions for the repo
   owner/business/security-database-owning team, same status as the C01–C20 Migration-Plan decisions this report
   also does not resolve.
8. **No Entra group→role mapping has been selected** (Q3.1/Q3.9) and **EF6 is retained by default** pending an
   explicit approval — neither is asserted as resolved anywhere in this report, including in the synthesized Web
   section's "Target" column, which states the retain/no-mapping default on every row rather than prescribing an
   outcome.
9. **The 10 catalog rows added this revision (BL-G045–BL-G054) and the gap-closure updates to BL-G034/BL-G035/BL-G038
   do not have a test-execution claim of any kind** — this mapping identifies that these behaviors/defects exist in
   source; it does not and cannot prove what any of them currently produce at runtime without a passing build/test
   run, which remains blocked (gap #4 above).
10. **This mapping identifies business logic found by source reading; it does not itself constitute the "preservation
    proof" the migration ultimately needs.** Per § Acceptance Criteria, golden-fixture/equivalence tests against a
    passing build are still required for every item before any status here could move past Inventoried.

---

**Next smallest safe source-adjacent task (not a recommendation to override any prerequisite):** with the previously
open comparer/Maps/Serializer/BulkInsert/16-payment-model/CrossBorder-SQL gaps now closed, the smallest remaining
mapping-only unit of work (not code-changing) is a line-by-line pass over DB-G003's 59 individual `UPDATE` blocks
(open gap #1 above) if DB-side formula-level fidelity is needed before any FDMR/CrossBorder-adjacent Phase 2 code is
touched — this does not require any of the currently-blocked external contracts (C01/C03/C08/C13) or the package-feed
remedy, and produces no source change.

