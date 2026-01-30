import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { RootState } from "./store";
import { loginPayload } from "../types/LoR";
import { fetchData } from "../API/FetchAPI";

export type RoleType =
  | "teacher"
  | "admin"
  | "schoolPrinciple"
  | "student"
  | null;

export interface UserDataType {
  id: string;
  role: RoleType;
  email: string;
  authorized: boolean;
}

const initialState: UserDataType = {
  id: "",
  role: null,
  email: "",
  authorized: false,
};

export const checkAuthorized = createAsyncThunk<
  // Return Type
  UserDataType,
  // Input Type
  void,
  { state: RootState }
>("login/auth", async (_, thunkAPI) => {
  const Authorize_API = "auth/me";
  const { data, success } = await fetchData<null, UserDataType>({
    URL: Authorize_API,
    method: "get",
    credentials: true,
    payload: null,
  });
  if (!success) return thunkAPI.rejectWithValue("Expires or Not valid cookies");
  return { ...(data as UserDataType), authorized: true };
});

export const requestLogin = createAsyncThunk<
  { success: boolean },
  loginPayload,
  { state: RootState; rejectValue: { errorMessage: string } }
>("login/req", async (payload, thunkAPI) => {
  const loginApiUrl = "login";
  const { success, data } = await fetchData<loginPayload>({
    URL: loginApiUrl,
    method: "post",
    credentials: true,
    payload: payload,
  });
  if (!success)
    return thunkAPI.rejectWithValue({
      errorMessage: typeof data === "string" ? data : "Invalid Id or Password",
    });
  return { success: success };
});

export const loginSlice = createSlice({
  name: "login",
  initialState,
  reducers: {
    setId(state, action: PayloadAction<string>) {
      state.id = action.payload;
    },
    setRole(state, action: PayloadAction<RoleType>) {
      state.role = action.payload;
    },
    setEmail(state, action: PayloadAction<string>) {
      state.email = action.payload;
    },
    setAuth(state, action: PayloadAction<boolean>) {
      state.authorized = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(checkAuthorized.rejected, (state) => {
        state.authorized = false;
      })
      .addCase(checkAuthorized.fulfilled, (state, action) => {
        return action.payload;
      })
      .addCase(requestLogin.rejected, (state) => {
        state.authorized = false;
      });
  },
});

export const { setId, setRole, setEmail, setAuth } = loginSlice.actions;

export default loginSlice.reducer;
