# ConfigLens MVP — Plan

## Scope

This build delivers the full MVP described in `CLAUDE.md` in one pass: all
four scan categories, explicit comparison, authentication, Docker Compose,
and start/stop scripts. It does not implement anything listed under
"Explicit Non-Goals" (CLAUDE.md section 31).

## The four scan categories

1. **AKS Deployment Scan** — inspects Kubernetes/AKS inventory (Deployments,
   Pods, CronJobs, Jobs, Services, HPA, KEDA ScaledObjects) from deterministic
   sample data. UI groups by namespace → application/service → resource type.
2. **Application Configuration Scan** — returns a masked, hierarchical
   configuration tree for an application/environment/tenant/session year via
   a controlled "configuration-inspection API" stand-in. Sensitive values are
   masked before they ever leave the backend.
3. **Configuration Characteristics Scan** — evaluates a named rule set
   (`default` or `strict`) against an application's real (unmasked,
   server-side only) configuration, producing PASS/WARNING/FAIL/NOT_CHECKED/
   NOT_APPLICABLE/ERROR findings.
4. **Dependency Accessibility Scan** — reports accessibility status for
   Azure SQL, Table/Blob Storage, Service Bus, Key Vault, and an external API,
   from deterministic sample scenarios covering every `DependencyStatus`.

## Comparison

Comparison is a separate, explicit user action (never automatic). The user
picks a current scan and a baseline scan of the *same* category from scan
history; the backend rejects category-mismatched or unknown scan IDs. Each
category has its own comparer and its own comparison UI — there is no shared
generic diff.

## What "done" means (CLAUDE.md section 30)

- `docker compose up` (via `scripts/start`) runs the full stack locally.
- Sign-in works with the hardcoded MVP account, implemented server-side only.
- Users select one or more scan categories; only the relevant parameter
  fields are shown.
- Each category has its own API endpoint, sample data, result UI, and
  drill-down.
- Sensitive configuration values are masked everywhere, including in scan
  history and comparison output.
- Every completed scan gets a unique `SCAN-yyyyMMdd-NNNNN` ID and can be
  looked up later; comparison is an explicit, separate step.
- Errors are surfaced with a category (invalid input / auth / authz /
  infrastructure / application / dependency / timeout / validation), never
  swallowed silently.
- No Kubernetes, Azure, or application resource is ever modified.

## Non-goals

Everything listed in CLAUDE.md section 31 (infrastructure modification,
secret rotation, production identity, full RBAC, notifications, scheduling,
AI recommendations, etc.) is explicitly out of scope for this build.

See `docs/ARCHITECTURE.md` for the layering, domain model, and API shape.
