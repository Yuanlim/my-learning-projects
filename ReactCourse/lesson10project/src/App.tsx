import React, { useState } from "react";
import Main from "./Main";

function App() {
  const [bgcColor, setBgcColor] = useState("");
  const [hexColor, setHexColor] = useState("");
  const [isDarkBackground, setIsDarkBackground] = useState(false);

  const checkBrightness = (hexCode: string | undefined) => {
    if (hexCode) {
      // is not empty
      const color: number[] = [
        parseInt(hexCode.substring(1, 3), 16),
        parseInt(hexCode.substring(3, 5), 16),
        parseInt(hexCode.substring(5, 7), 16),
      ];
      const brightness: number =
        0.2126 * color[0] + 0.7152 * color[1] + 0.0722 * color[2];
      console.log(brightness);
      if (brightness < 128) {
        // dark
        setIsDarkBackground(true);
      } else {
        setIsDarkBackground(false);
      }
    } else {
      setIsDarkBackground(false);
    }
  };

  return (
    <Main
      bgcColor={bgcColor}
      hexColor={hexColor}
      isDarkBackground={isDarkBackground}
      setBgcColor={setBgcColor}
      setHexColor={setHexColor}
      checkBrightness={checkBrightness}
    />
  );
}

export default App;
