# ConfigLens Frontend — Developer Guide

This document explains how the frontend is put together: how components
compose, what lives in Redux versus local state, and — end to end — how a
user action turns into an API call. It does not enumerate every component;
it picks one representative flow through Redux and one representative flow
that stays entirely local, since the same two patterns repeat across the
rest of the app.

Stack: React 19 + TypeScript, Vite, React Router v7, Redux Toolkit.

## 1. Project layout

```
frontend/src/
├── app/                  Redux store, typed hooks, router, route guard
│   ├── store.ts
│   ├── hooks.ts
│   ├── router.tsx
│   └── ProtectedRoute.tsx
├── api/                  Everything that talks to the backend
│   ├── httpClient.ts     fetch wrapper: base URL, auth header, error shape
│   ├── errors.ts         AppError type + ProblemDetails → AppError mapping
│   ├── types.ts          TS types mirroring backend DTOs
│   ├── authApi.ts, scanApi.ts, comparisonApi.ts, referenceDataApi.ts
├── features/             One folder per feature/domain area
│   ├── auth/              LoginPage, LoginForm, authSlice        (Redux)
│   ├── scanSelection/     ScanCategorySelector, scanSelectionSlice (Redux)
│   ├── scanParameters/    ScanParametersForm + per-category field groups
│   ├── newScan/           NewScanPage (orchestrates a scan run)
│   ├── aks/ appConfig/ characteristics/ dependency/
│   │                      per-category *ResultsView, *ComparisonView,
│   │                      drill-down drawers/tables
│   ├── scanDetail/        ScanDetailPage + useScanRecord (fetch by ID)
│   ├── history/           ScanHistoryPage, ComparisonSetupPage, CompareResultPage
│   └── referenceData/     useReferenceData (dropdown source data)
├── components/common/     Presentational primitives: AppShell, StatusBadge,
│                          SeverityBadge, ErrorBanner, LoadingSpinner
└── styles/                theme.css (tokens), global.css (utility classes)
```

Two organizing rules that explain most of the folder structure:

- **Feature-first, not type-first.** Everything about "characteristics
  results" lives in `features/characteristics/`, not spread across a global
  `components/` and `pages/` split.
- **Redux only where state is genuinely shared.** Two slices exist:
  `auth` and `scanSelection`. Everything else — form inputs, fetched data,
  loading/error flags, which drawer is open — is local `useState` in the
  component that owns it. Section 3 explains why.

## 2. Component composition

Every protected page is composed the same way:

```
<Provider store={store}>              main.tsx
  <RouterProvider router={router}>    main.tsx
    <ProtectedRoute>                  app/router.tsx wraps each protected route
      <AppShell>                      redirects to /login if no token
        <NewScanPage /> | <ScanHistoryPage /> | ...   nav bar + page body
      </AppShell>
    </ProtectedRoute>
  </RouterProvider>
</Provider>
```

`ProtectedRoute` (`app/ProtectedRoute.tsx`) reads `state.auth.token` and
either renders `<Navigate to="/login" />` or wraps `children` in `AppShell`
(`components/common/AppShell.tsx`), which renders the top nav and a
`Sign out` button. `/login` itself is the one route outside this wrapper.

### Example composition tree: New Scan page

```
NewScanPage                                   features/newScan/NewScanPage.tsx
├── ScanCategorySelector                      features/scanSelection/
│    └── <input type="checkbox"> × 4          (reads/writes Redux scanSelection)
├── ScanParametersForm                        features/scanParameters/
│    ├── AksParamsFields                      (rendered only if AksDeployment selected)
│    ├── AppConfigParamsFields                (rendered only if ApplicationConfiguration selected)
│    ├── CharacteristicsParamsFields          (rendered only if ConfigurationCharacteristics selected)
│    └── DependencyParamsFields               (rendered only if DependencyAccessibility selected)
└── ResultCard × N (one per category that ran)
     ├── AksResultsView
     │    ├── AksNamespaceGroup × N
     │    │    └── AksResourceTable
     │    └── AksResourceDrawer                (shown when a row is selected)
     ├── AppConfigResultsView
     │    └── ConfigTree → ConfigSearchBox, ConfigTreeNode (recursive)
     ├── CharacteristicsResultsView
     │    ├── ValidationSummary
     │    ├── FindingsTable
     │    └── FindingDetailDrawer              (shown when a row is selected)
     └── DependencyResultsView
          ├── DependencyHealthGrid
          └── DependencyDetailDrawer           (shown when a card is selected)
```

