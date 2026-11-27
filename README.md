# RPA Claim Statements

The Claim Statement Generator (CSG) generates a claim statement for every BPS claim upon every change in settlement
position. It runs on .NET 10, directly on Windows or as non-root Linux containers.

See the project wiki for more information: https://dev.azure.com/defradev/Defra%20DDTS%20Development%20Team/_wiki/wikis/Defra%20DDTS%20Development%20Team%20Wiki/48/RPA.Claim-Statement-Generator

**Azure deployment and PDF output parity with the original application have not been verified.**

For maintenance and handover, see the [developer guide](./docs/developer-guide.md) and the
[operator guide](./docs/operator-guide.md).

The solution is `RPA.ClaimStatements/RPA.ClaimStatements.sln`:

| Project | Purpose |
|---|---|
| `RPA.ClaimStatements.Generator` | Console application. Imports DAX and SITI files into SQL Server and outputs claim statements as PDFs. |
| `RPA.ClaimStatements.Web` | ASP.NET Core Razor Pages application with Entra ID sign-in. Lets users check generation failures, manage suppressions and configuration, and trigger generation. It runs the Generator in-process. |
| `RPA.ClaimStatements.Generator.Tests`, `RPA.ClaimStatements.Web.Tests` | NUnit tests. |
| `PdfCompare` | Standalone Node.js utility that compares PDF content between two builds (see the developer guide). |

## Build and test

Run from the `RPA.ClaimStatements` folder with the .NET 10 SDK:

```powershell
dotnet restore RPA.ClaimStatements.sln
dotnet build RPA.ClaimStatements.sln -c Release
dotnet test RPA.ClaimStatements.sln -c Release
dotnet publish RPA.ClaimStatements.Generator -c Release -o <folder>
dotnet publish RPA.ClaimStatements.Web -c Release -o <folder>
```

