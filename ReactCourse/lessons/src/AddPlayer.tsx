import React from "react";
import { useRef } from "react";
import { MdAdd } from "react-icons/md";

type Props = {
  playerName: string;
  setPlayerName: React.Dispatch<React.SetStateAction<string>>;
  submitHandler: (e: React.FormEvent<HTMLFormElement>) => void;
};

function AddPlayer({ playerName, setPlayerName, submitHandler }: Props) {
  const inputRef = useRef<HTMLInputElement>(null);

  return (
    <form className="form" onSubmit={(e) => submitHandler(e)}>
      <label htmlFor="addItem" className="out">
        AddItem
      </label>
      <input
        type="text"
        name="player"
        id="addItem"
        ref={inputRef}
        className="form__inputText"
        placeholder="Item name"
        value={playerName}
        required
        onChange={(e) => setPlayerName(e.target.value)}
      />
      <button
        type="submit"
        className="addItem__submit"
        aria-label="Add Item"
        onClick={() => inputRef.current!.focus()}
      >
        <MdAdd className="addItem__icon" />
      </button>
    </form>
  );
}

export default AddPlayer;
