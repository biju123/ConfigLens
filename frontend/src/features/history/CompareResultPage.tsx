import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { comparisonApi } from '../../api/comparisonApi'
import { ApiRequestError } from '../../api/httpClient'
import type { AppError } from '../../api/errors'
import type { ComparisonResponse } from '../../api/types'
import { ErrorBanner } from '../../components/common/ErrorBanner'
import { LoadingSpinner } from '../../components/common/LoadingSpinner'
import { AksComparisonView } from '../aks/AksComparisonView'
import { AppConfigComparisonView } from '../appConfig/AppConfigComparisonView'
import { CharacteristicsComparisonView } from '../characteristics/CharacteristicsComparisonView'
import { DependencyComparisonView } from '../dependency/DependencyComparisonView'
import type {
  AksComparisonResult,
  AppConfigComparisonResult,
  CharacteristicsComparisonResult,
  DependencyComparisonResult,
} from '../../api/types'

export function CompareResultPage() {
  const { currentScanId, baselineScanId } = useParams<{ currentScanId: string; baselineScanId: string }>()
  const [data, setData] = useState<ComparisonResponse | null>(null)
  const [error, setError] = useState<AppError | null>(null)

  useEffect(() => {
    if (!currentScanId || !baselineScanId) return
    comparisonApi
      .compare(currentScanId, baselineScanId)
      .then(setData)
      .catch((err) => setError(err instanceof ApiRequestError ? err.appError : { kind: 'application-failure', message: 'Comparison failed.' }))
  }, [currentScanId, baselineScanId])

  if (error) return <ErrorBanner error={error} />
  if (!data) return <LoadingSpinner label="Comparing scans…" />

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-4)' }}>
      <h1>Comparison: {data.category}</h1>
      <p className="muted">
        Current <span className="mono">{data.currentScanId}</span> vs baseline{' '}
        <span className="mono">{data.baselineScanId}</span>
      </p>
      {data.category === 'AksDeployment' && <AksComparisonView result={data.result as AksComparisonResult} />}
      {data.category === 'ApplicationConfiguration' && (
        <AppConfigComparisonView result={data.result as AppConfigComparisonResult} />
      )}
      {data.category === 'ConfigurationCharacteristics' && (
        <CharacteristicsComparisonView result={data.result as CharacteristicsComparisonResult} />
      )}
      {data.category === 'DependencyAccessibility' && (
        <DependencyComparisonView result={data.result as DependencyComparisonResult} />
      )}
    </div>
  )
}
