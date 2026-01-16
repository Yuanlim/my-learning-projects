import { API_URL } from "../App";

export type FetchDataType<P> = {
  URL: string;
  method: "get" | "post" | "put" | "delete";
  credentials: boolean;
  payload?: P;
};

export type ErrorType = {
  title: string;
  message: string;
  statusCode: number;
  traceId: string;
};

async function isJsonString(
  response: Response
): Promise<{ ok: boolean; data: unknown } | { ok: false }> {
  try {
    const data = await response.clone().json();
    return { ok: true, data };
  } catch (e) {
    return { ok: false };
  }
}

export async function fetchData<P = unknown, Re = unknown>(
  args: FetchDataType<P>
): Promise<{ data: Re | string | null; success: boolean }> {
  const { URL, method, credentials, payload } = args;
  const requestURL = API_URL + URL;
  try {
    const response = await fetch(requestURL, {
      method: method,
      headers: { "Content-Type": "application/json" },
      credentials: credentials ? "include" : "omit",
      body:
        method === "get" || payload === undefined
          ? undefined
          : JSON.stringify(payload),
    });

    const canBeJson = await isJsonString(response);
    const dataOrErr = canBeJson.ok ? canBeJson.data : await response.text();
    if (!response.ok) {
      const errorMessage =
        (dataOrErr as ErrorType).message ||
        `Request failed with status ${response.status}`;
      return { data: errorMessage, success: false };
    }
    const data = dataOrErr as Re;
    return { data, success: true };
  } catch (error) {
    return { data: "", success: false };
  }
}
