import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { fetchData } from "../API/FetchAPI";
import { useAppDispatch } from "../hooks/useReduxHook";
import { setPrompText } from "./generic";
import {
  LoadProductsPayload,
  ShoppingReturn,
} from "../types/Product";
import { ProductInitialType } from '../types/Product';

const initialState: ProductInitialType = {
  products: [],
  targetProducts: undefined,
};

// Load In ProductList
export const LoadIn = createAsyncThunk<
  ShoppingReturn[],
  LoadProductsPayload,
  { rejectValue: { errorMessage: string } }
>("Products/LoadIn", async (payload, thunkAPI) => {
  const URL = `Shopping/Product/GetList/${payload.SearchString ?? ""}
              &${payload.RequestExpandTimes}`;
  const { data, success } = await fetchData<
    LoadProductsPayload,
    ShoppingReturn[]
  >({
    URL: URL,
    method: "get",
    credentials: true,
  });
  if (success) {
    return data as ShoppingReturn[];
  } else
    return thunkAPI.rejectWithValue({
      errorMessage: (data as string) ?? "Unhandle Error",
    });
});

// Load In Current Looked Product
export const LoadInTarget = createAsyncThunk<
  ShoppingReturn,
  number,
  { rejectValue: { errorMessage: string } }
>("Products/LoadInTarget", async (productId, thunkAPI) => {
  const URL = `Shopping/Product/Get/${productId}`;
  const { data, success } = await fetchData<null, ShoppingReturn>({
    URL: URL,
    method: "get",
    credentials: true,
  });
  if (success) {
    return data as ShoppingReturn;
  } else
    return thunkAPI.rejectWithValue({
      errorMessage: (data as string) ?? "Unhandle Error",
    });
});

export const productSlice = createSlice({
  name: "product",
  initialState,
  reducers: {},
  extraReducers(builder) {
    builder
      .addCase(LoadIn.rejected, (_, action) => {
        const dispatch = useAppDispatch();
        dispatch(
          setPrompText(action.payload?.errorMessage ?? "Unhandle Error")
        );
      })
      .addCase(LoadIn.fulfilled, (state, action) => {
        state.products = action.payload;
      })
      .addCase(LoadInTarget.rejected, (_, action) => {
        const dispatch = useAppDispatch();
        dispatch(
          setPrompText(action.payload?.errorMessage ?? "Unhandle Error")
        );
      })
      .addCase(LoadInTarget.fulfilled, (state, action) => {
        state.targetProducts = action.payload;
      });
  },
});

export default productSlice.reducer;