The tests use mocks and in-process hosting; they need no SQL Server, Entra or Azure. The PDF rendering tests call the
real renderer, which needs Python 3.10 or later and WeasyPrint 70.0 with its native Pango and fontconfig
dependencies on the PATH (see [WeasyPrint installation](https://doc.courtbouillon.org/weasyprint/v70.0/first_steps.html#installation)).
Without them those tests are skipped; the container image already contains everything. The Word templates
(`ClaimTemplate.docx`, `ClaimTemplateXB.docx`) are not in source control and are not needed to build or test.

## Configuration

Both applications are configured through environment variables (`__` separates sections), plus user secrets for the
Web project in development. There is no `App.config` or per-environment transform. Everything else the application
needs (feature switches, folders, transformations, conversion rates, scheme years) is in the database tables
`CS.Configuration`, `CS.Folders`, `CS.Transformations`, `CS.ConversionRates` and `CS.SchemeYears`.

| Variable | Requirement |
|---|---|
| `ConnectionStrings__ClaimStatementsContext` | Required SQL Server connection string. Use `Authentication=Active Directory Managed Identity;User Id=<client id>` in Azure; supply credentials only as a secret. Missing or invalid values stop both applications at startup, without echoing the value. |
| `CLAIMSTATEMENTS_ROOT` | Required absolute path of the working folder that holds `Templates`, `XML`, `DOCX`, `HTML`, `PDF` and `Uploads`. A missing, blank or relative value stops startup. The images set `/var/lib/claimstatements/work`. |
| `Mail__Enabled` | Sends the tracking report through Microsoft Graph. Defaults to `false` when unset; any other value must be `true` or `false`. |
| `Mail__ManagedIdentityClientId` | Client ID (GUID) of the user-assigned managed identity that sends the mail. Required when mail is enabled; attach it to both the Web app and the Generator job. The sender mailbox and recipient are the `Email Account` value in `CS.Configuration`. See the [operator guide](./docs/operator-guide.md#tracking-mail-platform-prerequisites). |
| `AzureAd__TenantId`, `AzureAd__ClientId` | Web only. Required. The tenant must be this organisation's tenant GUID, not `common`, `organizations` or `consumers`. |
| `AzureAd__ClientCredentials__0__SourceType`, `AzureAd__ClientCredentials__0__ClientSecret` | Web only. The credential for the confidential client, for example `ClientSecret` plus a Key Vault secret reference. |
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | Optional. Telemetry is off unless it is set. `ApplicationInsights__ConnectionString` is the alternative; the former wins. |
| `Telemetry__ManagedIdentityClientId` | Optional client ID (GUID) of the user-assigned managed identity that authenticates ingestion. Needs a connection string; a blank or invalid value fails startup. Required when local authentication is disabled on the Application Insights resource (the identity needs **Monitoring Metrics Publisher**). |
| `DataProtection__Enabled`, `__BlobUri`, `__KeyIdentifier`, `__ApplicationName`, `__ManagedIdentityClientId` | Web only. Shared Data Protection keys: enable in every Azure environment. See below. |
| `CLAIMSTATEMENTS_APPLY_MIGRATIONS` | Developer machines only: `true` applies EF Core migrations at startup. Never set it in a deployed environment. |

### Data Protection keys

Data Protection protects the sign-in cookie and anti-forgery tokens. Without shared keys each replica or revision
has its own key ring and rejects the others' cookies. With no `DataProtection` setting at all, the ASP.NET Core local
defaults apply (local development). If any `DataProtection` setting exists, `Enabled` must be `true` or `false`, and
when `true` all settings are required:

| Variable | Value |
|---|---|
| `DataProtection__BlobUri` | HTTPS URI of the key-ring blob, without SAS token, query, credentials or fragment. |
| `DataProtection__KeyIdentifier` | HTTPS versionless Key Vault **key** URI: `https://<vault>.vault.azure.net/keys/<name>`. |
| `DataProtection__ApplicationName` | Stable name, identical for all replicas and revisions of one environment and different between environments. |
| `DataProtection__ManagedIdentityClientId` | Client ID of the user-assigned managed identity used for Blob and Key Vault access. No developer-credential fallback exists. |

Missing, invalid or unknown `DataProtection` settings fail startup. The settings do not prove the Azure resources
exist; the identity needs Storage Blob Data Contributor on the container and Key Vault Crypto User on the key.

### Telemetry

The Web application uses the Azure Monitor OpenTelemetry distribution with its default collection and sampling;
`/health/live` is excluded. The Generator exports its own spans through the Azure Monitor trace exporter. Both
also export the `ClaimStatements.Run` span (duration, statement type, maximum batch size, outcome). Cloud role
names are `RPA.ClaimStatements.Web` and `RPA.ClaimStatements.Generator`. Export is best effort, not an audit store.

### Working folder root and folder paths

`CLAIMSTATEMENTS_ROOT` holds the temporary working folders. The XSLT transformations ship with the application
(`XSLT` next to the binaries). Import, archive and PDF outbound paths come from the `CS.Folders` table, not from
`CLAIMSTATEMENTS_ROOT`; they must be valid for the operating system the application runs on (Linux paths in the
containers). The Generator's `ALL` command creates the working subfolders; with `BPS` or `XB` they must exist.

## Running the Generator

```powershell
dotnet RPA.ClaimStatements.Generator.dll ALL 10   # imports, then BPS and XB generation, at most 10 statements per type
dotnet RPA.ClaimStatements.Generator.dll BPS 5    # imports, then BPS generation only (XB likewise)
dotnet RPA.ClaimStatements.Generator.dll I        # imports only
```

- The statement type is uppercase and the batch size a positive 32-bit integer; `I` takes no batch size.
- Missing, invalid or extra arguments print usage to stderr and exit with code **2** before any processing starts.
  Without arguments the program prompts, but only when stdin is interactive.
- An unhandled exception, including a missing required setting, is written to stderr and exits with code **1**;
  telemetry is flushed first.
- Exit code **0** means the processing call returned normally, not that every statement succeeded: check the
  application error and log records and the outputs.
- Each run imports SITI (managed FTP), XB, DAX AP and DAX AR files, generates statements (only when `Generation`
  is `Active`), publishes the PDFs and reports tracking results (mail is optional and off by default). Mutexes give per-host coordination only, not
  exclusion across container replicas or jobs.

## Run locally

### Directly on Windows

1. Provide a SQL Server database (Express, LocalDB or full). To create the schema in a new local database set
   `CLAIMSTATEMENTS_APPLY_MIGRATIONS=true` for the first run; for an existing database see the
   [developer guide](./docs/developer-guide.md#database-schema-and-migrations).
2. Set the required variables in your shell or, for Web, in user secrets:

   ```powershell
   $env:ConnectionStrings__ClaimStatementsContext = 'Server=.\SQLEXPRESS;Database=<database>;Integrated Security=True;Encrypt=True;TrustServerCertificate=True'
   $env:CLAIMSTATEMENTS_ROOT = 'C:\csg\work'
   $env:Mail__Enabled = 'false'
   ```

   Put the approved `ClaimTemplate.docx` and `ClaimTemplateXB.docx` in `C:\csg\work\Templates`.
   `TrustServerCertificate=True` is for a local self-signed certificate only.
3. Run the Web application (`https://localhost:64158`, from `launchSettings.json`) or the Generator:

   ```powershell
   dotnet run --project RPA.ClaimStatements\RPA.ClaimStatements.Web
   dotnet run --project RPA.ClaimStatements\RPA.ClaimStatements.Generator -- BPS 5
   ```

Web sign-in uses Entra ID even locally: set `AzureAd__TenantId`, `AzureAd__ClientId` and the client credential, and
register the redirect URIs `https://localhost:64158/signin-oidc` and `https://localhost:64158/signout-callback-oidc`.
Every request, including static files, requires sign-in. PDF rendering also needs Python and WeasyPrint on the PATH
(see above); rendering on Windows is not verified, and Windows fonts differ from the container's, so use the
container to judge PDF output.

### Docker (Web and PDF testing)

[compose.yaml](./compose.yaml) builds the same Dockerfile targets that are deployed and runs them locally. It is
for local testing only. Settings come from one private env file outside Git; copy
[container/web.env.example](./container/web.env.example) and fill it in. Do not print the resolved Compose
configuration, which contains secrets.

```powershell
docker compose --env-file <private env file> up --build web
docker compose --env-file <private env file> run --rm --build generator BPS 1
```

- The Web application is published at `http://localhost:64158` (plain HTTP). Register the redirect URIs
  `http://localhost:64158/signin-oidc` and `http://localhost:64158/signout-callback-oidc`; Entra accepts `http`
  for `localhost`.
- Mounted from the host: the templates folder (read-only) and the PDF archive and outbound folders. The
  `CS.Folders` rows `PDF Archive` and `PDF Outbound` must contain the container paths
  `/var/lib/claimstatements/archive` and `/var/lib/claimstatements/outbound`. The folders must be writable by UID 1000.
- A SQL Server on the host is reachable through `host.docker.internal`. Windows integrated authentication does not
  work from a Linux container; use a SQL login.
- Set `NUGET_SOURCE` in the env file if nuget.org is not reachable from the build machine.
- Keep `Mail__Enabled=false` locally; Compose and the env example expose only the two `Mail__*` variables.
- Rendering changes only need `--build`; the WeasyPrint layers stay cached. `docker compose down` removes the
  containers and the container-local working folder.

## Container images

The [Dockerfile](./Dockerfile) follows the [DEFRA .NET parent image](https://github.com/DEFRA/defra-docker-dotnetcore)
standard. Both images run on Linux (`linux-musl-x64`) as the non-root `dotnet` user (UID/GID 1000) and carry the
`uk.gov.defra.parent-image` and OCI labels.

| Target | Image |
|---|---|
| `production` (last stage, so the default target) | The Web application, which also runs the Generator in-process. Listens on HTTP 8080. |
| `generator` | The console application for run-to-completion jobs. Contains no Web files. |

```powershell
docker build -t rpa-claim-statements-web .
docker build --target generator -t rpa-claim-statements-generator .
```

- Build arguments: `PARENT_VERSION` (default `2.1.9-dotnet10.0`), `NUGET_SOURCE` (optional package source instead of
  nuget.org; never put credentials in it) and `BUILD_VERSION`, `VCS_REF`, `BUILD_DATE` for the labels.
- A shared `base` stage holds WeasyPrint 70.0 (the source archive is SHA256-checked and installed from a hash-pinned
  requirements file without further downloads), the native dependencies and the working folders.
- The images define no Docker `HEALTHCHECK`; configure the platform probes explicitly.
- Scan the images as described in the DEFRA parent image's `IMAGE_SCANNING.md` before release.

## Azure Container Apps requirements

The receiving team supplies the environment, identities, secrets and mounts, and verifies the following before
production. See the [operator guide](./docs/operator-guide.md) for the runbook.

- **Ingress:** HTTPS-only external ingress targeting HTTP 8080. Use `GET /health/live` for liveness and startup
  probes; it is anonymous and checks the process only. Set `AllowedHosts` to the approved hostnames.
- **Forwarded headers:** ACA terminates TLS, but the application contains no forwarded-header configuration, so the
  generated Entra callback URL would use `http`. Enable forwarding explicitly (for example
  `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true`, which also removes the known-proxy restrictions) only after the platform
  owner confirms that requests reach the container only through trusted ingress. Otherwise an explicit trusted-proxy
  allowlist must be added to the application. This is an open item.
- **Entra:** a single-tenant web app registration with the redirect URIs `https://<host>/signin-oidc` and
  `https://<host>/signout-callback-oidc`, a valid client credential, and the app roles in the
  [operator guide](./docs/operator-guide.md#entra-setup-app-roles).
- **SQL:** grant the runtime identity least-privilege access, use managed identity authentication with encryption and
  certificate validation (`TrustServerCertificate=False`). Startup does not prove connectivity or permissions.
- **Schema:** deployed environments never migrate at startup. Apply migrations with an EF migration bundle using an
  identity with DDL rights. Existing databases need the one-time adoption script (developer guide).
- **Storage:** the code uses ordinary file paths. Mount persistent storage at the `CS.Folders` paths (archive,
  outbound, imports) and read-only at `$CLAIMSTATEMENTS_ROOT/Templates`; the database owner sets the production
  `CS.Folders.Path` values to the mount paths. Mounts must be writable by UID/GID 1000. The working folder
  (`XML`, `DOCX`, `HTML`, `PDF`, `Uploads`) can stay on the container filesystem. The final PDF destination may be an
  existing share owned by another team; the platform and database owners arrange mount access.
- **Shared keys and telemetry:** configure `DataProtection__*` and, optionally, the telemetry settings above.
- **Generator:** run as a job with the arguments `BPS N`, `XB N`, `ALL N` or `I`. Do not run overlapping jobs.
- **Mail:** keep `Mail__Enabled=false` until the [Exchange and platform prerequisites](./docs/operator-guide.md#tracking-mail-platform-prerequisites)
  are met. Graph uses the managed identity and needs no mail secret.
- **Secrets:** inject the SQL credentials (if any) and client credential from Key Vault as secrets.
  Never put them in the image, the repository or build arguments.

## Troubleshooting

| Symptom | Check |
|---|---|
| Startup fails naming a missing setting | `ConnectionStrings__ClaimStatementsContext`, `CLAIMSTATEMENTS_ROOT` and the `AzureAd__*` values are required. |
| Startup reports pending migrations or an unreachable database | Check the connection string and network, then the migration state (developer guide). |
| Web fails at startup with `DataProtection` errors | Check the settings, the Blob and Key Vault URIs and the identity's roles. A leftover `DataProtection__KeyDirectory` is rejected. |
| Tracking mail not sent or not received | Check `Mail__Enabled` and, when enabled, `Mail__ManagedIdentityClientId`, the identity's attachment to the app or job, and the Exchange authorization. A bad `Mail__*` value does not stop startup; it appears as a warning at the end of the run. HTTP 202 means accepted, not delivered. |
| Redirect loop, `http` callback URL or Entra redirect mismatch | Check forwarded headers, the registered redirect URIs and the external hostname. |
| HTTP 403 after sign-in | The user has no matching Entra app role (see the operator guide). |
| PDF rendering failed (exit code, timeout) | Check Python and WeasyPrint, the HTML and the template images; each statement has a 60-second limit. The Generator logs the failure per statement. |
| Missing templates or XSLT | Templates come from `$CLAIMSTATEMENTS_ROOT/Templates`; the XSLT ships in the application folder. |
| Generator image: usage and exit code 2 | Supply arguments: `BPS N`, `XB N`, `ALL N` or `I`. |
| Docker build cannot restore packages | Set `NUGET_SOURCE` to a reachable package source. |
