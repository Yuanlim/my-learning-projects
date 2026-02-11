if __name__ == "__main__":
    nested_list = []

    for _ in range(int(input())):
        name = input()
        score = float(input())
        nested_list.append([name, score])

    nested_list.sort(key=lambda x: (x[1], x[0]))

    # multiple lowest
    i = 0  # this note second lowest index
    # search the sec lowest
    while i < len(nested_list) and nested_list[i][1] == nested_list[0][1]:
        i += 1

    second_scorce = nested_list[i][1]
    while i < len(nested_list) and second_scorce == nested_list[i][1]:
        print(nested_list[i][0])
        i += 1
