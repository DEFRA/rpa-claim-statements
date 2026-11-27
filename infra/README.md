# Claim Statements infrastructure handoff

Repository-only Bicep for the current .NET 10 application. **Not a deployment,
production approval or evidence of Azure/PDF parity.** No customer/production
Azure resources have been deployed or modified by this template set.

The design follows the CPH Non Subsidy foundation/application split and pinned
AVM conventions, adapted to the actual Claim Statements code. The agreed model
is a workload-specific foundation on an existing customer-owned network,
private/internal ACA, approved existing Azure Files shares and a manual-only
Generator job. No application source, configuration or pipelines are changed.

> A separate, Contoso-sandbox-only proof-of-concept stack (an isolated test
> network, foundation and a dedicated Files account) was used to validate this
> design against a live, non-production subscription. That POC-only material,
> its parameters and its deployment/review notes are intentionally **not**
> included in this handover; it never used customer values, networks or data.

## Requirements assessment and evidence

Current code and the current developer/operator guides take precedence over
the historical October assessment and framework migration plan.

| Area | Source-confirmed requirement | Infrastructure response |
|---|---|---|
| Components | [Web startup](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Program.cs) hosts Razor Pages and the Generator in-process. [Console startup](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Program.cs) runs `BPS N`, `XB N`, `ALL N` or `I`. | One Web app and one separate manual job; two image digests from the Dockerfile's `production` and `generator` targets. |
| Runtime | [Dockerfile](../Dockerfile): .NET 10, Alpine/musl, UID/GID 1000, Python/WeasyPrint 70.0, native fonts/Pango/ICU, Web HTTP 8080. | ACA Consumption, parameterized supported CPU/memory pairs. No VM, IIS, App Service Plan or separate renderer service. Size/load-test both images; rendering uses up to 16 concurrent operations. |
| Health | [Request pipeline](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Pages/OperatorPagesExtensions.cs) exposes anonymous `/health/live` only. | Startup/liveness probes on 8080 with an explicit Host header. No invented `/health/ready`; platform readiness is not a dependency health test. |
| Database | [EF context](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Context/ClaimStatementsContext.cs), [bulk copy](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Components/PumpingStation/BulkInsert/SQLBulkInsert.cs): one shared SQL Server database, CRUD and bulk ingestion; configuration and folder paths live in SQL. | Private Entra-only Azure SQL Database is the proposed managed target, subject to DBA compatibility approval. No schema, seed, user, migration or data import is executed by Bicep. |
| Files | [Folder service](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Services/FolderService.cs), [transform service](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Components/Turbine/TransformService.cs): ordinary filesystem paths, DOCX templates and persistent import/archive/outbound folders. | Four existing SMB shares registered with ACA: read-only templates, read/write feed data, PDF archive and PDF outbound. Working XML/DOCX/HTML/PDF/Uploads remain image-local. |
| Identity | [Entra configuration](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Security/EntraAuthenticationExtensions.cs) and [role map](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Security/AppRolePermissionMap.cs): single tenant, confidential code/PKCE flow, seven fixed app roles. | Existing customer Entra registration; UAMI for Azure access. No Graph writes, Easy Auth, Windows authentication or second Security database. |
| Shared cookies | [Data Protection](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Security/DataProtectionConfiguration.cs): Blob ring, versionless Key Vault key, stable environment discriminator, explicit UAMI. | Private versioned Blob container, key-scoped wrap/unwrap access; operator creates/maintains the wrapping key before activation. |
| Telemetry | [Web telemetry](../RPA.ClaimStatements/RPA.ClaimStatements.Web/Extensions/TelemetryExtensions.cs), [Generator telemetry](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Services/TelemetrySettings.cs): Application Insights connection string plus optional UAMI. | Workspace-based Application Insights with local auth disabled; UAMI ingestion and CPH-style private-only AMPLS, endpoint and DNS ownership choices. Both monitoring resources are associated; ACA environment/app diagnostics feed the workspace. |
| Integrations | [SITI importer](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Components/PumpingStation/SITIImportService.cs) reads FTP credentials from SQL; [FTP client](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Components/PumpingStation/FTPService.cs) uses **plain passive FTP**, downloads then deletes remote files. [SMTP settings](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Services/SmtpSettings.cs) require TLS when credentials are used. | Owner-provided private routing/egress to approved FTP/passive-data ports and SMTP relay. No FTP/SMTP server, NAT, firewall rule or protocol conversion is invented. SMTP password uses a Vault reference. |
| Concurrency | [PowerPlant](../RPA.ClaimStatements/RPA.ClaimStatements.Generator/Components/PowerPlant.cs) has host-local, statement-type mutexes. Web can trigger work too. | Exactly one Web replica, Single revision mode, manual job, one replica per execution and no automatic retry. **These do not enforce global mutual exclusion.** |
| Other artifacts | [PDF comparator](../PdfCompare/package.json) is a local utility; [DAX transfer](../DAXAPFileTransfer.ps1) is Windows file automation. CrossBorder/FDMR contain standalone SQL, not additional solution hosts. | No Node hosting, Windows scheduler, message bus or cache resources. Their external operational ownership remains explicit. |

### Recommendations, not source-derived customer requirements

- Private hosting was explicitly selected for this handoff. `ingressExternal:
  true` exposes Web to clients of the **internal environment**, not the internet.
  The application guide's external-ingress wording does not mandate public hosting.
- New resource names derive from approved `prefix`, `environmentName` and a
  resource-group-scoped unique suffix, with `csg` separating this workload from
  CPH. No actual subscription, tenant, region, network or resource name is supplied.
- Required baseline tags are `application`, `environment`, `owner`, `costCentre`;
  additional string tags are supported. These are proposed CAF-aligned keys,
  **not a discovered customer tagging policy**. Confirm policy before deployment.
- Azure SQL GP serverless, always-on compute (`autoPauseDelay=-1`), Premium ACR,
  explicit storage redundancy, retention and zone choices follow the reference.
  They are not sizing, cost, residency or recovery commitments.
- Existing service DNS zones remain platform prerequisites. Monitor follows the
  reference's explicit Create/Existing contracts, not an assumption that shared
  AMPLS/endpoint/Monitor DNS already exist or that new ones are safe to create.
  Obtain approved inventory and DNS-domain ownership before selecting modes.

## Files and ownership

| File | Purpose |
|---|---|
| [main.bicep](./main.bicep) | Foundation only: UAMI, private ACR, Vault, key-ring storage/container, SQL server/database, workspace, Insights, internal ACA environment, four service endpoints, generated environment DNS and explicitly owned Monitor connectivity. No app/job starts. |
| [storage.bicep](./storage.bicep) | Four ACA environment storage registrations referencing existing shares and Vault-held account keys. Does not create/change storage accounts or shares. |
| [app.bicep](./app.bicep) | Web, manual Generator job, shared runtime settings, mounts and key-scoped crypto grant after bootstrap. |
| [modules/contracts.bicep](./modules/contracts.bicep) | Shared typed tags, mount contracts and permitted CPU/memory pairs. |
| [modules/private-endpoint.bicep](./modules/private-endpoint.bicep) | New service private endpoint and zone group against the supplied existing DNS zone. |
| [modules/internal-environment-dns.bicep](./modules/internal-environment-dns.bicep) | New environment-generated domain, apex/wildcard A records and VNet link. |
| [modules/monitor-private-link-scope.bicep](./modules/monitor-private-link-scope.bicep) | CPH AMPLS creation with PrivateOnly ingestion/query and workspace/Insights associations; Existing mode never PUTs the parent. Replaces the former associations-only module. |
| [modules/monitor-private-endpoint.bicep](./modules/monitor-private-endpoint.bicep) | CPH `azuremonitor` endpoint and five-zone DNS group, only when Create is selected. |
| [modules/private-dns-zone.bicep](./modules/private-dns-zone.bicep) | CPH Create/Existing zone and Create/Existing/External VNet-link contracts. |
| [main.parameters.example.json](./main.parameters.example.json), [storage.parameters.example.json](./storage.parameters.example.json), [app.parameters.example.json](./app.parameters.example.json) | Intentionally incomplete contracts. Every `null` is unknown, **not a valid deployment value**. |
| [bicepconfig.json](./bicepconfig.json), [Test-Templates.ps1](./Test-Templates.ps1) | Local compilation/lint, compiled-shape checks, schema/semantic checks and negative regressions. |

