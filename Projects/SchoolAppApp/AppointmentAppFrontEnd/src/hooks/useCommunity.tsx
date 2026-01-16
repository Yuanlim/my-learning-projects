import { useEffect, useState } from "react";
import {
  OrderByType,
  PostPayload,
  PostType,
  ReplyPayload,
  ThumbUpPayload,
} from "../types/Community";

// import { setPrompText } from "../redux/generic";
import { OrderingBy } from "../types/Other";
import { fetchData } from "../API/FetchAPI";


function useCommunity() {
  const [posts, setPosts] = useState<PostType[]>([]);
  const [stepAmount, setStepAmount] = useState<number>(1); // TODO
  const [orderBy, setOrderBy] = useState<OrderByType>("Date");
  const [searchString, setSearchString] = useState<string>("");
  const [postContent, setPostContent] = useState<string>("");
  const [orderingBy, setOrderingBy] = useState<OrderingBy>("asc");
  const [nextRefresh, setNextRefresh] = useState<number>(0);


  useEffect(() => {
    const fetch = async () => {
      const URL = `Community/GetMainDiscussion?orderBy=${orderBy}&ordering=${orderingBy}&stepAmount=${stepAmount}&searchString=${searchString}`;
      const { data } = await fetchData<undefined, PostType[]>(
        { URL: URL, method: "get", credentials: true, payload: undefined }
      );
      setPosts(data as PostType[]);
    };

    fetch();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [orderBy, stepAmount, orderingBy, searchString, nextRefresh]);

  const handleOrderBy = (): void =>
    orderBy === "Date" ? setOrderBy("ThumbsUp") : setOrderBy("Date");

  const handleOrderingBy = (): void =>
    orderingBy === "asc" ? setOrderingBy("desc") : setOrderingBy("asc");

  const handleAddPost = async (): Promise<void> => {
    const URL = `Community/MainPost/Post`;
    const { success } = await fetchData<PostPayload, void>(
      { URL: URL, method: "get", credentials: true, payload: { Content: postContent } }
    );
    setPostContent("");
    if (success) setNextRefresh((curr) => curr + 1);
  };

  const handleReply = async (payload: ReplyPayload): Promise<void> => {
    const URL = `Community/ReplyPost/Post`;
    const { success } = await fetchData<ReplyPayload, void>(
      { URL: URL, method: "get", credentials: true, payload: payload }
    );
    if (success) setNextRefresh((curr) => curr + 1);
  };

  const handleThumbs = async (payload: ThumbUpPayload) => {
    const URL = `Community/ThumbsUp/MainPost/`;
    const { success } = await fetchData<ThumbUpPayload, void>(
      { URL: URL, method: "get", credentials: true, payload: payload }
    );
    if (success) setNextRefresh((curr) => curr + 1);
  }

  const handleSearch = (search: string) => setSearchString(search);


  return {
    posts, orderBy, handleOrderBy,
    handleAddPost, postContent, setPostContent,
    orderingBy, handleOrderingBy, handleSearch,
    handleReply, handleThumbs
  };
}

export default useCommunity;
