using My_DS_Modules;

class Stack_Test
{
  public static void StackTest()
  {
    Console.WriteLine("fixed 10 size stack");
    Stack<string> CoursesJourney = new Stack<string>(10);
    string[] Journey = ["Computer programming", "Web development", "Data structure", "Algorithm", "Assembly Language", "Android Apps Labatory", "System Design and Analysis", "Computer Graphic", "Digital Image Processing", "Computer Vision"];

    Console.WriteLine("10 member inserted");
    for (int i = 0; i < Journey.Length; i++)
    {
      CoursesJourney.Push(Journey[i]);
    }

    Console.WriteLine($"IsFull: {CoursesJourney.IsFull()}");
    Console.WriteLine($"IsEmpty: {CoursesJourney.IsEmpty()}");
    // CoursesJourney.Push("Quantum Computing"); // Error exceed max

    // Pop until empty
    if (!CoursesJourney.IsEmpty()) Console.Write(CoursesJourney.Pop());
    while (!CoursesJourney.IsEmpty())
    {
      Console.Write(" ←-- " + CoursesJourney.Pop());
    }
    Console.WriteLine();

    // CoursesJourney.Pop(); // Error nothing to pop
  }
  public static void DynamicStackTest()
  {
    DynamicStack<string> CoursesJourney = new();
    string[] Journey = ["Computer programming", "Web development", "Data structure", "Algorithm", "Assembly Language", "Android Apps Labatory", "System Design and Analysis", "Computer Graphic", "Digital Image Processing", "Computer Vision",
    "Computer programming", "Web development", "Data structure", "Algorithm", "Assembly Language", "Android Apps Labatory", "System Design and Analysis", "Computer Graphic", "Digital Image Processing", "Computer Vision"];

    Console.WriteLine("20 member inserted");
    for (int i = 0; i < Journey.Length; i++)
    {
      CoursesJourney.Push(Journey[i]);
    }

    Console.WriteLine($"NumElement: {CoursesJourney.NumElement()}");
    Console.WriteLine($"MaxElement: {CoursesJourney.MaxElement}");
    Console.WriteLine($"IsEmpty: {CoursesJourney.IsEmpty()}");
    Console.WriteLine($"Inserting 1 more course: {CoursesJourney.IsEmpty()}");
    CoursesJourney.Push("Quantum Computing");
    Console.WriteLine($"NumElement: {CoursesJourney.NumElement()}");
    Console.WriteLine($"MaxElement: {CoursesJourney.MaxElement}");

    // Pop until empty
    if (!CoursesJourney.IsEmpty()) Console.Write(CoursesJourney.Pop());
    while (!CoursesJourney.IsEmpty())
    {
      Console.Write(" ←-- " + CoursesJourney.Pop());
    }
    Console.WriteLine();

    // CoursesJourney.Pop(); // Error nothing to pop
  }
}