The three stages target the **same existing resource group**. Platform resources may
be in other authorized resource groups/subscriptions in the same deployment
tenant. Same prefix/environment/scope must be retained across stages. Changing
these selects another stack; it does not migrate state.

The existing VNet, subnets, service DNS zones, Azure Files accounts/shares, Entra
registration, build agents, notification routes and external integrations are
**not redeployed**. Existing Monitor scopes/endpoints/zones/links are not PUT.
Only explicitly selected new Monitor resources, service DNS links and Project associations
are managed. New endpoint zone groups do write records in supplied shared DNS
zones: explicit owner permission is required. New AMPLS association names must
be approved and unused; a name collision could otherwise retarget another workload.

## Parameters

Copy all three examples outside Git. Supply approved values rather than using
test fixtures. The checked-in examples contain no actual IDs,
credentials, names, image digests, sizing or region defaults.

- **Common:** `prefix` (2-6 lowercase alphanumeric), `environmentName` (2-3),
  `location` where required, tags. `tags.environment` must match environment.
- **Network:** distinct existing ACA/endpoint subnet IDs in the same approved
  VNet. ACA subnet must already be dedicated and delegated to
  `Microsoft.App/environments`, /27 or larger, with capacity for infrastructure,
  revisions and scale. Confirm no ACA reserved-range conflicts. Endpoint subnet
  must be undelegated with capacity for four service endpoints and, when created,
  the multi-IP Monitor endpoint plus headroom. Do not budget one IP per endpoint.
  Configure NSG/UDR/private endpoint policies through the platform owner.
- **DNS:** exact existing SQL, Vault, Blob and registry zone IDs.
  `serviceDnsLinks.sql/vault/registry` now explicitly select the CPH Link
  contract: Create or Existing with approved `name`, or External with proven
  forwarding. Create writes only the selected new VNet link in the existing
  zone's exact RG/subscription, with auto-registration disabled. Existing and
  External never PUT a link. Blob link ownership remains solely in
  `monitoringPrivateDns.blobLink`, avoiding duplicate declarations. Confirm ACR
  data endpoint records and resolution from the workload and operator networks.
- **Monitor:** `monitoring` retains only retention values. `monitoringAmpls`,
  `monitoringPrivateEndpoint` and `monitoringPrivateDns` select the reference's
  ownership modes below. No ownership mode has a template default.
- **Database:** approved name (letters/digits/dot/underscore/hyphen), Entra admin
  name/object ID and User/Group type, min/max vCores, size, PITR/LTR and backup
  redundancy. Admin is not the runtime UAMI. Zero LTR durations require an
  intentional owner decision. Geo redundancy needs residency approval.
- **Storage/security:** key-ring redundancy, Blob/container soft-delete days,
  Vault soft-delete days, explicit zone choice. Purge protection is always on.
  Wrapping-key size/rotation/expiry/retention are owner-bootstrap decisions,
  not hidden template defaults.
- **Images:** registry repository paths without hostname/tag and actual pushed
  `sha256:<64 lowercase hex>` registry digests. Build/push each image target
  separately; do not substitute a local image ID or assume identical digests.
- **Entra:** tenant/client GUIDs and the name of the owner-populated client-secret
  secret in the workload Vault. Fixed role values remain in the application.
