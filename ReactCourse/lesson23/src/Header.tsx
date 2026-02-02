import React from "react";
import Nav from "./Nav";
import { FaLaptop, FaTabletAlt, FaMobileAlt } from "react-icons/fa";
import useWindowSize from "./hooks/useWindowSize";

type Props = {
  title: string;
};

function Header({ title }: Props) {
  const { width } = useWindowSize();

  return (
    <header className="Header">
      <div className="header__title__container">
        <h1 className="header__h1">{title}</h1>
        {width < 432 ? (
          <FaMobileAlt className="titleIcon" />
        ) : width < 900 ? (
          <FaTabletAlt className="titleIcon" />
        ) : (
          <FaLaptop className="titleIcon" />
        )}
      </div>
      <Nav />
    </header>
  );
}

export default Header;
