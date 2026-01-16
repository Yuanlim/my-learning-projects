import { Link } from "react-router-dom"
import useCheckDirectAccessor from "./hooks/useCheckDirectAccessor"
import { useAppSelector } from "./hooks/useReduxHook";
import { useEffect } from "react";
import { usePendingContext } from "./hooks/useContext";

const Friend = () => {
  useCheckDirectAccessor();
  const { handleReFetchPending } = usePendingContext();
  const list = useAppSelector((state) => state.relation["Pending"]);

  useEffect(() => {
    handleReFetchPending();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);


  return (
    <main className='main card FriendPage__Main'>
      <Link role="button" to="/Pending"
        className="card contextCenter FriendPage__Button asButton"
        style={{ position: "relative" }} // Notify pending friend numbers
      >
        <p
          className="flex contextCenter"
          style={{
            backgroundColor: "red", width: "30px", height: "30px",
            position: "absolute", top: "0", right: "0",
            borderRadius: "50%"
          }}
        >
          {list.length} {/* ToDo: make it pending length */}
        </p>
        Pending Request.
      </Link>
      <Link role="button" to="/AddFriend"
        className="card contextCenter FriendPage__Button asButton"
      >
        Add Friend.
      </Link>
      <Link role="button" to="/Accepted"
        className="card contextCenter FriendPage__Button asButton"
      >
        Accepted Friends.
      </Link>
      <Link role="button" to="/Block"
        className="card contextCenter FriendPage__Button asButton"
      >
        Blocked Users.
      </Link>
    </main >
  )
}

export default Friend