- **Mounts:** `fileShares` must match between storage/activation files. Each
  purpose needs a distinct share and registration name, account name, share name,
  Vault account-key secret name and nonoverlapping canonical Linux mount path.
  Templates must use `/var/lib/claimstatements/work/Templates`. Other mounts
  must not hide working/home/scratch folders. The DBA maps actual `CS.Folders`
  rows to the agreed mount paths; Bicep cannot prove that mapping.
- **Runtime:** choose both container sizes; job statement type, positive batch
  size, timeout and Web shutdown allowance. `I` emits **one** argument; other
  modes emit type plus batch size. There is deliberately no cron parameter.
- **Approvals:** `trustedIngressConfirmed` and `nonOverlapControlsConfirmed`
  must explicitly be `true`. These are operator attestations, not technical
  proof of safe networking or distributed locking.
- **SMTP:** blank host/username/password-secret name means no configured relay;
  provide explicit port and TLS choice. Email failures are logged and do not
  fail the run. Username/password secret must be supplied together with TLS.
  Report sender/recipient comes from the database, not these parameters.

## Azure Monitor private connectivity (CPH alignment)

The source pattern is the reference repository's **root** [main.bicep](../../../Handover/rpa-cph-non-subsidy/infra/main.bicep),
not its alternative `aca` or `appservice` stack. The reference files reused are
`modules/monitor-private-link-scope.bicep`,
`modules/monitor-private-endpoint.bicep` and `modules/private-dns-zone.bicep`.
The DNS module additionally exports its existing `Link` type. Claim Statements
retains its own naming and existing VNet/subnet/service-DNS inputs.
Modules are included locally; the reference checkout is not a deployment dependency.

| Input | Ownership and behaviour |
|---|---|
| `monitoringAmpls.mode: Create` | Creates `${prefix}-${environmentName}-csg-ampls`, global, with ingestion and query `PrivateOnly`. `associationOwner` must be `Project`; two deterministic workspace/Insights association names are generated. |
| `monitoringAmpls.mode: Existing` | Supply the approved full `resourceId`. Parent access modes/tags are never changed. `associationOwner: Project` creates two explicitly named, unused associations. `Platform` performs no association writes: the platform must link both exact workload resources before activation. |
| `monitoringPrivateEndpoint.mode: Create` | Creates `${prefix}-${environmentName}-csg-monitor-pe` in `privateEndpointSubnetResourceId`, targeting the selected AMPLS with group ID `azuremonitor`. Depends on Project associations (when managed here) and DNS configuration. Creates the `default` five-zone DNS group. |
| `monitoringPrivateEndpoint.mode: Existing` | Supply the approved full endpoint `resourceId`; no endpoint or zone-group writes occur. Requires Existing AMPLS and Existing Monitor zones. Verify the actual endpoint targets that scope, is Approved and reachable, and contains exactly the selected five zone IDs. |
| `monitoringPrivateDns.monitor/oms/ods/agentsvc` | Each uses CPH's `{ zone, link, zoneGroupOwner }` contract. `zone` is Create or Existing with `resourceId`. `link` is Create or Existing with explicit `name`, or External for proven platform forwarding. `zoneGroupOwner` must be `Project`, granting record-management permission, not read-only zone reuse. |
| `monitoringPrivateDns.blobLink` | The same Link contract for the existing key-ring Blob DNS zone. No second Blob zone is declared. Select Existing with the actual link name when already linked; Create only for an absent approved link; External requires proven resolution to the workload VNet. |

The Monitor endpoint's five zones are:

1. `privatelink.monitor.azure.com`
2. `privatelink.oms.opinsights.azure.com`
3. `privatelink.ods.opinsights.azure.com`
4. `privatelink.agentsvc.azure-automation.net`
5. **The existing `privateDnsZoneResourceIds.blob`** (`privatelink.blob.core.windows.net`).

New zones are created in the workload RG. Existing-zone links are created, if
requested, in that zone's exact subscription/RG. Links target the VNet derived
from `acaSubnetResourceId`, with registration disabled. Existing links and
External forwarding are not modified or proven by Bicep. Both monitoring services
retain disabled public ingestion/query; no Open mode or public fallback is added.

