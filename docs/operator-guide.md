# Operator guide: Azure Container Apps

For teams moving the Claim Statement Generator (CSG) from a Windows server deployment to the supplied Linux
containers on Azure Container Apps (ACA). Use the README for [configuration](../README.md#configuration) and
[local commands](../README.md#run-locally), and the [developer guide](./developer-guide.md) for implementation details.

**Status:** Azure deployment and PDF parity with the original application have not been verified. This is a runbook
for the receiving team's deployment and acceptance, not a claim that the environment, alerts or recovery procedures
already exist. Infrastructure definitions and pipelines are not part of this repository.

## The operating model has changed

| Windows server habit | Container Apps equivalent |
|---|---|
| Copy binaries or patch the installed runtime | Build and test a replacement image, push it to the registry and deploy it. |
| Recycle IIS or log on to repair files | Inspect the failing revision or replica; correct the image or the external configuration. Local container changes are disposable. |
| Keep machine-local configuration, `web.config` and `App.config` | Supply non-secret settings as environment variables and secrets as Key Vault references. |
| Run the Generator from Task Scheduler | Run the `generator` image as a scheduled or manually triggered Container Apps job. |
| Use the server's identity and the `Security` database | Entra ID signs users in; a user-assigned managed identity (UAMI) accesses Azure services. |
| Keep cookie keys on disk | Share the encrypted Data Protection key ring in Blob Storage, wrapped by a Key Vault key. |
| Read event logs and files | Use the ACA console and system logs, Log Analytics and Application Insights. |

Containers and their filesystems are replaceable. SQL data, configuration, folders on mounted storage, identities and
encryption keys must survive their replacement.

## Deployment model and ownership

Two images are built from one Dockerfile (see the README): the Web application (default target) and the Generator.

| Item | Setting or owner |
|---|---|
| Web app | One container, external HTTPS-only ingress targeting HTTP 8080, non-root. Keep `ASPNETCORE_ENVIRONMENT=Production` (the image default). |
| Generator | A Container Apps job running `dotnet RPA.ClaimStatements.Generator.dll`, with the arguments `BPS N`, `XB N`, `ALL N` or `I`. It has no ingress. |
| Replicas | Use one Web replica unless concurrent generation has been assessed: the Generator's mutexes coordinate within one host only, so two replicas, or a Web-triggered run and a job, can process the same imports twice. Do not run overlapping jobs. |
| Configuration | Environment variables from the README table. The platform team supplies the values. |
| Storage | Persistent storage mounted at the `CS.Folders` paths and read-only at `$CLAIMSTATEMENTS_ROOT/Templates` (see below). |
| Database | The database team owns the schema, the `CS.Folders` paths and the adoption and migration steps. |

### Identity and permission checklist

| Need | Requirement |
|---|---|
| SQL | The runtime UAMI is a database user with least-privilege access to the application's schemas. Connection string: `Server=<sql-endpoint>;Database=<database>;Authentication=Active Directory Managed Identity;User Id=<uami-client-id>;Encrypt=Mandatory;TrustServerCertificate=False`. `User Id` is the identity's client ID, not its object or resource ID. |
| Schema changes | A separate deployment identity with DDL rights runs the migration bundle. The runtime identity must not have DDL rights. |
| Key Vault secrets | ACA resolves Key Vault secret references through its configured identity. Do not pass unresolved vault URLs as settings. The application itself does not read Key Vault secrets. |
| Data Protection | The UAMI named in `DataProtection__ManagedIdentityClientId` has Storage Blob Data Contributor on the key-ring container and Key Vault Crypto User on the key. |
| Telemetry | The UAMI named in `Telemetry__ManagedIdentityClientId` has Monitoring Metrics Publisher on Application Insights. Without it, connection-string-only ingestion needs local authentication enabled. |
| Tracking mail | `Mail__ManagedIdentityClientId` is the client ID of a UAMI attached to **both the Web app and the Generator job**. It needs scoped permission to send as one mailbox; see below. |
| Final PDF destination | May be an existing share owned by another team. Platform and database owners arrange mount access; UID/GID 1000 must be able to write. |

### Tracking mail platform prerequisites

Keep `Mail__Enabled=false` until the platform and Exchange owners have provided and verified:

1. **A mailbox in Exchange Online.** The `Email Account` value in `CS.Configuration` is both sender and recipient.
2. **The managed identity** attached to the Web app and the Generator job (not only to the ACA environment).
   `Mail__ManagedIdentityClientId` is its client ID, not its object or resource ID. There is no fallback to
   developer credentials.
3. **Network:** outbound HTTPS to `graph.microsoft.com`.
4. **Permission limited to that one mailbox.** Use Exchange Online RBAC for Applications with the identity's service
   principal and the Application Mail.Send role scoped to the mailbox. Do not also grant an unscoped Graph `Mail.Send`
   in Entra: independent grants add up and are not limited by the RBAC scope. Exchange administrators should test that
   sending as the approved mailbox works and sending as another mailbox is refused.
5. **Classification and retention.** The report contains FRNs and error messages. `saveToSentItems` is false, but the
   recipient's mailbox still keeps the message.

Behaviour to know: a send has a 30-second limit and is not retried. Graph answering HTTP 202 means the message was
accepted, not delivered. A failed send writes a warning to the console and to telemetry; the run continues and the
exit code does not change. With `Mail__Enabled` unset or `false`, nothing is sent and nothing is logged as an error.
To check delivery, use a test mailbox; a live send needs separate approval.

### Storage

The application uses ordinary file paths, so mount Azure Files (or another supported storage) at the paths the
database holds:

- the PDF archive, PDF outbound and import folders listed in `CS.Folders`, writable by UID/GID 1000. The database
  owner sets `CS.Folders.Path` to these Linux mount paths before production testing;
- the templates folder, `$CLAIMSTATEMENTS_ROOT/Templates` (`/var/lib/claimstatements/work/Templates` in the image),
  read-only, containing `ClaimTemplate.docx` and `ClaimTemplateXB.docx`. Neither template is bundled.

The working folders (`XML`, `DOCX`, `HTML`, `PDF`, `Uploads`) can stay on the container filesystem: successful runs
clean up their XML, DOCX and HTML intermediates. The XSLT files ship in the image, so mounts never hide them. A mount
at a path hides whatever the image has there, so do not mount over the whole working folder. Which folders get
persistent storage is a decision for the platform and database owners.

## Entra setup: app roles

### 1. Confirm the web application registration

- Single tenant, with the Web redirect URIs `https://<app-host>/signin-oidc` and
  `https://<app-host>/signout-callback-oidc` for each hostname in use.
- A valid client credential (a Key Vault secret reference, for example). Do not enable the implicit-grant ID tokens
  option: the application uses the authorization-code flow with PKCE.
- `AzureAd__TenantId` is the organisation's own tenant GUID, never `common`, `organizations` or `consumers`.

### 2. Define and assign the application roles

Define these roles on the registration (`appRoles`) and assign them to users or groups. The application reads only
the `roles` claim, never a `groups` claim.

| Entra app role value | Permission checked by the pages |
|---|---|
| `ClaimStatementGenerator.TriggerGeneration` | Claim Statement Generator: Trigger Generation |
| `ClaimStatementGenerator.ViewClaimStatements` | Claim Statement Generator: View Claim Statements |
| `ClaimStatementGenerator.ManageExceptions` | Claim Statement Generator: Manage Exceptions |
| `ClaimStatementGenerator.ManageSuppressions` | Claim Statement Generator: Manage Suppressions |
| `ClaimStatementGenerator.DownloadData` | Claim Statement Generator: Download Data |
| `ClaimStatementGenerator.ManualUpload` | Claim Statement Generator: Manual Upload |
| `ClaimStatementGenerator.ForceGeneration` | Claim Statement Generator: Force Generation |

Role values cannot contain spaces, hence the dotted form. Defining a role does not assign it to anyone. A user with no
matching role gets HTTP 403 after signing in; an unknown role value grants nothing.

### 3. Verify assignments safely

Sign in as a test user and confirm the expected pages and 403 responses, then check the role assignments in the
Entra portal. Never paste tokens, authorization codes or credentials into tickets or chat.

## First deployment and release acceptance

1. Build and scan the images (the DEFRA parent image's `IMAGE_SCANNING.md` describes the Trivy and Grype checks) and
   push them to the registry.
2. Confirm the database matches the model, then run the one-time adoption script (existing databases) or the
   migration bundle (new databases). See the developer guide. Deployed environments never migrate at startup.
3. Create the secrets, settings, mounts, identities and role assignments above, and the Data Protection resources.
4. Deploy the Web container. Configure the liveness and startup probes as HTTP GET `/health/live` on port 8080.
   Set `AllowedHosts` to the application hostnames, including the host the probes use.
5. **Forwarded headers (open item):** ACA terminates TLS, but the application has no forwarded-header configuration,
   so the OpenID Connect callback URL would use `http`. Either set `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true`
   (which clears the known-proxy restrictions, so confirm that requests reach the container only through ACA
   ingress) or add an explicit trusted-proxy allowlist to the application. Verify the redirect and callback URLs use
   `https` and that untrusted forwarded headers cannot change them.
6. Accept the Web application: sign in, check that each role sees the right pages, and that sessions survive a
   restart, a new revision and a second replica (shared keys).
7. Run the Generator job manually with a small batch (for example `BPS 1`) and check: exit code, the application
   error and log tables, the generated PDFs in the outbound and archive folders, and cleanup of the working folders.
   If mail is enabled, check the tracking report arrives in the test mailbox (HTTP 202 alone is not proof).
8. Have the owners review real statements for every supplied template variant against the original output. This
   acceptance has not been done.
9. Review the logs, telemetry ingestion and alerts you intend to rely on.

## Routine health checks and diagnosis

| Symptom | Check |
|---|---|
| Revision does not start | The console log names the missing or invalid setting: `ConnectionStrings__ClaimStatementsContext`, `CLAIMSTATEMENTS_ROOT`, `AzureAd__*`, `DataProtection__*` or `Telemetry__*`. Failures do not echo secret values. |
| Startup reports pending migrations | The database does not match the deployed image: apply the migration bundle or the adoption script. |
| Startup fails with an unreachable database | Network access, firewall, identity grants and SQL certificate trust. |
| Liveness passes but pages fail | `/health/live` only proves the process runs. Check SQL access, Entra sign-in and the logs. |
| Redirect loop or wrong callback scheme | Forwarded-header trust, ingress HTTPS and the registered redirect URIs. |
| Users are signed out or see anti-forgery errors after a deployment | Data Protection settings, the Blob and Key Vault access, and a stable `ApplicationName`. |
| HTTP 403 after sign-in | The user lacks an app role (see above). |
| Generator exits with code 2 | Arguments missing or invalid; usage is printed to stderr. |
| Generator exits with code 1 | An unhandled error or missing setting is on stderr and in telemetry. |
| Statements are missing or errors accumulate | Check the application error and log tables, the `Generation` and `Validation` switches in `CS.Configuration`, and the PDF renderer messages (a statement that fails to render does not stop the run). |
| PDF rendering fails or times out | Memory and CPU limits on the job, the template images (BMP and WMF are rejected) and the 60-second per-statement limit. |
| PDF output looks different from the original | Fonts and layout differ from the old renderer; compare against real statements (developer guide). |
| Tracking mail not sent or not received | Check `Mail__Enabled` and, when enabled, `Mail__ManagedIdentityClientId`, the identity's attachment to the app or job, and the Exchange authorization. A bad `Mail__*` value does not stop startup; it appears as a warning at the end of the run. HTTP 202 means accepted, not delivered. |

Telemetry is best effort and not an audit store; the database error and log records remain authoritative.

## Releases, scaling and rollback

- **Releases:** build and test a new image, scan it, then deploy it as a new revision. Update the Generator job's image
  in step. Apply database migrations first, because the application refuses to work against a schema with pending
  migrations.
- **Scaling:** see the replica guidance above. CPU and memory for the Generator job also bound the PDF renderer, which
  starts one Python process per statement.
- **Concurrency:** schedule jobs so that two runs never overlap, and avoid triggering generation from the Web
  application while a job is running.

### Rollback checklist

1. Stop or avoid starting Generator jobs.
2. Re-activate the previous Web revision (or redeploy the previous image) and point the job at the previous image.
3. If a migration was applied, decide with the database owner whether it is backward compatible. Migrations are not
   reversed automatically, and a rollback to an older image against a newer schema is not supported unless the
   migration is compatible.
4. Verify sign-in, a Generator run with a small batch, and the output folders.
5. Record the cause before attempting the release again.

## Secret and key maintenance

- **Client credential and SQL credentials:** rotate them in Key Vault and refresh the container app
  revision so the new values are injected. Allow overlap where the provider supports it.
- **Data Protection key-ring wrapping key:** keep every Key Vault key version that was used to wrap the ring, so
  existing cookies and keys can still be decrypted. Never delete the key-ring blob. There is no local or unencrypted
  fallback; access failures stop the Web application at startup.
- **Managed identities:** keep the roles listed in the checklist on the same identities that the settings name.
- **Mail:** no mail secret. Exchange owners maintain the one-mailbox authorization; platform owners maintain
  retention of the report.
- **Package and image patches:** rebuild and scan the images regularly. WeasyPrint, Python and Alpine packages are
  patched through the image, separately from NuGet auditing.

## Platform references

- [Azure Container Apps health probes](https://learn.microsoft.com/azure/container-apps/health-probes)
- [Manage secrets in Azure Container Apps](https://learn.microsoft.com/azure/container-apps/manage-secrets)
- [Storage mounts in Azure Container Apps](https://learn.microsoft.com/azure/container-apps/storage-mounts)
- [Jobs in Azure Container Apps](https://learn.microsoft.com/azure/container-apps/jobs)
- [Revisions in Azure Container Apps](https://learn.microsoft.com/azure/container-apps/revisions)
- [Add app roles to an application (Microsoft Entra)](https://learn.microsoft.com/entra/identity-platform/howto-add-app-roles-in-apps)
- [Configure ASP.NET Core to work with proxy servers and load balancers](https://learn.microsoft.com/aspnet/core/host-and-deploy/proxy-load-balancer)
- [Enable Azure Monitor OpenTelemetry](https://learn.microsoft.com/azure/azure-monitor/app/opentelemetry-enable)
- [Microsoft Graph sendMail](https://learn.microsoft.com/graph/api/user-sendmail)
- [Exchange Online RBAC for Applications](https://learn.microsoft.com/exchange/permissions-exo/application-rbac)
- [DEFRA .NET parent image](https://github.com/DEFRA/defra-docker-dotnetcore)
