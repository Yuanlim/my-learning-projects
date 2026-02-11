def runs_ex(a: int, b: int) -> None:
    sumOf(a, b)
    difference(a, b)
    productOf(a, b)


def sumOf(a: int, b: int) -> None:
    print(a + b)


def difference(a: int, b: int) -> None:
    print(a - b)


def productOf(a: int, b: int) -> None:
    print(a * b)


if __name__ == "__main__":
    a = int(input())
    b = int(input())
    runs_ex(a, b)
