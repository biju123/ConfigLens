import { useState, type ReactNode } from 'react'
import { Link } from 'react-router-dom'
import { useAppSelector } from '../../app/hooks'
import { scanApi } from '../../api/scanApi'
import { ApiRequestError } from '../../api/httpClient'
import type { AppError } from '../../api/errors'
import type {
  AksScanResponse,
  AppConfigScanResponse,
  CharacteristicsScanResponse,
  DependencyScanResponse,
  ScanCategory,
} from '../../api/types'
import { ErrorBanner } from '../../components/common/ErrorBanner'
import { LoadingSpinner } from '../../components/common/LoadingSpinner'
import { AksResultsView } from '../aks/AksResultsView'
import { AppConfigResultsView } from '../appConfig/AppConfigResultsView'
import { CharacteristicsResultsView } from '../characteristics/CharacteristicsResultsView'
import { DependencyResultsView } from '../dependency/DependencyResultsView'
import { useReferenceData } from '../referenceData/useReferenceData'
import { ScanCategorySelector } from '../scanSelection/ScanCategorySelector'
import { ScanParametersForm, type ScanParameters } from '../scanParameters/ScanParametersForm'

interface RunResults {
  aks?: AksScanResponse
  appConfig?: AppConfigScanResponse
  characteristics?: CharacteristicsScanResponse
  dependency?: DependencyScanResponse
}

function toAppError(err: unknown): AppError {
  return err instanceof ApiRequestError ? err.appError : { kind: 'application-failure', message: 'The scan failed to run.' }
}

export function NewScanPage() {
  const { data: referenceData, error: referenceDataError, loading: referenceDataLoading } = useReferenceData()
  const selected = useAppSelector((state) => state.scanSelection.selectedCategories)
  const [running, setRunning] = useState(false)
  const [results, setResults] = useState<RunResults>({})
  const [errors, setErrors] = useState<Partial<Record<ScanCategory, AppError>>>({})

  async function handleRun(parameters: ScanParameters) {
    setRunning(true)
    setResults({})
    setErrors({})

    const nextResults: RunResults = {}
    const nextErrors: Partial<Record<ScanCategory, AppError>> = {}

    const tasks: Promise<void>[] = []

    if (selected.includes('AksDeployment')) {
      tasks.push(
        scanApi
          .runAksDeploymentScan({ ...parameters.aks, namespace: parameters.aks.namespace || undefined })
          .then((r) => {
            nextResults.aks = r
          })
          .catch((err) => {
            nextErrors.AksDeployment = toAppError(err)
          }),
      )
    }
    if (selected.includes('ApplicationConfiguration')) {
      tasks.push(
        scanApi
          .runApplicationConfigurationScan({ ...parameters.appConfig, namespace: parameters.appConfig.namespace || undefined })
          .then((r) => {
            nextResults.appConfig = r
          })
          .catch((err) => {
            nextErrors.ApplicationConfiguration = toAppError(err)
          }),
      )
    }
    if (selected.includes('ConfigurationCharacteristics')) {
      tasks.push(
        scanApi
          .runCharacteristicsScan(parameters.characteristics)
          .then((r) => {
            nextResults.characteristics = r
          })
          .catch((err) => {
            nextErrors.ConfigurationCharacteristics = toAppError(err)
          }),
      )
    }
    if (selected.includes('DependencyAccessibility')) {
      tasks.push(
        scanApi
          .runDependencyScan({
            ...parameters.dependency,
            dependencyType: parameters.dependency.dependencyType || undefined,
          })
          .then((r) => {
            nextResults.dependency = r
          })
          .catch((err) => {
            nextErrors.DependencyAccessibility = toAppError(err)
          }),
      )
    }

    await Promise.all(tasks)
    setResults(nextResults)
    setErrors(nextErrors)
    setRunning(false)
  }

  if (referenceDataLoading) return <LoadingSpinner label="Loading reference data…" />
  if (referenceDataError) return <ErrorBanner error={referenceDataError} />
  if (!referenceData) return null

  const hasResults = Object.keys(results).length > 0 || Object.keys(errors).length > 0

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-4)' }}>
      <h1>New Scan</h1>
      <ScanCategorySelector />
      <ScanParametersForm referenceData={referenceData} onRun={handleRun} running={running} />

      {hasResults && (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-5)' }}>
          <h2>3. Results</h2>

          {errors.AksDeployment && <ErrorBanner error={errors.AksDeployment} />}
          {results.aks && (
            <ResultCard title="AKS Deployment Scan" scanId={results.aks.metadata.scanId}>
              <AksResultsView metadata={results.aks.metadata} resources={results.aks.resources} />
            </ResultCard>
          )}

          {errors.ApplicationConfiguration && <ErrorBanner error={errors.ApplicationConfiguration} />}
          {results.appConfig && (
            <ResultCard title="Application Configuration Scan" scanId={results.appConfig.metadata.scanId}>
              <AppConfigResultsView metadata={results.appConfig.metadata} tree={results.appConfig.tree} />
            </ResultCard>
          )}

          {errors.ConfigurationCharacteristics && <ErrorBanner error={errors.ConfigurationCharacteristics} />}
          {results.characteristics && (
            <ResultCard title="Configuration Characteristics Scan" scanId={results.characteristics.metadata.scanId}>
              <CharacteristicsResultsView
                metadata={results.characteristics.metadata}
                findings={results.characteristics.findings}
                summary={results.characteristics.summary}
              />
            </ResultCard>
          )}

          {errors.DependencyAccessibility && <ErrorBanner error={errors.DependencyAccessibility} />}
          {results.dependency && (
            <ResultCard title="Dependency Accessibility Scan" scanId={results.dependency.metadata.scanId}>
              <DependencyResultsView metadata={results.dependency.metadata} checks={results.dependency.checks} />
            </ResultCard>
          )}
        </div>
      )}
    </div>
  )
}

function ResultCard({ title, scanId, children }: { title: string; scanId: string; children: ReactNode }) {
  return (
    <div>
      <div className="section-heading">
        <h2>{title}</h2>
        <Link className="btn" to={`/results/${scanId}`}>
          Open full view
        </Link>
      </div>
      {children}
    </div>
  )
}
