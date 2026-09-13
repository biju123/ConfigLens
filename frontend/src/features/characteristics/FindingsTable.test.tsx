import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi } from 'vitest'
import { characteristicsScanResponseFixture } from '../../test/fixtures'
import { FindingsTable } from './FindingsTable'

describe('FindingsTable (flow 6: filter results)', () => {
  it('shows every finding by default and filters by status', async () => {
    render(<FindingsTable findings={characteristicsScanResponseFixture.findings} onSelect={vi.fn()} />)

    expect(screen.getByText('Database retry count is required')).toBeInTheDocument()
    expect(screen.getByText('Database connection string is required')).toBeInTheDocument()

    await userEvent.selectOptions(screen.getByRole('combobox'), 'Fail')

    expect(screen.getByText('Database retry count is required')).toBeInTheDocument()
    expect(screen.queryByText('Database connection string is required')).not.toBeInTheDocument()
  })
})
