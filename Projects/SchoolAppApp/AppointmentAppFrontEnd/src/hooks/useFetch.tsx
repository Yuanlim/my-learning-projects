import { useState } from 'react'
import { useAppDispatch } from './useReduxHook'
import { setPrompText } from '../redux/generic';
import { API_URL } from '../App';

export type FetchDataType<P> = {
  URL: string,
  method: "get" | "post" | "put" | "delete",
  credentials: boolean,
  payload?: P
}

const useFetch = <R = unknown>() => {
  const [data, setData] = useState<R | null>(null);
  const dispatch = useAppDispatch();

  async function isJsonString(response: Response):
    Promise<{ ok: boolean; data: unknown; } | { ok: false }> {
    try {
      const data = await response.clone().json();
      return { ok: true, data };
    } catch (e) {
      return { ok: false };
    }
  }

  async function fetchData<P = unknown, Re = unknown>
    (args: FetchDataType<P>)
    : Promise<{ data: Re | null; success: boolean; }> {

    const { URL, method, credentials, payload } = args;
    const requestURL = API_URL + URL

    try {
      const response = await fetch(requestURL, {
        method: method,
        headers: { "Content-Type": "application/json" },
        credentials: credentials ? "include" : "omit",
        body: method === "get" || payload === undefined ? undefined : JSON.stringify(payload),
      });

      const canBeJson = await isJsonString(response);
      const dataOrErr = canBeJson.ok ? canBeJson.data : await response.text();
      if (!response.ok) {
        throw Error(
          dataOrErr as string || `Request failed with status ${response.status}`
        );
      }
      const data = dataOrErr as Re;
      setData(dataOrErr as R);
      return { data, success: true };
    } catch (error) {
      if (error instanceof Error) dispatch(setPrompText(error.message));
      else dispatch(setPrompText("error"));
      return { data: null, success: false };
    }
  }

  return { data, fetchData };
}

export default useFetch