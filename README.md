# Vue Web Enterprise SQL Template

A `dotnet new` template for scaffolding a clean architecture solution with a Vue client, .NET API, Application, Infrastructure, and Domain projects. Uses **MS SQL Server** as the database provider.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (or whichever version matches the template's target framework)

## Installation

### 1. Authentication

Create a **Classic** Personal Access Token (PAT) at [github.com/settings/tokens](https://github.com/settings/tokens) with the following scopes:

- `read:packages`
- `repo`

> **Note:** Fine-Grained Tokens do **not** work with the GitHub NuGet Registry. You must use a Classic PAT. If your organisation requires SSO, enable SSO for the token after creation.

### 2. Environment Variable

Store your token as a persistent user-level environment variable so it survives restarts:

```powershell
[System.Environment]::SetEnvironmentVariable("GH_PACKAGE_TOKEN", "your_token_here", "User")
```

Restart your terminal after running the command.

### 3. NuGet Source

Add (or update) the GitHub Packages NuGet source:

```powershell
dotnet nuget add source "https://nuget.pkg.github.com/markcouvaras/index.json" `
  --name "GitHub_Enterprise" `
  --username markcouvaras `
  --password "$env:GH_PACKAGE_TOKEN" `
  --store-password-in-clear-text
```

If the source already exists, update it instead:

```powershell
dotnet nuget update source "GitHub_Enterprise" `
  --source "https://nuget.pkg.github.com/markcouvaras/index.json" `
  --username markcouvaras `
  --password "$env:GH_PACKAGE_TOKEN" `
  --store-password-in-clear-text
```

### 4. Install the Template

```powershell
dotnet new install VueWebEnterpriseSQL.Template --version "1.0.*" --nuget-source "GitHub_Enterprise"
```

### From a Local Clone

```bash
git clone https://github.com/markcouvaras/VueWebEnterpriseSQLTemplate.git
dotnet new install ./VueWebEnterpriseSQLTemplate
```

### Verify Installation

```bash
dotnet new list vwe-sql-app
```

### Troubleshooting

If you encounter stale template data or installation issues, clear the template cache and re-install:

```bash
dotnet new --clear-cache
```

## Creating a New Project

```bash
dotnet new vwe-sql-app -n YourProjectName -o YourProjectName
```

This will:

1. Create a new folder called `YourProjectName`.
2. Copy all template files into it, replacing every occurrence of `VueWebEnterpriseSQL` with `YourProjectName` (folder names, file names, namespaces, project references, etc.).
3. Automatically run `dotnet restore` to pull down NuGet packages.

### Example

```bash
dotnet new vwe-sql-app -n Contoso -o Contoso
```

Produces:

```
Contoso/
  Contoso.sln
  Contoso.Api/
  Contoso.Application/
  Contoso.Domain/
  Contoso.Infrastructure/
  contoso.client/
```

## Template Options

| Parameter           | Type | Default | Description                                              |
| ------------------- | ---- | ------- | -------------------------------------------------------- |
| `--IncludeDatabase` | bool | `true`  | Include the Infrastructure project with EF Core / database support. |

### Excluding the Infrastructure Project

To scaffold without the Infrastructure project (no EF Core, Dapper, or database wiring):

```bash
dotnet new vwe-sql-app -n YourProjectName --IncludeDatabase false
```

This will:

- Omit the `YourProjectName.Infrastructure/` folder entirely.
- Remove the Infrastructure project entry and build configurations from the `.sln` file.
- Remove the Infrastructure `ProjectReference` from the API project.

## Architecture & Folder Structure

This template follows **Clean Architecture** principles. Each project has a clear responsibility — use the table below to decide where a new file should go.

| Project | Responsibility | What goes here |
| ------- | -------------- | -------------- |
| **Domain** | Core business logic with zero external dependencies | Entities, Enums, Exceptions |
| **Application** | Use-case orchestration and contracts | Interfaces, DTOs, Validators |
| **Infrastructure** | External concerns and implementation details | Database Context, Migrations, Service Implementations (Email, PDF, etc.), HTML Templates |
| **Api** | HTTP entry point | Controllers, Middleware |

### Dependency rule

Dependencies flow **inward** only:

```
Api --> Infrastructure --> Application --> Domain
```

- **Domain** references nothing.
- **Application** references Domain.
- **Infrastructure** references Application (and therefore Domain).
- **Api** references Infrastructure and Application.

### Project Deep Dive

#### VueWebEnterpriseSQL.Domain (The Core)

The "Heart" of your business. It has **NO dependencies** on any other project. It doesn't know about databases, APIs, or JSON.

**What lives here?**

- **Entities:** `User.cs`, `Product.cs`, `Order.cs` — the main database objects.
- **Value Objects:** `Address.cs`, `Money.cs` — complex types that aren't tables.
- **Enums:** `OrderStatus.cs` (e.g., Draft, Paid, Shipped).
- **Exceptions:** `InsufficientFundsException.cs` — custom error logic.
- **Domain Events:** `OrderCompletedEvent.cs` — triggers for logic.

#### VueWebEnterpriseSQL.Application (The Brain)

The "Orchestrator." It defines **what** the system can do. It holds the business rules but not the technical implementation.

**What lives here?**

- **Interfaces:** `ICurrentUserService.cs`, `IEmailService.cs`, `IPdfService.cs` — contracts.
- **DTOs (Data Transfer Objects):** `UserDto.cs`, `CreateUserRequest.cs` — safe data shapes for the UI.
- **Validators:** `CreateUserValidator.cs` — FluentValidation rules.
- **Services / Use Cases:** `UserService.cs` — the logic: "Check rules, then save."
- **Mappings:** `MappingProfile.cs` — AutoMapper rules to convert Entities to DTOs.

#### VueWebEnterpriseSQL.Infrastructure (The Toolbelt)

The "Implementation." It connects your clean Application to the real world (databases, files, 3rd-party APIs).

**What lives here?**

- **Database:** `AppDbContext.cs`, `Migrations/` — EF Core.
- **Service Implementations:** `EmailService.cs` (SendGrid/SMTP), `PdfService.cs` (Puppeteer), `CurrentUserService.cs` (HttpContext).
- **File Access:** Code that reads/writes to disk or Azure Blob Storage.
- **Templates:** `Templates/Email/Welcome.html`, `Templates/Pdf/Invoice.html`.
- **External Clients:** `StripePaymentClient.cs` — code that talks to Stripe.

#### VueWebEnterpriseSQL.Api (The Mouth)

The "Entry Point." It translates HTTP requests into Application calls.

**What lives here?**

- **Controllers:** `UsersController.cs`, `OrdersController.cs`.
- **Program.cs:** The startup script that wires everything up.
- **Dependency Injection:** Configuring which Infrastructure classes satisfy which Application interfaces.
- **Middleware:** `GlobalExceptionHandler.cs` (catching errors), `JwtMiddleware.cs`.
- **Filters:** Logging attributes or security headers.

### Quick examples

| "I need to ..." | Create it in |
| ---------------- | ------------ |
| Add a new database entity | `Domain/Entities/` |
| Define a service interface | `Application/Interfaces/` |
| Add a request/response model | `Application/DTOs/` |
| Add input validation | `Application/Validators/` |
| Implement an email sender | `Infrastructure/Services/` |
| Add a new EF migration | `Infrastructure/Migrations/` |
| Add an HTML email template | `Infrastructure/Templates/` |
| Create a new API endpoint | `Api/Controllers/` |
| Add auth or logging middleware | `Api/Middleware/` |

### Standardized Folder Structure

The template ships with the following folder layout out of the box. Every folder is pre-created so the team follows a consistent convention from day one.

```
YourProject.Domain/                    # The Core
  Common/                              # Base entities (e.g. AuditableEntity)
  Entities/                            # User.cs, Product.cs, Order.cs
  Enums/                               # OrderStatus.cs, PaymentType.cs
  ValueObjects/                        # Address.cs, Money.cs

YourProject.Application/               # The Brain
  Common/
    Behaviours/                        # LoggingBehaviour, ValidationBehaviour
    Exceptions/                        # NotFoundException, ValidationException
    Mappings/                          # AutoMapper profiles
    MetaData.cs                        # Pagination metadata (CurrentPage, TotalPages, etc.)
    PagedResponse.cs                   # Generic response: Items, MetaData, AggregatedData
    RequestParameters.cs               # Abstract base: Page, PageSize, OrderBy, OrderDirection
    TableFilterSummary.cs              # Generic facet bucket: TableFilterSummary<T>
    UserParameters.cs                  # SearchTerm, Department, Status filters
  DTOs/                                # UserDto.cs, CreateUserRequest.cs
  Features/
    Users/Commands/CreateUser/         # CreateUserCommand + Validator (example)
    Users/Queries/                     # GetUsersWithPaginationQuery (MediatR CQRS)
  Interfaces/                          # IEmailService.cs, IUserReadRepository.cs
  DependencyInjection.cs               # AddApplication() — registers MediatR + behaviours

YourProject.Infrastructure/            # The Toolbelt
  Data/                                # AppDbContext.cs, Migrations/
  Messaging/                           # MailKitEmailService, EmailManager, MailSettings
    Jobs/                              # ProcessSingleEmailJob, ProcessBulkEmailJob
  Queries/                             # UserQueries.cs (SQL factory), UserQuery.cs, OrderQueryBuilder.cs
  Services/                            # CurrentUserService.cs, PdfService.cs
  Templates/
    Email/                             # _Layout.html, Welcome.html
    Pdf/                               # Invoice.html, Report.html

YourProject.Api/                       # The Mouth
  Controllers/
    BaseController.cs                  # Versioned route + lazy MediatR
    V1/                                # Version 1 controllers
      UsersController.cs               # [ApiVersion("1.0")]
  Extensions/
    ServiceCollectionExtensions.cs     # AddWebServices() — layers, CORS, versioning
    SwaggerExtensions.cs               # AddSwaggerWithVersioning() — per-version docs
  Filters/                             # ApiKeyAuthAttribute.cs
  Infrastructure/                      # GlobalExceptionHandler.cs (IExceptionHandler)
  Middleware/                          # JwtMiddleware.cs, custom middleware
```

### Included Plumbing

The template ships with the following pre-wired components so you don't have to set them up from scratch.

#### A. Global Exception Handler (The Safety Net)

**File:** `Api/Infrastructure/GlobalExceptionHandler.cs`

Implements the .NET 8+ `IExceptionHandler` pattern to convert unhandled exceptions into clean JSON responses. Without this, your Vue app receives a raw HTML stack trace or a generic 500 error that breaks the UI.

- `ValidationException` maps to **400 Bad Request** — returns the grouped error dictionary directly
- `KeyNotFoundException` maps to **404 Not Found**
- Everything else maps to **500 Server Error** (ProblemDetails format)

**Example 400 response** (when validation fails):

```json
{
  "status": 400,
  "title": "Validation Failed",
  "errors": {
    "Name": ["Name is required.", "Name must be at least 4 characters."],
    "Email": ["A valid email address is required."]
  }
}
```

#### B. Service Extensions (The Cleaner)

**File:** `Api/Extensions/ServiceCollectionExtensions.cs`

The `AddWebServices()` extension method keeps `Program.cs` readable by consolidating all service registration into one call. It wires up:

- Application layer (`AddApplication()`)
- Infrastructure layer (`AddInfrastructure()`)
- Global exception handler
- `HttpContextAccessor`
- API versioning
- Enterprise CORS policy

#### C. Validation Behaviour (The Guard)

**File:** `Application/Common/Behaviours/ValidationBehaviour.cs`

A MediatR pipeline behaviour that automatically runs all registered FluentValidation validators before your handler executes. If any validation rules fail, it throws a `ValidationException` before your business logic ever runs.

This means you never write `if (request.Name == null) return error;` inside your handlers again.

#### D. Serilog Logging (The Black Box)

**Packages:** `Serilog.AspNetCore`, `Serilog.Sinks.File`

Professional structured logging that replaces the default .NET logger. Configured in `appsettings.json` with two sinks:

- **Console** — coloured output during development
- **File** — daily rolling logs written to `Logs/log-YYYY-MM-DD.txt` (30 day retention)

`Program.cs` wires it up with:
- `builder.Host.UseSerilog()` — replaces the default logger
- `app.UseSerilogRequestLogging()` — logs every HTTP request (method, path, status code, duration)

#### E. Logging Behaviour (The Recorder)

**File:** `Application/Common/Behaviours/LoggingBehaviour.cs`

A MediatR pipeline behaviour that wraps every request with structured log entries:

- `[START]` — logs the request name and data when a request begins
- `[END]` — logs the request name and execution time in milliseconds
- `[ERROR]` — logs the exception details and execution time if the handler throws

Runs before the `ValidationBehaviour` in the pipeline so validation failures are also captured in logs.

#### F. Application Dependency Injection

**File:** `Application/DependencyInjection.cs`

The `AddApplication()` extension method registers:
- FluentValidation validators (assembly scan)
- MediatR handlers (assembly scan)
- `LoggingBehaviour` pipeline (runs first)
- `ValidationBehaviour` pipeline (runs second)

#### G. Advanced Paginated, Filtered & Sortable Users API with Faceted Search (CQRS-Lite)

A full backend-driven pagination system with **faceted aggregations** and **multi-select filtering** using the **CQRS-Lite** pattern: EF Core for writes, **Dapper** for reads. Returns paginated data, total count, and sidebar filter totals (Department, Status) in a **single SQL round-trip** using CTEs. SQL is managed via the **Static Query Factory** pattern — all SQL lives in `UserQueries.cs`, keeping the repository focused on execution and mapping.

**Pattern:** `Controller → MediatR Query → IUserReadRepository → UserQuery → UserQueries (SQL Factory) → Dapper CTE → SQL Server`

**Application layer files:**

| File | Purpose |
| ---- | ------- |
| `Common/RequestParameters.cs` | Abstract base: `Page`, `PageSize` (max 50), `OrderBy`, `OrderDirection`, `Fields` |
| `Common/UserParameters.cs` | Extends `RequestParameters`: adds `SearchTerm`, `Departments` (`List<string>`), `Statuses` (`List<int>`) for multi-select filtering |
| `Common/MetaData.cs` | Pagination metadata: `CurrentPage`, `TotalPages`, `PageSize`, `TotalCount`, `HasNext`, `HasPrevious` |
| `Common/PagedResponse<T>.cs` | Response wrapper: `Items`, `MetaData`, `AggregatedData` (facet totals) |
| `Common/TableFilterSummary<T>.cs` | Generic facet bucket: `Value` (string or int) + `Count`. Use `TableFilterSummary<string>` for Departments, `TableFilterSummary<int>` for Status |
| `DTOs/UserDto.cs` | Safe data shape: `Id`, `Email`, `FullName`, `Department`, `Status`, `LastLoginAt` |
| `Interfaces/IUserReadRepository.cs` | Read-side contract |
| `Features/Users/Queries/GetUsersWithPaginationQuery.cs` | MediatR query + handler |

**Infrastructure layer files:**

| File | Purpose |
| ---- | ------- |
| `Queries/OrderQueryBuilder.cs` | Uses **reflection** to validate `OrderBy` against actual DTO properties — prevents SQL injection without manual allow-lists |
| `Queries/UserQueries.cs` | **Static query factory** — all SQL lives here. Builds CTE + `DynamicParameters` with `IN @param` for multi-value SQL Server filtering |
| `Queries/UserQuery.cs` | Repository implementation — calls `UserQueries` for SQL, executes via Dapper `QueryMultipleAsync`, maps result sets |

**Static Query Factory Pattern:**

SQL is never written inline in the repository. The `UserQueries` static class owns all query construction and returns a tuple of `(string Sql, DynamicParameters Parameters)`:

```csharp
// UserQuery.cs (repository) — clean separation of concerns
var (sql, dbParams) = UserQueries.GetUsersWithFacets(parameters, orderByClause, skip, take);
await using var multi = await connection.QueryMultipleAsync(sql, dbParams);
```

This is reusable: when you add a `ProductQuery`, create a `ProductQueries` static class with the same pattern.

**Multi-Value Filtering:**

`UserParameters` uses `List<string>` for Departments and `List<int>` for Statuses so the Vue frontend can send multi-select filters. The SQL uses Dapper's `IN @param` expansion:

```sql
-- Single department:  ?departments=Engineering
-- Multi department:   ?departments=Engineering&departments=Marketing
WHERE [Department] IN @Departments AND [Status] IN @Statuses
```

**SQL Injection Protection:** The `OrderQueryBuilder.BuildOrderClause<TEntity>()` method uses reflection to scan the public properties of the target DTO. If the client-supplied `OrderBy` doesn't match any property name (case-insensitive), it falls back to the default sort. No raw user input ever reaches SQL.

```csharp
// Reflection-based — no manual dictionary needed
var orderByClause = OrderQueryBuilder.BuildOrderClause<UserDto>(parameters.OrderBy, parameters.OrderDirection);
// If OrderBy="FullName" and OrderDirection="desc" → [FullName] DESC
// If OrderBy="InvalidColumn" → [Id] ASC (default)
```

**CTE Query Strategy:**

The `UserQueries.GetUsersWithFacets()` method builds a single SQL statement with a CTE that filters once and reads four result sets:

```sql
;WITH filtered AS (SELECT * FROM [Users] WHERE ...)

SELECT COUNT(*) FROM filtered;                                    -- 1. Total count
SELECT [Department], COUNT(*) FROM filtered GROUP BY ...;         -- 2. Department facets
SELECT [Status], COUNT(*) FROM filtered GROUP BY ...;             -- 3. Status facets
SELECT ... FROM filtered ORDER BY ... OFFSET ... FETCH NEXT ...;  -- 4. Paginated data
```

**Example request:**

```
GET /api/v1/users?page=1&pageSize=10&searchTerm=john&orderBy=FullName&orderDirection=desc&departments=Engineering&departments=Marketing
```

**Example response:**

```json
{
  "items": [
    { "id": "...", "email": "john@example.com", "fullName": "John Smith", "department": "Engineering", "status": 1, "lastLoginAt": "..." }
  ],
  "metaData": {
    "currentPage": 1,
    "totalPages": 5,
    "pageSize": 10,
    "totalCount": 42,
    "hasNext": true,
    "hasPrevious": false
  },
  "aggregatedData": {
    "DepartmentTotals": [
      { "value": "Engineering", "count": 18 },
      { "value": "Marketing", "count": 12 }
    ],
    "StatusTotals": [
      { "value": 1, "count": 35 },
      { "value": 0, "count": 7 }
    ]
  }
}
```

#### H. API Versioning (The Contract)

**Packages:** `Asp.Versioning.Mvc`, `Asp.Versioning.Mvc.ApiExplorer`

URL path versioning is enforced across all endpoints: `/api/v1/resource`, `/api/v2/resource`, etc.

**Configuration** (`ServiceCollectionExtensions.cs`):
- `ReportApiVersions = true` — returns `api-supported-versions` header in every response
- `AssumeDefaultVersionWhenUnspecified = true` — falls back to v1.0 for backward compatibility
- `DefaultApiVersion = new ApiVersion(1, 0)`
- `GroupNameFormat = "'v'VVV"` — formats version groups as `v1`, `v2`
- `SubstituteApiVersionInUrl = true` — replaces `{version}` in route templates

**BaseController** (`Controllers/BaseController.cs`):

All versioned controllers inherit from `BaseController` which provides:
- Versioned route template: `[Route("api/v{version:apiVersion}/[controller]")]`
- Lazy-resolved `Mediator` property — no need to inject `IMediator` in every controller

**Versioned Swagger** (`Extensions/SwaggerExtensions.cs`):
- Dynamically generates a separate Swagger document per API version
- Swagger UI shows a dropdown to switch between v1, v2, etc.
- `SwaggerDefaultValues` operation filter fixes parameter display and marks deprecated endpoints

**Adding a new version:**

1. Create a `Controllers/V2/` folder
2. Add a controller inheriting from `BaseController`
3. Tag it with `[ApiVersion("2.0")]`

```csharp
[ApiVersion("2.0")]
public class UsersController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        // V2 implementation
    }
}
```

The new version automatically appears in the Swagger dropdown — no additional config needed.

#### I. Health Checks (The Heartbeat)

**Packages:** `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore`, `AspNetCore.HealthChecks.SqlServer`, `AspNetCore.HealthChecks.Redis`

A `/health` endpoint that verifies infrastructure connectivity and returns a JSON status report. Useful for container orchestrators (Kubernetes, Azure Container Apps), load balancers, and uptime monitors.

**Endpoint:** `GET /health`

**Checks registered:**

| Check | Package | Purpose |
| ----- | ------- | ------- |
| `database` | EF Core Health Checks | Verifies EF Core DbContext connectivity |
| `sqlserver` | AspNetCore.HealthChecks.SqlServer | Verifies raw SQL Server connectivity |
| `redis` | AspNetCore.HealthChecks.Redis | Verifies Redis connectivity (only when Redis is configured) |

**Example response:**

```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "database",
      "status": "Healthy",
      "description": null,
      "duration": "42ms"
    },
    {
      "name": "sqlserver",
      "status": "Healthy",
      "description": null,
      "duration": "15ms"
    },
    {
      "name": "redis",
      "status": "Healthy",
      "description": null,
      "duration": "3ms"
    }
  ]
}
```

Possible statuses: `Healthy`, `Degraded`, `Unhealthy`.

The health checks are registered in `Infrastructure/DependencyInjection.cs` and mapped in `Program.cs`. The database connection also uses `EnableRetryOnFailure()` for resilience during startup when the database container may still be initializing.

#### J. Enterprise CORS Policy (The Gatekeeper)

**Config:** `appsettings.json` → `AllowedOrigins`

CORS origins are read from configuration instead of being hardcoded, so each environment (dev, staging, production) can define its own allowed frontends.

```json
"AllowedOrigins": [
  "https://localhost:5173",
  "https://localhost:4173"
]
```

The named policy `AllowFrontend` allows any method, any header, and credentials. Add your production domain to the array when deploying.

#### K. FluentValidation Pipeline (The Gatekeeper)

**Packages:** `FluentValidation`, `FluentValidation.DependencyInjectionExtensions`

A robust validation system that automatically validates every MediatR request before it reaches your handler.

**How it works:**

```
HTTP Request → Controller → MediatR → ValidationBehaviour → Handler
                                         ↓ (if fails)
                            GlobalExceptionHandler → 400 Bad Request
```

1. `ValidationBehaviour` runs all registered `AbstractValidator<T>` classes for the current request
2. If any rules fail, it throws a custom `ValidationException` with a grouped error dictionary
3. `GlobalExceptionHandler` catches it and returns a clean 400 response

**Application layer files:**

| File | Purpose |
| ---- | ------- |
| `Common/Exceptions/ValidationException.cs` | Custom exception with `IDictionary<string, string[]> Errors` — groups failures by property name |
| `Common/Behaviours/ValidationBehaviour.cs` | MediatR pipeline that runs validators and throws on failure |
| `Features/Users/Commands/CreateUser/CreateUserCommand.cs` | Example MediatR command |
| `Features/Users/Commands/CreateUser/CreateUserCommandValidator.cs` | Example validator with `NotEmpty`, `MinimumLength`, `EmailAddress` rules |

**Adding validation to a new command:**

1. Create your command: `public record CreateProductCommand : IRequest<Guid> { ... }`
2. Create a validator in the same folder:

```csharp
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(4).WithMessage("Name must be at least 4 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}
```

That's it. The validator is automatically discovered by `AddValidatorsFromAssembly()` in `DependencyInjection.cs` and executed by the `ValidationBehaviour` pipeline — no manual wiring needed.

### Caching Architecture

The template includes a **cloud-agnostic distributed caching** strategy that works on **Azure** (Azure Cache for Redis), **AWS** (ElastiCache), or **On-Prem** with zero code changes.

**How it works:**

The `AddInfrastructure()` method in `Infrastructure/DependencyInjection.cs` reads the `ConnectionStrings:Redis` value at startup:

| Connection String | Registered Service | Best For |
| ----------------- | ------------------ | -------- |
| **Empty / not set** | `IDistributedCache` backed by **local server memory** (`AddDistributedMemoryCache`) | Internal apps, low-traffic services, local development |
| **Provided** | `IDistributedCache` backed by **Redis** (`AddStackExchangeRedisCache`) | High-traffic apps, multi-server / load-balanced deployments |

Both paths register `IDistributedCache`, so your application code always injects the same interface regardless of the backing store.

**Configuration:**

In `appsettings.json` the Redis connection string defaults to empty, which means local developers automatically get the in-memory fallback with no extra setup:

```json
"ConnectionStrings": {
  "DefaultConnection": "...",
  "Redis": ""
}
```

When deploying to an environment with Redis, set the connection string:

```
# Azure Cache for Redis
"Redis": "your-app.redis.cache.windows.net:6380,password=your-access-key,ssl=True,abortConnect=False"

# AWS ElastiCache for Redis
"Redis": "your-cluster.xxxxx.use1.cache.amazonaws.com:6379"
```

**Usage in your code:**

Inject `IDistributedCache` in any service or handler — it works the same whether backed by memory or Redis:

```csharp
public class MyService
{
    private readonly IDistributedCache _cache;

    public MyService(IDistributedCache cache) => _cache = cache;

    public async Task<string?> GetCachedValue(string key)
    {
        return await _cache.GetStringAsync(key);
    }
}
```

### Email System (MailKit + Hangfire)

A **cloud-agnostic email system** built on [MailKit](https://github.com/jstedfast/MailKit) with background job processing via Hangfire. It works with any SMTP relay: **Amazon SES**, **Azure Communication Services**, **Microsoft 365**, **SendGrid**, or any on-prem SMTP server.

**Architecture:**

```
Controller / Handler
        ↓
  IEmailManager.EnqueueSingleEmail(message)     ← returns immediately (Hangfire job ID)
        ↓
  Hangfire Background Job
        ↓
  ProcessSingleEmailJob → IEmailService.SendEmailAsync → MailKit SMTP
```

**Files:**

| File | Layer | Purpose |
| ---- | ----- | ------- |
| `Application/DTOs/EmailMessage.cs` | Application | DTO: `To`, `Subject`, `HtmlBody`, `Attachments` |
| `Application/DTOs/EmailAttachment.cs` | Application | DTO: `FileName`, `Content` (byte[]), `ContentType` |
| `Application/Interfaces/IEmailService.cs` | Application | Contract: `SendEmailAsync`, `SendBulkEmailAsync` |
| `Application/Interfaces/IEmailManager.cs` | Application | Contract: `EnqueueSingleEmail`, `EnqueueBulkEmail` |
| `Infrastructure/Messaging/MailSettings.cs` | Infrastructure | Config POCO: `Host`, `Port`, `Username`, `Password`, `SenderEmail`, `SenderName` |
| `Infrastructure/Messaging/MailKitEmailService.cs` | Infrastructure | MailKit implementation with single-send and persistent-connection bulk send |
| `Infrastructure/Messaging/EmailManager.cs` | Infrastructure | Wraps `IBackgroundJobClient.Enqueue` for fire-and-forget email dispatch |
| `Infrastructure/Messaging/Jobs/ProcessSingleEmailJob.cs` | Infrastructure | Hangfire job: sends one email |
| `Infrastructure/Messaging/Jobs/ProcessBulkEmailJob.cs` | Infrastructure | Hangfire job: sends bulk emails with optional summary report |
| `Infrastructure/Messaging/TemplateProcessor.cs` | Infrastructure | Reads HTML templates from `Templates/Email/`, injects `_Layout.html`, replaces `{{placeholders}}` |
| `Infrastructure/Templates/Email/_Layout.html` | Infrastructure | Shared email branding (header, footer, styles) |
| `Infrastructure/Templates/Email/Welcome.html` | Infrastructure | Example template with `{{UserName}}` placeholder |

**Two send paths:**

| Method | Behaviour |
| ------ | --------- |
| `SendEmailAsync` | Connects, authenticates, sends one email, disconnects. Best for event-triggered emails (welcome, password reset). |
| `SendBulkEmailAsync` | Opens a **persistent SMTP connection**, iterates all messages, disconnects once. Optionally sends a summary report (success/failure counts) to a `reportRecipient`. Best for newsletters and mass notifications. |

**Configuration (`appsettings.json`):**

```json
"MailSettings": {
  "Host": "email-smtp.us-east-1.amazonaws.com",
  "Port": 587,
  "Username": "",
  "Password": "",
  "SenderEmail": "noreply@yourdomain.com",
  "SenderName": "Your App Name"
}
```

**Local development (`appsettings.Development.json`):**

Configured for a local mock SMTP server (no authentication required):

```json
"MailSettings": {
  "Host": "localhost",
  "Port": 1025,
  "Username": "",
  "Password": "",
  "SenderEmail": "dev@localhost",
  "SenderName": "Dev App"
}
```

Run a local SMTP server to catch emails without sending them:

```bash
# MailHog (Go-based, includes web UI at http://localhost:8025)
docker run -d -p 1025:1025 -p 8025:8025 mailhog/mailhog

# OR smtp4dev (.NET-based, web UI at http://localhost:5000)
dotnet tool install -g Rnwood.Smtp4dev
smtp4dev
```

**Cloud provider examples:**

| Provider | Host | Port | Credentials |
| -------- | ---- | ---- | ----------- |
| **Amazon SES** | `email-smtp.{region}.amazonaws.com` | 587 | SMTP credentials generated in the SES console (**not** IAM access keys) |
| **Azure Communication Services** | `smtp.azurecomm.net` | 587 | Entra application with `Mail.Send` permission |
| **Microsoft 365** | `smtp.office365.com` | 587 | Licensed mailbox with SMTP AUTH enabled |
| **SendGrid** | `smtp.sendgrid.net` | 587 | `apikey` as username, API key as password |

> **Amazon SES Note:** You must generate **SMTP-specific credentials** from the SES console (SES > SMTP Settings > Create Credentials). Standard IAM access keys will not work for SMTP authentication.

**Retries:** All emails are dispatched via Hangfire background jobs. If the SMTP relay is temporarily unavailable, Hangfire automatically retries the job with exponential backoff (default: 10 attempts over ~26 hours).

**Usage example:**

```csharp
// Single email (fire-and-forget via Hangfire)
var html = TemplateProcessor.LoadTemplate("Welcome.html", new()
{
    { "UserName", "Alice" },
    { "AppName", "Contoso" },
    { "Year", DateTime.UtcNow.Year.ToString() }
});

_emailManager.EnqueueSingleEmail(new EmailMessage
{
    To = "alice@example.com",
    Subject = "Welcome to Contoso",
    HtmlBody = html
});

// Bulk email with summary report
_emailManager.EnqueueBulkEmail(messages, sendReport: true, reportRecipient: "admin@example.com");
```

## Local Development Setup

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (or Docker Engine with Compose)

### 1. Start Infrastructure Services

From the project root, spin up SQL Server, Redis, and MailHog:

```bash
docker-compose up -d
```

This starts the following containers:

| Service | Container | Port | Purpose |
| ------- | --------- | ---- | ------- |
| **SQL Server 2022** | `app-sqlserver` | `1433` | Application database (persistent volume) |
| **Redis** (alpine) | `app-redis` | `6379` | Distributed cache |
| **MailHog** | `app-mailhog` | `1025` (SMTP) / `8025` (Web UI) | Email testing — catches all outbound emails |

### 2. Verify Services

Check that all containers are running:

```bash
docker-compose ps
```

### 3. MailHog Email Testing

Open the MailHog Web UI in your browser:

```
http://localhost:8025
```

All emails sent by the application (via the MailKit email service) are intercepted by MailHog. No emails are delivered to real inboxes during development. Use this UI to inspect email content, HTML rendering, headers, and attachments.

### 4. Health Check Endpoint

Once the API is running, verify infrastructure connectivity:

```
GET https://localhost:{port}/health
```

The `/health` endpoint returns a JSON report showing the status of each infrastructure dependency:

```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "database",
      "status": "Healthy",
      "description": null,
      "duration": "42ms"
    },
    {
      "name": "sqlserver",
      "status": "Healthy",
      "description": null,
      "duration": "15ms"
    },
    {
      "name": "redis",
      "status": "Healthy",
      "description": null,
      "duration": "3ms"
    }
  ]
}
```

Possible statuses: `Healthy`, `Degraded`, `Unhealthy`.

### 5. Stopping Services

```bash
docker-compose down
```

To also remove the persistent SQL Server volume (deletes all data):

```bash
docker-compose down -v
```

## Uninstalling the Template

If installed from GitHub Packages:

```bash
dotnet new uninstall VueWebEnterpriseSQL.Template
```

If installed from a local clone:

```bash
dotnet new uninstall ./VueWebEnterpriseSQLTemplate
```

## Template Details

| Field       | Value                              |
| ----------- | ---------------------------------- |
| Identity    | `MyTemplate.Architecture.SqlServer`|
| Short Name  | `vwe-sql-app`                      |
| Source Name | `VueWebEnterpriseSQL`                  |
| Type        | Solution                           |
| Language    | C#                                 |

### What Gets Excluded

The template excludes the following from generated projects to keep them clean:

- `**/bin/**`
- `**/obj/**`
- `**/.vs/**`
- `MyTemplatePack.csproj`
- `README.md`

### Post Actions

After project creation, `dotnet restore` runs automatically to restore all NuGet packages.
