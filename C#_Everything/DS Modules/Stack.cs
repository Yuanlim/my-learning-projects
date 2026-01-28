// Fixed size Stack
class Stack<T>(int maxElement)
{
  public T[] Data { get; set; } = new T[maxElement];
  public int Top { get; set; } = -1;
  public int MaxElement { get; init; } = maxElement;

  public T Pop()
  {
    // Nothing to Pop
    if (IsEmpty())
      throw new Exception("Stack is out of elements to pop");

    // Get element before minus index by 1
    return Data[Top--];
  }

  public void Push(T value)
  {
    // Push but is full
    if (IsFull())
      throw new Exception("Stack have max elements can't be push");

    // Add index by 1 before push
    Data[++Top] = value;
  }

  public bool IsEmpty()
  {
    // Nothing: Top is -1 or (lower "cant be happen")
    if (Top <= -1) return true;

    return false;
  }

  public bool IsFull()
  {
    // Max: Top is MaxElement -1 or (higher "cant be happen")
    if (Top >= MaxElement - 1) return true;

    return false;
  }
}

// Dynamic size Stack
class DynamicStack<T>()
{
  public T[] Data { get; set; } = new T[1];
  public int Top { get; private set; } = -1;
  public int MaxElement { get; private set; } = 1;

  public T Pop()
  {
    // Nothing to Pop
    if (IsEmpty())
      throw new Exception("Stack is out of elements to pop");

    // Get element before minus index by 1
    return Data[Top--];
  }

  public void Push(T value)
  {
    // Push but size is full * 2 the spaces
    if (IsFull())
    {
      // Update MaxElement
      MaxElement *= 2;

      // new * 2 T array spaces obj
      var newData = new T[MaxElement];

      // Assign original data back
      for (int i = 0; i <= Top; i++)
        newData[i] = Data[i];

      // Push New Data
      Data = newData;
    }

    // Add index by 1 before push
    Data[++Top] = value;
  }

  public bool IsEmpty()
  {
    // Nothing: Top is -1 or (lower "cant be happen")
    if (Top <= -1) return true;

    return false;
  }

  private bool IsFull()
  {
    // Max: Top is MaxElement -1 or (higher "cant be happen")
    if (Top >= MaxElement - 1) return true;

    return false;
  }

  public int NumElement() => Top + 1;
}