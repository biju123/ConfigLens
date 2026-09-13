# ConfigLens Architecture

## Backend layering

```
ConfigLens.Api            (controllers, auth middleware, error handling, DI wiring)
        |
        +--> ConfigLens.Application   (scan services, validation engine, comparers, auth)
        |         |
        +--> ConfigLens.Infrastructure (Domain port implementations, backed by SampleData)
                  |
                  +--> ConfigLens.SampleData (deterministic JSON fixtures + POCOs, no deps)

ConfigLens.Domain (Scan, Configuration, Findings, Comparison, Reference — no project references)
```

`Application` and `Infrastructure` both depend only on `Domain`, not on each
other. `Domain` defines the ports (`IKubernetesInventoryReader`,
`IApplicationConfigurationClient`, `IRuleSetProvider`,
`IDependencyAccessibilityChecker`, `IReferenceDataProvider`, `IScanRepository`)
that `Infrastructure` implements (currently backed by `SampleData`; a real
Azure/Kubernetes SDK integration later replaces only these implementations)
and that `Application`'s scan services consume via constructor injection.
`Api` is the only project that references both `Application` and
`Infrastructure`, wiring concrete implementations to interfaces in
`Program.cs`.

This differs slightly from a first sketch that placed these interfaces under
`Infrastructure/`: since `Application` must depend on the interfaces but not
on `Infrastructure` itself, the ports live in `Domain` instead — the standard
"core defines the port, edges implement it" shape.

## Domain model

- **Scan** (`Domain/Scan`): `ScanId` (`SCAN-yyyyMMdd-NNNNN`), `ScanCategory`,
  `ScanStatus`, `ScanMetadata`, `ScanRecord` (`Metadata` + `IScanResult`).
  Each category has its own `IScanResult` implementation
  (`AksScanResult`, `AppConfigScanResult`, `CharacteristicsScanResult`,
  `DependencyScanResult`) sharing only `ScanMetadata` — no generic result
  shape.
- **Configuration** (`Domain/Configuration`): `ConfigItem`/`ConfigSection`/
  `ConfigTree` (the hierarchical tree), `ValidationRule`/`RuleSet` (pure data,
  loaded from `SampleData/Data/rule-sets/*.json` — rules never live in
  frontend code).
- **Findings** (`Domain/Findings`): the 13-field `Finding` record, used only
  by the Characteristics category.
- **Comparison** (`Domain/Comparison`): four independent result records
  (`AksComparisonResult`, `AppConfigComparisonResult`,
  `CharacteristicsComparisonResult`, `DependencyComparisonResult`) plus
  `CategoryMismatchException`.

## Validation engine

`IValidationEngine` dispatches each `ValidationRule` to the `IRuleEvaluator`
registered for its `RuleType` (strategy pattern — `RequiredExists`,
`NotEmpty`, `FormatMatch`, `AllowedValues`, `NumericRange`,
`SessionYearExists`, `CrossApplicationConsistency`,
`CrossEnvironmentConsistency`). Evaluators never see raw sensitive values in
the findings they emit — `RuleEvaluationContext.DisplayValue` masks any
value from a sensitive-looking key before it's embedded in Finding text.

## Comparison

`IComparisonService` looks up both scans, throws `ScanNotFoundException`
(404) or `CategoryMismatchException` (400) as appropriate, then dispatches to
one of four independent static comparers (`AksComparer`, `AppConfigComparer`,
`CharacteristicsComparer`, `DependencyComparer`) — there is no shared generic
diff algorithm. The HTTP response is a thin envelope
(`{currentScanId, baselineScanId, category, result}`) where `result` is
whichever category-specific shape applies.

## Per-category API

| Category | Endpoint | Request | Result |
|---|---|---|---|
| AKS Deployment | `POST /api/scans/aks-deployment` | subscription, cluster, environment, tenant, namespace? | `AksResource[]` |
| Application Configuration | `POST /api/scans/application-configuration` | application, environment, tenant, namespace?, sessionYear | masked `ConfigTree` |
| Configuration Characteristics | `POST /api/scans/configuration-characteristics` | application, environment, tenant, sessionYear, ruleSetId | `Finding[]` + summary counts |
| Dependency Accessibility | `POST /api/scans/dependency-accessibility` | application, environment, namespace, dependencyType? | `DependencyCheckResult[]` |

Common: `GET /api/scans` (history, optional `category` filter),
`GET /api/scans/{scanId}`, `POST /api/comparisons`, `GET /api/reference-data`,
`POST /api/auth/login`, `GET /api/health`.

## Error envelope

RFC7807 `ProblemDetails` via a central `IExceptionHandler`
(`ConfigLensExceptionHandler`) mapping to a stable `type`: `invalid-input`
(400 — includes FluentValidation failures and unknown rule sets),
`not-found` (404 — unknown scan ID), `category-mismatch` (400),
`authentication-failure` (401, including JWT challenge failures),
`authorization-failure` (403), `application-failure` (500, detail
suppressed). **Dependency-inaccessible, timeout, and rule-fail outcomes are
not HTTP errors** — they're legitimate scan results returned as `200 OK`
with a status field in the body (`DependencyCheckResult.Status`,
`Finding.Status`). The frontend's `AppError.kind` mirrors this: the first 5
kinds come from real HTTP errors; `dependency-failure`, `timeout`, and
`validation-failure` are constructed client-side from body content when
needed.

Validation itself runs through `ValidateRequestFilter`, an MVC action filter
that resolves `IValidator<T>` for each action argument — not the framework's
built-in implicit-required-for-non-nullable-reference-types behavior, which
is explicitly suppressed (`ApiBehaviorOptions.SuppressModelStateInvalidFilter`)
so FluentValidation is the single source of truth for request validation.

## Frontend

Vite + React 19 + TypeScript + Redux Toolkit (used only for the two pieces
of genuinely shared state: `auth` and `scanSelection` — everything else is
local component state or fetched per-page). Dark, Grafana-inspired styling
lives in `src/styles/{theme.css,global.css}` as CSS custom properties plus a
handful of reusable utility classes (`.card`, `.btn`, `.data-table`,
`.badge`, …); a few structurally unique components (`AppShell`) use a CSS
Module instead.

Each category has its own `*ResultsView` (presentational, used both right
after running a scan and when viewing a historical scan by ID) and its own
`*ComparisonView`. `ScanDetailPage` and `CompareResultPage` are the two
route-level components that fetch by scan ID and dispatch to the correct
category-specific view based on `result.category` / `response.category` —
there is no generic result or diff renderer.

The runtime API base URL is read from `window.__CONFIGLENS_API_BASE__`,
written by `docker-entrypoint.sh` into `env-config.js` at container start
(falling back to the Vite build-time `VITE_API_BASE_URL` for local `npm run
dev`) — the same built image can point at a different backend without a
rebuild.
