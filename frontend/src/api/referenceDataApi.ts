import { httpClient } from './httpClient'
import type { ReferenceData } from './types'

export const referenceDataApi = {
  get: () => httpClient.get<ReferenceData>('/api/reference-data'),
}
