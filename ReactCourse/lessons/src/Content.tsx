import type { Character } from "./App";
import ListItems from "./ListItems";

function Content({
  characters,
  changeCombatState,
  eliminatedCharacter,
}: {
  characters: Character[];
  changeCombatState: (id: string) => void;
  eliminatedCharacter: (id: string) => void;
}) {
  return (
    <>
      {/* cluster component with many files for more organized code */}
      <ListItems
        characters={characters}
        changeCombatState={changeCombatState}
        eliminatedCharacter={eliminatedCharacter}
      />
      <p
        style={
          characters.length === 0 ? { display: "block" } : { display: "none" }
        }
      >
        Empty list
      </p>
    </>
  );
}

export default Content;

// Lesson 5 - onClink, onDoubleClick
// const getRandName = (): string => {
//     const name: string[] = ["Yuan", "Dave", "Charlie"];
//     const target: number = Math.floor(Math.random() * name.length);
//     return name[target];
//   };

//   const clickHandler = ():void => {
//     console.log("You clicked it")
//   }

//   const clickHandlerWithParam = (name:string):void => {
//     console.log(`${name} clicked it`)
//   }

//   const clickHandlerWithEvent = (e:React.MouseEvent<HTMLButtonElement>):void => {
//     console.log(e.currentTarget.innerText)
//   }

//   return (
//     <main>
//       <p>Hi, {getRandName()}!</p>
//       <button onClick={() => clickHandler()}>Click it!</button>
//       {/* <input type="text" name="name" id="user" /> */}
//       <button onClick={() => clickHandlerWithParam("Yuan")}>Click it!</button>
//       <button onClick={(e) => clickHandlerWithEvent(e)}>Click it!</button>

//     </main>
//   );

// Lesson 6: useState()
// const [name, setName] = useState("Yuan");
//   const [counter, setCounter] = useState(0);

//   const getRandName = ():void => {
//     const name: string[] = ["Yuan", "Dave", "Charlie"];
//     const target: number = Math.floor(Math.random() * name.length);
//     setName(name[target]);
//   };

//   const counterIncrement = ():void => {
//     setCounter(counter + 1); // counter is always the same when enter so both + 1 gets you the same result.
//     setCounter(counter + 1);
//     console.log(counter) // Will show original counter value
//   }

//   const getCurrentCounter = ():void => {
//     console.log(counter)
//   }

//   return (
//     <main>
//       <p>Hi, {name}!</p>
//       <button onClick={getRandName}>Change name!</button>
//       <p>Counter: {counter}</p>
//       <button onClick={counterIncrement}>Increase it!</button>
//       <button onClick={getCurrentCounter}>Show it!</button>

//     </main>
//   );

// Lesson 7: list
// const Characters = [
//   { id: 1, name: "Steve", Attack: 50, Speed: 50, isInCombat: false },
//   { id: 2, name: "Alex", Attack: 50, Speed: 50, isInCombat: true },
//   { id: 3, name: "Notch", Attack: 50, Speed: 50, isInCombat: false },
// ];

// const [character, setCharacter] = useState(Characters);

// const changeCombatState = (id: number): void => {
//   setCharacter(
//     character.map((e) =>
//       e.id === id ? { ...e, isInCombat: !e.isInCombat } : e
//     )
//   );

//   localStorage.setItem("characterInfo", JSON.stringify(character));
// };

// const eliminatedCharacter = (id: number): void => {
//   setCharacter(character.filter((c) => c.id !== id));
// };

// return (
//   <main>
//     <ul className="main__ul">
//       {character.map((e) => (
//         <li
//           className="ul__li"
//           key={e.id}
//           style={
//             e.isInCombat
//               ? { backgroundColor: "red" }
//               : { backgroundColor: "lightgreen" }
//           }
//         >
//           <input
//             type="checkbox"
//             checked={e.isInCombat}
//             onChange={() => changeCombatState(e.id)}
//           />
//           <label
//             style={
//               e.isInCombat ? { textDecoration: "line-through" } : undefined
//             }
//           >
//             {e.name}
//           </label>
//           <FaSkull
//             role="button"
//             onClick={() => eliminatedCharacter(e.id)}
//             aria-label={`Delete ${e.name}`}
//           />
//         </li>
//       ))}
//     </ul>
//   </main>
// );