`ScanCategorySelector` only touches Redux; `ScanParametersForm` and every
`*ResultsView` below it only touch local state and props. `NewScanPage`
itself is the bridge: it reads Redux (`selected` categories) to decide which
API calls to make, but stores the results of those calls locally.

### Example composition tree: Configuration Characteristics comparison

```
CompareResultPage                             features/history/CompareResultPage.tsx
  (fetches ComparisonResponse by useParams scan IDs, switches on response.category)
└── CharacteristicsComparisonView              features/characteristics/
     ├── FindingList (new failures)
     ├── FindingList (resolved failures)
     ├── FindingList (new warnings)
     ├── FindingList (resolved warnings)
     ├── status-changes table (StatusBadge per row)
     └── rule-changes list (StatusBadge per row)
```

Every category has this same shape: one route-level component
(`ScanDetailPage` / `CompareResultPage`) that fetches by ID and picks a
component based on `category`, handing off to a purely presentational
`*ResultsView` / `*ComparisonView` that takes typed data as props and knows
nothing about routing or fetching.

## 3. State management model

| State | Where it lives | Why |
|---|---|---|
| `token`, `username`, login status/error | Redux (`features/auth/authSlice.ts`) | Read by `ProtectedRoute`, `AppShell`, and `httpClient` (indirectly, via `sessionStorage`) — genuinely global. |
| `selectedCategories` | Redux (`features/scanSelection/scanSelectionSlice.ts`) | Written by `ScanCategorySelector`, read by both `ScanParametersForm` (which fields to show) and `NewScanPage` (which scans to run) — two unrelated components need the same value. |
| Form field values (params, login inputs, search terms) | `useState` in the owning component | Nobody outside that component/its direct children needs them. |
| Fetched data + loading/error flags | `useState`, populated by a `useEffect` or an event handler that calls the `api/*` module directly | Each page fetches what it needs when it needs it; there's no cross-page cache to justify Redux. |
| "Which row/card is selected" (drill-down) | `useState` in the `*ResultsView`/`*ComparisonView` | Purely local UI state. |

If you're adding a new piece of state, default to local `useState`. Only
promote it to Redux if two or more components that aren't in a direct
parent/child relationship need to read or write it.

## 4. Flow A — Redux-based: signing in

**Components involved:** `LoginPage` → `LoginForm` → (dispatch) →
`authSlice` thunk → `api/authApi.ts` → `api/httpClient.ts` → backend →
`authSlice` reducer → any component reading `state.auth.*` re-renders
(`ProtectedRoute`, `AppShell`).

```mermaid
sequenceDiagram
    participant U as User
    participant LF as LoginForm.tsx
    participant TH as authSlice: login thunk
    participant AA as api/authApi.ts
    participant HC as api/httpClient.ts
    participant BE as Backend /api/auth/login
    participant RD as Redux store (auth slice)
    participant PR as ProtectedRoute / AppShell

    U->>LF: click "Sign in"
    LF->>LF: handleSubmit(event) - event.preventDefault()
    LF->>TH: dispatch(login({ username, password }))
    TH->>RD: login.pending -> status = 'loading'
    RD-->>LF: useAppSelector(state => state.auth.status) re-renders (button disabled)
    TH->>AA: authApi.login(request)
    AA->>HC: httpClient.post('/api/auth/login', request)
    HC->>HC: attach Accept/Content-Type headers (no token yet)
    HC->>BE: fetch(POST /api/auth/login)
    alt credentials valid
        BE-->>HC: 200 { token, expiresAtUtc, username }
        HC-->>AA: parsed JSON
        AA-->>TH: LoginResponse
        TH->>RD: login.fulfilled -> token, username set; setSession() persists to sessionStorage
        RD-->>LF: useAppSelector picks up new token (indirectly, via re-render)
        LF->>LF: login.fulfilled.match(result) is true -> onSuccess()
        LF->>PR: navigate('/new-scan') (via useNavigate in LoginPage)
        PR->>PR: ProtectedRoute reads state.auth.token, now truthy -> renders AppShell + page
    else credentials invalid
        BE-->>HC: 401 ProblemDetails { type: "authentication-failure", title }
        HC-->>AA: throws ApiRequestError(AppError)
        AA-->>TH: (rejected)
        TH->>RD: login.rejected -> rejectWithValue(appError); status='failed', error=appError
        RD-->>LF: useAppSelector(state => state.auth.error) re-renders
        LF->>U: <ErrorBanner error={error} /> shown
    end
```

Step by step, with the actual code:

1. **Click.** `LoginForm.tsx` renders `<form onSubmit={handleSubmit}>`. The
   submit button has no `onClick` — submitting the form (click or Enter)
   fires `handleSubmit`.
