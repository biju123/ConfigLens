import { createAsyncThunk, createSlice, type PayloadAction } from '@reduxjs/toolkit'
import { authApi } from '../../api/authApi'
import { ApiRequestError } from '../../api/httpClient'
import { getToken, getUsername, setSession } from '../../api/httpClient'
import type { AppError } from '../../api/errors'
import type { LoginRequest } from '../../api/types'

interface AuthState {
  token: string | null
  username: string | null
  status: 'idle' | 'loading' | 'failed'
  error: AppError | null
}

const initialState: AuthState = {
  token: getToken(),
  username: getUsername(),
  status: 'idle',
  error: null,
}

export const login = createAsyncThunk<{ token: string; username: string }, LoginRequest, { rejectValue: AppError }>(
  'auth/login',
  async (request, { rejectWithValue }) => {
    try {
      const response = await authApi.login(request)
      return { token: response.token, username: response.username }
    } catch (err) {
      if (err instanceof ApiRequestError) {
        return rejectWithValue(err.appError)
      }
      throw err
    }
  },
)

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    loggedOut(state) {
      setSession(null, null)
      state.token = null
      state.username = null
      state.error = null
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(login.pending, (state) => {
        state.status = 'loading'
        state.error = null
      })
      .addCase(login.fulfilled, (state, action: PayloadAction<{ token: string; username: string }>) => {
        state.status = 'idle'
        state.token = action.payload.token
        state.username = action.payload.username
        setSession(action.payload.token, action.payload.username)
      })
      .addCase(login.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.payload ?? { kind: 'application-failure', message: 'Login failed.' }
      })
  },
})

export const { loggedOut } = authSlice.actions
export default authSlice.reducer
