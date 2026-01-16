// Static variable in class: shared element
type roleLiteral = 'student' | 'teacher';

interface IPersonsInfo {
  name: string;
  role: roleLiteral;
  age: number;
}

type CourseResult = {
  CourseName: string;
  TestResult: number;
};

interface IStudentInfo extends IPersonsInfo {
  StudentId: string;
  StudentTestResults: ReadonlyArray<CourseResult>;

  // Overloads: Declared return type in scenerio to avoid unsafe return
  setResults(CourseResults: CourseResult): void;
  setResults(CourseResults: CourseResult, returnUpdatedList: true): ReadonlyArray<CourseResult>;

  isPass(): boolean;
}

interface ITeacherInfo extends IPersonsInfo {
  TeacherId: string;
  Office: string;
}

class Student implements IStudentInfo {
  private static passGoal: number = 60; // Many new Student Obj shared property only has one

  constructor(
    public name: string,
    public StudentTestResults: ReadonlyArray<CourseResult>,
    public StudentId: string,
    public age: number,
    public readonly role: roleLiteral = 'student',
  ) {}

  public setResults(result: CourseResult): void;
  public setResults(result: CourseResult, returnUpdatedList: true): ReadonlyArray<CourseResult>;
  public setResults(
    result: CourseResult,
    returnUpdatedList?: boolean,
  ): void | ReadonlyArray<CourseResult> {
    this.StudentTestResults = [...this.StudentTestResults, result];
    if (returnUpdatedList) return this.StudentTestResults;
  }

  public isPass(): boolean {
    let meanOfResults,
      totalResult: number = 0;

    this.StudentTestResults.forEach((element) => {
      totalResult += element.TestResult;
    });
    meanOfResults = totalResult / this.StudentTestResults.length;

    return meanOfResults > Student.passGoal;
  }

  public static setGoal = (goal: number): void => {
    Student.passGoal = goal;
  };
}

let Alice = new Student('Alice', [], 'F345678', 60);
Student.setGoal(70);
Alice.setResults({ CourseName: 'C# course', TestResult: 80 });
Alice.setResults({ CourseName: 'TS course', TestResult: 76 });
Alice.setResults({ CourseName: 'Computer Vision', TestResult: 60 });
Alice.setResults({ CourseName: 'Image Proccessing', TestResult: 69 });
console.log(Alice.isPass() ? 'passed' : 'failed');