2. **Local state → thunk dispatch.** `handleSubmit` reads `username`/
   `password` from local `useState` (this part is *not* Redux — the draft
   values only matter to this form) and calls:
   ```ts
   const result = await dispatch(login({ username, password }))
   ```
   `login` is a `createAsyncThunk` defined in `features/auth/authSlice.ts`.
   Dispatching it immediately dispatches the auto-generated `login.pending`
   action; the reducer's `extraReducers` sets `status: 'loading'`, which
   flows back into `LoginForm` via `useAppSelector((s) => s.auth.status)`
   and disables the submit button.
3. **Thunk body calls the API layer.** Inside the thunk, `authApi.login(request)`
   (`api/authApi.ts`) calls `httpClient.post('/api/auth/login', request)`.
4. **httpClient does the actual `fetch`.** `api/httpClient.ts` resolves the
   base URL (`window.__CONFIGLENS_API_BASE__`, set by the Docker entrypoint,
   or `VITE_API_BASE_URL` in dev), attaches JSON headers and — for
   *authenticated* endpoints — the bearer token from `sessionStorage` (not
   needed here, since login itself is unauthenticated). On a non-2xx
   response it parses the ProblemDetails body and throws an
   `ApiRequestError` carrying a typed `AppError`.
5. **Thunk resolves/rejects.** Success: the thunk returns
   `{ token, username }`, which Redux Toolkit turns into a `login.fulfilled`
   action. Failure: the thunk catches `ApiRequestError` and calls
   `rejectWithValue(err.appError)`, producing `login.rejected`.
6. **Reducer updates state.** `authSlice`'s `extraReducers` handle both
   cases: on success it sets `token`/`username` and calls `setSession(...)`
   to persist them to `sessionStorage` (so a page refresh doesn't log the
   user out); on failure it sets `error`.
7. **Re-render, driven by the store update.** Because `LoginForm` (for the
   error) and `ProtectedRoute`/`AppShell` (for the token) both call
   `useAppSelector`, React-Redux re-renders them automatically — no manual
   wiring. `LoginForm` checks `login.fulfilled.match(result)` on the
   `dispatch(...)` return value to decide whether to call `onSuccess()`,
   which `LoginPage` wires to `navigate('/new-scan', { replace: true })`.
8. **Route guard reacts.** The next time `ProtectedRoute` renders (React
   Router re-evaluates on navigation), `state.auth.token` is now truthy, so
   it renders `AppShell` + the target page instead of redirecting.

Note what Redux is buying here: `LoginForm` never touches `ProtectedRoute`
or `AppShell` directly. They're unrelated components three routes apart, and
the store is how the token gets from one to the others.

## 5. Flow B — plain component flow: running a scan

**Components involved:** `ScanParametersForm` (button) → `NewScanPage`
(handler, local state) → `api/scanApi.ts` → `api/httpClient.ts` → backend →
`NewScanPage` local state → `*ResultsView` re-renders. No Redux after the
initial read of which categories are selected.

```mermaid
sequenceDiagram
    participant U as User
    participant SPF as ScanParametersForm.tsx
    participant NSP as NewScanPage.tsx
    participant SA as api/scanApi.ts
    participant HC as api/httpClient.ts
    participant BE as Backend /api/scans/*
    participant RV as AksResultsView (etc.)

    U->>SPF: click "Start Scan"
    SPF->>NSP: onRun(parameters)  (prop callback, not Redux)
    NSP->>NSP: setRunning(true); setResults({}); setErrors({})
    NSP-->>SPF: running=true flows back as a prop -> button shows "Running scan..."
    loop for each selected category (read once from Redux at render time)
        NSP->>SA: scanApi.runAksDeploymentScan(request)
        SA->>HC: httpClient.post('/api/scans/aks-deployment', request)
        HC->>HC: attach Authorization: Bearer <token> (from sessionStorage)
        HC->>BE: fetch(POST ...)
        alt success
            BE-->>HC: 200 AksScanResponse
            HC-->>SA: parsed JSON
            SA-->>NSP: .then(r => nextResults.aks = r)
        else failure
            BE-->>HC: 4xx/5xx ProblemDetails
            HC-->>SA: throws ApiRequestError
            SA-->>NSP: .catch(err => nextErrors.AksDeployment = toAppError(err))
        end
    end
    NSP->>NSP: await Promise.all(tasks)
    NSP->>NSP: setResults(nextResults); setErrors(nextErrors); setRunning(false)
    NSP->>RV: <AksResultsView metadata=... resources=... /> (new props)
    RV->>U: renders grouped table; row click opens AksResourceDrawer (local useState, no API call)
```

