// Class
class Coder {
    name: string;

    constructor(name: string){
        this.name = name;
    }
}

// instead write: avoid redundancy
class CoderV2 {
    secondLang!: string // tell ts i am not going to initilize it rightaway, I know what i am doing

    constructor(
        public readonly name: string, // immutable once declared
        public age: number, // can be only access inside of the class or inherit ones
        private lang: string, // can be only access inside of the class // defualt 'ts'
        protected music: string = "Random", // can be access out of the class or in
    ) {
        this.name = name;
        this.music = music;
        this.age = age;
        this.lang = lang;
    };

    public getAge = ():number => {return this.age};
};

class WebDev extends CoderV2 { // extends existing class methods or data
    constructor(
        private computer: string,
        name:string,
        age: number,
        lang: string
    ) {
        super(name, age, lang);
        this.computer = computer;
    }

    public getMusic = ():void => {console.log(this.music);} // because protected it is possible to access
}

// new the class obj
const Dave = new CoderV2("Dave", 42, "ts");
// Dave.name = "lol?"; // Readonly invalid
console.log(Dave.getAge());

const Lily = new WebDev("Windows", "Lily", 27, 'js');
Lily.getMusic();
Lily.getAge();