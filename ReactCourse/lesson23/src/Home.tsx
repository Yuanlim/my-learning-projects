import Feed from "./Feed";
import { useAppSelector } from "./hooks/useReduxHooks";
import { searchResult } from "./redux/post";

type Props = {
  fetchError: string | null;
  isLoading: boolean;
};

function Home({ fetchError, isLoading }: Props) {
  const result = useAppSelector((state) => searchResult(state));
  // console.log(result);

  return (
    <main className="main">
      {isLoading && <p>Loading...</p>}
      {!isLoading && fetchError && <p style={{ color: "red" }}>{fetchError}</p>}
      {!fetchError &&
        !isLoading &&
        (result.length ? (
          result.map((post) => <Feed post={post} key={post.id} />)
        ) : (
          <p>No post to be shown here.</p>
        ))}
    </main>
  );
}

export default Home;
