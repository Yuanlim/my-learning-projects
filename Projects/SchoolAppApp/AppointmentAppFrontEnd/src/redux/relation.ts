import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { PersonDataType } from "../types/Friend";
import { RootState } from "./store";
import { fetchData } from "../API/FetchAPI";
import { useAppDispatch } from "../hooks/useReduxHook";

export type RelationType = "Accepted" | "Pending" | "Block";

type removeUserFromListPayload = { AskFor: RelationType; UserId: number };

type moveUserFromListPayload = {
  AskFor: RelationType;
  To: RelationType;
  UserId: number;
};
type moveUserReturn = {
  To: RelationType;
  data: PersonDataType[];
};

// Block and |Accepted Pending| list are in different endpoints
// and fetch json are different types.
type getRelationPayload = { AskFor: RelationType };
type returnType = {
  AskFor: RelationType;
  data: PersonDataType[];
};

// A Personal User List Type
type RelationListType = {
  Accepted: PersonDataType[];
  Pending: PersonDataType[];
  Block: PersonDataType[];
  Loading: boolean;
};

// Initialize list
const initialState: RelationListType = {
  Accepted: [],
  Pending: [],
  Block: [],
  Loading: true,
};

/**
 * Returns after filter status list.
 *
 * @remarks
 * Revoke this method to re/initialize user relation list
 * of either "Pending" "Accepted" "Block" list
 *
 * @param AskFor - Selection which to status type to remove
 * @returns AskFor && Personal filtered relation list in type PersonDataType[]
 *
 * @beta
 */
export const removeUserFromList = createAsyncThunk<
  returnType,
  removeUserFromListPayload,
  { state: RootState }
>("Relation/removeUserFromList", async (payload, { getState }) => {
  const listState = getState().relation[payload.AskFor];
  return {
    AskFor: payload.AskFor,
    data: listState.filter((ls) => ls.userId !== payload.UserId),
  };
});

/**
 * Returns after added new member status list.
 *
 * @remarks
 * Revoke this method to re/initialize user relation list
 * of either "Pending" "Accepted" "Block" list
 *
 * @param AskFor - Selection which to status type to remove
 * @returns AskFor && Personal new added relation list in type PersonDataType[]
 *
 * @beta
 */
export const moveUserFromList = createAsyncThunk<
  moveUserReturn,
  moveUserFromListPayload,
  { state: RootState }
>("Relation/moveUserFromList", (payload, { getState }) => {
  const dispatch = useAppDispatch();
  const target = getState().relation[payload.AskFor];
  const toList = getState().relation[payload.To];
  dispatch(removeUserFromList(payload));
  return { To: payload.To, data: { ...toList, target } };
});

/**
 * Returns the met condition of user and other user status list.
 *
 * @remarks
 * Revoke this method to re/initialize user relation list
 * of either "Pending" "Accepted" "Block" list
 *
 * @param AskFor - Selection which to reinitialize
 * @returns Personal relation list in type PersonDataType[]
 *
 * @beta
 */
export const getRelationList = createAsyncThunk<
  returnType,
  getRelationPayload,
  {
    state: RootState;
    rejectValue: { AskFor: RelationType; errorMessage: string };
  }
>("friend/getRelationList", async (payload, thunkAPI) => {
  try {
    const URL =
      payload.AskFor === "Block"
        ? `FriendShip/GetBlock`
        : `FriendShip/GetWithStatus?Status=${payload.AskFor}`;

    const { data, success } = await fetchData<null, string | PersonDataType[]>({
      URL: URL,
      method: "get",
      credentials: true,
      payload: null,
    });

    if (!success)
      return thunkAPI.rejectWithValue({
        AskFor: payload.AskFor,
        errorMessage: typeof data === "string" ? data : "Unhandle error!",
      });
    return { AskFor: payload.AskFor, data: (data as PersonDataType[]) ?? [] };
  } catch (error) {
    return thunkAPI.rejectWithValue({
      AskFor: payload.AskFor,
      errorMessage: error instanceof Error ? error.message : "Network error!",
    });
  }
});

export const relationSlice = createSlice({
  name: "friended",
  initialState,
  reducers: {
    setNewRelationList(
      state,
      action: PayloadAction<{ rel: RelationType; data: PersonDataType[] }>
    ) {
      action.payload["data"].forEach((p) =>
        state[action.payload["rel"]].push(p)
      );
    },
    setNewRelation(
      state,
      action: PayloadAction<{ rel: RelationType; data: PersonDataType }>
    ) {
      state[action.payload["rel"]].push(action.payload["data"]);
    },
    setLoading(state, action: PayloadAction<boolean>) {
      state["Loading"] = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(getRelationList.rejected, (state, action) => {
        const askFor = action.payload?.AskFor ?? action.meta.arg.AskFor;
        state[askFor] = [];
      })
      .addCase(getRelationList.fulfilled, (state, action) => {
        state[action.payload["AskFor"]] = action.payload.data;
      })
      .addCase(removeUserFromList.fulfilled, (state, action) => {
        state[action.payload["AskFor"]] = action.payload.data;
      })
      .addCase(moveUserFromList.fulfilled, (state, action) => {
        state[action.payload["To"]] = action.payload.data;
      });
  },
});

export const { setNewRelation, setNewRelationList, setLoading } =
  relationSlice.actions;

export default relationSlice.reducer;
