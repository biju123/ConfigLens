import { useEffect, useState } from 'react'
import { referenceDataApi } from '../../api/referenceDataApi'
import { ApiRequestError } from '../../api/httpClient'
import type { AppError } from '../../api/errors'
import type { ReferenceData } from '../../api/types'

export function useReferenceData() {
  const [data, setData] = useState<ReferenceData | null>(null)
  const [error, setError] = useState<AppError | null>(null)

  useEffect(() => {
    let cancelled = false
    referenceDataApi
      .get()
      .then((result) => {
        if (!cancelled) setData(result)
      })
      .catch((err) => {
        if (cancelled) return
        setError(err instanceof ApiRequestError ? err.appError : { kind: 'application-failure', message: 'Failed to load reference data.' })
      })
    return () => {
      cancelled = true
    }
  }, [])

  return { data, error, loading: !data && !error }
}
