from numpy import pi
import numpy as np
from qiskit.visualization import plot_histogram
from scipy.optimize import minimize
from dataclasses import dataclass, field
from typing import List, Dict
from skopt import gp_minimize
import random

from CircuitBaseCode import *
from MakeProblem import *
from ALLOBJ import *


class Method:
    """
    QAOA optimizer by GA. 
    @Depreciated
    """
    class GAForQAOA:

        @dataclass
        class chromosome:
            gamma: List[float] = field(default_factory=list)
            beta: List[float] = field(default_factory=list)
            cost: float = 0.0
            variation: float = 0.0
            standardDeviation: float = 0.0
            mean: float = 0.0
            probOfEachVote: List[float] = field(default_factory=list)
            modifiedStatistics: Dict = field(default_factory=dict)
            maxBinary: str = ''
            statistics: Dict = field(default_factory=dict)
            previousCost: float = 0.0

        def __init__(self):
            # default
            self.popMaxSize = 10
            self.probC = 0.3
            self.probG = 0.5
            self.probM = 0.4
            self.N = 5
            self.adjMatrix = problemInitialization(N=5, randomlyCreate=True)
            self.nShots = 50
            self.p = 2
            self.k = 3
            self.alphaWeightFactor = 0.1
            self.lambdaWeightFactor = 1
            self.mutationMethod = "Method1"
            self.chromosomes = [self.chromosome(standardDeviation=0.1, mean=0.0) for _ in range(self.popMaxSize)]
            self.mode = 'AllLayerOnce'  # LayerWise or AllLayerOnce

        
        def mutationConfig(self, probM, mean, standardDeviation, mutationMethod, alphaWeightFactor, lambdaWeightFactor):
            self.probM = probM
            for i in range(self.popMaxSize):
                self.chromosomes[i].mean = mean
                self.chromosomes[i].standardDeviation = standardDeviation
            self.mutationMethod = mutationMethod
            self.alphaWeightFactor = alphaWeightFactor
            self.lambdaWeightFactor = lambdaWeightFactor

        def crossoverConfig(self, probC, probG):
            self.probC = probC
            self.probG = probG

        def problemConfig(self, N, adjMatrix):
            self.N = N
            self.adjMatrix = adjMatrix

        def circuitConfig(self, nShots, p):
            self.nShots = nShots
            self.p = p

        def GAConfig(self, popMaxSize, k):
            self.popMaxSize = popMaxSize
            self.k = k

        def createChromosome(self):
            for member in range(self.popMaxSize):
                gammaAnsatz = [random.uniform(-2 * pi, 2 * pi) for _ in range(self.p)]
                betaAnsatz = [random.uniform(-pi, pi) for _ in range(self.p)]
                self.chromosomes[member].gamma = gammaAnsatz
                self.chromosomes[member].beta = betaAnsatz

                # print(f"Mygamma: {gammaAnsatz}")
                # print(f"Mybeta: {betaAnsatz}")

        def crossover(self, indexOfParents):
            # Simulation of a coin to perform uniform crossover
            offspring1 = self.chromosome()
            offspring2 = self.chromosome()

            for everyLayer in range(self.p):
                if random.uniform(0, 1) <= self.probG:  # Heads
                    # Access parent chromosomes from the list self.chromosomes
                    offspring1.beta.append(self.chromosomes[indexOfParents[0]].beta[everyLayer])
                    offspring1.gamma.append(self.chromosomes[indexOfParents[0]].gamma[everyLayer])

                    offspring2.beta.append(self.chromosomes[indexOfParents[1]].beta[everyLayer])
                    offspring2.gamma.append(self.chromosomes[indexOfParents[1]].gamma[everyLayer])
                else:  # Tails
                    offspring1.beta.append(self.chromosomes[indexOfParents[1]].beta[everyLayer])
                    offspring1.gamma.append(self.chromosomes[indexOfParents[1]].gamma[everyLayer])

                    offspring2.beta.append(self.chromosomes[indexOfParents[0]].beta[everyLayer])
                    offspring2.gamma.append(self.chromosomes[indexOfParents[0]].gamma[everyLayer])

            offspring1.previousCost = max(self.chromosomes[indexOfParents[0]].cost,
                                          self.chromosomes[indexOfParents[1]].cost)

            return [offspring1]

        def mutation(self, indexOfParent):

            # produces beta prime and gamma prime to optimize circuit
            offspring = self.chromosome()

            lowerBound = self.chromosomes[indexOfParent].mean - self.chromosomes[indexOfParent].standardDeviation / 2
            upperBound = self.chromosomes[indexOfParent].mean + self.chromosomes[indexOfParent].standardDeviation / 2

            if self.mode == 'AllLayerOnce':
                offspring.beta = [self.chromosomes[indexOfParent].beta[everyBeta] +
                                  random.uniform(lowerBound, upperBound)
                                  for everyBeta in range(self.p)]

                offspring.gamma = [self.chromosomes[indexOfParent].gamma[everyGamma] +
                                   random.uniform(lowerBound, upperBound)
                                   for everyGamma in range(self.p)]

                offspring.previousCost = self.chromosomes[indexOfParent].cost

            else:

                offspring.beta = [self.chromosomes[indexOfParent].beta[everyBeta]
                                  for everyBeta in range(self.p)]

                # gradually decreases
                # Cite: Quantum Approximate Optimization Algorithm: Performance, Mechanism, and Implementation on Near-Term Devices Leo Zhou
                randomNumber = random.uniform(upperBound, lowerBound)
                if randomNumber > 0:
                    offspring.beta[self.p - 1] = offspring.beta[self.p - 1] - randomNumber
                else:
                    offspring.beta[self.p - 1] = offspring.beta[self.p - 1] + randomNumber

                offspring.gamma = [self.chromosomes[indexOfParent].gamma[everyGamma]
                                   for everyGamma in range(self.p)]

                # gradually increases
                randomNumber = random.uniform(upperBound, lowerBound)
                if randomNumber > 0:
                    offspring.gamma[self.p - 1] = offspring.gamma[self.p - 1] + randomNumber
                else:
                    offspring.gamma[self.p - 1] = offspring.gamma[self.p - 1] - randomNumber

                offspring.previousCost = self.chromosomes[indexOfParent].cost

            return offspring

        def normalizationBinary(self):
            # measurement '1011' is same as '1101' adding the flip bit to the count

            for member in range(self.popMaxSize):
                getKeys = list(self.chromosomes[member].statistics.keys())
                doneKeys = []
                for key in getKeys:
                    if key in doneKeys:
                        continue

                    flipedKeys = ''.join('1' if bit == '0' else '0' for bit in key)

                    if flipedKeys in self.chromosomes[member].statistics.keys() and flipedKeys != key:
                        # print(f"self.chromosomes[{i}].statistics[{key}]: {self.chromosomes[i].statistics[key]}")
                        # print(f"self.chromosomes[{i}].statistics[{flipedKeys}]: {self.chromosomes[i].statistics[flipedKeys]}")

                        changedValue = self.chromosomes[member].statistics[key] + self.chromosomes[member].statistics[
                            flipedKeys]
                        self.chromosomes[member].modifiedStatistics[key] = changedValue
                        doneKeys.append(flipedKeys)

                        # print(f"doneKeys in chromosomes {i}: {doneKeys}")

            # print(self.chromosomes[0].probOfEachVote2)

            # Method 1: Calculate each statistics voting probability to get its variation

            if self.mutationMethod == "Method1":
                for member in range(self.popMaxSize):
                    print(f"self.chromosomes[{member}].currentCost = {self.chromosomes[member].cost}")
                    print(f"self.chromosomes[{member}].previousCost = {self.chromosomes[member].previousCost}")
                    self.chromosomes[member].probOfEachVote = [value / self.nShots for value in
                                                               self.chromosomes[member].modifiedStatistics.values()]
                    self.chromosomes[member].standardDeviation = np.std(self.chromosomes[member].probOfEachVote)
                    self.chromosomes[member].mean = np.mean(self.chromosomes[member].probOfEachVote)
                    self.chromosomes[member].variation = np.var(self.chromosomes[member].probOfEachVote)
                    self.chromosomes[member].previousCost = float(self.chromosomes[member].cost)

                    # print(f"self.chromosomes[{member}].standardDeviation = {self.chromosomes[member].standardDeviation}")
                    # print(f"self.chromosomes[{member}].mean = {self.chromosomes[member].mean}")
                    # print(f"self.chromosomes[{member}].variation = {self.chromosomes[member].variation}")

            # Method 2:
            # 𝜇=𝛼⋅(𝑚𝑖𝑛⁡〖(〖𝑝𝑟𝑒𝑣𝑖𝑜𝑢𝑠〗_𝑐𝑜𝑠𝑡−〖𝑐𝑢𝑟𝑟𝑒𝑛𝑡〗_𝑐𝑜𝑠𝑡,  0〗 )),  𝑤ℎ𝑒𝑟𝑒 𝛼 𝑖𝑠 𝑤𝑒𝑖𝑔ℎ𝑡 𝑓𝑎𝑐𝑡𝑜𝑟
            # 𝜇=𝛼⋅(𝑚𝑎𝑥⁡〖(〖𝑝𝑟𝑒𝑣𝑖𝑜𝑢𝑠〗_𝑐𝑜𝑠𝑡−〖𝑐𝑢𝑟𝑟𝑒𝑛𝑡〗_𝑐𝑜𝑠𝑡,  0〗 )), 𝑚𝑖𝑛𝑖𝑚𝑢𝑚 𝐶𝑂𝑃 𝑒𝑥𝑎𝑚𝑝𝑙𝑒
            # 𝜎=𝜆⋅𝑣𝑎𝑟(𝑐𝑜𝑠𝑡_ℎ𝑖𝑠𝑡𝑜𝑟𝑦),𝑤ℎ𝑒𝑟𝑒 𝜆 𝑖𝑠 𝑤𝑒𝑖𝑔ℎ𝑡 𝑓𝑎𝑐𝑡𝑜𝑟
            else:
                for member in range(self.popMaxSize):
                    print(f"self.chromosomes[{member}].cost = {self.chromosomes[member].cost}")
                    print(f"self.chromosomes[{member}].previousCost = {self.chromosomes[member].previousCost}")
                    if self.chromosomes[member].previousCost > self.chromosomes[member].cost:
                        self.chromosomes[member].mean = self.alphaWeightFactor * (
                                self.chromosomes[member].previousCost - self.chromosomes[member].cost)
                    else:
                        self.chromosomes[member].previousCost = float(self.chromosomes[member].cost)
                        self.chromosomes[member].mean = 0
                    probOfEachVote = [value / self.nShots for value in
                                      self.chromosomes[member].modifiedStatistics.values()]
                    self.chromosomes[member].variation = np.var(probOfEachVote)
                    self.chromosomes[member].standardDeviation = self.lambdaWeightFactor * self.chromosomes[
                        member].variation

        def interpolationStartAnsatz(self, best):
            modifiedGamma = []
            modifiedBeta = []

            # Initialize the first and last values of gamma and beta
            modifiedGamma.append(best.gamma[0])
            modifiedBeta.append(best.beta[0])

            p = self.p

            # Loop from 1 to p-1 for interpolation
            for i in range(1, p - 1):
                # Interpolation formula 、
                interpGamma = ((i / p) * best.gamma[i - 1]) + (((p - i) / p) * best.gamma[i])
                interpBeta = ((i / p) * best.beta[i - 1]) + (((p - i) / p) * best.beta[i])

                modifiedGamma.append(interpGamma)
                modifiedBeta.append(interpBeta)

            # Append the last value of gamma and beta from best
            modifiedGamma.append(best.gamma[-1])
            modifiedBeta.append(best.beta[-1])

            # Assign to all chromosomes
            for member in range(self.popMaxSize):
                self.chromosomes[member].gamma = modifiedGamma[:]
                self.chromosomes[member].beta = modifiedBeta[:]
                randomValueForGamma = random.uniform(-pi / 4, pi / 4)
                randomValueForBeta = random.uniform(-pi / 8, pi / 8)
                modifiedGamma = [modifiedGamma[i] + randomValueForGamma for i in range(self.p)]
                modifiedBeta = [modifiedBeta[i] + randomValueForBeta for i in range(self.p)]

        def executeGA(self, best=None):
            # random initialize ansatz popSize Chromosome
            if self.mode == 'AllLayerOnce' or self.p == 1:
                self.createChromosome()
            else:
                self.interpolationStartAnsatz(best)

            genBest = self.chromosome()
            maxGen = 10
            for gen in range(maxGen):

                print(f"Generation: {gen}")
                # print each chromosome layer gamma and beta
                # for member in range(self.popMaxSize):
                #   print(f"Chromosome: {member}")
                #   print(f"Gamma: {self.chromosomes[member].gamma}")
                #   print(f"Beta: {self.chromosomes[member].beta}")

                # create circuit for different ansatz
                for member in range(self.popMaxSize):
                    # print(self.chromosomes[i])
                    # print(self.chromosomes[i+1])

                    maxBinary, staticals = createQuantumCircuit(nShots=self.nShots, N=self.N,
                                                                adjMatrix=self.adjMatrix,
                                                                gamma=self.chromosomes[member].gamma,
                                                                beta=self.chromosomes[member].beta, p=self.p)

                    # print(f"executeGA: {maxBinary}")

                    # Calculate cost
                    self.chromosomes[member].cost = MaxCutObjectiveFunction(maxBinary=maxBinary,
                                                                            adjMatrix=self.adjMatrix)

                    # store maxBinry and statics
                    self.chromosomes[member].maxBinary = maxBinary
                    self.chromosomes[member].statistics = staticals

                # calculate the voting precision of quantum measurement of each chromosome

                # normalizationBinary a proccess of calculated
                # each chomosome standard deviation, mean and variation

                self.normalizationBinary()

                # sort the cost for elitism selection

                # define what to sort in this example is chromosomes all cost
                sortingKey = lambda chromo: chromo.cost
                self.chromosomes.sort(key=sortingKey, reverse=True)

                # Note best cost individual from each gen, if same cost get the lowest variation

                for chromo in self.chromosomes:
                    if chromo.cost > genBest.cost:
                        genBest.cost = chromo.cost
                        genBest.gamma = chromo.gamma
                        genBest.beta = chromo.beta
                        genBest.variation = chromo.variation
                        genBest.mean = chromo.mean
                        genBest.standardDeviation = chromo.standardDeviation
                        genBest.statistics = chromo.statistics
                        genBest.maxBinary = chromo.maxBinary
                    elif chromo.cost == genBest.cost and chromo.variation < genBest.variation:
                        genBest.cost = chromo.cost
                        genBest.gamma = chromo.gamma
                        genBest.beta = chromo.beta
                        genBest.variation = chromo.variation
                        genBest.mean = chromo.mean
                        genBest.standardDeviation = chromo.standardDeviation
                        genBest.statistics = chromo.statistics
                        genBest.maxBinary = chromo.maxBinary

                print(f"Best Cost: {genBest.cost}")
                print(f"Best Variation: {genBest.variation}")

                # Elitism: Get best k individual
                print(f"Elitism: Get best {self.k} individual")
                self.chromosomes = self.chromosomes[0: self.k]

                print(len(self.chromosomes))

                # Mutation & crossover to Repoduces offspring
                if (gen != maxGen):
                    while (len(self.chromosomes) != self.popMaxSize):
                        if random.uniform(0, 1) <= self.probC and len(
                                self.chromosomes) <= self.popMaxSize - 1:  # if pC = 0.3 0 -> 0.3 perform crossover
                            # https://stackoverflow.com/questions/22842289/generate-n-unique-random-numbers-within-a-range
                            print(len(self.chromosomes))
                            offspring = self.crossover(indexOfParents=random.sample(range(0, self.k - 1), 2))
                            for everyOffSpring in offspring:
                                self.chromosomes.append(everyOffSpring)
                        if len(self.chromosomes) == self.popMaxSize - 1 or (
                                len(self.chromosomes) != self.popMaxSize and random.uniform(0, 1) <= self.probM):
                            offspring = self.mutation(indexOfParent=random.randint(0, self.k - 1))
                            self.chromosomes.append(offspring)

                    print(f"after C&M lenght of the chromosomes member: {len(self.chromosomes)}")

            print("best Founded:")
            # plot_histogram(genBest.statistics)
            print(f"Gamma: {genBest.gamma}")
            print(f"Beta: {genBest.beta}")
            print(f"Cost: {genBest.cost}")
            print(f"Variation: {genBest.variation}")
            print(f"Mean: {genBest.mean}")
            print(f"Standard Deviation: {genBest.standardDeviation}")

            return genBest

        def executeGAwithInterpolationAndLayerwiseTraining(self):
            self.mode = 'LayerWise'
            self.probC = 0
            self.probM = 1
            # First layer ->... p layer
            userSetLayer = self.p
            genBest = self.chromosome()
            for layer in range(1, userSetLayer + 1):
                self.p = layer
                genBest = self.executeGA(best=genBest)

            return genBest
    """
    QAOA optimize by Bayesian.
    @Depreciate
    """
    class BayesianOptimizationBasedQAOA:

        def __init__(self):
            self.N = 5
            self.adjMatrix = problemInitialization(N=5, randomlyCreate=True)
            self.nShots = 50
            self.p = 2
            self.priorObservationsMaxDataPoints = 10  # number of evaluations of obj func
            self.l = 0  # ℓ: points characteristic 0: no correlation ∞: all points correlated equally
            self.varianceOfTheVar = -float('inf')  # random variables variances

        def problemConfig(self, N, adjMatrix):
            self.N = N
            self.adjMatrix = adjMatrix

        def circuitConfig(self, nShots, p):
            self.nShots = nShots
            self.p = p

        def execute(self):
            # Citing:Bayesian optimization for QAOA by Simone Tibaldi

            def objF(ansatz):
                # print(f"ansatz: {ansatz}")
                gamma = ansatz[0: self.p]
                beta = ansatz[self.p:]
                cost = MaxCutObjectiveFunction(
                    maxBinary=createQuantumCircuit(nShots=self.nShots, N=self.N,
                                                   adjMatrix=self.adjMatrix, gamma=gamma, beta=beta, p=self.p)[0],
                    adjMatrix=self.adjMatrix
                )
                return -cost

            # Set Obj function as Gaussian process
            dimension = [(0.0, 2 * pi)] * self.p + [(0.0, pi)] * self.p
            guassianProcess = gp_minimize(objF,
                                          dimension,  # the bounds on each dimension of x
                                          acq_func="EI",  # the acquisition function
                                          n_calls=100,  # the number of evaluations of f
                                          n_random_starts=10,
                                          )  # the random seed

            # # get best gamma and beta
            # gamma = guassianProcess.x[0 : self.p]
            # beta = guassianProcess.x[self.p :]
            # print(f"gamma: {gamma}")
            # print(f"beta: {beta}")

            # return gamma, beta

            # # Initiallize Bayesian Optimization
            # # preparing parior data of Nw
            # initialDataSets = []
            # for i in range(self.priorObservationsMaxDataPoints):
            #   data = self.DataPoint(
            #               gamma=[random.uniform(0, 2 * pi) for _ in range(self.p)],
            #               beta=[random.uniform(0, pi) for _ in range(self.p)],
            #               cost=0
            #               )
            #   data.cost = objectiveFunction(
            #                   maxBinary=createQuantumCircuit(nShots=self.nShots, N=self.N,
            #                   adjMatrix=self.adjMatrix, gamma=data.gamma,
            #                   beta=data.beta, p=self.p)[0]
            #                   , N=self.N,
            #                   adjMatrix=self.adjMatrix
            #                   )
            #   initialDataSets.append(data)

            # # Test
            # for i in range(len(initialDataSets)):
            #   print(f"Gamma: {initialDataSets[i].gamma}, Beta: {initialDataSets[i].beta}, Cost: {initialDataSets[i].cost}")

            # # Calculate σ² and ℓ
            # variance2 =

    """
    QAOA circuit optimize using SciPy avaliable optimizer.

    Parameters
    ----------
    N : int 
        An input data. "Problem size"
        **default=5**
    adjMatrix : N*N array 
        An input data. "Problem adjacent matrix"
        **default=auto created based on N**
    nShots : int
        An input data. "Quantum shots"
        **default=50**
    p : int
        An input data. "Experiment QAOA layer (default: 2)"
    method : string
        An input data. "Classical optimizer selection" 
        **default="Nelder-Mead"**
    knownSolution : array
        An input data. "TSP known solution" 
        Ps. use Sampling.py for supported format
        **default=[] Optional for not using SQ-QAOA**
    problemType : string
        An input data. "max-cut" || "TSP"
        Supported∈{max-cut, min-cut, TSP}
        **default="max-cut"**
    minOrMax : string
        An input data. "Goal of finding "max" || "min"."
        **default="max"**
    maxEdge : double or int
        An input data. "Max edge from TSP graph."
        **default=program find it** 
    tspStatePreparationMethod : string
        An input data. "Circuit initialize method. (max-cut ignore)."
        Supported∈{"h", "w-state", "x",
        "QRAM" (unselectable if knownSolution is not empty)}
        select in defualt"
        **default="w-state"**
    explorerMethod : string
        An input data. "Circuit mixer method. (max-cut ignore)."
        Supported∈{"x", "xy-mixer"}
        **default="xy-mixer"**
    maxOptimizeIter : int
        An input data. "All layer once limited optimize times."
        **default=100**
    tspApplicationCalling : bool
        An input data. "Excluded return to initial city T/F."
        **default=False**
    """
    class ScipyOpimizeQAOA:
        # TODO
        def __init__(
              self, N=5, adjMatrix=None, 
              nShots=50, p=2, method='Nelder-Mead',
              knownSolution=[], problemType="max-cut",
              minOrMax="max", maxEdge=0, 
              tspStatePreparationMethod="w-state",
              explorerMethod="xy-mixer",
              maxOptimizeIter = 100,
              tspApplicationCalling = False
            ):
            self.N = N
            self.adjMatrix = adjMatrix if adjMatrix is not None else problemInitialization(N, randomlyCreate=True)
            self.nShots = nShots
            self.p = p
            self.method = method
            self.problemType = problemType
            self.minOrMax = minOrMax
            self.knownSolutions = knownSolution
            self.tspAdjMatrix = []
            self.maxEdge = maxEdge
            self.tspStatePreparationMethod = tspStatePreparationMethod.lower()
            self.tspMixerMethod = explorerMethod.lower()
            self.maxOptimizeIter = maxOptimizeIter
            self.tspApplicationCalling = tspApplicationCalling

        def statePrepConfig(self, knownSolution):
            self.knownSolutions = knownSolution

        def problemConfig(
              self, N, 
              adjMatrix, 
              problemType='max-cut', 
              maxEdge=0
            ):
            self.N = N
            self.adjMatrix = adjMatrix
            self.problemType = problemType.lower()
            # Starting city fixed reduce adjMatrix element
            if self.problemType in ["tsp", "traveler salesman problem", "traveling salesman problem"]:
              for row in range(1, len(self.adjMatrix)):
                newRow = []
                for col in range(1, len(self.adjMatrix[row])):
                  newRow.append(self.adjMatrix[row][col])
                self.tspAdjMatrix.append(newRow)
              try: # numpy array doesnt work on this
                self.maxEdge = max( max( row for row in self.adjMatrix) )
              except:
                self.maxEdge = np.max(self.adjMatrix)

        def circuitConfig(
              self, nShots,
              p, tspStatePreparationMethod='w-state',
              explorerMethod="xy-mixer"
            ):
              self.tspStatePreparationMethod = tspStatePreparationMethod.lower()
              self.tspMixerMethod = explorerMethod.lower()
              self.nShots = nShots
              self.p = p
        """
        QAOA training method config.

        Parameters
        ----------
        startingWith : string
            An input data. "Layerwise learning next layer
            ansatz initial parameter."
            Supported∈{"0", "other"}
            **default="other"**
        method : string
            An input data. "Classical optimizer selection." 
            **default="Nelder-Mead"**
        minOrMax : string
            An input data. "Goal of finding "max" || "min"."
            **default="max"**
        """
        def methodConfig(
              self, method, 
              startingWith="Other", minOrMax="max"
            ):
            self.startingWith = startingWith.lower()
            self.method = method
            self.minOrMax = minOrMax.lower()

        
        """
        Execute in all layer at once learning.

        Returns
        -------
        gamma : p array
            An output data. "Optimized best gamma."
        beta : p array 
            An output data. "Optimized best beta."
        best : double or int
            An output data. "Best cost founded when optimize." 
        iterT : int
            An output data. "Total optimization iteration used."
        bestBinary : string
            An output data. "Binary solution string."
        """
        def execute(self):
            best = 0 if self.problemType in ["max-cut", "max cut", "min cut", "min-cut"] else float('inf')
            bestBinary = ''.join('0' for _ in range(self.N))
            iterT = 0
            forGammaBackUp = []
            forBetaBackUp = []

            def ownFlow(ansatz):
                nonlocal best
                nonlocal iterT
                nonlocal bestBinary
                nonlocal forGammaBackUp
                nonlocal forBetaBackUp

                iterT += 1

                if iterT < 100:
                    gamma = ansatz[:self.p]
                    beta = ansatz[self.p:]
                    if self.problemType in ["max-cut", "max cut", "min cut", "min-cut"]:
                      binary = createQuantumCircuit(nShots=self.nShots, N=self.N, adjMatrix=self.adjMatrix,
                                      gamma=gamma, beta=beta, p=self.p)[0]
                                      
                      cost = MaxCutObjectiveFunction(
                          maxBinary=binary,
                          adjMatrix=self.adjMatrix
                      )
                    else:
                      binary = createQuantumCircuit(nShots=self.nShots, N=self.N-1, adjMatrix=self.tspAdjMatrix,
                                  gamma=gamma, beta=beta, p=self.p,
                                  showCircuit=False, COPType="tsp", knownSolution=self.knownSolutions,
                                  givenStatePrepMethod=self.tspStatePreparationMethod,
                                  givenExploreMethod=self.tspMixerMethod,
                                  maxEdge=self.maxEdge)[0]
                      cost = TSPObjectiveFunction(
                            maxBinary=binary, 
                            adjMatrix=self.adjMatrix, 
                            N=self.N, 
                            maxEdge=self.maxEdge,
                            backToStart=not(self.tspApplicationCalling)
                          )

                    if self.minOrMax == "max" or self.minOrMax == "maximize":
                        if cost > best:
                            best = cost
                            bestBinary = binary
                    else:
                        if cost < best:
                            best = cost
                            bestBinary = binary

                    forGammaBackUp = gamma
                    forBetaBackUp = beta
                    return cost * (-1 if self.minOrMax == "max" or self.minOrMax == "maximize" else 1)
                else:
                    raise RuntimeError("Early stopping by maxiter")

            gamma = [random.uniform(0, 2 * pi) for _ in range(self.p)]
            beta = [random.uniform(0, pi) for _ in range(self.p)]

            bounds = [(0, 2 * pi) for _ in range(self.p)] + [(0, pi) for _ in range(self.p)]
            
            try:
              result = minimize(
                      ownFlow, 
                      gamma + beta, 
                      method=self.method,
                      bounds=bounds,
                      options={"maxiter":100, 'maxfun':100}
                    )
            except:
                return forGammaBackUp, forBetaBackUp, best, iterT, bestBinary
                
            gamma = result.x[:self.p]
            beta = result.x[self.p:]

            return gamma, beta, best, iterT, bestBinary

        """
        Execute in layerwise learning.

        Parameters
        -------
        layerMethod : string
            An input data. "Selection on different layerwise
            learning method"
            Supported∈{"interpolation", "standard"}            
            **default=standard**

        Returns
        -------
        previousGamma : p array
            An output data. "Optimized best gamma."
        previousBeta : p array 
            An output data. "Optimized best beta."
        best : double or int
            An output data. "Best cost founded when optimize." 
        iterT : int
            An output data. "Total optimization iteration used."
        bestBinary : string
            An output data. "Binary solution string."
        """
        def executeLayerWise(self, layerMethod="standard"):
            previousGamma = []
            previousBeta = []
            best = 0 if self.problemType in ["max-cut", "max cut", "min cut", "min-cut"] else float('inf')
            iterT = 0
            bestBinary = ''.join('0' for _ in range(self.N))
            layerMethod = layerMethod.lower()
            forGammaBackUp = []
            forBetaBackUp = []
            betaIterOptimizeValue = []
            gammaIterOptimizeValue = []
            histoCost = []
            def ownFlow(ansatz, ownFlowPreviousGamma, ownFlowPreviousBeta):
                nonlocal best
                nonlocal iterT
                nonlocal bestBinary
                nonlocal forGammaBackUp
                nonlocal forBetaBackUp

                iterT += 1
                if iterT < int(100/self.p):
                    if not ownFlowPreviousGamma:
                        gamma = [ansatz[0]]
                        beta = [ansatz[1]]
                    else:
                        gamma = ownFlowPreviousGamma + [ansatz[0]]
                        beta = ownFlowPreviousBeta + [ansatz[1]]

                    if self.problemType in ["max-cut", "max cut", "min cut", "min-cut"]:
                        binary = createQuantumCircuit(nShots=self.nShots, N=self.N, adjMatrix=self.adjMatrix,
                                        gamma=gamma, beta=beta, p=layer)[0]
                                        
                        cost = MaxCutObjectiveFunction(
                            maxBinary=binary,
                            adjMatrix=self.adjMatrix
                        )
                    else:
                        binary, statis = createQuantumCircuit(nShots=self.nShots, N=self.N-1, adjMatrix=self.tspAdjMatrix,
                                  gamma=gamma, beta=beta, p=layer,
                                  showCircuit=False, COPType="tsp", knownSolution=self.knownSolutions,
                                  givenStatePrepMethod=self.tspStatePreparationMethod,
                                  givenExploreMethod=self.tspMixerMethod,
                                  maxEdge=self.maxEdge)
                                  
                        cost = TSPObjectiveFunction(
                            maxBinary=binary, 
                            adjMatrix=self.adjMatrix, 
                            N=self.N, 
                            maxEdge=self.maxEdge,
                            backToStart=not(self.tspApplicationCalling)
                          )

                    if self.minOrMax == "max" or self.minOrMax == "maximize":
                        if best < cost:
                            best = cost
                            bestBinary = binary
                    else:
                        if best > cost:
                            best = cost
                            bestBinary = binary

                    forGammaBackUp = ansatz[0]
                    forBetaBackUp = ansatz[1]

                    # print(cost)
                    # print(statis)

                    return cost * (-1 if self.minOrMax == "max" or self.minOrMax == "maximize" else 1)
                else: 
                    iterT = 0
                    raise RuntimeError("Early stopping by maxiter")

            if layerMethod == "interpolation":
                gammaStep = 2 * pi / self.p
                betaStep = pi / self.p

                for layer in range(1, self.p + 1):
        
                    def interploation(gamma, beta):
                        
                        modifiedGamma = []
                        modifiedBeta = []

                        modifiedGamma.append(gamma[0])
                        modifiedBeta.append(beta[0])

                        p = layer

                        # Loop from 1 to p-1 for interpolation
                        for i in range(1, p - 1):
                            # Interpolation formula 
                            interpGamma = ((i / p) * gamma[i - 1]) + (((p - i) / p) * gamma[i])
                            interpBeta = ((i / p) * beta[i - 1]) + (((p - i) / p) * beta[i])

                            modifiedGamma.append(interpGamma)
                            modifiedBeta.append(interpBeta)

                        modifiedGamma.append(gamma[-1])
                        modifiedBeta.append(beta[-1])

                        return gamma, beta

                    if not previousGamma:
                        if self.startingWith == "0":
                            gamma = [0]
                            beta = [0]
                        else:
                            gamma = [random.uniform(0, layer * gammaStep)]
                            beta = [random.uniform(pi - layer * betaStep, pi)]
                        bounds = [(0, layer * gammaStep), (pi - layer * betaStep, pi)]
                    else:
                        # Gamma increases
                        gammaLowerBound = previousGamma[-1]
                        # Slicing Method
                        gammaUpperBound = layer * gammaStep

                        # Beta decreases
                        betaUpperBound = previousBeta[-1]
                        # Slicing Method
                        betaLowerBound = pi - layer * betaStep

                        # Sample gamma and beta
                        gamma = [random.uniform(gammaLowerBound, gammaUpperBound)]
                        beta = [random.uniform(betaLowerBound, betaUpperBound)]

                        bounds = [(gammaLowerBound, gammaUpperBound), (betaLowerBound, betaUpperBound)]

                    # print(gamma)
                    # print(beta)
                    # print(bounds)

                    try:
                        result = minimize(ownFlow, 
                                gamma + beta,
                                method=self.method,
                                args=(previousGamma, previousBeta),    
                                bounds=bounds,
                                options={"maxiter":int(100/self.p), "maxfun":int(100/self.p)}
                                )
                        previousGamma.append(result.x[0])
                        previousBeta.append(result.x[1])
                    
                    except:
                        previousGamma.append(forGammaBackUp)
                        previousBeta.append(forBetaBackUp)
                        if layer == self.p + 1:
                            return previousGamma, previousBeta, best, iterT, bestBinary
                        continue

                    if not (layer == self.p + 1):
                        previousGamma, previousBeta = interploation(previousGamma, previousBeta)

                return previousGamma, previousBeta, best, iterT, bestBinary
                
            elif layerMethod == "standard":

                for layer in range(1, self.p + 1):
                    if self.startingWith == "0": 
                      newGamma = [0]
                      newBeta = [0]
                    else:
                      newGamma = [random.uniform(0, 2 * pi)]
                      newBeta = [random.uniform(0, pi)]
                      
                    bounds = [(0, 2 * pi), (0, pi)]

                    try:
                        result = minimize(ownFlow, 
                                newGamma + newBeta,
                                method=self.method,
                                args=(previousGamma, previousBeta),    
                                bounds=bounds,
                                options={"maxiter":int(100/self.p), "maxfun":int(100/self.p)}
                                )
                        previousGamma.append(result.x[0])
                        previousBeta.append(result.x[1])         
                    except:
                        # print("failed")
                        previousGamma.append(forGammaBackUp)
                        previousBeta.append(forBetaBackUp)
                        if layer == self.p + 1:
                            return previousGamma, previousBeta, best, iterT, bestBinary
                        continue

                    # print(layer)
                        

                return previousGamma, previousBeta, best, iterT, bestBinary
            else:
                raise ValueError("Layer method do not supported, please use the following options (interpolation, standard)")


    class fullSearch:
        def __init__(self, N, adjMatrix):
            self.N = N
            self.adjMatrix = adjMatrix
            self.optWeight = 0
            self.solution = None
            self.minOrMax = "max"
            self.problemType = "max-cut"

        def methodConfig(self, minOrMax):
            self.minOrMax = minOrMax.lower()
        
        def problemConfig(self, problemType, maxEdge=0):
            self.problemType = problemType.lower()
            if self.problemType in ["tsp", "traveler salesman problem", "traveling salesman problem"] and maxEdge == 0:
              try: # numpy array doesnt work on this
                self.maxEdge = max( max( row for row in self.adjMatrix) )
              except:
                self.maxEdge = np.max(self.adjMatrix)

        def optSearch(self):
            if self.problemType in ["max-cut", "max cut", "min cut", "min-cut"]:
              possibleCombination = pow(2, self.N)
            elif self.problemType in ["tsp", "traveler salesman problem", "traveling salesman problem"]:
              self.optWeight = float('inf')
              possibleCombination = pow(2, (self.N - 1) * (self.N - 1))
              print(f"N={self.N}: 2^N={possibleCombination}")
            else:
              raise ValueError("Unknown COP type please refer to as \"tsp\" or \"max-cut\"")

            for eachCombination in range(possibleCombination):
                if self.problemType in ["max-cut", "max cut", "min cut", "min-cut"]:
                  binary = format(eachCombination, '0' + str(self.N) + 'b')
                  weight = MaxCutObjectiveFunction(maxBinary=binary, adjMatrix=self.adjMatrix)
                elif self.problemType in ["tsp", "traveler salesman problem", "traveling salesman problem"]:
                  binary = format(eachCombination, '0' + str(pow(self.N - 1, 2)) + 'b')
                  weight = TSPObjectiveFunction(maxBinary=binary, adjMatrix=self.adjMatrix, N=self.N, maxEdge=self.maxEdge)


                if self.minOrMax == "max" or self.minOrMax == "maximize":
                    if self.optWeight < weight:
                        self.optWeight = weight
                        self.solution = binary
                        # print(self.maxCut)
                else:
                    if self.optWeight > weight:
                        self.optWeight = weight
                        self.solution = binary
                        # print(self.optWeight)
                        
            numOpt = 0
            for eachCombination in range(possibleCombination):
                if self.problemType in ["max-cut", "max cut", "min cut", "min-cut"]:
                  binary = format(eachCombination, '0' + str(self.N) + 'b')
                  weight = MaxCutObjectiveFunction(maxBinary=binary, adjMatrix=self.adjMatrix)
                elif self.problemType in ["tsp", "traveler salesman problem", "traveling salesman problem"]:
                  binary = format(eachCombination, '0' + str(pow(self.N - 1, 2)) + 'b')
                  weight = TSPObjectiveFunction(maxBinary=binary, adjMatrix=self.adjMatrix, N=self.N, maxEdge=self.maxEdge)
                
                if self.optWeight == weight:
                    numOpt += 1
            
            binary = self.solution
            # change binary to City decibel for better representation
            if self.problemType in ["tsp", "traveler salesman problem", "traveling salesman problem"]:
              decomposeString = [
                        self.solution[i: i+self.N-1] 
                        for i in range(0, len(self.solution), self.N-1)
                        ]
              path = [ 0 for _ in range(self.N) ]
              for i in range(self.N - 1):
                forAllOnes = [j for j, value in enumerate(decomposeString[i]) if value == '1']
                path[i + 1] = forAllOnes[0] + 1

              self.solution = path

            return self.optWeight, self.solution, numOpt, binary

