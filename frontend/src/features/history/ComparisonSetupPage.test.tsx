import { screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import { renderWithProviders } from '../../test/renderWithProviders'
import { ComparisonSetupPage } from './ComparisonSetupPage'

describe('ComparisonSetupPage (flow 11: select and compare scans)', () => {
  it('enables Compare only once two different scans of the same category are picked', async () => {
    renderWithProviders(<ComparisonSetupPage />)

    const compareButton = screen.getByRole('button', { name: 'Compare' })
    expect(compareButton).toBeDisabled()

    await screen.findAllByText(/SCAN-20260912-00001/)
    await userEvent.selectOptions(screen.getByLabelText('Current scan'), 'SCAN-20260912-00001')
    await userEvent.selectOptions(screen.getByLabelText('Baseline scan'), 'SCAN-20260911-00001')

    expect(compareButton).toBeEnabled()
  })
})
