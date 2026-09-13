import type { ReferenceData } from '../../api/types'
import { SelectField } from './SelectField'
import type { AksParamsState } from './paramTypes'

export function AksParamsFields({
  value,
  onChange,
  referenceData,
}: {
  value: AksParamsState
  onChange: (value: AksParamsState) => void
  referenceData: ReferenceData
}) {
  return (
    <fieldset className="card">
      <legend>AKS Deployment Scan</legend>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: 'var(--space-3)' }}>
        <SelectField
          id="aks-subscription"
          label="Azure subscription"
          value={value.subscriptionId}
          onChange={(v) => onChange({ ...value, subscriptionId: v })}
          options={referenceData.subscriptions}
        />
        <SelectField
          id="aks-cluster"
          label="AKS cluster"
          value={value.clusterName}
          onChange={(v) => onChange({ ...value, clusterName: v })}
          options={referenceData.clusters}
        />
        <SelectField
          id="aks-environment"
          label="Environment"
          value={value.environment}
          onChange={(v) => onChange({ ...value, environment: v })}
          options={referenceData.environments}
        />
        <SelectField
          id="aks-tenant"
          label="Customer / Tenant"
          value={value.tenant}
          onChange={(v) => onChange({ ...value, tenant: v })}
          options={referenceData.tenants}
        />
        <SelectField
          id="aks-namespace"
          label="Namespace"
          value={value.namespace}
          onChange={(v) => onChange({ ...value, namespace: v })}
          options={referenceData.namespaces}
          allowEmpty
          emptyLabel="All namespaces"
        />
      </div>
    </fieldset>
  )
}
