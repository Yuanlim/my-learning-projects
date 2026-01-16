import React from "react";
import { Link } from "react-router-dom";
import { UserDataType } from "../redux/login";
import { useAppSelector } from "../hooks/useReduxHook";
import { LiaUserFriendsSolid } from "react-icons/lia";

const StudentNav = () => {
  const loginState: UserDataType = useAppSelector((state) => state.login);

  return (
    <nav
      className='title__nav'
      style={loginState.authorized ? { display: "flex" } : { display: "none" }}
    >
      <Link role="button" to="/Community">
        Community
      </Link>
      <Link role="button" to="/Chat">
        Chat
      </Link>
      <Link role="button" to="/Friend" className="header_container_with_icon withTip">
        <LiaUserFriendsSolid />
        <span
          className="tooltip down"
          style={{ left: "-300%" }}
        >
          Check or Add Friend
        </span>
      </Link>
      <Link role="button" to="/Self">
        {"Hi! " + loginState.id}
      </Link>
    </nav>
  );
};

export default StudentNav;
