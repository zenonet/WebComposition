# Data Types

The WebComposition programming language currently supports the following data types:

- Ints (32 bit integer values)
- Strings
- Bools
- Lambda functions

## Strings

Strings can be written as literals by containing it in "":

```wcp
s = "yeet"
```

Strings can also be concatenated using the + operator:

```wcp
a = "ye"
b = "et"
c = a + b
Text(c) // This would create a text `yeet`
```

and values can be inserted into them using string interpolation:

```
i = 6
j = 18
a = "I has a value of {i} and j has a value of {j}"
Text(a) // This would create a text `I has a value of 6 and j has a value of 18`
```

## Ints

Ints can also be written as literals:

```wcp
i = 6
```

Ints can also be used in mathematical expressions.

## Bools

Bools can be written as literals using the `true` and `false` keywords:
```wcp
b = true
```

## Lambda functions:

Lambda functions can be written like this:

```wcp

myLambda = {
    // any code in here is part of the lambda function
}
```

In some cases, a lambda functions is executed with a parameter. This parameter is accessible using the `it` keyword:

```wcp
t = init ""
myLambdaWithParameter = {
    t = it
}
```