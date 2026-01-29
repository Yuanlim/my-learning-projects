import React from "react";

export const Header = ({ title = "Defualt title" }) => {
  return (
    <header>
      <h1>{title}</h1>
    </header>
  );
};
