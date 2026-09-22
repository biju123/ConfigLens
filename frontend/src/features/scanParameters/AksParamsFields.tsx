import { useState } from 'react'
import { aksApi } from '../../api/aksApi'
import { ApiRequestError } from '../../api/httpClient'
import type { AppError } from '../../api/errors'
import type { ReferenceData } from '../../api/types'
import { ErrorBanner } from '../../components/common/ErrorBanner'
import { ComboBoxField } from './ComboBoxField'
import { SelectField } from './SelectField'
import type { AksParamsState } from './paramTypes'

function toAppError(err: unknown, fallbackMessage: string): AppError {
  return err instanceof ApiRequestError ? err.appError : { kind: 'application-failure', message: fallbackMessage }
}

export function AksParamsFields({
  value,
  onChange,
  referenceData,
}: {
  value: AksParamsState
  onChange: (value: AksParamsState) => void
  referenceData: ReferenceData
}) {
  const [clusters, setClusters] = useState<string[]>(referenceData.clusters)
  const [clustersLoading, setClustersLoading] = useState(false)
  const [clustersError, setClustersError] = useState<AppError | null>(null)

  const [namespaces, setNamespaces] = useState<string[]>(referenceData.namespaces)
  const [namespacesLoading, setNamespacesLoading] = useState(false)
  const [namespacesError, setNamespacesError] = useState<AppError | null>(null)

  async function loadClusters(subscriptionId: string, current: AksParamsState) {
    if (!subscriptionId.trim()) return
    setClustersLoading(true)
    setClustersError(null)
    try {
      const result = await aksApi.getClusters(subscriptionId)
      setClusters(result)
      if (result.length > 0 && !result.includes(current.clusterName)) {
        const nextCluster = result[0]
        onChange({ ...current, subscriptionId, clusterName: nextCluster })
        await loadNamespaces(subscriptionId, nextCluster)
      }
    } catch (err) {
      setClusters([])
      setClustersError(toAppError(err, 'Unable to load AKS clusters for this subscription.'))
    } finally {
      setClustersLoading(false)
    }
  }

  async function loadNamespaces(subscriptionId: string, clusterName: string) {
    if (!subscriptionId.trim() || !clusterName.trim()) return
    setNamespacesLoading(true)
    setNamespacesError(null)
    try {
      setNamespaces(await aksApi.getNamespaces(subscriptionId, clusterName))
    } catch (err) {
      setNamespaces([])
      setNamespacesError(toAppError(err, 'Unable to load namespaces for this AKS cluster.'))
    } finally {
      setNamespacesLoading(false)
    }
  }

  return (
    <fieldset className="card">
      <legend>AKS Deployment Scan</legend>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: 'var(--space-3)' }}>
        <ComboBoxField
          id="aks-subscription"
          label="Azure subscription"
          value={value.subscriptionId}
          onChange={(v) => onChange({ ...value, subscriptionId: v })}
          onCommit={(v) => loadClusters(v, { ...value, subscriptionId: v })}
          options={referenceData.subscriptions}
          loading={clustersLoading}
        />
        <SelectField
          id="aks-cluster"
          label="AKS cluster"
          value={value.clusterName}
          onChange={(v) => {
            onChange({ ...value, clusterName: v })
            loadNamespaces(value.subscriptionId, v)
          }}
          options={clusters}
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
        <ComboBoxField
          id="aks-namespace"
          label="Namespace"
          value={value.namespace}
          onChange={(v) => onChange({ ...value, namespace: v })}
          options={namespaces}
          placeholder="All namespaces"
          loading={namespacesLoading}
        />
      </div>
      {clustersError && <ErrorBanner error={clustersError} />}
      {namespacesError && <ErrorBanner error={namespacesError} />}
    </fieldset>
  )
}
