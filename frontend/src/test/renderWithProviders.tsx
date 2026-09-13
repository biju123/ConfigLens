import type { ReactElement } from 'react'
import { render } from '@testing-library/react'
import { combineReducers, configureStore } from '@reduxjs/toolkit'
import { Provider } from 'react-redux'
import { MemoryRouter } from 'react-router-dom'
import authReducer from '../features/auth/authSlice'
import scanSelectionReducer from '../features/scanSelection/scanSelectionSlice'

const testReducer = combineReducers({ auth: authReducer, scanSelection: scanSelectionReducer })

export function makeTestStore(preloadedState?: Partial<ReturnType<typeof testReducer>>) {
  return configureStore({ reducer: testReducer, preloadedState })
}

export function renderWithProviders(
  ui: ReactElement,
  { route = '/', store = makeTestStore() }: { route?: string; store?: ReturnType<typeof makeTestStore> } = {},
) {
  return {
    store,
    ...render(
      <Provider store={store}>
        <MemoryRouter initialEntries={[route]}>{ui}</MemoryRouter>
      </Provider>,
    ),
  }
}
