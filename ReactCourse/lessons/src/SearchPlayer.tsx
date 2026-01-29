import React from "react";

type Props = {
  searchText: string;
  setSearchText: React.Dispatch<React.SetStateAction<string>>;
};

function SearchPlayer({ searchText, setSearchText }: Props) {
  return (
    <form className="form">
      <input
        autoFocus
        type="text"
        role="searchbox"
        name="searchText"
        id="searchItem"
        className="form__inputText"
        value={searchText}
        placeholder="Search Player"
        onChange={(e) => setSearchText(e.target.value)}
      />
    </form>
  );
}

export default SearchPlayer;
