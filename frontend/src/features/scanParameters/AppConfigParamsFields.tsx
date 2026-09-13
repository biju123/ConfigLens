import type { ReferenceData } from '../../api/types'
import { SelectField } from './SelectField'
import type { AppConfigParamsState } from './paramTypes'

export function AppConfigParamsFields({
  value,
  onChange,
  referenceData,
}: {
  value: AppConfigParamsState
  onChange: (value: AppConfigParamsState) => void
  referenceData: ReferenceData
}) {
  return (
    <fieldset className="card">
      <legend>Application Configuration Scan</legend>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: 'var(--space-3)' }}>
        <SelectField
          id="appconfig-application"
          label="Application"
          value={value.application}
          onChange={(v) => onChange({ ...value, application: v })}
          options={referenceData.applications}
        />
        <SelectField
          id="appconfig-environment"
          label="Environment"
          value={value.environment}
          onChange={(v) => onChange({ ...value, environment: v })}
          options={referenceData.environments}
        />
        <SelectField
          id="appconfig-tenant"
          label="Customer / Tenant"
          value={value.tenant}
          onChange={(v) => onChange({ ...value, tenant: v })}
          options={referenceData.tenants}
        />
        <SelectField
          id="appconfig-namespace"
          label="Namespace"
          value={value.namespace}
          onChange={(v) => onChange({ ...value, namespace: v })}
          options={referenceData.namespaces}
          allowEmpty
          emptyLabel="(none)"
        />
        <SelectField
          id="appconfig-sessionyear"
          label="Session year"
          value={String(value.sessionYear)}
          onChange={(v) => onChange({ ...value, sessionYear: Number(v) })}
          options={referenceData.sessionYears.map(String)}
        />
      </div>
    </fieldset>
  )
}
