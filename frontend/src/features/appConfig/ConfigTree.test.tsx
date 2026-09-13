import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import { appConfigScanResponseFixture } from '../../test/fixtures'
import { ConfigTree } from './ConfigTree'

describe('ConfigTree (flow 8: view configuration)', () => {
  it('masks sensitive values and shows plain values for non-sensitive keys', () => {
    render(<ConfigTree tree={appConfigScanResponseFixture.tree} />)

    expect(screen.getByText('••••••••')).toBeInTheDocument()
    expect(screen.getByText('3')).toBeInTheDocument()
  })

  it('collapses a section when its header is clicked', async () => {
    render(<ConfigTree tree={appConfigScanResponseFixture.tree} />)

    expect(screen.getByText('RetryCount')).toBeInTheDocument()
    await userEvent.click(screen.getByText('Database'))
    expect(screen.queryByText('RetryCount')).not.toBeInTheDocument()
  })

  it('filters to sections/items matching the search term', async () => {
    render(<ConfigTree tree={appConfigScanResponseFixture.tree} />)

    await userEvent.type(screen.getByPlaceholderText(/search configuration/i), 'RetryCount')

    expect(screen.getByText('RetryCount')).toBeInTheDocument()
    expect(screen.queryByText('ConnectionString')).not.toBeInTheDocument()
  })
})
