import Feed from "./Feed";
import useDataContext from "./hooks/useDataContext";

function Home() {
  const { searchResult, fetchError, isLoading } = useDataContext();

  return (
    <main className="main">
      {isLoading && <p>Loading...</p>}
      {!isLoading && fetchError && <p style={{ color: "red" }}>{fetchError}</p>}
      {!fetchError &&
        !isLoading &&
        (searchResult.length ? (
          searchResult.map((post) => <Feed post={post} key={post.id} />)
        ) : (
          <p>No post to be shown here.</p>
        ))}
    </main>
  );
}

export default Home;
