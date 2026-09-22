import { httpClient } from './httpClient'

export const aksApi = {
  getClusters: (subscriptionId: string) =>
    httpClient.get<string[]>(`/api/aks/clusters?subscriptionId=${encodeURIComponent(subscriptionId)}`),

  getNamespaces: (subscriptionId: string, clusterName: string) =>
    httpClient.get<string[]>(
      `/api/aks/namespaces?subscriptionId=${encodeURIComponent(subscriptionId)}&clusterName=${encodeURIComponent(clusterName)}`,
    ),
}