### Parameters and ownership preflight

The checked-in example illustrates Create AMPLS/endpoint/four Monitor zones;
this is **not a customer ownership decision or a deployable default**. Link names
and all customer inputs remain null placeholders. Choose Existing instead wherever
the approved DNS domain already uses shared Monitor connectivity.

For example, replace the relevant parameter `value` objects with approved
Existing ownership (angle-bracket strings below are placeholders, not valid IDs):

```json
{
  "monitoringAmpls": {
    "value": {
      "mode": "Existing",
      "resourceId": "<approved-ampls-resource-id>",
      "associationOwner": "Project",
      "workspaceAssociationName": "<approved-unused-workspace-association-name>",
      "applicationInsightsAssociationName": "<approved-unused-insights-association-name>"
    }
  },
  "monitoringPrivateEndpoint": {
    "value": { "mode": "Existing", "resourceId": "<approved-monitor-endpoint-resource-id>" }
  }
}
```

For each Monitor DNS entry, Existing uses
`{ "zone": { "mode": "Existing", "resourceId": "<approved-zone-resource-id>" }, "link": { "mode": "Existing", "name": "<actual-vnet-link-name>" }, "zoneGroupOwner": "Project" }`.
For Platform associations, set `associationOwner: Platform` and **omit** both
association-name properties. The `monitoringConnectivity` output supplies AMPLS,
endpoint, five DNS zone IDs and both workload monitoring IDs for platform handoff.
Remove the old `monitoring.amplsResourceId`, `workspaceAssociationName` and
`applicationInsightsAssociationName` fields; they are now rejected there.

Before any authorized deployment, obtain owner-approved inventory covering the
whole VNet/shared DNS view, not only the workload RG:

- Confirm **one applicable AMPLS per shared DNS view**, including all other
  monitoring consumers affected by routing/DNS changes. Create requires approved
  isolation and proof there is no competing scope/zone. Existing scopes must be
  `PrivateOnly` for ingestion/query with no access-mode exclusions. Do not change
  another workload's scope, association, endpoint, DNS records or VNet link.
- Verify new names are unused and all Existing IDs, link names, association
  targets, zone groups and DNS-forwarding paths match actual inventory. Existing
  endpoint reuse also requires Approved connection and workload VNet reachability.
  New zone groups may alter shared records even when reusing an existing zone.
- Grant narrowly scoped deployment/association, endpoint/subnet join, DNS record
  and selected new-link permissions across the exact RGs/subscriptions. Obtain
  platform approval for every shared-resource write and endpoint connection.
- If Platform owns associations, arrange both links to the new workspace/Insights
  before activating the application. Do not treat successful foundation deployment
  or a supplied existing endpoint ID as proof those links exist.
- Verify endpoint IP capacity, NSG/UDR/port 443, private DNS resolution from Web,
  Generator and operator networks, and actual telemetry ingestion/query. Keep
  service-required egress; AMPLS does not privatize Entra sign-in, ARM or all
  platform diagnostic delivery.

These are mandatory receiving-team checks. Unlike CPH's inventory-aware
preflight script, this repository's local validator checks parameter contracts
and compiled resources only; it does not attest live inventory or approvals.
No customer inventory or private connectivity was queried or verified here.

## Identity, secrets and persistent data

The retained UAMI receives registry-scoped **AcrPull**, workload-Vault-scoped
**Key Vault Secrets User**, key-ring-container-scoped **Storage Blob Data
Contributor** and Insights-scoped **Monitoring Metrics Publisher**. Activation
adds **Key Vault Crypto Service Encryption User** on the exact wrapping key
(get metadata/wrap/unwrap rather than general key administration).

The ACA environment also has this UAMI for resolving file-share key secrets.
`accountKeyVaultProperties` is supported by the pinned storage API: no account
key, SQL password, client credential or SMTP password is a Bicep parameter,
output, inline value or `listKeys()` call.

