from math import floor, log2

def OnceInALifeTimeTSPSamplingTree(N):
    """
    Sampling N size TSP, all possible solutions.

    Parameter
    ---------
    N (Required): int
      Sample problem size.

    Return
    ------
    root : array
      An array of all binary solutions
    """
    root = [""]
    eachIndexArrayHadSolution = {0: []}
    timeStepInCityArrayString = ['0' * i + '1' + '0' * (N - i - 1) for i in range(N)]

    while len(root[0]) != N * N:
        result = []
        newIndexArrayHadSolution = {}

        for i in range(len(root)):
            currentPath = root[i]
            beenVisited = eachIndexArrayHadSolution[i]

            for eachCity in range(N):
                if eachCity in beenVisited:
                    continue

                newPath = currentPath + timeStepInCityArrayString[eachCity]
                result.append(newPath)

                newIndex = len(result) - 1
                newIndexArrayHadSolution[newIndex] = beenVisited + [eachCity]

        root = result
        eachIndexArrayHadSolution = newIndexArrayHadSolution

    qubitRequiredForAddress = floor(log2(len(root)))
    while len(root) != pow(2, qubitRequiredForAddress):
        for string in root:
            decomposeString = [
                      string[i:i+N] 
                      for i in range(0, len(string), N)
                      ]
            reversedSolutionString = ''.join(
                          eachTimeStepSolution 
                          for eachTimeStepSolution in reversed(decomposeString)
                          )

            if reversedSolutionString in root:
                root.remove(reversedSolutionString)
                break
            else:
                continue
                
    return root

