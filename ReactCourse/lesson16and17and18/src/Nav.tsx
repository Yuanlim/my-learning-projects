import React from "react";
import { Link } from "react-router-dom";

type Props = {
  searchText: string;
  setSearchText: React.Dispatch<React.SetStateAction<string>>;
};

function Nav({ searchText, setSearchText }: Props) {
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
            onChange={(e) => setSearchText(e.target.value)}
            placeholder="Search"
          />
        </div>
        <div className="form__button__container">
          <button type="submit" className="form__button">
            <Link className="button__link" to="/">
              Home
            </Link>
          </button>
          <button type="submit" className="form__button">
            <Link className="button__link" to="/post">
              Post
            </Link>
          </button>
          <button type="submit" className="form__button">
            <Link className="button__link" to="/about">
              About
            </Link>
          </button>
        </div>
      </form>
    </nav>
  );
}

export default Nav;
