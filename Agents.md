# ConfigLens

Deployment Configuration Inspection and Validation Tool for Assessor Projects.

ConfigLens is a read-only inspection and validation tool for hosted deployment
infrastructure and application configuration.

It provides visibility into:

1. What is deployed
2. What is configured
3. Whether the configuration is correct and working

The initial target is Assessor projects, but the architecture should allow
support for additional applications and products in the future.

---

# 1. Purpose

ConfigLens is intended to identify configuration and deployment issues before
or during deployment by inspecting:

* AKS infrastructure and deployment configuration
* Application configuration
* Configuration characteristics
* Application dependency accessibility
* Differences between previous and current scans

ConfigLens is an inspection and validation tool.

It MUST NOT modify:

* Kubernetes resources
* Azure resources
* Application configuration
* Secrets
* Databases
* Service Bus
* Storage
* Key Vault
* Application deployments

Automatic remediation is explicitly outside the scope of the MVP.

---

# 2. Core Product Model

ConfigLens is organized around three primary layers.

## Layer 1 — Infrastructure

What is actually deployed?

Examples:

* AKS clusters
* Namespaces
* Applications
* Deployments
* Pods
* CronJobs
* HPA
* KEDA
* Services
* CPU/memory configuration
* Replica configuration

## Layer 2 — Configuration

What configuration is the application running with?

Examples:

* Database configuration
* Service Bus configuration
* Storage configuration
* Key Vault configuration
* Application settings
* Environment-specific configuration
* Session-year-specific configuration

## Layer 3 — Validation

Is the configuration correct and working?

Examples:

* Required configuration exists
* Configuration value is valid
* Configuration is within expected range
* Dependency is accessible
* Configuration matches expected characteristics
* Configuration is consistent between applications/environments

Conceptually:

```
Infrastructure
      |
      v
Configuration
      |
      v
   Validation
      |
      v
   Findings
```

---

# 3. Scan Categories

The MVP should support the following scan categories:

1. AKS Deployment Scan
2. Application Configuration Scan
3. Configuration Characteristics Scan
4. Dependency Accessibility Scan

Each category has its own:

* API endpoints
* Request model
* Response model
* Result structure
* Validation requirements
* UI
* Drill-down experience
* Comparison logic

Do NOT force all scan categories into one generic result model.

Common metadata may be shared where appropriate.

---

# 4. Scan vs Validation vs Finding

These concepts must remain distinct.

## Scan

A scan collects information.

Examples:

* AKS resource scan
* Application configuration scan
* Dependency scan

## Validation

A validation evaluates collected information against a rule or requirement.

## Finding

A finding represents the outcome of a validation.

A finding may contain:

* Finding ID
* Scan ID
* Category
* Application
* Environment
* Namespace
* Resource
* Rule ID
* Rule name
* Severity
* Status
* Description
* Expected condition
* Actual condition
* Recommendation

Recommended statuses:

* PASS
* WARNING
* FAIL
* NOT_CHECKED
* NOT_APPLICABLE
* ERROR

Recommended severities:

* INFO
* LOW
* MEDIUM
* HIGH
* CRITICAL

---

# 5. Authentication

The user must be able to sign in.

## MVP

Use a hardcoded local development account:

Username:

```
user
```

Password:

```
password
```

Authentication must be implemented by the backend.

The password MUST NOT be embedded in React source code.

The authentication implementation should be replaceable later with enterprise
authentication such as OAuth2/OpenID Connect.

The MVP credentials are for local development only and must never be treated
as production authentication.

---

# 6. Scan Selection

After signing in, the user should see the available scan categories.

Example:

```
[ ] AKS Deployment Scan
[ ] Application Configuration Scan
[ ] Configuration Characteristics Scan
[ ] Dependency Accessibility Scan
```

The user can select one or more scan categories.

The UI should only request parameters relevant to the selected scan categories.

---

# 7. Scan Parameters

Different scan categories may require different parameters.

## AKS Deployment Scan

Potential parameters:

