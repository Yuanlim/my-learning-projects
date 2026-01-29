import React, { useEffect, useState } from "react";
import Header from "./Header";
import ListJsonData from "./ListJsonData";

function App() {
  const API_URL = "https://jsonplaceholder.typicode.com";
  const [request, setRequest] = useState("users");
  const [requestError, setRequestError] = useState("");
  const [data, setData] = useState<unknown>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchRequest = async () => {
      try {
        setRequestError("");
        const response = await fetch(`${API_URL}/${request}`);
        if (!response.ok)
          throw Error(
            `When try to load ${API_URL}/${request} data, it failed.`
          );
        const json = await response.json();
        setData(json);
      } catch (error) {
        if (error instanceof Error) setRequestError(error.message);
        else setRequestError(String(error));
      } finally {
        setIsLoading(false);
      }
    };

    setTimeout(async () => {
      await fetchRequest();
    }, 1000);
  }, [request]);

  const handleChangeUrl = (request: string): void => {
    setRequestError("");
    setIsLoading(true);
    setRequest(request);
  };

  return (
    <div>
      <Header currentRequest={request} handleChangeUrl={handleChangeUrl} />
      {isLoading && <p style={{ padding: "0.5rem" }}>Loading...</p>}
      {requestError && <p style={{ padding: "0.5rem" }}>{requestError}</p>}
      {!requestError && !isLoading && <ListJsonData data={data} />}
    </div>
  );
}

export default App;
