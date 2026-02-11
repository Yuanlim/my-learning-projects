if __name__ == "__main__":
    N = int(input())
    arr = []

    for _ in range(N):
        whatToDo = input().split()

        action = whatToDo[0]

        if action == "append":
            arr.append(int(whatToDo[1]))
        elif action == "insert":
            arr.insert(int(whatToDo[1]), int(whatToDo[2]))
        elif action == "remove":
            arr.remove(int(whatToDo[1]))
        elif action == "sort":
            arr.sort()
        elif action == "pop":
            arr.pop()
        elif action == "reverse":
            arr.reverse()
        elif action == "print":
            print(arr)
        else:
            pass
