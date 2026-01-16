// Index Signature: get interface property by their keys type (can be string, number, ....)
interface ITransactions {
  [index: string]: number; // set key type to string, and variable type should be number
  // readonly [index: string] :number // optional: set every variable to have read only modifier

  Food: number;
  Drinks: number;
  Entertainment: number;
  Earnings: number;
}

const DaveTransactions: ITransactions = {
  Food: -10,
  Drinks: -4,
  Entertainment: -20,
  Earnings: 100,
};

const checkEarned = (transactions: ITransactions): number => {
  let total: number = 0;
  for (const t in transactions) {
    console.log(t);
    total += transactions[t] ?? 0;
  }
  return total;
};

console.log(checkEarned(DaveTransactions));

// Key Of: without index signature setup but still want to access via key
interface IStudent {
  // [key: string]: number | string | number[] | undefined
  Name: string;
  GPA: number;
  Classes?: number[];
}

const logStudent = (student: IStudent): void => {
  for (const key in student) {
    console.log(student[key as keyof IStudent]);
  }
};

const logStudentByKey = (student: IStudent, key: keyof IStudent): void => {
  console.log(student[key]);
};

const studentDave: IStudent = { Name: 'Dave', GPA: 50, Classes: [100, 200] };
console.log('logStudent proccess:');
logStudent(studentDave);

console.log('obj keys proccess:');
Object.keys(studentDave).map((key) => {
  console.log(studentDave[key as keyof typeof studentDave]);
});

console.log('logStudentByKey proccess:');
logStudentByKey(studentDave, 'GPA');

// More Alternative to write index signature
// Downside: All data type couldn't be narrow all has to be the same or union

type TransactionKeys = 'income' | 'food' | 'drinks';

// type TransactionInfo = Record<TransactionKeys, number | string> // downside everything is number | string type
type TransactionInfo = Record<TransactionKeys, number>;

const LilyTransaction: TransactionInfo = {
  income: 100,
  food: -14,
  drinks: -5,
};

for (const key in LilyTransaction) {
  console.log(`${key}: ${LilyTransaction[key as keyof TransactionInfo]}`);
}
