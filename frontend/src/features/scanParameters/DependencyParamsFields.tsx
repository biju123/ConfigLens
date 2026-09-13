import type { ReferenceData } from '../../api/types'
import { SelectField } from './SelectField'
import type { DependencyParamsState } from './paramTypes'

export function DependencyParamsFields({
  value,
  onChange,
  referenceData,
}: {
  value: DependencyParamsState
  onChange: (value: DependencyParamsState) => void
  referenceData: ReferenceData
}) {
  return (
    <fieldset className="card">
      <legend>Dependency Accessibility Scan</legend>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: 'var(--space-3)' }}>
        <SelectField
          id="dep-application"
          label="Application"
          value={value.application}
          onChange={(v) => onChange({ ...value, application: v })}
          options={referenceData.applications}
        />
        <SelectField
          id="dep-environment"
          label="Environment"
          value={value.environment}
          onChange={(v) => onChange({ ...value, environment: v })}
          options={referenceData.environments}
        />
        <SelectField
          id="dep-namespace"
          label="Namespace"
          value={value.namespace}
          onChange={(v) => onChange({ ...value, namespace: v })}
          options={referenceData.namespaces}
        />
        <SelectField
          id="dep-type"
          label="Dependency type"
          value={value.dependencyType}
          onChange={(v) => onChange({ ...value, dependencyType: v as DependencyParamsState['dependencyType'] })}
          options={referenceData.dependencyTypes}
          allowEmpty
          emptyLabel="All dependency types"
        />
      </div>
    </fieldset>
  )
}
