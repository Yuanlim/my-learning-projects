if __name__ == "__main__":
    n = int(input())
    student_marks = {}
    for _ in range(n):
        name, *line = input().split()
        scores = list(map(float, line))
        student_marks[name] = scores
    query_name = input()

    n = len(student_marks[query_name])
    total = 0
    for i in range(n):
        total += student_marks[query_name][i]

    print(f"{total / n:.2f}")
