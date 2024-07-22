# Basic syntax

The WebComposition programming language is mainly inspired by Kotlin and C-like languages.

## Variables

Variables are dynamically typed and implicitly declared (simmilar to python)
You can assign a variable `m` and assign it the value 6 like this:
````wcp
m = 6
````
You can reassign the variable at any time with any value (even of another data type).
If you have a variable assignment in composable code, you can write init before the value to assign to the variable to make sure the variable
is only assigned at the start of the program and not on every recomposition:
```wcp
m = init 6
```

## Expressions

You can use expressions like in most other programming languages:
```wcp
a = 12
m = 6+a*2
```

## Flow statements

### If-Statements

You can check for conditions using an if statement like this:

```wcp
if(i < 5){
    // do something
}
```
This statement only runs the code in the block if the variable i is greater than 5. You can pass any bool value to an if statement as its condition.

You can also use else statements like this:

```wcp
if(i < 5){
    // only happens if i < 5
}
else{
    // only happens if not i < 5
}
```

### While loops

While loops repeat a block of code as long as it's condition evaluates to true:

```wcp
while(<condition(any bool value)>){
    // this is repeated until the condition switches to false
}
```

### For loops

You can also write for loops like in any C-like language:

```wcp
for(i=0;i<5;i++){
    // this is repeated 5 times (until the condition (i<5) is no longer true)
}
```

## Functions

Like in most other languages, you can call functions by writing their name followed by a pair of parentheses containing the parameters:
```wcp
i = int("612")
```
Function names are generally in lowercase.

## Composables

Composables are technically functions as well but their names are generally written in PascalCase.
Their syntax matches the one of normal functions but some, so called block-composables can have a content block behind their parentheses like this:
```wcp
Column(){
    Text("Yeet")
}
```
In this case, both Text() and Column() are composable calls but column is a block-composable and its content block contains the call to Text().

When a block composable does not have parameters, you can neglect their parentheses like this:
```wcp
Column{
    Text("Yeet")
}
```

### Styles

You can add CSS-styles to composable calls adding a .styled call to them like this:

```wcp
Column{

}.styled{
    background: "green"
}
```
In the block of the `.styled` extension, you can assign any CSS-property.
The values you are assigning are normal webcomposition expressions that are converted to strings in order to add them to the CSS.
This means that you can calculate them, for example like this:

```wcp
m = 6

Column{
    Text("yeet")
}.styled{
    width: m + 2 px,
    height: m / 2 px
}
```

As you can see, you can also add CSS-units to the end of the expression.

## Custom function definitions

You can define custom functions like this:

```wcp
func myFunction(myParameter1, myParameter2){
    // In here, you can do something with the parameters.
}

```

or custom composables like this:

```wcp
comp repeat(text, count){
    for(i=0;i<count;i++){
        Text(text)
    }
}
```