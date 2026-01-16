// Convert type
let aBoolean:boolean = true;
let aNumber:number = 0;
enum sugarLevel {No, Less, Full};
let aString:string = "5";
let aAny = {};

let toNumbers = Number(aString);
let toStrings = String(aNumber);
console.log("String type:" + aString + " to number type: " + toNumbers)
console.log("Number type:" + aNumber + " to number type: " + toStrings)
console.log(Number(aBoolean))

// Convert to more or less specific type
type One = string;
type Two = string|number;
type Three = 'hello';

let a: One = "hello";
let b = a as Two; // uneccessary -> less specific type 
let c = a as Three; // Usage: narrow type in runtime -> more specific

let d = <One>"world";
let e = <string | number>"world";

// as usage: promise compiler the return type
const addOrConcat = (a:number, b:number, c: "add" | "concat") 
:string|number => {
    if(c === "add") return a + b;
    return '' + a + b;
}

let myVal:string = addOrConcat(2, 2, 'concat') as string
let myNextVal:number = addOrConcat(2, 2, 'add') as number

// Becareful! you promise Ts, then ts sees no problem
// but sometimes it is wrong.
let myWrongVal:number = addOrConcat(2, 2, 'concat') as number

// The DOM
const img = document.querySelector("img")! // ! -> non-null assertion

// Both works
const myImg = document.getElementById("#imgId") as HTMLImageElement
const nextImg = <HTMLImageElement>document.getElementById("#imgId")

img.src
myImg.src