import React from "react";
import Nav from "./Nav";

type Props = {
  title: string;
  searchText: string;
  setSearchText: React.Dispatch<React.SetStateAction<string>>;
};

function Header({ title, searchText, setSearchText }: Props) {
  return (
    <header className="Header">
      <h1 className="header__h1">{title}</h1>
      <Nav searchText={searchText} setSearchText={setSearchText} />
    </header>
  );
}

export default Header;
