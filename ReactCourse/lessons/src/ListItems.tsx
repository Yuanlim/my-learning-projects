import React from "react";
import { Character } from "./App";
import LineItems from "./LineItems";

const ListItems = ({
  characters,
  changeCombatState,
  eliminatedCharacter,
}: {
  characters: Character[];
  changeCombatState: (id: string) => void;
  eliminatedCharacter: (id: string) => void;
}) => {
  return (
    <ul className="main__ul">
      {characters.map((c) => (
        <LineItems
          key={c.id}
          character={c}
          changeCombatState={changeCombatState}
          eliminatedCharacter={eliminatedCharacter}
        />
      ))}
    </ul>
  );
};

export default ListItems;
