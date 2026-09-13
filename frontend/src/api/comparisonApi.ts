import { httpClient } from './httpClient'
import type { ComparisonResponse } from './types'

export const comparisonApi = {
  compare: (currentScanId: string, baselineScanId: string) =>
    httpClient.post<ComparisonResponse>('/api/comparisons', { currentScanId, baselineScanId }),
}
