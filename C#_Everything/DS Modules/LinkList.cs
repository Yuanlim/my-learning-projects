namespace My_DS_Modules;

class LinkList<T> where T : notnull
{
  /// <summary>
  ///   Node data structure
  /// </summary>
  /// <param name="value">Node value</param>
  public class Node(T value)
  {
    public T Value { get; set; } = value;
    public Node? Prev { get; set; }
    public Node? Next { get; set; }
  };

  public Node? Head { get; private set; } = null;
  public Node? Tail { get; private set; } = null;
  public int Count { get; private set; } = 0;

  /// <summary>
  ///   Add node before Head node
  /// </summary>
  /// <param name="Value">Node value</param>
  public void AddFirst(T Value)
  {
    // Create new node
    Node node = new(Value);

    if (Head is null) // checks for empty link list
    {
      Head = Tail = node;
      Count = 1;
      return;
    }

    /* Senerio: Head->▭<-Tail */
    /* New node: ▭ ▭ */
    /* Head.Prev to Node: ▭-Head->▭<-Tail */
    /* Node.Next to Head: ▭=Head->▭<-Tail */
    /* Head as node: Head->▭=▭<-Tail */
    AddReLink(asBefore: node, asAfter: Head);
    Head = node;
    Count++;
  }

  /// <summary>
  ///   Add node after Tail node
  /// </summary>
  /// <param name="Value">Node value</param>
  public void AddLast(T Value)
  {
    // Create new node
    Node node = new(Value);

    if (Tail is null) // checks for empty link list
    {
      Head = Tail = node;
      Count = 1;
      return;
    }

    /* Senerio: Head->▭<-Tail */
    /* New node: ▭ ▭ */
    /* Node.Prev to Tail: Head->▭<-Tail-▭ */
    /* Tail.Next to Node: Head->▭<-Tail=▭ */
    /* Tail as node: Head->▭=▭<-Tail */
    AddReLink(asBefore: Tail, asAfter: node);
    Tail = node;
    Count++;
  }

  /// <summary>
  ///   Remove Head node
  /// </summary>
  public void RemoveFirst()
  {
    if (Head is null) return; // checks for empty link list
    if (Head.Next is null) // Only one node
    {
      Clear();
      return;
    }

    /* Senerio: Head->▭=▭<-Tail */
    /* Head to Head.Next: ▭=Head->▭<-Tail*/
    /* Head.Prev to null: Head->▭<-Tail*/
    Head = Head.Next;
    Head.Prev = null;
    Count--;
  }

  /// <summary>
  ///   Remove tail node
  /// </summary>
  public void RemoveLast()
  {
    if (Tail is null) return; // checks for empty link list
    if (Tail.Prev is null) // Only one node
    {
      Clear();
      return;
    }

    /* Senerio: Head->▭=▭<-Tail */
    /* Tail to Tail.Prev: Head->▭<-Tail=▭*/
    /* Tail.Next to null: Head->▭<-Tail*/
    Tail = Tail.Prev;
    Tail.Next = null;
    Count--;
  }

  /// <summary>
  ///   Insert new node before ref node.
  /// </summary>
  /// <param name="node">Add from</param>
  /// <param name="value">New node value</param>
  public void InsertBefore(Node? node, T value)
  {
    /* Only one or no Prev node (is Head) equivalent to Add first*/
    if (node is null || node.Prev is null)
    {
      AddFirst(value);
      return;
    }

    // Create new node
    Node newNode = new(value);

    // Head->▭=▭=▭=▭...=▭<-Tail
    //            ▭       
    // link new node first to before and after node: so we still has ref
    LinkNewNode(newNode, node);
    // Head->▭=▭=▭=▭...=▭<-Tail
    //          |  |
    //          --▭
    // Relink newNode Prev node and Next Node to new node
    node.Prev.Next = newNode;
    node.Prev = newNode;
    // Head->▭=▭=▭=▭=▭...=▭<-Tail
    //             |INSERTED
    Count++;
  }

  /// <summary>
  ///   Insert new node before ref node.
  /// </summary>
  /// <param name="node">Add from</param>
  /// <param name="value">New node value</param>
  public void InsertAfter(Node? node, T value)
  {
    /* Only one or no Next node (is Tail) equivalent to Add last*/
    if (node is null || node.Next is null)
    {
      AddLast(value);
      return;
    }

    // Create new node
    Node newNode = new(value);

    // Head->▭=▭=▭=▭...=▭<-Tail
    //            ▭       
    // link new node first to before and after node: so we still has ref
    LinkNewNode(newNode, node.Next);
    node.Next.Prev = newNode;
    node.Next = newNode;
    Count++;
  }

  private static void AddReLink(Node asBefore, Node asAfter)
  {
    asAfter.Prev = asBefore;
    asBefore.Next = asAfter;
  }

  private static void LinkNewNode(Node newNode, Node newNodeAfter)
  {
    newNode.Prev = newNodeAfter.Prev;
    newNode.Next = newNodeAfter;
  }

  public IEnumerable<T> Forward()
  {
    for (var current = Head; current is not null; current = current.Next)
      yield return current.Value;
  }

  public IEnumerable<T> Backward()
  {
    for (var current = Tail; current is not null; current = current.Prev)
      yield return current.Value;
  }

  public Node? Find(T value)
  {
    for (var current = Head; current is not null; current = current.Next)
      if (Equals(current.Value, value)) return current; // Match, immediate return

    return null; // No match or start empty
  }

  public bool Contains(T value)
  => Find(value) is not null;

  public void Remove(Node node)
  {
    if (node == Head) // Head same as Remove first
    {
      RemoveFirst();
      return;
    }

    if (node == Tail) // Tail same as Remove first
    {
      RemoveLast();
      return;
    }

    // Prev node next to Next node
    // Next node Prev to Prev node
    var prev = node.Prev;
    var next = node.Next;

    if (prev is not null) prev.Next = next;
    if (next is not null) next.Prev = prev;

    // node.Prev, node.Next is still link to node.Prev.Next & node.Next.Prev
    // Garbage collect
    node.Next = null;
    node.Prev = null;

    Count--;
  }

  /// <summary>
  ///   Remove node by value that is the first found
  /// </summary>
  /// <param name="value">Target value</param>
  /// <returns>bool: success</returns>
  public bool Remove(T value)
  {
    // Find target node
    Node? node = Find(value);

    // No target to remove
    if (node is null) return false; // Unsuccess

    // Has target Remove it
    Remove(node);

    // Success
    return true;
  }

  public void Clear()
  {
    Head = Tail = null;
    Count = 0;
  }
}