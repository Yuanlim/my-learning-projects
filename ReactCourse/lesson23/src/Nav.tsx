import React from "react";
import { Link } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "./hooks/useReduxHooks";
import { setSearchText } from "./redux/post";

function Nav() {
  const searchText = useAppSelector((state) => state.posts.searchText);
  const dispatch = useAppDispatch();

  return (
    <nav className="nav">
      <form
        onSubmit={(e) => {
          e.preventDefault();
        }}
        className="nav__form"
      >
        <div className="form__searchBox__container">
          <input
            type="search"
            name="searchText"
            id="searchMatchContext"
            className="SearchBox"
            value={searchText}
            onChange={(e) => dispatch(setSearchText(e.target.value))}
            placeholder="Search"
          />
        </div>
        <div className="form__button__container">
          <Link className="form__button" to="/">
            Home
          </Link>
          <Link className="form__button" to="/post">
            Post
          </Link>

          <Link className="form__button" to="/about">
            About
          </Link>
        </div>
      </form>
    </nav>
  );
}

export default Nav;
