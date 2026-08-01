# cCoder.Security

`cCoder.Security.Data` exposes its supporting persistence services through
`services.AddSecurityData(configuration.Security)`. The same strongly typed
`SecurityConfiguration` instance is passed onward to `cCoder.Security`; no
flat `ConnectionStrings` compatibility configuration is required.

`cCoder.Security` owns the SSO security boundary for the cCoder platform. It manages tenants, SSO users, registrations, invitations, password reset tokens, login sessions, and the Security account lifecycle events consumed by downstream domains.

## Local Configuration

Configuration binds directly into `SecurityConfiguration`. Leave secrets empty
in appsettings and define `Security__ConnectionString` and
`Security__DecryptionKey` as user-level or machine-level environment variables.
Restart Visual Studio, select the Web and HostedServices startup projects, and
press F5. No configuration conversion step is required.

Non-secret Argon2id password hashing parameters bind from `Security__Argon`.
The checked-in defaults use the OWASP minimum profile and the application
rejects weaker runtime values:

```json
"Argon": {
  "MemorySizeInKilobytes": 19456,
  "Iterations": 2,
  "DegreeOfParallelism": 1,
  "SaltSizeInBytes": 16,
  "HashSizeInBytes": 32
}
```

## Repository Layout

- `src/cCoder.Security`
  Main NuGet package, API controllers, service collection extensions, account orchestration, and event publication.
- `src/cCoder.Security.Data`
  SQL Server EF Core persistence, models, context factories, migrations, and typed Security event models.
- `src/Apps/Security.Web`
  Public API host for account, tenant, and SSO endpoints.
- `src/Apps/Security.HostedServices`
  Hosted-services app for background/event processing.
- `src/cCoder.Security.Tests`
  Unit tests for Security services and managers.
- `src/Apps/Security.Web.AcceptanceTests`
  HTTP acceptance tests for the public web app.
- `src/Apps/Security.HostedServices.AcceptanceTests`
  Startup and health acceptance tests for the hosted-services app.

## App Responsibilities

`Security.Web` hosts the public API surface only. It exposes account, authentication, registration, and invitation routes under `Api/Account`, SSO user APIs, tenant APIs, and `/Health`.

`Security.HostedServices` is the background/event host. It registers Security event handlers and exposes `/Health` for runner and deployment health checks.

Consumers should use the shared service collection extensions:

```csharp
services.AddSecurityWeb(configuration);
services.AddSecurityHostedServices(configuration);
```

## Account Lifecycle

Security owns the following account flows:

- Login and logout.
- Direct registration and registration confirmation.
- Forgot password and confirm forgot password.
- Invitation, resend invitation, and accept invitation.

Invitation creates an SSO user without requiring a password. The invited user activates the account by accepting the invite token and setting a password.

External consumers should use the `Api/Account` HTTP endpoints for these account lifecycle operations. `ITokenManager` is reserved for privileged internal business cases, such as issuing a `WorkflowExecution` token for a known SSO user while executing a trusted workflow.

## Event Contract

Security publishes typed account lifecycle events after Security-owned state changes complete. Event payloads use `SecurityAccountEvent` and include:

- `Kind`
- `User`
- `Tenant`
- `RequestDomain`
- `Token`
- `Culture`

Security does not include an app id in account events. Consumers resolve the app from `RequestDomain`.

Published event names:

- `security_account_registration_created`
- `security_account_registration_confirmed`
- `security_account_invitation_created`
- `security_account_invitation_accepted`
- `security_account_password_reset_requested`

Downstream ownership:

- `cCoder.AppSecurity` creates or updates app-local users and role links from Security account events.
- `cCoder.Core` queues app-template emails from Security account events.

## Configuration

Set SQL Server connection strings through appsettings or environment variables:

```powershell
Required secret environment variables:

- `Security__ConnectionString`
- `Security__DecryptionKey`

Leave their matching `appsettings.json` values empty, define them as user- or
machine-level environment variables, restart Visual Studio, and run with F5.
```

Acceptance tests also read:

```powershell
$env:Security__ConnectionString = "Data Source=.;Initial Catalog=dev-Members;MultipleActiveResultSets=True;Trusted_Connection=True;Trust Server Certificate=true;Encrypt=True"
```

SQLite support has been retired; SQL Server is the only supported EF provider.

## Build And Test

```powershell
dotnet restore src\cCoder.Security.slnx
dotnet build src\cCoder.Security.slnx --no-restore
dotnet test src\cCoder.Security.slnx --no-restore --no-build
```

Before submitting changes, run both apps locally and confirm there are no startup errors:

```powershell
dotnet run --project src\Apps\Security.Web\Security.Web.csproj
dotnet run --project src\Apps\Security.HostedServices\Security.HostedServices.csproj
```

## Package

This repository publishes:

- `cCoder.Security`
- `cCoder.Security.Data`

## License

This repository is licensed under The Standard Software License Version 1.0. See `LICENSE.txt` for details.