**SMB limitation:** the share mount still authenticates with an account key;
the UAMI authenticates only to Key Vault. This is not passwordless SMB or
share-scoped managed-identity authorization. Keys grant broad account access,
including on any approved cross-tenant account. Prefer dedicated approved
accounts, restrict Vault readership, and rotate/refresh mount registrations
through the owner's tested procedure. Read-only templates are enforced by the
environment registration, not by the storage account key.

Existing shares must be privately reachable on TCP 445 with secure transfer
and approved storage firewall/DNS settings. Mount options set UID/GID 1000 and
restrict directory/file modes; verify actual permissions, locking, rename/move
and read-only behavior in ACA. No share content, backup policy, external-tenant
permission or network connectivity is created by a storage registration.

FTP is an existing exception: credentials still come directly from
`CS.Folders`, and the code uses unencrypted FTP. Storing a copy in Vault would
not make the application consume it. Owner risk acceptance/private transport
controls or a separately scoped application change are required.

## Local validation

PowerShell 7 and an installed Bicep CLI are required. AVM modules restore from
Microsoft Container Registry, or use the existing local Bicep cache. No Azure
login, discovery, deployment, What-If or data-plane access is performed.

```powershell
.\infra\Test-Templates.ps1 -BicepPath "$HOME\.azure\bin\bicep.exe"
.\infra\Test-Templates.ps1 -BicepPath "$HOME\.azure\bin\bicep.exe" `
  -FoundationParameters 'C:\private\claim-statements\main.parameters.json' `
  -StorageParameters 'C:\private\claim-statements\storage.parameters.json' `
  -ApplicationParameters 'C:\private\claim-statements\app.parameters.json'
