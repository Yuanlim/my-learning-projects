import React from "react";
import { Character } from "./App";
import { FaSkull } from "react-icons/fa";

function LineItems({
  character,
  changeCombatState,
  eliminatedCharacter,
}: {
  character: Character;
  changeCombatState: (id: string) => void;
  eliminatedCharacter: (id: string) => void;
}) {
  return (
    <li
      className="ul__li"
      key={character.id}
      style={
        character.isInCombat
          ? { backgroundColor: "red" }
          : { backgroundColor: "lightgreen" }
      }
    >
      <input
        type="checkbox"
        checked={character.isInCombat}
        onChange={() => changeCombatState(character.id)}
      />
      <label
        style={
          character.isInCombat ? { textDecoration: "line-through" } : undefined
        }
      >
        {character.name}
      </label>
      <FaSkull
        role="button"
        onClick={() => eliminatedCharacter(character.id)}
        aria-label={`Delete ${character.name}`}
      />
    </li>
  );
}

export default LineItems;
