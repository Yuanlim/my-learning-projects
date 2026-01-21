import math
import random
import networkx as nx
from qiskit.visualization import array_to_latex


def problemInitialization(
    N, randomlyCreate=False, degree=False, 
    weighted=False, adjMatrix=None, d=3,
    weightBounds=[0, 10]
  ):
    """
    Many functionalities:
    1.Randomly create N size graph by weighted or unweighted
    2.Passed adjMatrix, linked edge random initialize weight.
    3.Randomly create degreed graph in weighted or unweighted.
    4.Randomly create completed graph in weighted or unweighted.
    
    Parameters
    ----------
    N (Required) : int
      Generate/Passed problem size.
    randomlyCreate (Optional) : bool (defualt = False)
      T/F on creates its own adjMatrix, linked edge weight randomly decide.
    degree (Optional) : bool (defualt = False)
      T/F on generating degreed based graph.
    weighted (Optional) : bool (defualt = False)
      T/F on generating weighted based graph.
    adjMatrix (Optional) : N*N array (defualt = None)
      Known graph link propreties, and random initialize weight.
    d (Optional) : int (defualt = 3)
      Each node amount of linked node limitation.
    weightBounds (Optional) : 2 element array (defualt = [0, 10])
      Edge weight range.

    Note
    ----
    If only N argument pass, means create complete graph.(Not weighted)
    """

    if adjMatrix is None:
        adjMatrix = []
    adjacencyMatrix = [[0 for _ in range(N)] for _ in range(N)]  # initial space needed

    if randomlyCreate and not degree:
        for row in range(N):
            for col in range(N):
                if row == col:
                    adjacencyMatrix[row].append(0)
                elif col > row:
                    if weighted:
                        weight = random.randint(weightBounds[0], weightBounds[1])
                    else:
                        weight = 1
                    adjacencyMatrix[row][col] = weight
                    adjacencyMatrix[col][row] = weight

    elif adjMatrix:
        for row in range(len(adjMatrix) - 1):
            for col, value in enumerate(adjMatrix[row][row:], start=row):
                # print(col, value)
                if value == 1:  # have edge
                    adjMatrix[row][col] = random.randint(weightBounds[0], weightBounds[1])
                    adjMatrix[col][row] = adjMatrix[row][col]
        adjacencyMatrix = adjMatrix

    elif degree:  # degree graph
        # https://stackoverflow.com/questions/71924219/generating-a-random-graph-with-equal-node-degree
        graph = nx.random_regular_graph(d=d, n=N)
        # get graph adjMatrix
        adjacencyMatrix = nx.to_numpy_array(graph).tolist()
        if weighted:
            for row in range(len(adjacencyMatrix) - 1):
                for col, value in enumerate(adjacencyMatrix[row][row:], start=row):
                    if value == 1:  # have edge
                        adjacencyMatrix[row][col] = random.randint(weightBounds[0], weightBounds[1])
                        adjacencyMatrix[col][row] = adjacencyMatrix[row][col]
    
    else: # Complete
        # print("Hi")
        graph = nx.complete_graph(N)
        adjacencyMatrix = nx.to_numpy_array(graph).tolist()
        if weighted:
            for row in range(len(adjacencyMatrix) - 1):
                for col, value in enumerate(adjacencyMatrix[row][row:], start=row):
                    if value == 1:  # have edge
                        adjacencyMatrix[row][col] = random.randint(weightBounds[0], weightBounds[1])
                        adjacencyMatrix[col][row] = adjacencyMatrix[row][col]

    return adjacencyMatrix


