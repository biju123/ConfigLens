import { httpClient } from './httpClient'
import type {
  AksScanRequest,
  AksScanResponse,
  AppConfigScanRequest,
  AppConfigScanResponse,
  CharacteristicsScanRequest,
  CharacteristicsScanResponse,
  DependencyScanRequest,
  DependencyScanResponse,
  ScanCategory,
  ScanRecord,
} from './types'

export const scanApi = {
  runAksDeploymentScan: (request: AksScanRequest) =>
    httpClient.post<AksScanResponse>('/api/scans/aks-deployment', request),

  runApplicationConfigurationScan: (request: AppConfigScanRequest) =>
    httpClient.post<AppConfigScanResponse>('/api/scans/application-configuration', request),

  runCharacteristicsScan: (request: CharacteristicsScanRequest) =>
    httpClient.post<CharacteristicsScanResponse>('/api/scans/configuration-characteristics', request),

  runDependencyScan: (request: DependencyScanRequest) =>
    httpClient.post<DependencyScanResponse>('/api/scans/dependency-accessibility', request),

  getScanById: (scanId: string) => httpClient.get<ScanRecord>(`/api/scans/${encodeURIComponent(scanId)}`),

  queryScans: (category?: ScanCategory) =>
    httpClient.get<ScanRecord[]>(`/api/scans${category ? `?category=${category}` : ''}`),
}
