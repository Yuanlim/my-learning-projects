const ApiRequest = async (
  url: string,
  options: RequestInit
): Promise<string | null> => {
  try {
    const reponse = await fetch(url, options);
    if (!reponse.ok) throw Error("Please reload the app");
  } catch (err) {
    if (err instanceof Error) {
      return err.message;
    } else {
      return String(err);
    }
  } finally {
    return null;
  }
};

export default ApiRequest;
