import { useEffect, useState } from "react";
import { PostType } from "../types/posts";
import axios, { AxiosError } from "axios";

function useAxiosFetch(dataUrl: string) {
  /* React tracking state */
  const [data, setData] = useState<PostType[]>([]);
  const [fetchError, setFetchError] = useState<string | null>("");
  const [isLoading, setIsLoading] = useState<boolean>(true);

  useEffect(() => {
    let isMounted = true; // Middle operation, possible refreshed cut the execution immediately.
    const source = axios.CancelToken.source(); // Create a cancellation token for this instance of requesting. While in the middle of requesting data, possible logout or refresh cancel it immediately.
    const fetchData = async (url: string) => {
      setIsLoading(true);

      try {
        const response = await axios.get(url, { cancelToken: source.token });
        if (isMounted) {
          setData(response.data);
          setFetchError(null);
        }
      } catch (error) {
        if (isMounted) {
          if (error instanceof (Error || AxiosError))
            setFetchError(error.message);
          setData([]);
        }
      } finally {
        isMounted && setTimeout(() => setIsLoading(false), 2000);
      }
    };

    fetchData(dataUrl);

    return () => {
      isMounted = false;
      source.cancel();
    };
  }, [dataUrl]);

  return { data, fetchError, isLoading };
}

export default useAxiosFetch;