* Azure subscription
* AKS cluster
* Namespace
* Environment
* Customer/Tenant

## Application Configuration Scan

Potential parameters:

* Application
* Environment
* Customer/Tenant
* Namespace
* Session year

## Configuration Characteristics Scan

Potential parameters:

* Application
* Environment
* Customer/Tenant
* Session year
* Rule set

## Dependency Accessibility Scan

Potential parameters:

* Application
* Environment
* Namespace
* Dependency type

The UI must not display unnecessary parameters for a scan category.

---

# 8. AKS Deployment Scan

The AKS scan inspects Kubernetes deployment information.

The MVP should support sample data for:

* Deployments
* Pods
* CronJobs
* Jobs
* Services
* HPA
* KEDA ScaledObjects

Where useful, display:

* Namespace
* Application/service
* Resource type
* Resource name
* Desired replicas
* Running replicas
* Ready replicas
* CPU request
* CPU limit
* Memory request
* Memory limit
* Container count
* Image/version
* Pod status
* Restart count

The UI should group information by:

1. Namespace
2. Application/service
3. Resource type

The user should be able to drill down into individual resources.

---

# 9. Application Configuration Scan

The application configuration scan inspects configuration used by an
application.

The production architecture should NOT simply download and expose a raw
`appsettings.json`.

Applications should expose a controlled configuration-inspection API.

The endpoint should:

* Return approved configuration sections
* Mask sensitive values
* Avoid returning secrets
* Support authentication/authorization
* Return relevant environment/application metadata

For the MVP, the backend can return deterministic dummy JSON.

Configuration should be displayed as a hierarchical tree.

Example:

```
Assessor
  |
  +-- Database
  |    +-- ConnectionString
  |    +-- CommandTimeout
  |    +-- RetryCount
  |
  +-- ServiceBus
  |    +-- Namespace
  |    +-- Queue
  |
  +-- Storage
       +-- Account
       +-- Table
```

Sensitive values MUST be masked.

---

# 10. Configuration Characteristics Scan

This scan validates application configuration against defined rules.

Examples:

* Required configuration exists
* Required configuration is not empty
* Value follows expected format
* Value belongs to an allowed set
* Numeric value is within a defined range
* Configuration exists for a session year
* Configuration is consistent across applications
* Configuration is consistent across environments

Validation rules MUST NOT be implemented directly inside React components.

Rules should be represented as backend/domain configuration.

---

# 11. Dependency Accessibility Scan

This scan validates that configured application dependencies are actually
accessible from the application's execution environment.

Potential dependencies:

* Azure SQL
* Azure Table Storage
* Azure Blob Storage
* Azure Service Bus
* Azure Key Vault
* External APIs

Results should distinguish between:

* Configuration missing
* Configuration invalid
* Dependency inaccessible
* Dependency accessible
* Timeout
* Validation error
* Not applicable

Where possible, accessibility should be tested from the same network/runtime
context as the application.

The fact that ConfigLens can access a dependency does NOT prove that the
application pod can access it.

---

# 12. Scan History

Every completed scan must have a unique Scan ID.

Example:

```
SCAN-20260912-00124
```

The system should retain scan metadata sufficient to allow the user to
identify previous scans.

Examples:

* Scan ID
* Scan category
* Date/time
* Environment
* Customer/Tenant
* Application
* Cluster
* Namespace
* Session year
* Status
* Initiating user

Persistent scan history is not required for the MVP, but the API and domain
model should support it.

---

# 13. Comparison

Comparison is a separate user action.

A scan MUST NOT automatically compare itself with a previous scan.

The user explicitly selects:

1. Current scan
2. Previous/baseline scan

Then selects:

```
Compare
```

The comparison operation creates a separate comparison result.

Conceptually:

```
Scan
  |
  +----> View Results
  |
  +----> Compare
            |
            v
      Select Baseline
            |
            v
        Comparison
            |
            v
   Category-specific Diff
```

Only compatible scans should be compared.

Examples:

