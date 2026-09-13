import { useState } from 'react'
import { useAppSelector } from '../../app/hooks'
import type { ReferenceData } from '../../api/types'
import { AksParamsFields } from './AksParamsFields'
import { AppConfigParamsFields } from './AppConfigParamsFields'
import { CharacteristicsParamsFields } from './CharacteristicsParamsFields'
import { DependencyParamsFields } from './DependencyParamsFields'
import type { AksParamsState, AppConfigParamsState, CharacteristicsParamsState, DependencyParamsState } from './paramTypes'

export interface ScanParameters {
  aks: AksParamsState
  appConfig: AppConfigParamsState
  characteristics: CharacteristicsParamsState
  dependency: DependencyParamsState
}

function defaultParameters(referenceData: ReferenceData): ScanParameters {
  const first = (values: string[]) => values[0] ?? ''
  return {
    aks: {
      subscriptionId: first(referenceData.subscriptions),
      clusterName: first(referenceData.clusters),
      environment: first(referenceData.environments),
      tenant: first(referenceData.tenants),
      namespace: '',
    },
    appConfig: {
      application: first(referenceData.applications),
      environment: first(referenceData.environments),
      tenant: first(referenceData.tenants),
      namespace: '',
      sessionYear: referenceData.sessionYears[0] ?? new Date().getFullYear(),
    },
    characteristics: {
      application: first(referenceData.applications),
      environment: first(referenceData.environments),
      tenant: first(referenceData.tenants),
      sessionYear: referenceData.sessionYears[0] ?? new Date().getFullYear(),
      ruleSetId: referenceData.ruleSets[0]?.ruleSetId ?? '',
    },
    dependency: {
      application: first(referenceData.applications),
      environment: first(referenceData.environments),
      namespace: first(referenceData.namespaces),
      dependencyType: '',
    },
  }
}

export function ScanParametersForm({
  referenceData,
  onRun,
  running,
}: {
  referenceData: ReferenceData
  onRun: (parameters: ScanParameters) => void
  running: boolean
}) {
  const selected = useAppSelector((state) => state.scanSelection.selectedCategories)
  const [parameters, setParameters] = useState<ScanParameters>(() => defaultParameters(referenceData))

  if (selected.length === 0) {
    return null
  }

  return (
    <div className="card">
      <h2>2. Enter scan parameters</h2>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-3)' }}>
        {selected.includes('AksDeployment') && (
          <AksParamsFields
            value={parameters.aks}
            onChange={(aks) => setParameters((p) => ({ ...p, aks }))}
            referenceData={referenceData}
          />
        )}
        {selected.includes('ApplicationConfiguration') && (
          <AppConfigParamsFields
            value={parameters.appConfig}
            onChange={(appConfig) => setParameters((p) => ({ ...p, appConfig }))}
            referenceData={referenceData}
          />
        )}
        {selected.includes('ConfigurationCharacteristics') && (
          <CharacteristicsParamsFields
            value={parameters.characteristics}
            onChange={(characteristics) => setParameters((p) => ({ ...p, characteristics }))}
            referenceData={referenceData}
          />
        )}
        {selected.includes('DependencyAccessibility') && (
          <DependencyParamsFields
            value={parameters.dependency}
            onChange={(dependency) => setParameters((p) => ({ ...p, dependency }))}
            referenceData={referenceData}
          />
        )}
      </div>
      <div style={{ marginTop: 'var(--space-4)' }}>
        <button className="btn btn-primary" disabled={running} onClick={() => onRun(parameters)}>
          {running ? 'Running scan…' : 'Start Scan'}
        </button>
      </div>
    </div>
  )
}
