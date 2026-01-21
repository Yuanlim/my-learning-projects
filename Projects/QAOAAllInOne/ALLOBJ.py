def MaxCutObjectiveFunction(maxBinary, adjMatrix) -> int:
    """
    Given binary solution, calculate cut cost

    Parameters
    ----------
    maxBinary (Required) : string
      Solution string.
    adjMatrix (Required) : array
      Graph propreties.
    
    Return
    ------
    cost : int
      Cut cost
    """
    cost = 0

    # get different group
    forAllOnes = [i for i, value in enumerate(maxBinary) if value == '1']
    forAllZeros = [i for i, value in enumerate(maxBinary) if value == '0']
    # print(forAllOnes)
    # print(forAllZeros)

    # Calculate cost of max-cut
    for indexOfOnes in forAllOnes:
        for indexOfZeros in forAllZeros:
            cost += adjMatrix[indexOfOnes][indexOfZeros]

    return cost


def TSPObjectiveFunction(maxBinary, adjMatrix, N, maxEdge, backToStart=True):
    """
    Given binary solution, calculate tour cost.

    Parameters
    ----------
    maxBinary (Required) : string
      Solution string.
    adjMatrix (Required) : array
      Graph propreties.
    N (Required) : int
      TSP problem size 
    maxEdge (Required) : int
      Graph maximum edge. For panelties infeasible.
    backToStart (Optional) : bool (defualt = True)
      T/F on including, back to initial city route cost.
    
    Return
    ------
    cost : int
      Tour cost

    Note
    ----
    If detected infeasible solution, will return 2 * N * maxEdge
    """
    cost = 0
    beenThere = []
    # Decompose solution
    decomposeString = [
              maxBinary[i: i+N-1] 
              for i in range(0, len(maxBinary), N-1)
              ]

    # print(decomposeString)
    # To city number
    path = [ 0 for _ in range(N) ]
    for i in range(N - 1):
      forAllOnes = [j for j, value in enumerate(decomposeString[i]) if value == '1']
      # print(forAllOnes[0] + 1)
      # print(forAllOnes)
      if len(forAllOnes) != 1 or (forAllOnes[0] + 1 in beenThere): # invalid tour
        return 2 * (maxEdge * N)
      else:
        path[i + 1] = forAllOnes[0] + 1
        beenThere.append(forAllOnes[0] + 1)

    # Calculate cost of TSP
    for eachTImeLine in range(N - 1):
      # print(path)
      cost += adjMatrix[path[eachTImeLine]][path[eachTImeLine + 1]]

    # Return to initial City
    if backToStart == True:
      cost += adjMatrix[N - 1][0]

    return cost