Step by step:

1. **Click.** `ScanParametersForm.tsx` has a plain `<button onClick={() =>
   onRun(parameters)}>`. `parameters` is that component's own local
   `useState` (one object per category's field group). `onRun` is a prop —
   `NewScanPage` passed its `handleRun` function down; there is no dispatch
   involved.
2. **Handler sets local "in flight" state.** `NewScanPage.handleRun`
   (`features/newScan/NewScanPage.tsx`) immediately does
   `setRunning(true)`, clearing previous `results`/`errors` — all
   `useState`, all local to this one component.
3. **Handler reads Redux once, to decide what to call.** The *only* Redux
   involvement in this flow is `const selected =
   useAppSelector((state) => state.scanSelection.selectedCategories)` at the
   top of the component (set earlier by `ScanCategorySelector` — Flow A's
   pattern, reused). `handleRun` gates each API call on
   `selected.includes('AksDeployment')` etc. — nothing here is Redux state
   itself.
4. **Direct API calls, no thunk.** For each selected category,
   `handleRun` calls the matching function in `api/scanApi.ts` directly
   (`scanApi.runAksDeploymentScan(...)`, etc.) and chains `.then`/`.catch`
   to populate local `nextResults`/`nextErrors` objects. All four calls are
   pushed into a `tasks` array and run concurrently via `Promise.all(tasks)`.
5. **`scanApi` → `httpClient`.** Same `httpClient.post` as Flow A, except
   this time a token *is* present in `sessionStorage`, so `httpClient`
   attaches `Authorization: Bearer <token>` before calling `fetch`.
6. **Response (or error) becomes local state.** After `Promise.all`
   resolves, `handleRun` calls `setResults(nextResults)` and
   `setErrors(nextErrors)` — plain `useState` setters, not Redux actions.
7. **Re-render is local to the tree under `NewScanPage`.** Only
   `NewScanPage` and its children re-render (React's normal
   parent-state-down-props re-render, not a store subscription). Each
   `*ResultsView` (`AksResultsView`, `AppConfigResultsView`, etc.) receives
   the new data as props and renders it; further interaction inside them
   (row clicks opening a drawer, the config-tree search box, the findings
   status filter) is *another* layer of local `useState` inside those
   components — no API call, no Redux, just derived UI state.

The same shape (button → handler in the owning page → direct `api/*` call →
local `useState` → props down) is how every other "fetch and show a
category-specific result" screen works: `ScanHistoryPage`,
`ScanDetailPage`/`useScanRecord`, `ComparisonSetupPage`, and
`CompareResultPage` all follow it, just triggered by `useEffect` on mount or
route-param change instead of a button click.

## 6. Supporting infrastructure both flows share

- **`api/httpClient.ts`** is the single `fetch` call site in the app. It:
  resolves the API base URL, attaches the bearer token from
  `sessionStorage` when present, and — critically — normalizes every
  non-2xx response into a typed `ApiRequestError` (wrapping an `AppError`
  from `api/errors.ts`) so callers never deal with raw `Response` objects.
  On a `401` specifically, it clears the session and calls a listener
  registered once in `app/store.ts` (`onUnauthorized(() =>
  store.dispatch(loggedOut()))`) — this is how an expired/invalid token
  anywhere in the app forces a logout without every feature needing to know
  about auth.
- **`api/errors.ts`** defines `AppError { kind, message, fieldErrors? }` and
  maps backend ProblemDetails `type` values to one of the 8 `AppErrorKind`s.
  `components/common/ErrorBanner.tsx` is the one component that renders an
  `AppError`, used identically in both flows above (`LoginForm` and
  `NewScanPage`).
- **`app/hooks.ts`** (`useAppDispatch`, `useAppSelector`) are the only way
  components touch Redux — thin typed wrappers around `react-redux`'s
  hooks, so no component imports `store.ts` or slice files' `dispatch`
  directly.

## 7. Adding a new flow — quick checklist

- Does the new state need to be read by two+ components that aren't in a
  parent/child relationship? → add it to an existing slice, or a new one
  registered in `app/store.ts`. Otherwise → `useState` where it's used.
- Talking to the backend? Add a function to the relevant `api/*.ts` module
  that calls `httpClient.get/post` — never call `fetch` directly from a
  component or a thunk.
- Rendering a result? Prefer a presentational `*ResultsView`/
  `*ComparisonView` component that takes typed props and has no fetching
  logic, so it can be reused by both "just ran a scan" (Flow B) and "viewing
  a historical scan by ID" (`ScanDetailPage`) call sites, matching every
  existing category.
