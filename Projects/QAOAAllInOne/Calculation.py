from QAOAMethod import Method
from Graphing import *
from CircuitBaseCode import *
from ALLOBJ import *


def modelApproximateCalculation(gamma, beta, N, p, adjMatrix, runs, minOrMax, optCut):

    cost = []
    for i in range(runs):
        values, staticals = createQuantumCircuit(
                      nShots=500, N=N,
                      adjMatrix=adjMatrix, gamma=gamma,
                      beta=beta, p=p
                    )

        # plot_histogram(staticals)
        cost.append(MaxCutObjectiveFunction(maxBinary=values, adjMatrix=adjMatrix))
        print(f"Cost: {cost[i]}")

    avgApproximateRatioCutOfModel = sum([cost[i] / optCut for i in range(len(cost))]) / len(cost)
    print(f"avgApproximateRatioCutOfModel: {avgApproximateRatioCutOfModel}")

    return avgApproximateRatioCutOfModel


def modelPrecision(gamma, beta, N, p, adjMatrix, runs, minOrMax, optCut):
    successfullyFindOpt = 0

    for i in range(runs):
        values, staticals = createQuantumCircuit(
                      nShots=500, N=N,
                      adjMatrix=adjMatrix, gamma=gamma,
                      beta=beta, p=p
                    )

        cost = MaxCutObjectiveFunction(maxBinary=values, adjMatrix=adjMatrix)

        if cost == optCut:
            successfullyFindOpt += 1

        # display(plot_histogram(staticals))
    precision = successfullyFindOpt / runs

    print(f"precision: {precision}")

    return precision