* AKS scan with AKS scan
* Configuration scan with Configuration scan
* Characteristics scan with Characteristics scan
* Dependency scan with Dependency scan

Compatibility rules should be defined per category.

---

# 14. Category-Specific Comparison

Comparison logic must understand the scan category.

## AKS

Compare:

* Resources
* Replica counts
* CPU
* Memory
* Images
* CronJobs
* HPA
* KEDA
* Services

## Application Configuration

Compare:

* Added configuration
* Removed configuration
* Changed configuration
* Unchanged configuration

## Configuration Characteristics

Compare:

* New failures
* Resolved failures
* New warnings
* Resolved warnings
* Status changes
* Rule changes

## Dependency Accessibility

Compare:

* Dependency status
* Accessibility changes
* New failures
* Resolved failures

Do not implement one generic diff algorithm for all categories.

---

# 15. Category-Specific UI

Each scan category should have a UI optimized for its result structure.

## AKS

Use:

* Operational tables
* Resource hierarchy
* Namespace grouping
* Application grouping
* CPU/memory columns
* Replica/status indicators

## Application Configuration

Use:

* Hierarchical configuration tree
* Search
* Expand/collapse
* Masked values
* Configuration metadata

## Configuration Characteristics

Use:

* Validation summary
* Pass/fail/warning counts
* Rule grouping
* Severity
* Expected vs actual
* Finding drill-down

## Dependency Accessibility

Use:

* Dependency health/status view
* Application grouping
* Accessibility status
* Failure reason
* Dependency drill-down

## Comparison

Use a category-specific comparison view.

Do NOT force every result into one generic table.

---

# 16. Backend Architecture

Use:

* ASP.NET Core Web API
* ReactJS
* Redux only where shared state genuinely requires it
* REST APIs

Conceptual backend structure:

```
backend/
  |
  +-- API
  |
  +-- Application
  |     +-- Scans
  |     +-- Validation
  |     +-- Comparison
  |     +-- Services
  |
  +-- Domain
  |     +-- Scan
  |     +-- Configuration
  |     +-- Finding
  |     +-- Comparison
  |
  +-- Infrastructure
  |     +-- Azure
  |     +-- Kubernetes
  |     +-- Application APIs
  |
  +-- SampleData
```

Keep business logic out of controllers.

Keep infrastructure SDKs out of the domain layer.

---

# 17. Scan Architecture

Each scan category should be independently implemented.

Conceptually:

```
AKS Scanner
Configuration Scanner
Characteristics Scanner
Dependency Scanner
        |
        v
   Scan Services
        |
        v
 Validation Engine
        |
        v
      Results
```

Common infrastructure may be reused internally.

Do not create unnecessary abstractions solely for theoretical future
requirements.

---

# 18. Sample Data

The MVP uses deterministic dummy JSON data.

Sample data should represent realistic scenarios.

Include:

* Multiple namespaces
* Multiple applications
* Multiple pods
* Multiple replicas
* CronJobs
* CPU/memory configurations
* Application configuration
* Passing validations
* Warnings
* Failures
* Missing configuration
* Invalid configuration
* Dependency failures

Do not use random sample values.

Sample data should be separated from business logic.

---

# 19. Security

The tool is read-only.

It MUST:

* Never expose secrets
* Never return Kubernetes Secret values
* Mask passwords
* Mask tokens
* Mask API keys
* Mask credentials
* Mask connection strings containing credentials
* Never log sensitive values
* Never store Azure credentials in source code
* Never put credentials into Docker images
* Never expose backend credentials to React

Real integrations must use appropriate authentication mechanisms.

---

# 20. Docker

The MVP should run locally using Docker.

Prefer:

```
docker compose up
```

for multi-container local execution.

Configuration should be supplied using environment variables.

Do not hardcode environment-specific configuration into Docker images.

---

# 21. Scripts

Scripts should exist under:

```
scripts/
```

Provide:

* start
* stop

Support:

* Windows
* macOS
* Linux

