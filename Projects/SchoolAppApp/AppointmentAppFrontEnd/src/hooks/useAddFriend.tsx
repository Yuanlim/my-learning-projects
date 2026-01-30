import { useEffect, useState } from 'react';

import { useAppDispatch, useAppSelector } from './useReduxHook';
import { setPrompText } from '../redux/generic';
import { BlockCreatedType, BlockRequestPayload, PersonDataType, StatusRequestPayload } from '../types/Friend';
import { fetchData } from '../API/FetchAPI';

function useAddFriend() {
  const [searchId, setSearchId] = useState<string>("");
  const [personInfo, setPersonInfo] = useState<PersonDataType | null>(null);
  const dispatch = useAppDispatch();
  const { id } = useAppSelector((state) => state.login)

  // Load When Start
  useEffect(() => {

  }, [])

  // Handle when searching for new friend.
  const handleSearch = async () => {
    // check search yourself || null, immediately return string 
    if (searchId === id) { dispatch(setPrompText("You cant search Yourself")); return; }
    if (!searchId) { dispatch(setPrompText("Nothing was inputed")); return; }

    const URL = `FriendShip/FindFriend?id=${searchId}`;
    const { data, success } = await fetchData<undefined, PersonDataType>(
      { URL: URL, method: "get", credentials: true, payload: undefined }
    );
    if (!success) { dispatch(setPrompText(data as string)); return; }
    else setPersonInfo(data as PersonDataType);
  }

  // TYPE GRUAD
  function isStatusRequest(x: StatusRequestPayload | BlockRequestPayload): x is StatusRequestPayload {
    return "Frps" in x;
  }

  // Request Change of Relation status between User
  const handleRequest = async (payload: StatusRequestPayload | BlockRequestPayload) => {
    const URL = isStatusRequest(payload) ? "FriendShip/Send" : "Common/Block";
    console.log(payload);
    const { data, success } = await fetchData<StatusRequestPayload | BlockRequestPayload, string | BlockCreatedType>(
      { URL: URL, method: "post", credentials: true, payload: payload }
    );
    if (typeof data === "string") dispatch(setPrompText(data));
    else if (typeof data === "object")
      dispatch(
        setPrompText(
          data?.blocked ? "Blocked" : "Something when wrong....")
      );
    return success;
  }

  return { searchId, setSearchId, personInfo, handleSearch, handleRequest }
}

export default useAddFriend