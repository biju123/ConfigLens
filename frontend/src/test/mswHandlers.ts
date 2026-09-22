import { http, HttpResponse } from 'msw'
import type { ComparisonResponse } from '../api/types'
import {
  aksScanRecordFixture,
  aksScanRecordFixture2,
  aksScanResponseFixture,
  appConfigScanResponseFixture,
  characteristicsScanResponseFixture,
  dependencyScanResponseFixture,
  referenceDataFixture,
} from './fixtures'

export const handlers = [
  http.post('*/api/auth/login', async ({ request }) => {
    const body = (await request.json()) as { username: string; password: string }
    if (body.username === 'user' && body.password === 'password') {
      return HttpResponse.json({ token: 'test-token', expiresAtUtc: '2026-09-12T18:00:00Z', username: 'user' })
    }
    return HttpResponse.json(
      { type: 'authentication-failure', title: 'Invalid username or password.', status: 401 },
      { status: 401 },
    )
  }),

  http.get('*/api/reference-data', () => HttpResponse.json(referenceDataFixture)),

  http.get('*/api/aks/clusters', ({ request }) => {
    const subscriptionId = new URL(request.url).searchParams.get('subscriptionId')
    return HttpResponse.json(subscriptionId === 'sub-assessor-prod-01' ? ['aks-assessor-prod-eastus', 'aks-assessor-prod-westus'] : [])
  }),
  http.get('*/api/aks/namespaces', ({ request }) => {
    const clusterName = new URL(request.url).searchParams.get('clusterName')
    return HttpResponse.json(clusterName === 'aks-assessor-prod-eastus' ? ['assessor-prod', 'assessor-prod-riverbend'] : [])
  }),

  http.post('*/api/scans/aks-deployment', () => HttpResponse.json(aksScanResponseFixture)),
  http.post('*/api/scans/application-configuration', () => HttpResponse.json(appConfigScanResponseFixture)),
  http.post('*/api/scans/configuration-characteristics', () => HttpResponse.json(characteristicsScanResponseFixture)),
  http.post('*/api/scans/dependency-accessibility', () => HttpResponse.json(dependencyScanResponseFixture)),

  http.get('*/api/scans/:scanId', ({ params }) =>
    HttpResponse.json(params.scanId === aksScanRecordFixture2.metadata.scanId ? aksScanRecordFixture2 : aksScanRecordFixture),
  ),
  http.get('*/api/scans', () => HttpResponse.json([aksScanRecordFixture, aksScanRecordFixture2])),

  http.post('*/api/comparisons', () =>
    HttpResponse.json({
      currentScanId: aksScanRecordFixture.metadata.scanId,
      baselineScanId: aksScanRecordFixture2.metadata.scanId,
      category: 'AksDeployment',
      result: {
        addedResources: aksScanResponseFixture.resources,
        removedResources: [],
        changedResources: [],
      },
    } satisfies ComparisonResponse),
  ),
]
