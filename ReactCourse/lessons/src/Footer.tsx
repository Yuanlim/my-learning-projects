import React from "react";
import type { Character } from "./App";

function Footer({ characters }: { characters: Character[] }) {
  // const thisYear: number = new Date().getFullYear();
  return (
    <footer>
      <p>{characters.length} List items</p>
    </footer>
  );
}

export default Footer;
