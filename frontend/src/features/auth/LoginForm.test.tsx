import { screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi } from 'vitest'
import { renderWithProviders } from '../../test/renderWithProviders'
import { LoginForm } from './LoginForm'

describe('LoginForm (flow 1: sign in)', () => {
  it('calls onSuccess after signing in with valid credentials', async () => {
    const onSuccess = vi.fn()
    renderWithProviders(<LoginForm onSuccess={onSuccess} />)

    await userEvent.type(screen.getByLabelText('Username'), 'user')
    await userEvent.type(screen.getByLabelText('Password'), 'password')
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }))

    await waitFor(() => expect(onSuccess).toHaveBeenCalled())
  })

  it('shows an authentication-failure error banner for invalid credentials', async () => {
    const onSuccess = vi.fn()
    renderWithProviders(<LoginForm onSuccess={onSuccess} />)

    await userEvent.type(screen.getByLabelText('Username'), 'user')
    await userEvent.type(screen.getByLabelText('Password'), 'wrong')
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }))

    expect(await screen.findByText(/Invalid username or password/i)).toBeInTheDocument()
    expect(onSuccess).not.toHaveBeenCalled()
  })
})
