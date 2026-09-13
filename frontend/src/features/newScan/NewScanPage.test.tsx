import { screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import { renderWithProviders } from '../../test/renderWithProviders'
import { NewScanPage } from './NewScanPage'

describe('NewScanPage (flows 4/5/7: start scan, view results, drill down)', () => {
  it('runs the selected scan and renders results with a working drill-down', async () => {
    renderWithProviders(<NewScanPage />)

    await userEvent.click(await screen.findByRole('checkbox', { name: /AKS Deployment Scan/i }))
    await userEvent.click(screen.getByRole('button', { name: /start scan/i }))

    expect(await screen.findByText(/SCAN-20260912-00001/)).toBeInTheDocument()
    expect(screen.getByText('assessor-api', { selector: 'td' })).toBeInTheDocument()

    // Drill down: click the resource row, expect the drawer with full detail to open.
    await userEvent.click(screen.getByText('assessor-api', { selector: 'td' }))
    expect(await screen.findByRole('heading', { name: 'assessor-api', level: 2 })).toBeInTheDocument()
    expect(screen.getByText('Resource type')).toBeInTheDocument()
  })
})
