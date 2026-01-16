// lesson 2 : arrays and objects
let stringArr = ["One", "Two", "Three"];
let mixed2typesArr = ["Hi", "Hello", true];
let mixed3typesArr = ["Hi", 5, true];
let anyTypeArr = [];
let explicitTypesArr: string[] = [];
// tuple -> when declared indicies type is fixed and should be "fixed array"
let tupleTypesArr: [boolean, string] = [true, ""]

// // illegal
// mixed2typesArr.push(6) // not included type when declared

mixed3typesArr.push(6) // append at the end of list
let arrayLenght = mixed3typesArr.unshift(5) // append at the top of the list
stringArr[0] = "zero" // indicies assign

mixed2typesArr = stringArr; // works because has string types

// tupleTypesArr[2] = true // illegal
tupleTypesArr.push("")
// tupleTypesArr[2] = "Redifined";

// Object
let myObj: object;
myObj = []
console.log(typeof myObj)
myObj = {}

// think as structure + must initialize & static
const exampleOBj = {
    prop1: "Dave",
    prop2: true,
}