Scripts should preferably delegate to Docker Compose.

---

# 22. Frontend

Visual direction:

* Black/dark background
* Grafana-inspired operational UI
* Information-dense
* Clear status indicators
* Tables for infrastructure data
* Tree views for configuration
* Minimal decoration

Prioritize operational usability.

Do not introduce unnecessary UI complexity.

---

# 23. Error Handling

The UI must clearly distinguish:

* Invalid input
* Authentication failure
* Authorization failure
* Infrastructure failure
* Application failure
* Dependency failure
* Timeout
* Validation failure

Never silently ignore errors.

---

# 24. Logging

Backend logging should be structured.

Useful fields:

* Scan ID
* Scan category
* Application
* Environment
* Namespace
* Resource
* Operation
* Duration
* Result

Never log sensitive values.

---

# 25. Performance

The eventual tool may scan a large number of resources.

Avoid:

* One backend request per table row
* Loading unnecessary configuration
* Large unnecessary payloads
* Blocking API requests for long-running scans

Synchronous scans are acceptable for the MVP.

The architecture should allow background/asynchronous scans later.

---

# 26. Testing

Backend tests should cover:

* Scan orchestration
* Validation rules
* Configuration parsing
* Finding generation
* Comparison logic
* API validation
* Error handling

Frontend tests should cover critical user flows.

Minimum flows:

1. Sign in
2. Select scan category
3. Enter scan parameters
4. Start scan
5. View results
6. Filter results
7. Drill down
8. View configuration
9. View findings
10. Select previous scan
11. Compare scans
12. View category-specific comparison

---

# 27. Development Principles

1. Use current stable versions of libraries.
2. Use idiomatic approaches.
3. Keep the solution simple.
4. Do not over-engineer.
5. Do not add features outside scope.
6. Keep business logic out of React components.
7. Keep infrastructure integrations isolated.
8. Prefer deterministic sample data.
9. Make failures visible.
10. Never expose secrets.
11. Identify root cause before fixing issues.
12. Do not guess when evidence can be obtained.
13. Prefer simple explicit code.
14. Avoid unnecessary defensive programming.
15. Reuse existing frontend functionality where appropriate.
16. Do not rewrite working functionality without a reason.

---

# 28. Existing Frontend

A frontend MVP already exists under:

```
frontend/
```

Before changing it:

1. Inspect the implementation.
2. Understand existing routes.
3. Identify reusable components.
4. Identify existing state management.
5. Preserve useful functionality.
6. Integrate it with the backend.

Do not rewrite the frontend simply because a backend is being introduced.

---

# 29. Working Documentation

All project documentation belongs under:

```
docs/
```

Before implementation:

1. Read `docs/PLAN.md`
2. Read `docs/ARCHITECTURE.md`
3. Review the existing frontend
4. Understand the API design
5. Understand the scan model
6. Understand validation rules

Documentation should remain concise and implementation-focused.

---

# 30. Definition of Done

The MVP is complete when:

* Application runs locally using Docker
* User can sign in
* User can select scan categories
* User can enter scan parameters
* Backend exposes category-specific APIs
* Dummy scan data is available
* AKS results can be displayed
* Application configuration can be displayed
* Configuration characteristics can be displayed
* Dependency results can be displayed
* Each category has an appropriate UI
* Results can be filtered
* Users can drill down
* Sensitive values are masked
* Scan IDs are generated
* Previous scans can be selected
* User can explicitly compare compatible scans
* Comparison results are category-specific
* Errors are displayed clearly
* Start/stop scripts exist
* No infrastructure is modified
* No secrets are exposed

---

# 31. Explicit Non-Goals

Do not implement unless explicitly requested:

* Infrastructure modification
* Kubernetes deployment
* Configuration deployment
* Automatic remediation
* Secret rotation
* Azure resource creation/deletion
* Production identity integration
* Full RBAC
* Notifications
* Email
* Scheduling
* AI recommendations
* Unrelated dashboards
* Complex distributed processing
* Full production persistence
