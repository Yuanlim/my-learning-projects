import matplotlib.pyplot as plt
import networkx as nx
import math


def showCutedGraph(group, adjMatrix, layout="spring_layout"):
    """
    Plot max-cut graph cut result.

    Parameters
    ----------
    group : string (Required)
        An input data, measured solution string.
    adjMatrix : N*N array (Required)
        An input data, graph propreties.
    layout : string (Optional)
        An input data, NetworkX supported
        plot layout.
        **default = "spring_layout"** 
    """
    G = nx.Graph()
    group = list(group)
    
    # For grid layout
    if layout == "grid_layout":

        # 4x4 grid graph
        N = int(math.sqrt(len(adjMatrix)))

        # which num pixel correlation to its row&col = (i * 4 + j) 
        # i is which row if next row is step of 4
        # (0, 0): is 0 pixel .... (1, 0) is 4 pixel (nodes coordinate):label as
        mapping = {(i, j): i * N + j for i in range(N) for j in range(N)}
        G = nx.relabel_nodes(G, mapping)

        # spring layout auto， too random
        # pos = nx.spring_layout(G)
        # print(pos)

        # manually invert y coordinate (-i)
        pos = {i * N + j: (j, -i) for i in range(N) for j in range(N)}

        # define edge weights
        edge_labels = {}
        for i in range(len(adjMatrix)):
            for j in range(len(adjMatrix[i])):
                value = adjMatrix[i][j]
                if value != 0:
                    G.add_edge(i, j, weight=value)
                    edge_labels[(i, j)] = f'{value:.2f}'

        fig = plt.figure(1, figsize=(10, 10))

        forAllOnes = [i for i, value in enumerate(group) if value == '1']
        colors = ['skyblue' if node in forAllOnes else 'red' for node in G.nodes]

        # define how to draw
        nx.draw(
            G, pos, node_color=colors, with_labels=True, node_size=700, font_size=15, edge_color="black", width=2
        )

        # draw edge labels & define how draw
        nx.draw_networkx_edge_labels(G, pos, edge_labels=edge_labels, font_size=15)
        plt.title('4x4 patches image graph')
        # Show the plot
        plt.show()
    else:
        G.add_nodes_from(range(len(adjMatrix)))

        layoutFunc = getattr(nx, layout, None)

        if layoutFunc is None:
            raise ValueError(f"Layout '{layout}' not found in networkx.")

        forAllOnes = [i for i, value in enumerate(group) if value == '1']
        colors = ['skyblue' if node in forAllOnes else 'red' for node in G.nodes]

        edge_labels = {}
        for i, row in enumerate(adjMatrix):
            for j, value in enumerate(row):
                if value != 0:
                    G.add_edge(i, j, weight=value)
                    edge_labels[(i, j)] = f'{value:.2f}'

        pos = layoutFunc(G)

        # Draw out the problem graph node and edges
        fig = plt.figure(1, figsize=(10, 10))

        nx.draw(G, with_labels=True, node_color=colors, width=2,
                font_size=16, node_size=700, pos=pos)

        # Draw weight to graph
        nx.draw_networkx_edge_labels(G, pos, edge_labels=edge_labels, font_size=20, bbox=dict(alpha=0))
        plt.show()


def showGraph(adjMatrix, layout="spring_layout"):
    """
    Plot any graph based on adjMatrix.

    Parameters
    ----------
    adjMatrix : N*N array (Required)
        An input data, graph propreties.
    layout : string (Optional)
        An input data, NetworkX supported
        plot layout.
        **default = "spring_layout"** 
    """
    # For grid layout
    if layout == "grid_layout":

        # 4x4 grid graph
        N = int(math.sqrt(len(adjMatrix)))
        G = nx.Graph()
        
        # which num pixel correlation to its row&col = (i * 4 + j) 
        # i is which row if next row is step of 4
        # (0, 0): is 0 pixel .... (1, 0) is 4 pixel (nodes coor):label as
        mapping = {(i, j):i * N + j for i in range(N) for j in range(N)}
        G = nx.relabel_nodes(G, mapping)

        # spring layout auto, plt show (pixel is invert)
        # pos = nx.spring_layout(G)
        # print(pos)

        # manually invert y coor (-i)
        pos = {i * N + j: (j, -i) for i in range(N) for j in range(N)}

        # define edge weights
        edge_labels = {}
        for i in range(len(adjMatrix)):
            for j in range(len(adjMatrix[i])):
                value = adjMatrix[i][j]
                if value != 0:
                    G.add_edge(i, j, weight=value)
                    edge_labels[(i, j)] = f'{value:.2f}'

        fig = plt.figure(1, figsize=(10, 10))

        # define how to draw
        nx.draw(
            G, pos, node_color="skyblue", with_labels=True, node_size=700, font_size=15, edge_color="black", width=2
        )

        # draw edge labels & define how draw
        nx.draw_networkx_edge_labels(G, pos, edge_labels=edge_labels, font_size=15)
        plt.title('4x4 patches image graph')
        # Show the plot
        plt.show()

    else:
        # set function as nx.(layout = what user inpuy)
        layoutFunc = getattr(nx, layout, None)

        if layoutFunc is None:
            raise ValueError(f"Layout '{layout}' not found in networkx.")

        G = nx.Graph()
        edge_labels = {}
        for i in range(len(adjMatrix)):
            for j in range(i + 1, len(adjMatrix[i])):
                value = adjMatrix[i][j]
                if value != 0:
                    G.add_edge(i, j, weight=value)
                    edge_labels[(i, j)] = f'{value:.2f}'

        pos = layoutFunc(G)

        fig = plt.figure(1, figsize=(10, 10))

        nx.draw(G, with_labels=True, node_color="skyblue", width=2, font_size=12, node_size=700, pos=pos)

        nx.draw_networkx_edge_labels(G, pos, edge_labels=edge_labels, font_size=15, bbox=dict(alpha=0))
        plt.title('Max-Cut Graph')
        plt.show()
