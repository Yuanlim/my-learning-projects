import colorNames from "colornames";
import React from "react";

type Props = {
  bgcColor: string;
  hexColor: string;
  isDarkBackground: boolean;
  setBgcColor: React.Dispatch<React.SetStateAction<string>>;
  setHexColor: React.Dispatch<React.SetStateAction<string>>;
  checkBrightness: (hexCode: string | undefined) => void;
};

function Main({
  bgcColor,
  hexColor,
  isDarkBackground,
  setBgcColor,
  setHexColor,
  checkBrightness,
}: Props) {
  return (
    <main>
      <div
        className="main__div"
        style={{
          backgroundColor: bgcColor,
          color: isDarkBackground ? "whitesmoke" : "black",
        }}
      >
        <p>{bgcColor ? bgcColor : "Empty Value"}</p>
        <p>{hexColor}</p>
      </div>
      <form className="main__form" onSubmit={(e) => e.preventDefault()}>
        <label htmlFor="changeColor" className="out">
          Change Color
        </label>
        <input
          autoFocus
          type="text"
          name="bgcColor"
          id="changeColor"
          className="form__input"
          value={bgcColor}
          placeholder="Input Color"
          onChange={(e) => {
            setBgcColor(e.target.value);
            setHexColor(colorNames(e.target.value)!);
            checkBrightness(colorNames(e.target.value)!);
          }}
        />
      </form>
    </main>
  );
}

export default Main;
