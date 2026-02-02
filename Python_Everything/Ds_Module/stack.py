from typing import Generic, List, TypeVar

T = TypeVar('T');

class stack:

  def __init__(self, maxElement:int):
    self.maxElement:int = maxElement
    self.top:int = -1


  def pop(self) -> T:
    """
    Pop last in element from stack
    
    :param self: object itself(do not pass)
    """
    if self.isEmpty():
      raise ValueError("The stack is empty nothing to pop")

    returnValue = self.stack[self.top]
    self.top -= 1
    return returnValue


  def isEmpty(self) -> bool:
    """
    Check wether stack is empty
    
    :param self: object itself(do not pass)
    """
    if self.top == -1:
      return True
    return False
  

  def isFull(self) -> bool:
    """
    Check wether stack is full
    
    :param self: object itself(do not pass)
    """
    if self.top == self.maxElement - 1:
      return True
    return False
  


class fixedStack(stack, Generic[T]):

  def  __init__(self, maxElement: int):
    super().__init__(maxElement)
    self.stack:List[T] = [None for _ in range(maxElement)]
  

  def push(self, data: T):
    """
    Docstring for push
    
    :param self: object itself(do not pass)
    :param data: Desired store data that meets the instance data type
    :type data: T
    """
    if self.isFull():
      raise ValueError("The stack is full cant push any data anymore")
    
    self.top += 1
    self.stack[self.top] = data
    return
  

class dynamicStack(stack, Generic[T]):
  
  def __init__(self):
    super().__init__(1)
    self.stack:List[T] = [None]

  
  def push(self, data: T):
    """
    Docstring for push
    
    :param self: object itself(do not pass)
    :param data: Desired store data that meets the instance data type
    :type data: T
    """

    if self.isFull():
      # saves the previous list
      backUpList:List[T] = self.stack

      # update max capacity
      self.maxElement *= 2

      # double the stack capacity
      self.stack:List[T] = [None for i in range(self.maxElement * 2)]

      # reassign back the stack data
      for i in range(len(backUpList)):
        self.stack[i] = backUpList[i]

    # add new data to the stack
    self.top += 1
    self.stack[self.top] = data
    return
  

  def numElement(self) -> int:
    return self.top + 1
  
  
  def maximum(self) -> int:
    return self.maxElement
  

  def remainingSpace(self) -> int:
    return self.maxElement - self.top - 1