```

The script builds/lints **every** local Bicep file with zero diagnostics,
checks example fields/types against the emitted ARM type definitions while
allowing example-only nulls, validates complete synthetic fixtures, exercises
38 exact-error negative cases and checks compiled deployment arguments and
native resource shapes. Supplied files cannot contain nulls, unknown parameters
or misspelled properties; matching scope identifiers, tags, DB and mounts are
cross-checked when related files are supplied. Temporary ARM JSON is cleaned up.

Checks include DNS/service-ID shapes, distinct subnets, SQL min/max, valid
GUIDs, immutable digests, template mount/path isolation, distinct share targets,
SMTP credential/TLS combinations, actual identity/health settings, singleton
Web, manual/no-retry job, private resource arguments and pinned AVM versions. Monitor tests cover Create,
Existing Project/Platform ownership, existing endpoints, Existing/External links,
PrivateOnly modes, both associations, five-zone groups, shared Blob DNS, dependency
ordering and rejection of incompatible ownership/zone combinations.
Checks cover all four foundation service endpoint groups/zones, service DNS link
ownership, ACR trusted-service/IP bypass disabled, no SQL firewall exceptions,
and the dedicated Files account's private access, encrypted SMB and File endpoint.
It does **not** query subnet delegation/capacity, ownership/collisions, region/
quota/policy support, prices, existing AMPLS access modes, DNS reachability,
actual image content, secret existence, SQL compatibility or deployed behavior.
Passing local checks is not production certification. Parameter files for all
three stages should be checked together immediately before any approved release.

Pinned AVM versions match the relevant reference modules:
identity **0.6.0**, workspace **0.16.1**, Insights **0.8.0**, ACR **0.13.1**,
Vault **0.14.2**, Storage **0.33.1**, SQL **0.22.1**, ACA environment **0.16.0**,
ACA app **0.23.0**. Endpoint/DNS/association helpers, file registrations and job
use native resource declarations with explicit API versions.

## Receiving-team deployment and acceptance (not executed here)

1. Approve inventory/ownership first: exact subscription/RG, naming/tag policy,
   resource-name collisions, connected VNet ranges, delegation/capacity, routes,
   NSGs, shared DNS record permissions and AMPLS association/endpoint readiness.
   Confirm region, zones, Consumption limits, SQL SKU, budget, retention and
   RPO/RTO. Do not deploy into a discovered matching name without ownership review.
2. Validate complete parameter files locally. In a separately authorized
   deployment session, review What-If and deploy **foundation only**. Operators
   need deployment permissions plus narrowly scoped role-assignment rights and
   the selected DNS/AMPLS/endpoint creation or association/link write permissions.
   Complete the Monitor ownership preflight above first. No deployment has been
   executed from this handover.
3. Use a private-network-connected operator/runner. Verify private DNS/routing
   to SQL, Vault, Blob, ACR/data endpoints, Monitor and approved shares. Do not
   temporarily enable public access or trusted-service bypass to fix bootstrap.
4. Build/test/scan and push both images with a separate build identity. Runtime
   AcrPull does not permit pushing. Record immutable registry digests.
5. Configure the external single-tenant Entra registration with the callback
   URLs from the foundation origin plus `/signin-oidc` and
   `/signout-callback-oidc`. Define/assign the seven roles in the
   [operator guide](../docs/operator-guide.md#entra-setup-app-roles). Populate the
   client credential, optional SMTP password and approved SMB account keys in
   the workload Vault without logging or committing their values.
6. With separate key-management rights, create/verify the private Vault's
   **RSA `data-protection` key** and approved rotation policy. Foundation
   deliberately sends `keys: []`: ARM creation is not automatically routed
   through a private endpoint. Preserve every wrapping-key version needed by
   the persisted XML ring; rotation alone does not rewrap old XML. Verify
   expiry/retention and rehearse Blob/key recovery. Do not create an empty
   key-ring blob; the application initializes it.
7. DBA approves Azure SQL compatibility, migrates/restores data, verifies
   schema and applies the EF bundle (or the one-time
   [existing-DB adoption script](../RPA.ClaimStatements/Database/adopt-existing-database.sql)
   only after comparison). Provision runtime contained user and object/schema-
   scoped CRUD/bulk-copy permissions, plus migration-history reads; no DDL,
   `db_owner` or SQL administrator rights for runtime. A separate identity
   applies migrations. Seed approved configuration, transformations, scheme
   years/rates and folder rows through the DBA's controlled process.
8. Supply both approved DOCX templates. Map DAX AP/AR, SITI and XB inbound/archive
   paths under the `data` mount, and PDF Archive/Outbound to their respective
   mounts. Arrange file producers, consumers and the cross-tenant delivery
   agreement. This does not port the Windows DAX transfer script.
9. After secret/RBAC propagation is verified, deploy storage registrations.
   Confirm private Vault key resolution and SMB mounts. A successful ARM
   deployment alone is not proof of data-plane access.
10. Only after ingress-trust review, set both approval parameters and activate
    Web/job. The Web flag enables ASP.NET Core forwarding with broad proxy
    trust; confirm that requests can reach it **only through trusted ACA
    ingress**, that ingress sanitizes forwarded headers and HTTPS callbacks
    cannot be spoofed. Otherwise stop: an explicit trusted-proxy application
    change is outside this handoff.
11. Test HTTPS/Entra role matrix/logout, shared cookies across restart/revision,
    SQL identity/schema, file permissions, real PDFs and private telemetry.
    Confirm startup/liveness is not mistaken for dependency readiness.
12. Establish exclusive operational control over Web triggers/imports and job
    starts; never start overlapping executions or deploy while work is running.
    Single revision mode can still overlap replicas during rollout; job
    `parallelism=1` limits **one execution**, not concurrent manual starts.
    Run a small approved batch, inspect SQL error/log records, PDFs and email.
    Exit 0 does not establish that every statement succeeded.

Web generation/import requests run synchronously. Large batches can outlive
ingress request timeouts even while processing continues. Verify this behavior
under realistic load; use the manual job for long-running batches and keep
Web-triggered work excluded during those executions.

The following are optional **receiving-team-only** commands for a separately
approved future session; replace all placeholders and use the same validated
parameter set/scope. They were **not run**:

```powershell
az deployment group what-if --resource-group '<approved-resource-group>' `
  --template-file '.\infra\main.bicep' `
  --parameters '@C:\private\claim-statements\main.parameters.json'
