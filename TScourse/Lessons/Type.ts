// https://www.youtube.com/watch?v=gieEQFIfgYc
// lesson 1 : types

let myName: string;
let meaningOfLife: number;
let isLoading: boolean;
let album: any;
let unionType: string | number;
let regexpType : RegExp;

myName = "Yuan"
meaningOfLife = 5
isLoading = true
album = true
album = "Hello-Adell"
unionType = 7
unionType = "hmm"
regexpType = /^[a-zA-Z0-9._%+-]+(@gmail.com|@email.com)$/

const sum = (a: number, b: number) => {
    return a + b
}