from typing import Generic, List, Optional, TypeVar

T = TypeVar('T');

class Node(Generic[T]):
  def __init__(
    self, Data: T, 
    Prev: Optional[Node[T]] = None, 
    Next: Optional[Node[T]] = None
  ):
    self.Data = Data
    self.Prev = Prev
    self.Next = Next


class LinkList(Generic[T]):

  # Field 
  def __init__(self):
    self.Head: Optional[Node[T]] = None;
    self.Tail: Optional[Node[T]] = None;
    self.Count: int = 0;


  def AddFirst(self, data: T) -> None:
    # New node
    node = Node(data)

    # No node
    if self.Head is None:
      self.Head = self.Tail = node
      Count = 1
      return
    
    # Relink together
    self.__AddRelink(asBefore=node, asAfter=self.Head)

    # New Head
    self.Head = node
    self.Count += 1


  def AddLast(self, data: T) -> None:
    # New node
    node = Node(data)

    # No node
    if(self.Tail is None):
      self.Head = self.Tail = node
      Count = 1
      return
    
    # Relink together
    self.__AddRelink(asBefore=self.Tail, asAfter=node)

    # New tail
    self.Tail = node
    self.Count += 1
    

  def __AddRelink(self, asBefore: Node[T], asAfter: Node[T]) -> None: # Not Optional
    asAfter.Prev = asBefore
    asBefore.Next = asAfter


  def InsertBefore(self, refNode: Node[T], data: T) -> None:
    if refNode is None: 
      raise ValueError("Null Reference error: refNode must be none empty")
      return

    # is head or one Node
    if refNode.Prev is None:
      self.AddFirst(data)
      return
    
    # defualt: Two Node not link
    newNode = Node(data);
    newNode.Prev = refNode.Prev
    newNode.Next = refNode
    
    self.InsertNodeLink(asNewNode=newNode, asNewNodeAfter=refNode)

    self.Count += 1


  def InsertAfter(self, refNode: Node[T], data: T) -> None:
    if refNode is None: 
      raise ValueError("Null Reference error: refNode must be none empty")
      return

    # is head or one Node
    if refNode.Prev is None:
      self.AddFirst(data)
      return
    
    # defualt: Two Node not link
    newNode = Node(data);
    newNode.Prev = refNode
    newNode.Next = refNode.Next

    self.InsertNodeLink(asNewNode=newNode, asNewNodeAfter=refNode.Next)
    self.Count += 1;


  def InsertNodeLink(self, asNewNode: Node[T], asNewNodeAfter: Node[T]):
    asNewNodeAfter.Prev.Next = asNewNode
    asNewNodeAfter.Prev = asNewNode

  
  def RemoveFirst(self) -> None:
    if self.Head is None:
      return
    
    # One node
    if self.Head.Next is None:
      self.Clear()
      return

    self.Head = self.Head.Next
    self.Head.Prev = None
    self.Count -= 1;


  def RemoveLast(self) -> None:
    if self.Tail is None:
      return
    
    # One node
    if self.Tail.Prev is None:
      self.Clear()
      return

    self.Tail = self.Tail.Prev
    self.Tail.Next = None
    self.Count -= 1;


  def Remove(self, refNode:Node[T]) -> None:
    if refNode is None: 
      raise ValueError("Null Reference error: refNode must be none empty")
      return

    # is Head
    if refNode.Prev is None:
      self.RemoveFirst()
      return
    
    # is tail
    if refNode.Next is None:
      self.RemoveLast()
      return
    
    refNode.Next.Prev = refNode.Prev
    refNode.Prev.Next = refNode.Next

    self.Count -= 1
    

  def Contains(self, data: T) -> bool:
    current = self.Head;
    while current is not None:
      # Match
      if current.Data == data:
        return True
      
      # Check Next
      current = current.Next
    return False
  

  def Find(self, data: T) -> Optional[Node[T]]:
    current = self.Head;
    while current is not None:
      # Match
      if current.Data == data:
        return current
      
      # Check Next
      current = current.Next
    return None
  

  def Forward(self) -> List[T]:
    current = self.Head
    returnList:List[T] = []
    while current is not None:
      returnList.append(current.Data)
      
      current = current.Next
    return returnList
  

  def Backward(self) -> List[T]:
    current = self.Tail
    returnList:List[T] = []
    while current is not None:
      returnList.append(current.Data)
      
      current = current.Prev
    return returnList


  def GetCount(self) -> int:
    return self.Count


  def Clear(self) -> None:
    self.Head = self.Tail = None
    self.Count = 0