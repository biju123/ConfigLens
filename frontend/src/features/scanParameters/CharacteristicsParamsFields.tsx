import type { ReferenceData } from '../../api/types'
import { SelectField } from './SelectField'
import type { CharacteristicsParamsState } from './paramTypes'

export function CharacteristicsParamsFields({
  value,
  onChange,
  referenceData,
}: {
  value: CharacteristicsParamsState
  onChange: (value: CharacteristicsParamsState) => void
  referenceData: ReferenceData
}) {
  return (
    <fieldset className="card">
      <legend>Configuration Characteristics Scan</legend>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: 'var(--space-3)' }}>
        <SelectField
          id="char-application"
          label="Application"
          value={value.application}
          onChange={(v) => onChange({ ...value, application: v })}
          options={referenceData.applications}
        />
        <SelectField
          id="char-environment"
          label="Environment"
          value={value.environment}
          onChange={(v) => onChange({ ...value, environment: v })}
          options={referenceData.environments}
        />
        <SelectField
          id="char-tenant"
          label="Customer / Tenant"
          value={value.tenant}
          onChange={(v) => onChange({ ...value, tenant: v })}
          options={referenceData.tenants}
        />
        <SelectField
          id="char-sessionyear"
          label="Session year"
          value={String(value.sessionYear)}
          onChange={(v) => onChange({ ...value, sessionYear: Number(v) })}
          options={referenceData.sessionYears.map(String)}
        />
        <SelectField
          id="char-ruleset"
          label="Rule set"
          value={value.ruleSetId}
          onChange={(v) => onChange({ ...value, ruleSetId: v })}
          options={referenceData.ruleSets.map((r) => r.ruleSetId)}
        />
      </div>
    </fieldset>
  )
}