def imageToAdj(
  patches, numIter, sigma,
  wMaxValue=0, wMinimumValue=0,
  enhancedAdj=False, flipAdjWeight=False
  ):
    """
    Patch section of image, to adjMatrix.

    Parameters
    ----------
    patches (Required) : @dataclass
      Section of image parent class.
    numIter (Required) : int 
      Current application interation. Initial "1" means need to note max/min Weight.
    sigma (Required) : double or int
      For guassian similarity.
    wMaxValue (Optional) : double or int (defualt = 0)
      Already known whole image max weight.
    wMinimumValue (Optional) : double or int (defualt = 0)
      Already known whole image min weight.
    enhancedAdj (Optional) : bool (defualt = False)
      Makes adjMatrix, dont have two kinds of value positive/negative.
    flipAdjWeight (Optional) : bool (defualt = False)
      Flip adjMatrix weight.

    Returns
    -------
    adjMatrix : size*size array
      Image section patches graph propreties.
    wMaxValue : double or int
      For recursive usage.
    wMinimumValue : double or int
      For recursive usage.
    """

    explore = [[-1, 0], [1, 0], [0, -1], [0, 1]]

    lenghtOfPatches = len(patches)

    lenOfEachRowElement = int(math.sqrt(lenghtOfPatches))

    adjMatrix = [
        [0 for _ in range(len(patches))]
        for _ in range(len(patches))
    ]
    wPrime = [
        [0 for _ in range(len(patches))]
        for _ in range(len(patches))
    ]

    sigma = sigma

    modImage = [ [] for _ in range(lenOfEachRowElement) ]

    for rowPatch in range(lenOfEachRowElement):
        for patch in range(lenOfEachRowElement):
            modImage[rowPatch].append(patches[
              rowPatch * lenOfEachRowElement + patch
              ].pixelPatchValues)

    # display(array_to_latex(modImage, prefix="\\text{image patches mean = }\n", max_size=20))

    for eachPixel in range(lenghtOfPatches):  # get each pixel from patch

      # correlation to image index
      currentRowPixel = eachPixel // lenOfEachRowElement # 0, 0, 0
      currentColPixel = eachPixel % lenOfEachRowElement # 0, 1, 2
      
      for eachExplore in explore:
          # Explore
          neighborRow = currentRowPixel + eachExplore[0]
          neighborCol = currentColPixel + eachExplore[1]
          
          # check explore is out of bounds, which is illegal
          if 0 <= neighborRow < len(modImage) and 0 <= neighborCol < len(modImage[0]):
              # cal the index for the explore to what number of (index 4 step)
              neighborPixel = neighborRow * lenOfEachRowElement + neighborCol

              # cal wPrime
              distanceOfPixel = modImage[currentRowPixel][currentColPixel] - modImage[neighborRow][neighborCol]
              wPrime[eachPixel][neighborPixel] = 1 - math.exp(-(pow(distanceOfPixel, 2)/(2 * pow(sigma, 2))))

    if numIter == 1:
        nonZeroArray = [ value for row in wPrime 
                  for value in row if value != 0]
        wMinimumValue = min( nonZeroArray )
        wMaxValue = max( nonZeroArray )

    a = -1
    b = 1

    for row in range(len(wPrime)):
        for col in range(len(wPrime[row])):
          if wPrime[row][col] != 0:
            numerator = (b - a) * (wPrime[row][col] - wMinimumValue)
            denominator = wMaxValue - wMinimumValue
            adjMatrix[row][col] = (-1 if flipAdjWeight == False else 1) * ( numerator / denominator + a )

    # display(array_to_latex(wPrime, prefix="\\text{image wPrime = }\n", max_size=20))
    # additional transition
    if enhancedAdj == True:
        adjMatrix = EnhancedImageToAdj(adjMatrix=adjMatrix, flipped=flipAdjWeight)

    return adjMatrix, wMaxValue, wMinimumValue


def EnhancedImageToAdj(adjMatrix, flipped):
    """
    Set edge who is larger than 0 to 0,
    And flip adjMatrix value 
    Switched goal: find max

    Parameter
    ----------
    adjMatrix : int
      Graph propreties.
    flipped : bool
      T/F on flipping adjMatrix value

    Return
    ------
    adjMatrix : size*size array
      Changed graph propreties.
    """

    for eachRow in range(len(adjMatrix)):
        for eachCol in range(len(adjMatrix[0])):
            if adjMatrix[eachRow][eachCol] > 0:
                adjMatrix[eachRow][eachCol] = 0
            else:
                adjMatrix[eachRow][eachCol] = (
                  -1 if flipped else 1) * (adjMatrix[eachRow][eachCol]
                )

    return adjMatrix


def genRandomDegreeGraphOfN(size, degreeBounds, weighted, weightBounds):
    """
    Create user based configuration random degree graph 
    based on given propreties.

    Parameters
    ----------
    size : int
      Generated size.
    degreeBounds : 2 element array
      Generated degree range.
    weighted : bool
      Graph weight > 1.
    weightBounds : 2 element array 
      Generated weight range.

    Return
    ------
    adjMatrix : size*size array
      Generated graph propreties.
    """
    def checkBoundsValid(bounds) -> None:
        if len(bounds) != 2:
            raise ValueError("Bounds lenght error")
        for value in bounds:
            if not isinstance(value, int):
                raise ValueError("Bounds value not integer")
            if size < value:
                raise ValueError("Bounds value larger than graph node size")
        
    if not isinstance(size, int):
        raise ValueError("Size must be integer")
    
    checkBoundsValid(degreeBounds)
    degreeBounds.sort()
    degreeList = [ 
      random.randint(degreeBounds[0], degreeBounds[1])
      for _ in range(size)
    ]
    print(degreeList)
    graph = nx.random_degree_sequence_graph(degreeList, seed=None, tries=10)
    adjMatrix = nx.to_numpy_array(graph).tolist()

    if weighted:
        for row in range(len(adjMatrix) - 1):
            for col, value in enumerate(adjMatrix[row][row:], start=row):
                if value == 1:  # have edge
                    adjMatrix[row][col] = random.randint(weightBounds[0], weightBounds[1])
                    adjMatrix[col][row] = adjMatrix[row][col]

    return adjMatrix

