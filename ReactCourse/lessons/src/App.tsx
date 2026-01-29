import React, { useEffect, useState } from "react";
import { Header } from "./Header";
import Content from "./Content";
import Footer from "./Footer";
import AddPlayer from "./AddPlayer";
import SearchPlayer from "./SearchPlayer";
import ApiRequest from "./ApiRequest";

export type Character = {
  id: string;
  name: string;
  attack: number;
  speed: number;
  isInCombat: boolean;
};

function App() {
  const API_URL = "http://localhost:3500/characters";
  const headerType = { "Content-type": "appication/json" };

  const [characters, setCharacter]: [
    Character[],
    React.Dispatch<React.SetStateAction<Character[]>>
  ] = useState(() => {
    const stored = localStorage.getItem("CharacterInfo");
    const json = stored ?? "[]";
    return JSON.parse(json) as Character[];
  });

  const [playerName, setPlayerName]: [
    string,
    React.Dispatch<React.SetStateAction<string>>
  ] = useState("");

  const [searchText, setSearchText]: [
    string,
    React.Dispatch<React.SetStateAction<string>>
  ] = useState("");

  const [fetchError, setFetchError] = useState("");

  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    localStorage.setItem("CharacterInfo", JSON.stringify(characters));
  }, [characters]); // [] list of dependencies will trigger this effect

  useEffect(() => {
    const fetchData = async () => {
      try {
        const reponse = await fetch(API_URL);
        if (!reponse.ok) throw Error("Exception: Data is not received.");
        const json = await reponse.json();
        setCharacter(json);
        setFetchError("");
      } catch (err: unknown) {
        if (err instanceof Error) setFetchError(err.message);
        else setFetchError(String(err));
      } finally {
        setIsLoading(false);
      }
    };

    setTimeout(async () => {
      await fetchData();
    }, 2000);
  }, []);

  const submitHandler = async (
    e: React.FormEvent<HTMLFormElement>
  ): Promise<void> => {
    console.log("hi");
    e.preventDefault();
    const newPlayerId: string = characters.length
      ? String(Number(characters[characters.length - 1].id) + 1)
      : String(1);
    const newPlayer: Character = {
      id: newPlayerId,
      name: playerName,
      attack: 50,
      speed: 50,
      isInCombat: false,
    };

    setCharacter([...characters, newPlayer]);
    setPlayerName("");

    const options: RequestInit = {
      method: "POST",
      headers: headerType,
      body: JSON.stringify(newPlayer),
    };

    const err = await ApiRequest(API_URL, options);
    if (err) setFetchError(err);
  };

  const changeCombatState = async (id: string): Promise<void> => {
    setCharacter(
      characters.map((e) =>
        e.id === id ? { ...e, isInCombat: !e.isInCombat } : e
      )
    );

    const target = characters.find((c) => c.id === id);
    if (!target) return;
    const updateTarget = { ...target, isInCombat: !target.isInCombat };

    const options: RequestInit = {
      method: "PATCH",
      headers: headerType,
      body: JSON.stringify(updateTarget),
    };
    const url: string = `${API_URL}/${id}`;
    console.log(url);
    const err = await ApiRequest(url, options);
    if (err) setFetchError(err);
  };

  const eliminatedCharacter = async (id: string): Promise<void> => {
    setCharacter(characters.filter((c) => c.id !== id));

    const options: RequestInit = { method: "DELETE", headers: headerType };
    const url: string = `http://localhost:3500/characters/${id}`;

    const err = await ApiRequest(url, options);
    if (err) setFetchError(err);
  };

  return (
    <div className="App">
      <Header title="Hello" />
      <SearchPlayer searchText={searchText} setSearchText={setSearchText} />
      <AddPlayer
        playerName={playerName}
        setPlayerName={setPlayerName}
        submitHandler={submitHandler}
      />
      <main>
        {isLoading && <p>Loading...</p>}
        {fetchError && <p style={{ color: "red" }}>{fetchError}</p>}
        {!isLoading && !fetchError && (
          <Content
            characters={characters.filter((c) =>
              c.name.toLowerCase().includes(searchText.toLowerCase())
            )}
            changeCombatState={changeCombatState}
            eliminatedCharacter={eliminatedCharacter}
          />
        )}
      </main>
      <Footer characters={characters} />
    </div>
  );
}

