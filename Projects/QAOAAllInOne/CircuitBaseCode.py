from typing import Text
from qiskit import ClassicalRegister, QuantumCircuit, QuantumRegister
from qiskit_aer import AerSimulator
from ALLOBJ import *
from math import log2, ceil, sqrt, acos
from qiskit.circuit.library import DiagonalGate, MCXGate, RXXGate, RYYGate, MCPhaseGate
import numpy as np

def createQuantumCircuit(nShots, N, adjMatrix, gamma, beta, p,
             showCircuit=False, COPType="max-cut", knownSolution=[],
             givenStatePrepMethod="superposition", 
             givenExploreMethod="x", maxEdge=0):
    """
    Create QAOA circuit and execute
    based on propreties.

    Parameters
    ----------
    nShots : int (Required)
        Amount of quantum shots.
    N : int (Required)
        Solve problem size.
    adjMatrix : N*N array (Required)
        Graph propreties.
    gamma, beta: p double array (Required)
        QAOA ansatz.
    p : int (Required)
        QAOA layer.
    showCircuit : bool (Optional) default = False
        Plot constructed circuit T/F.
    COPType : string (Optional) default = "max-cut"
        Solve problem type. Supported∈{"max-cut", "TSP"}
    knownSolution : array (Optional) default = []
        TSP known solution. Only neccesary, if invoke SQ-QAOA. \n\t\tPs. use Sampling.py for supported format.
    givenStatePrepMethod : string (Optional) defualt = "superposition"
        QAOA circuit initial strategies. Supported∈{"h", "w-state", "x"}.
    givenExploreMethod : string (Optional) defualt = "x"
        QAOA circuit mixer strategies. Supported∈{"x", "xy-mixer", "gm-mixer"(Depreciated)}.
    maxEdge : double or int (Optional) defualt = 0
        TSP graph max edge for panelty.

    return
    ------
    maxBinary : string 
      Maximum measured solution string
    statistics : dict
      Qiskit measured histogram.
    """
    if COPType.lower() in ["max-cut", "max cut", "min cut", "min-cut"]:
        # create quantum circuit
        qc = QuantumCircuit(N)

        # superposition all
        for qubit in range(N):
            qc.h(qubit)

        for i in range(p):
            # Ising model
            for row in range(N):
                for col in range(row, N):
                    if adjMatrix[row][col] != 0:
                        # qc.cx(col, row)
                        # qc.rz(2 * adjMatrix[row][col] * gamma[i], row)
                        # qc.cx(col, row)
                        qc.rzz(2 * adjMatrix[row][col] * gamma[i], row, col)

            qc.barrier()
            # Exploration circuit
            for qubit in range(N):
                qc.rx(2 * beta[i], qubit)
            qc.barrier()

        # measurement
        qc.measure_all()

        if showCircuit == True:
          display(qc.draw('mpl'))

        # execute quantum circuit
        # result = Fake7QPulseV1().run(qc, shots=nShots).result()
        result = AerSimulator().run(qc, shots=nShots).result()

        statistics = result.get_counts()
        # print(statistics.items())

        maxValue = 0
        maxBinary = ''

        for binary, value in statistics.items():
            if maxValue < value:
                maxValue = value
                maxBinary = binary

        # display(plot_histogram(statistics))
        # print(f"tryGamma: {gamma}, tryBeta: {beta}")
        # print(f"maxBinary: {maxBinary}, cost:{MaxCutObjectiveFunction(maxBinary, adjMatrix)}")

        return maxBinary, statistics

    elif COPType.lower() in ["tsp", "traveler salesman problem", "traveling salesman problem"]:
      # print("Slow warning")
      # create quantum circuit

      if knownSolution:
        # address bus required qubit
        lenOfSolution = len(knownSolution)
        qubitRequiredForSolution = ceil(log2(len(knownSolution)))
        a = QuantumRegister(qubitRequiredForSolution)
        qaoa = QuantumRegister(pow(N, 2))
        register = ClassicalRegister(pow(N, 2))

        qc = QuantumCircuit(a, qaoa, register)
        
        # superposition address bus
        for qubit in range(qubitRequiredForSolution):
          qc.h(a[qubit])
        
        for eachCombination in range(lenOfSolution):
          forAllZeros = [i for i, value 
                  in enumerate(format(eachCombination, '0' + str(qubitRequiredForSolution) + 'b')) 
                  if value == '0']
          # print(forAllZeros)
          forAllOnesInSolution = [i for i, value 
                      in enumerate(knownSolution[eachCombination]) 
                      if value == '1']
          # print(forAllOnesInSolution)

          # qc.barrier()

          for index in forAllZeros:
            qc.x(a[index])

          for index in range(len(forAllOnesInSolution)):
              qc.append(MCXGate(num_ctrl_qubits=qubitRequiredForSolution), 
                      a[:qubitRequiredForSolution] 
                      + [qaoa[forAllOnesInSolution[index]]])

          for index in forAllZeros:
            qc.x(a[index])
        # print("pass through QRAM")
        qc.barrier()

      else: # none Qram version
        qubitRequiredForSolution = 0 # solution has nothing
        # construct W state circuit
        qaoa = QuantumRegister(pow(N, 2))
        register = ClassicalRegister(pow(N, 2))
        qc = QuantumCircuit(qaoa, register)
        if givenStatePrepMethod in ["w", "w-state", "w state"]:
          qc = wStateCircuit(qc=qc, N=N)

        elif givenStatePrepMethod in ["superposition", "h"]:
          qc.h(range(pow(N, 2)))

        else: # possible unrecognize
          city = 0
          for i in range(0, pow(N, 2), N): # 1000...,010....,001...,000...001 
            qc.x(i + city)
            city += 1

        qc.barrier()

      # print(adjMatrix)
      for i in range(p):
      # Ising model
        for T in range(N - 1):
          for row in range(N):
            for col in range(N):
              if adjMatrix[row][col] != 0 and row != col:
                currentTimeStepSectionQubit = T * N
                nextTimeStepSectionQubit = (T + 1)  * N
                cityToCityCost = 2 * adjMatrix[row][col] * gamma[i]
                expectedDiagnolElements = [1, 
                              1, 
                              1,
                              np.exp(1j * cityToCityCost)
                              ]
                qc.append(DiagonalGate(expectedDiagnolElements),
                    [currentTimeStepSectionQubit + row + qubitRequiredForSolution,
                    nextTimeStepSectionQubit + col + qubitRequiredForSolution])
            qc.barrier()

        # Penalty
        expectedDiagnolElements = []
        expectedDiagnolElements.append(np.exp(1j * (2 * maxEdge * N) * gamma[i]))
        for _ in range(pow(2, N) - 1):
          expectedDiagnolElements.append(1)
        for T in range(N):
          currentTimeStepSectionQubit = T * N + qubitRequiredForSolution
          # print([currentTimeStepSectionQubit + i for i in range(N)])
          qc.append(DiagonalGate(expectedDiagnolElements),
              [currentTimeStepSectionQubit + m for m in range(N)])
        # print("pass penalty one")

        expectedDiagnolElements = [1, 
                      1, 
                      1,
                      np.exp(1j * (2 * maxEdge * N) * gamma[i])
                      ]
        
        for n in range(N):
          for T in range(N - 1):
            for m in range(N - T - 1):
              currentTimeSectionCityQubit = T * N + n + qubitRequiredForSolution
              nextTimeSectionCityQubit = (T + 1 + m) * N + n + qubitRequiredForSolution
              # print(currentTimeStepSectionMCityQubit, currentTimeStepSectionJCItyQubit)
              qc.append(
                DiagonalGate(expectedDiagnolElements),
                [currentTimeSectionCityQubit, nextTimeSectionCityQubit]
              )
          qc.barrier()


        # QAOA Mixer
        if givenExploreMethod in ["xy", "xy-mixer", "xy mixer"]:
          for m in range(N):
            for T in range(N - 1):
              currentTimeStepSectionQubit = T * N + m + qubitRequiredForSolution
              nextTimeStepSectionQubit = (T + 1)  * N + m + qubitRequiredForSolution
              qc.append(RXXGate(2 * beta[i]), [currentTimeStepSectionQubit, nextTimeStepSectionQubit])
              qc.append(RYYGate(2 * beta[i]), [currentTimeStepSectionQubit, nextTimeStepSectionQubit])
            qc.barrier()
            
        elif givenExploreMethod in ["gm", "grover mixer", "grover-mixer"]:
          for eachCombination in reversed(range(lenOfSolution)):
            forAllZeros = [m for m, value 
                    in enumerate(format(eachCombination, '0' + str(qubitRequiredForSolution) + 'b')) 
                    if value == '0']

            forAllOnesInSolution = [m for m, value 
                      in enumerate(knownSolution[eachCombination]) 
                      if value == '1']
                      
            for index in forAllZeros:
              qc.x(a[index])

            for index in reversed(range(len(forAllOnesInSolution))):
              qc.append(MCXGate(num_ctrl_qubits=qubitRequiredForSolution), 
                      a[:qubitRequiredForSolution] 
                      + [qaoa[forAllOnesInSolution[index]]])

            for index in forAllZeros:
              qc.x(a[index])
            
            qc.barrier()

          # qc.h(list(range(qubitRequiredForSolution, len(qaoa) + qubitRequiredForSolution)))
          qc.x(list(range(qubitRequiredForSolution, len(qaoa) + qubitRequiredForSolution)))
          qc.barrier()
          # expectedDiagnolElements = []
          # for _ in range(pow(2, pow(N, 2)) - 1):
          #   expectedDiagnolElements.append(1)

          # expectedDiagnolElements.append(np.exp(1j * -beta[i]))
          # qc.append(DiagonalGate(expectedDiagnolElements),
          #       [qubitRequiredForSolution + m for m in range(pow(N, 2))])
          qc.append(MCPhaseGate(lam=-1 * beta[i], num_ctrl_qubits=len(qaoa) - 1)
                , [qubitRequiredForSolution + m for m in range(pow(N, 2))])

          qc.barrier()
          qc.x(list(range(qubitRequiredForSolution, len(qaoa) + qubitRequiredForSolution)))
          # qc.h(list(range(qubitRequiredForSolution, len(qaoa) + qubitRequiredForSolution)))
          qc.barrier()

          for eachCombination in range(lenOfSolution):
            forAllZeros = [m for m, value 
                    in enumerate(format(eachCombination, '0' + str(qubitRequiredForSolution) + 'b')) 
                    if value == '0']
            
            forAllOnesInSolution = [m for m, value 
                      in enumerate(knownSolution[eachCombination]) 
                      if value == '1']

            for index in forAllZeros:
                qc.x(a[index])

            for index in range(len(forAllOnesInSolution)):
                qc.append(MCXGate(num_ctrl_qubits=qubitRequiredForSolution), 
                        a[:qubitRequiredForSolution] 
                        + [qaoa[forAllOnesInSolution[index]]])

            for index in forAllZeros:
              qc.x(a[index])
            
            qc.barrier()
        else: # possible unrecognize
          for m in range(pow(N, 2)):
            qc.rx(2 * beta[i], m + qubitRequiredForSolution)
          qc.barrier()

      for i in range(pow(N, 2)):
        qc.measure(qaoa[i], register[i])

      if showCircuit == True:
        display(qc.draw('mpl'))

      # execute quantum circuit
      # result = Fake7QPulseV1().run(qc, shots=nShots).result()
      result = AerSimulator().run(qc, shots=nShots).result()

      # print("in qc result")
      statistics = result.get_counts()
      # print(statistics.items())
      # print("in qc statistics")
      maxValue = 0
      maxBinary = ''

      for binary, value in statistics.items():
          if maxValue < value:
              maxValue = value
              maxBinary = binary

      # display(plot_histogram(statistics))
      # print(f"tryGamma: {gamma}, tryBeta: {beta}")
      # print(f"maxBinary: {maxBinary}, cost:{MaxCutObjectiveFunction(maxBinary, adjMatrix)}")
      # print(f"Maximum measurement:{maxValue}")

      return maxBinary, statistics
        
    else:

      raise ValueError("Unknown COP type please refer to as \"tsp\" or \"max-cut\"")


