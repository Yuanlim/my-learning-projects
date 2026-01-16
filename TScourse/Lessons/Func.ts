// Function -> both pass functions

const adder = (a: number) => (b: number) : number => {return a + b};

// First parameter
const adderWithaAs5 = adder(5);

// execute other code to determine b and give it after
let hp = Number(magician.Hp);
let b: number = hp > 100 ? 100 : hp

let results: number = adderWithaAs5(b)
console.log(results)

// Plain functions declaration
let subtract = function (c: number, d: number): number {
    return c - d;
}

// Organize using type since adder and subtract accepts the same types,
// and return the same type
type mathFunctions = (a: number, b: number) => number
const adderV2: mathFunctions = function (a, b) {
    return a + b;
}
const subV2: mathFunctions = function(a, b) {
    return a - b;
}

let a = 5
console.log(adderV2(a, b))
console.log(subV2(a, b))

// Optional parameters functions
const sum = (a: number, b: number, c?: number)
: number => {
    if(typeof c !== 'undefined') {
        return a + b + c;
    }
    return a + b
}

// With defualt value
const sumV2 = (a: number, b: number, c: number = 2) : number => {
    return a + b + c;
}

// Test 
console.log(sumV2(a, b));
console.log(sumV2(a, b, 5));

// Type conditional functions
type distanceType =
| {kind:"euc2d"; x:number; y:number} 
| {kind:"euc3d"; x:number; y:number; z:number}

// used in function
const distanceCal = function (params:distanceType): (number|string) {
    switch (params.kind) {
        case "euc2d":
            return Math.sqrt(Math.pow(params.x, 2) + Math.pow(params.y, 2))
        case "euc3d":
            return Math.sqrt(Math.pow(params.x, 2) + Math.pow(params.y, 2) + Math.pow(params.z, 2)) 
        default:
            return "Error"
    }
}

distanceCal({kind:"euc2d", x:4, y:5})
distanceCal({kind:"euc3d", x:4, y:5, z:6})