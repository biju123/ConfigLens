import { screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import { renderWithProviders } from '../../test/renderWithProviders'
import { ScanHistoryPage } from './ScanHistoryPage'

describe('ScanHistoryPage (flow 10: select a previous scan)', () => {
  it('lists previous scans with a link to their detail view', async () => {
    renderWithProviders(<ScanHistoryPage />)

    const link = await screen.findByRole('link', { name: 'SCAN-20260912-00001' })
    expect(link).toHaveAttribute('href', '/results/SCAN-20260912-00001')
  })
})
