if __name__ == "__main__":
    s = input()
    hasAlnum = hasAlpha = hasDigit = hasLower = hasUpper = False

    for c in s:
        if not hasAlnum and c.isalnum():
            hasAlnum = True
        if not hasAlpha and c.isalpha():
            hasAlpha = True
        if not hasDigit and c.isdigit():
            hasDigit = True
        if not hasLower and c.islower():
            hasLower = True
        if not hasUpper and c.isupper():
            hasUpper = True

    print(hasAlnum)
    print(hasAlpha)
    print(hasDigit)
    print(hasLower)
    print(hasUpper)
