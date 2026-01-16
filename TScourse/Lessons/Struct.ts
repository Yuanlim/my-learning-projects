// think as structure + must initialize & static
const exampleOBj = {
    prop1: "Dave",
    prop2: true,
}

// Type aliases

// function return parameter type and return type contract 
type SpeakFunction = () => string;

// A union define an often used union type contract
type stringOrNumber = string | number;
type stringOrNumberArr = (string | number)[];

type Character = {
    readonly name: string, // when declared cannot be reassign
    AttackPower: number,
    Hp: number|string,
    Moves?: string[], // optional
    Dead: boolean,
    Speak: SpeakFunction
};

const magician: Character = {
    name: "Sam",
    AttackPower: 100,
    Hp: 105,
    Moves: ["Water Gun", "Shield"],
    Dead: false,
    Speak(): string {
        return "Greetings";
    }
};

// magician.name = "Tom" // illegal
console.log(magician.name + ": " + magician.Speak());

const greetCharacter = (character: Character) => {
    return `Hello ${character.name}!`; // important ts use backticks `` as format string
}

const characterNumMoves = (character: Character) => {
    if (character.Moves != null)
    {
        return character.Moves.length;
    }
    return "No moves"
}

console.log(greetCharacter(magician));
console.log(characterNumMoves(magician));