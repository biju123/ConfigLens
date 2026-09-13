import { Link, useParams } from 'react-router-dom'
import { LoadingSpinner } from '../../components/common/LoadingSpinner'
import { ErrorBanner } from '../../components/common/ErrorBanner'
import { AksResultsView } from '../aks/AksResultsView'
import { AppConfigResultsView } from '../appConfig/AppConfigResultsView'
import { CharacteristicsResultsView } from '../characteristics/CharacteristicsResultsView'
import { DependencyResultsView } from '../dependency/DependencyResultsView'
import { useScanRecord } from './useScanRecord'

export function ScanDetailPage() {
  const { scanId } = useParams<{ scanId: string }>()
  const { data, error, loading } = useScanRecord(scanId)

  if (loading) return <LoadingSpinner label="Loading scan…" />
  if (error) return <ErrorBanner error={error} />
  if (!data) return null

  const { metadata, result } = data

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-4)' }}>
      <div className="section-heading">
        <h1>{metadata.category.replace(/([A-Z])/g, ' $1').trim()}</h1>
        <Link className="btn" to="/history">
          Back to history
        </Link>
      </div>
      {result.category === 'AksDeployment' && <AksResultsView metadata={metadata} resources={result.resources} />}
      {result.category === 'ApplicationConfiguration' && <AppConfigResultsView metadata={metadata} tree={result.tree} />}
      {result.category === 'ConfigurationCharacteristics' && (
        <CharacteristicsResultsView
          metadata={metadata}
          findings={result.findings}
          summary={{
            pass: result.passCount,
            warning: result.warningCount,
            fail: result.failCount,
            notChecked: result.notCheckedCount,
            notApplicable: result.notApplicableCount,
            error: result.errorCount,
          }}
        />
      )}
      {result.category === 'DependencyAccessibility' && <DependencyResultsView metadata={metadata} checks={result.checks} />}
    </div>
  )
}