az deployment group create --resource-group '<approved-resource-group>' `
  --template-file '.\infra\main.bicep' `
  --parameters '@C:\private\claim-statements\main.parameters.json'
# After bootstrap, separately review and deploy storage.bicep, then app.bicep,
# each using its own validated parameter file. Neither starts the manual job.
```

Configure owner-approved alerts/notification routes for revision/probe failures,
job failure/timeout/missed runs, SQL capacity/failures, storage availability,
Vault access/key/secret expiry and telemetry loss. No thresholds, action group,
on-call addresses, alert SLA or audit retention are invented. Console/telemetry
are best effort; application error/log records remain authoritative and may
contain sensitive claim information. Restrict access and approve retention.

For rollback, stop new work and verify no running executions before restoring
the prior Web/job digests/configuration. Coordinate schema rollback separately
with the DBA. Retain the identity, key ring, keys, data and foundation; do not
delete resources to retry activation. Recovery targets require rehearsals.

## Open customer decisions and differences from CPH

- Confirm complete platform inventory/shared-service ownership, private client
  access, DNS forwarding, FTP/passive/SMTP egress and cross-tenant share approval.
- Confirm SQL target suitability: the separate FDMR script uses BCP/
  `xp_cmdshell` and database-qualified SQL; Azure SQL Database cannot host that
  workflow unchanged. CrossBorder SQL targets a separate Payment Filter
  integration database. These are external workflows, not silently migrated
  components. If they must move with this service, stop and redesign their
  execution/SQL target before approving deployment.
- Approve tags, region, resource sizes, independent service availability needs,
  backup/residency/retention, key lifecycle, budget, alert routing and RPO/RTO.
  The single `zoneRedundant` input selects the foundation's zone posture;
  validate support for every selected service/SKU or revise before deployment.
- Approve broad forwarded-header trust or commission the separate application
  change; define enforceable operating exclusion or implement distributed locking.
- Approve FTP credential/transport risk, SMTP relay requirements and a tested
  account-key rotation procedure. No blanket Key Vault compliance claim is made.
- Complete live Azure integration, realistic load/performance, recovery and
  original-to-modern PDF acceptance; none is established by compilation.

Compared with CPH, this handoff has a third mount-registration stage, a second
image/manual job, persistent file contracts, native PDF resource needs and
FTP/SMTP/cross-tenant dependencies. It omits CPH authorization/audit-claim
parameters, `/health/ready`, autoscaling and its App Service/alternative ACA
stacks. It reuses existing platform subnets and service DNS instead of cloning
CPH's VNet. AMPLS, Monitor endpoint and Monitor DNS now use the root CPH
Create/Existing ownership pattern; the existing Blob DNS zone is shared rather
than redeclared. The key-ring, identity, diagnostics, private-access,
bootstrap staging, null-example and pinned-module patterns are retained.

## Platform references

- [Azure Files mount contract](https://learn.microsoft.com/azure/templates/microsoft.app/2025-07-01/managedenvironments/storages)
- [ACA storage mounts](https://learn.microsoft.com/azure/container-apps/storage-mounts)
- [ACA jobs](https://learn.microsoft.com/azure/container-apps/jobs)
- [ACA private networking](https://learn.microsoft.com/azure/container-apps/networking)
- [Azure Monitor Private Link design](https://learn.microsoft.com/azure/azure-monitor/fundamentals/private-link-design)
- [Key Vault built-in roles](https://learn.microsoft.com/azure/role-based-access-control/built-in-roles/security#key-vault-crypto-service-encryption-user)
- [Azure Verified Modules](https://azure.github.io/Azure-Verified-Modules/)
