import { useEffect, useState } from 'react'
import { scanApi } from '../../api/scanApi'
import { ApiRequestError } from '../../api/httpClient'
import type { AppError } from '../../api/errors'
import type { ScanRecord } from '../../api/types'

export function useScanRecord(scanId: string | undefined) {
  const [data, setData] = useState<ScanRecord | null>(null)
  const [error, setError] = useState<AppError | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    if (!scanId) return
    let cancelled = false
    setLoading(true)
    setError(null)
    scanApi
      .getScanById(scanId)
      .then((result) => {
        if (!cancelled) setData(result)
      })
      .catch((err) => {
        if (cancelled) return
        setError(err instanceof ApiRequestError ? err.appError : { kind: 'application-failure', message: 'Failed to load scan.' })
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [scanId])

  return { data, error, loading }
}