def wStateCircuit(qc, N):
    """
    Create QAOA circuit and execute
    based on propreties.

    Parameters
    ----------
    qc : QuantumCircuit object (Required)
        An input data, desired initialize circuit using w-state.
    N : int (Required)
        An input data, TSP problem size.
    
    Return
    ------
    qc : QuantumCircuit object
        After initialized quantum circuit
    """
    for eachTimeStep in range(N):
        nextSectionQubit = eachTimeStep * N
        probabilityGiven = [ 1/N for _ in range(N) ]
        if probabilityGiven[0] > 1:
            probabilityGiven[0] = 1
        rot = 2 * acos(sqrt(probabilityGiven[0]))
        qc.ry(rot, [nextSectionQubit])
        probTakes = probabilityGiven[0]
        for i in range(1, len(probabilityGiven)-1):
            if 1 - probTakes < 0:
                amp = 0
            else:
                amp = sqrt(1 - probTakes)
            if probabilityGiven[i] == 0.0:
                qc.cx([nextSectionQubit + (i - 1)], [nextSectionQubit + i])
            else:
                try:
                    rot = 2 * acos(sqrt(probabilityGiven[i]) / amp)
                    qc.cry(rot, [nextSectionQubit + (i - 1)], [nextSectionQubit + i])
                except:
                    qc.cry(0, [nextSectionQubit + (i - 1)], [nextSectionQubit + i])
                probTakes += probabilityGiven[i]
        for i in reversed(range(1, len(probabilityGiven))):
            qc.cx([nextSectionQubit + (i - 1)], [nextSectionQubit + i])
        qc.x([nextSectionQubit])
    
    return qc
    # qc.measure_all()
    # result = AerSimulator().run(qc, shots=500).result()
    # statistics = result.get_counts()
    # print(statistics)
    # display(plot_histogram(statistics))
    # display(qc.draw('mpl'))