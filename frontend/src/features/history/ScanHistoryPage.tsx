import { useEffect, useState } from 'react'
import { scanApi } from '../../api/scanApi'
import { ApiRequestError } from '../../api/httpClient'
import type { AppError } from '../../api/errors'
import type { ScanCategory, ScanRecord } from '../../api/types'
import { ErrorBanner } from '../../components/common/ErrorBanner'
import { LoadingSpinner } from '../../components/common/LoadingSpinner'
import { ScanHistoryTable } from './ScanHistoryTable'

const CATEGORY_FILTERS: (ScanCategory | 'All')[] = [
  'All',
  'AksDeployment',
  'ApplicationConfiguration',
  'ConfigurationCharacteristics',
  'DependencyAccessibility',
]

export function ScanHistoryPage() {
  const [category, setCategory] = useState<ScanCategory | 'All'>('All')
  const [scans, setScans] = useState<ScanRecord[] | null>(null)
  const [error, setError] = useState<AppError | null>(null)

  useEffect(() => {
    let cancelled = false
    setScans(null)
    scanApi
      .queryScans(category === 'All' ? undefined : category)
      .then((result) => {
        if (!cancelled) setScans(result)
      })
      .catch((err) => {
        if (!cancelled) {
          setError(err instanceof ApiRequestError ? err.appError : { kind: 'application-failure', message: 'Failed to load scan history.' })
        }
      })
    return () => {
      cancelled = true
    }
  }, [category])

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-4)' }}>
      <div className="section-heading">
        <h1>Scan History</h1>
        <select className="input" value={category} onChange={(e) => setCategory(e.target.value as ScanCategory | 'All')}>
          {CATEGORY_FILTERS.map((c) => (
            <option key={c} value={c}>
              {c}
            </option>
          ))}
        </select>
      </div>
      {error && <ErrorBanner error={error} />}
      {!scans && !error && <LoadingSpinner label="Loading scan history…" />}
      {scans && scans.length === 0 && <div className="muted">No scans have been run yet.</div>}
      {scans && scans.length > 0 && (
        <div className="card">
          <ScanHistoryTable scans={scans} />
        </div>
      )}
    </div>
  )
}
