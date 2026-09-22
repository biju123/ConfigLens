// The 8 error categories CLAUDE.md section 23 requires the UI to
// distinguish. The first 5 come from real HTTP error responses (see
// httpClient.ts); the last 3 describe scan-RESULT content (a dependency
// check that failed, timed out, or a rule that failed) which the backend
// returns as 200 OK payloads, not HTTP errors - UI code that wants to
// surface one of those as a banner constructs the AppError itself.
export type AppErrorKind =
  | 'invalid-input'
  | 'authentication-failure'
  | 'authorization-failure'
  | 'infrastructure-failure'
  | 'application-failure'
  | 'dependency-failure'
  | 'timeout'
  | 'validation-failure'

export interface AppError {
  kind: AppErrorKind
  message: string
  fieldErrors?: Record<string, string[]>
}

interface ProblemDetailsBody {
  type?: string
  title?: string
  detail?: string
  errors?: Record<string, string[]>
}

const PROBLEM_TYPE_TO_KIND: Record<string, AppErrorKind> = {
  'invalid-input': 'invalid-input',
  'not-found': 'invalid-input',
  'category-mismatch': 'invalid-input',
  'authentication-failure': 'authentication-failure',
  'authorization-failure': 'authorization-failure',
  'infrastructure-failure': 'infrastructure-failure',
  'application-failure': 'application-failure',
  timeout: 'timeout',
}

function fallbackKindForStatus(status: number): AppErrorKind {
  if (status === 401) return 'authentication-failure'
  if (status === 403) return 'authorization-failure'
  if (status === 400 || status === 404 || status === 422) return 'invalid-input'
  if (status === 502 || status === 503) return 'infrastructure-failure'
  if (status === 504) return 'timeout'
  return 'application-failure'
}

export function appErrorFromProblemDetails(status: number, body: ProblemDetailsBody | undefined): AppError {
  const kind = (body?.type ? PROBLEM_TYPE_TO_KIND[body.type] : undefined) ?? fallbackKindForStatus(status)
  const message = body?.title ?? body?.detail ?? `Request failed with status ${status}.`
  return { kind, message, fieldErrors: body?.errors }
}

export function networkAppError(): AppError {
  return { kind: 'infrastructure-failure', message: 'Unable to reach the ConfigLens API. Check your connection and try again.' }
}
