// Generics: used when types are unknown in advance

// declaring a function that is generic
const unnessaryLog = <T>(value: T): void => {
  console.log(value);
};

// test
unnessaryLog('1');
unnessaryLog([1, 2, 3]);
unnessaryLog(1);
unnessaryLog({ Pizza: 19 });
unnessaryLog(true);

// Check pass in type is obj type
const isObj = <T>(value: T): boolean => {
  return typeof value === 'object' && value && !Array.isArray(value);
};

console.log(isObj('1'));
console.log(isObj([1, 2, 3]));
console.log(isObj(1));
console.log(isObj({ Pizza: 19 }));
console.log(isObj(true));

// Different type purpose function & generic interface
// Also learn double return function

// A function that test if a this is truely meaningful (non-empty)
type checkerReturn<T> = { value: T; is: boolean };
const isTrue = <T>(value: T): checkerReturn<T> => {
  // If is array immediately return false, value is null
  if (Array.isArray(value)) {
    return { value: value, is: value.length > 0 };
  }
  if (isObj(value)) {
    return { value: value, is: Object.keys(value as object).length > 0 };
  }

  return { value: value, is: Boolean(value) };
};

console.log(isTrue(false));
console.log(isTrue(0));
console.log(isTrue(true));
console.log(isTrue(1));
console.log(isTrue('Dave'));
console.log(isTrue(''));
console.log(isTrue(null));
console.log(isTrue(undefined));
console.log(isTrue({})); // modified
console.log(isTrue({ name: 'Dave' }));
console.log(isTrue([])); // modified
console.log(isTrue([1, 2, 3]));
console.log(isTrue(NaN));
console.log(isTrue(-0));

// Narrow generic type with extends
// the pass in value must given an id

interface IUser {
  id: number;
}

const HasID = <T extends IUser>(value: T): T => {
  return value;
};

console.log(HasID({ id: 1, name: 'Dave' })); // not presented in interface but statisfy the id required
// console.log(HasID({ name: 'Dave' })); // id missing: error.

// Get specific user property by key
const getUsersProperty = <T extends IUser, K extends keyof T>(users: T[], key: K): T[K][] => {
  return users.map((user) => user[key]);
};

const usersArray = [
  {
    id: 1,
    name: 'Leanne Graham',
    username: 'Bret',
    email: 'Sincere@april.biz',
    address: {
      street: 'Kulas Light',
      suite: 'Apt. 556',
      city: 'Gwenborough',
      zipcode: '92998-3874',
      geo: {
        lat: '-37.3159',
        lng: '81.1496',
      },
    },
    phone: '1-770-736-8031 x56442',
    website: 'hildegard.org',
    company: {
      name: 'Romaguera-Crona',
      catchPhrase: 'Multi-layered client-server neural-net',
      bs: 'harness real-time e-markets',
    },
  },
  {
    id: 2,
    name: 'Ervin Howell',
    username: 'Antonette',
    email: 'Shanna@melissa.tv',
    address: {
      street: 'Victor Plains',
      suite: 'Suite 879',
      city: 'Wisokyburgh',
      zipcode: '90566-7771',
      geo: {
        lat: '-43.9509',
        lng: '-34.4618',
      },
    },
    phone: '010-692-6593 x09125',
    website: 'anastasia.net',
    company: {
      name: 'Deckow-Crist',
      catchPhrase: 'Proactive didactic contingency',
      bs: 'synergize scalable supply-chains',
    },
  },
];

console.log(getUsersProperty(usersArray, 'email'));
console.log(getUsersProperty(usersArray, 'username'));

// class with generic
class StateObject<T> {
  private someData: T;

  constructor(someData: T) {
    this.someData = someData;
  }

  get state(): T {
    return this.someData;
  }
  set state(value: T) {
    this.someData = value;
  }
}

const store = new StateObject('John');
console.log(store.state);
store.state = 'Dave';
console.log(store.state);
//store.state = 12

const myState = new StateObject<(string | number | boolean)[]>([15]);
myState.state = ['Dave', 42, true];
console.log(myState.state);
