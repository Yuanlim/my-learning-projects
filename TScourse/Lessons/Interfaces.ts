// think as signature: something inherit it must implement every property
type getDetailsType = () => null;  
interface IExample
{
    name : string;
    Id : string;
    GetDetails : getDetailsType;
}

// function signature
interface IFunctionExample
{
    _(a: number, b: number) : number;
}

// new intances
const newExample: IExample =
{
    name: "John",
    Id: "F123456",
    GetDetails() {
     console.log(this.name + " " + this.Id);
     return null;
    }
};

const newFunc: IFunctionExample = 
{
    _(a: number, b: number): number {
        return a + b;
    }
}

newFunc._(3, 5)

// Interface: also "contract" that can work with functions or classes
interface IMathFunctions
{
    Subtract(a: number, b: number): number
    Adding(a: number, b: number): number
}

class MyMath implements IMathFunctions
{
    Subtract(a: number, b: number): number {
        return a - b;
    }
    Adding: IMathFunctions["Adding"] = (a, b) => {return a + b;}
}

// Interfaces with data
type Amount = number;

interface Car
{
    CarId: number;
    MaxSpeed: number;
    Model: string;
    Location: [number, number];
    getSpeed():number;
}

class MyCar implements Car
{
    constructor(
        public CarId: number,
        public MaxSpeed: number,
        public Model: string,
        public Location: [number, number] = [0, 0]
    ) {
        this.CarId = CarId;
        this.MaxSpeed = MaxSpeed;
        this.Model = Model;
        this.Location = Location;
    }

    public getSpeed = (): number => this.MaxSpeed;
}

type Garage = {
  OwnerName: string;
  Cars: Map<MyCar, Amount>;
};

let BMW = new MyCar(1, 200, "BMW", [1, 1]);
let Audi = new MyCar(2, 220, "Audi", [5, 5]);
let DaveGarage:Garage = {
    OwnerName: "Dave",
    Cars: new Map<MyCar, Amount>(
        [[BMW, 1], 
        [Audi, 2]]
    )
}

