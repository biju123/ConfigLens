import { createSlice, type PayloadAction } from '@reduxjs/toolkit'
import type { ScanCategory } from '../../api/types'

interface ScanSelectionState {
  selectedCategories: ScanCategory[]
}

const initialState: ScanSelectionState = {
  selectedCategories: [],
}

const scanSelectionSlice = createSlice({
  name: 'scanSelection',
  initialState,
  reducers: {
    toggleCategory(state, action: PayloadAction<ScanCategory>) {
      const index = state.selectedCategories.indexOf(action.payload)
      if (index === -1) {
        state.selectedCategories.push(action.payload)
      } else {
        state.selectedCategories.splice(index, 1)
      }
    },
    clearSelection(state) {
      state.selectedCategories = []
    },
  },
})

export const { toggleCategory, clearSelection } = scanSelectionSlice.actions
export default scanSelectionSlice.reducer
