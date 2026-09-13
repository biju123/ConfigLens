import { configureStore } from '@reduxjs/toolkit'
import { onUnauthorized } from '../api/httpClient'
import authReducer, { loggedOut } from '../features/auth/authSlice'
import scanSelectionReducer from '../features/scanSelection/scanSelectionSlice'

export const store = configureStore({
  reducer: {
    auth: authReducer,
    scanSelection: scanSelectionReducer,
  },
})

// A 401 from any API call clears the session, wherever it happened to fire
// from - avoids every feature slice having to know about auth.
onUnauthorized(() => store.dispatch(loggedOut()))

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch
