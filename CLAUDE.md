# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project status

This repository currently contains only `Agents.md`, the product/architecture
specification. No backend, frontend, Docker setup, or scripts have been
implemented yet. There are no build/lint/test commands to run because there is
no code — when implementation begins, this file should be updated with the
actual commands (dotnet/npm scripts, docker compose invocations, test
runners, etc.).

`Agents.md` is the authoritative spec for this project. Read it in full before
implementing anything — the summary below is not a substitute. If `Agents.md`
and this file ever disagree, `Agents.md` wins; update this file to match.

## What ConfigLens is

A **read-only** inspection and validation tool for hosted deployment
infrastructure and application configuration, initially targeting "Assessor"
projects. It must never modify Kubernetes resources, Azure resources, app
config, secrets, databases, Service Bus, storage, Key Vault, or deployments.
Automatic remediation is explicitly out of scope.

## Core model: three layers feeding into Findings

```
Infrastructure -> Configuration -> Validation -> Findings
```

- **Infrastructure**: what's deployed (AKS clusters, namespaces, deployments,
  pods, CronJobs, HPA, KEDA, services, CPU/memory, replicas).
- **Configuration**: what the app is configured with (DB, Service Bus,
  Storage, Key Vault, app settings, environment/session-year config).
- **Validation**: whether configuration is correct/working (required values
  present, valid format/range, dependency reachable, cross-env consistency).

A **Scan** collects data, a **Validation** evaluates it against a rule, and a
**Finding** is the outcome of a validation. Keep these three concepts
distinct in code — don't conflate scanning with validating.

## Four scan categories — each is independently implemented

1. AKS Deployment Scan
2. Application Configuration Scan
3. Configuration Characteristics Scan
4. Dependency Accessibility Scan

**Do not force these into one generic model.** Each category gets its own API
endpoints, request/response models, result structure, UI, drill-down, and
comparison logic. Common metadata may be shared, but resist the urge to
unify them into a single generic scan/result/diff abstraction — the spec
explicitly calls this out multiple times (sections 3, 14, 15, 17).

Conceptual scan pipeline:

```
{AKS,Configuration,Characteristics,Dependency} Scanner -> Scan Services -> Validation Engine -> Results
```

## Comparison is explicit, never automatic

A scan never auto-compares itself to a previous scan. The user explicitly
picks a current scan and a baseline, then triggers `Compare`. Only
same-category scans may be compared (AKS-with-AKS, etc.), and each category
implements its own diff logic (added/removed/changed config; new/resolved
findings; AKS resource/replica/image diffs). No single generic diff
algorithm.

## Backend architecture (ASP.NET Core Web API)

```
backend/
  API              (controllers — no business logic here)
  Application      (Scans, Validation, Comparison, Services)
  Domain           (Scan, Configuration, Finding, Comparison)
  Infrastructure   (Azure, Kubernetes, Application APIs)
  SampleData
```

- Keep business logic out of controllers.
- Keep infrastructure SDKs out of the domain layer.
- Application config scanning must go through a controlled inspection API
  (approved sections only, sensitive values masked) — never expose raw
  `appsettings.json`.
- Validation rules live in backend/domain config, never inside React
  components.
- For the MVP, backend returns deterministic dummy JSON (no randomness);
  sample data must be kept separate from business logic.

## Frontend

- ReactJS; use Redux only where shared state genuinely requires it.
- A frontend MVP is expected to live under `frontend/` once created — before
  changing it, inspect existing routes/components/state management and
  preserve working functionality rather than rewriting it.
- Visual direction: dark/black background, Grafana-style operational UI,
  information-dense, tables for infra data, tree views for config, minimal
  decoration.
- Each scan category gets a UI suited to its result shape (operational
  tables for AKS, tree+search for config, pass/fail/severity summaries for
  characteristics, health/status view for dependencies) — not one generic
  table for everything.

## Security constraints (non-negotiable)

Never expose secrets, Kubernetes Secret values, passwords, tokens, API keys,
credentials, or connection strings containing credentials — mask them.
Never log sensitive values. Never store Azure credentials in source or
Docker images. Never expose backend credentials to React.

Auth for the MVP: hardcoded local dev account (`user`/`password`)
implemented in the backend, never embedded in React source. Must be
designed to be swapped for OAuth2/OIDC later.

## Docker & scripts

Local execution is via `docker compose up`. Config comes from environment
variables, never hardcoded into images. `start`/`stop` scripts belong under
`scripts/` and should support Windows/macOS/Linux, delegating to Docker
Compose where possible.

## Documentation location

Project docs (PLAN.md, ARCHITECTURE.md, etc.) belong under `docs/`. Read
`docs/PLAN.md` and `docs/ARCHITECTURE.md` before implementation if they
exist.

## Development principles worth repeating

- Do not build one generic model/diff/UI across scan categories — this is
  the most-repeated constraint in the spec.
- No unnecessary abstractions for hypothetical future requirements.
- Synchronous scans are fine for the MVP, but design so background/async
  scanning can be added later without a rewrite.
- Identify root cause before fixing; don't guess when evidence is available.
