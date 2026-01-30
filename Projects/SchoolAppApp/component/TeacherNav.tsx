import React from 'react'
import { UserDataType } from '../redux/login';
import { useAppSelector } from '../hooks/useReduxHook';
import { Link } from 'react-router-dom';
import { IoBookOutline } from "react-icons/io5";
import { IoCartOutline } from "react-icons/io5";

const TeacherNav = () => {
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
      <Link role="button" to="/StudyMat" className="header_container_with_icon">
        <IoBookOutline className='header__icon' />
      </Link>
      <Link role="button" to="/Shopping" className="header_container_with_icon">
        <IoCartOutline className='header__icon' />
      </Link>
      <Link role="button" to="/Self" className="header_container_with_icon">
        {/* <FaChalkboardTeacher className='header__icon'/> */}
        <p style={{ fontSize: "1rem" }}>{"Hi! " + loginState.id}</p>
      </Link>
    </nav>
  );
}

export default TeacherNav