export default App;

// Lesson 1 code

// const name:string = "Yuan";
//   const add = (a:number, b:number): number => {
//     return a + b;
//   }
//   const threeisBTFour:boolean = 3 > 4 ? true : false;
//   type Grade = "A" | "B";
//   interface student {
//     name:string;
//     grade:Grade;
//     GetName():string;
//   }
//   const Alice:student = {name:"Alice", grade:"A", GetName() {
//     return this.name;
//   }};
//   const randPerson = ():string => {
//     const nameList:string[] = ["Yuan", "Dave", "John", "Joker"];
//     const randomName:string = nameList[Math.floor(Math.random() * nameList.length)]
//     return randomName;
//   }

//   return (
//     <div className="App">
//       <header className="App-header">
//         <img src={logo} className="App-logo" alt="logo" />
//         <p>
//           Hello {name}!
//         </p>
//         <a
//           className="App-link"
//           href="https://reactjs.org"
//           target="_blank"
//           rel="noopener noreferrer"
//         >
//           Learn React
//         </a>
//         <p>{add(6, 7)}, {Alice.GetName()}</p>
//         <p>{randPerson()}</p>
//       </header>
//     </div>
//   );

// Lesson 2 : component
/* 
  <div className="App">
    <Header />
    <Content />
    <Footer />
  </div> 
*/

// lesson 9: props
// function App() {
//   const [characters, setCharacter]: [
//     Character[],
//     React.Dispatch<React.SetStateAction<Character[]>>
//   ] = useState([
//     { id: 1, name: "Steve", attack: 50, speed: 50, isInCombat: false },
//     { id: 2, name: "Alex", attack: 50, speed: 50, isInCombat: true },
//     { id: 3, name: "Notch", attack: 50, speed: 50, isInCombat: false },
//   ]);

//   const [playerName, setPlayerName]: [
//     string,
//     React.Dispatch<React.SetStateAction<string>>
//   ] = useState("");

//   const [searchText, setSearchText]: [
//     string,
//     React.Dispatch<React.SetStateAction<string>>
//   ] = useState("");

//   const submitHandler = (e: React.FormEvent<HTMLFormElement>): void => {
//     e.preventDefault();
//     const newPlayerId: number = characters.length
//       ? characters[characters.length - 1].id + 1
//       : 1;
//     setCharacter([
//       ...characters,
//       {
//         id: newPlayerId,
//         name: playerName,
//         attack: 50,
//         speed: 50,
//         isInCombat: false,
//       },
//     ]);
//     setPlayerName("");
//   };

//   const changeCombatState = (id: number): void => {
//     setCharacter(
//       characters.map((e) =>
//         e.id === id ? { ...e, isInCombat: !e.isInCombat } : e
//       )
//     );

//     localStorage.setItem("characterInfo", JSON.stringify(characters));
//   };

//   const eliminatedCharacter = (id: number): void => {
//     setCharacter(characters.filter((c) => c.id !== id));
//   };

//   return (
//     <div className="App">
//       <Header title="Hello" />
//       <SearchPlayer searchText={searchText} setSearchText={setSearchText} />
//       <AddPlayer
//         playerName={playerName}
//         setPlayerName={setPlayerName}
//         submitHandler={submitHandler}
//       />
//       <Content
//         characters={characters.filter((c) =>
//           c.name.toLowerCase().includes(searchText.toLowerCase())
//         )}
//         changeCombatState={changeCombatState}
//         eliminatedCharacter={eliminatedCharacter}
//       />
//       <Footer characters={characters} />
//     </div>
//   );
// }
