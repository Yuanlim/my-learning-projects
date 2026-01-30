import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export interface GenericDataType {
  prompText: string;
}

const initialState: GenericDataType = {
  prompText: "",
};

export const genericSlice = createSlice({
  name: "generic",
  initialState,
  reducers: {
    setPrompText(state, action: PayloadAction<string>) {
      state.prompText = action.payload;
    },
  },
});

export const { setPrompText } = genericSlice.actions;

export default genericSlice.reducer;
