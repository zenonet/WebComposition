# WebComposition

WebComposition is a composition based UI Framework for the web. It is based on and deeply integrated with the WebComposition Programming language.
The Programming Language is an interpreted language and the interpreter is written in C# meaning it can run nearly everywhere but most importantly:
in the Web as WASM.

## Quickstart

The easiest way to run WebComposition for yourself is to use the Live Editor hosted [here](https://zenonet.de/interactive/webcomposition).
There, you can make changes to the source code, hit run and see your UI in action.

Example to start with:

```
// Initialize the counter variable with 0. The init keyword is needed so that the variable is not reassigned everytime the UI updates
counter = init 0
// Align every UI element with this block in a column
Column{
    // Display the current counter value. This will update automatically when counter is changed
    Text(counter)
    
    // Align the 2 button next to each other
    Row{
    
        Button({ // The code within this block will be executed when the Button is clicked
            counter = counter + 1 // Increment counter. You can also write this as counter++ but I want to encourage playing around with expressions
        }){
            Text("Increment")
        }
        
        Button({
            counter = counter - 1 // Decrement counter. Again, you could also write counter--
        }){
            Text("Decrement")
        }
        
    }
}
```

I hope you have a little fun playing around with this :)

## What makes WebComposition unique?

WebComposition is heavily inspired by Jetpack Compose for Android however in constrast to Jetpack Compose, WebComposition runs in the Web and
therefore allows easily creating webapps. (Yes, I know that there's Compose Multiplatform but it's Web support is experiemental and probably uses a custom
renderer and not HTML)

Also, since WebComposition is a language and a UI Framework at the same time, it's very easy to automatically recompose UI when state changes because the
Composer has direct access to variables and can detect changes without using any wrapper classes for state.

## The WebComposition Programming Language

The language is developed in this repo as well and its only reason to exist is the UI Framework. It currently is dynamically typed (even though I might change that at some point) and lacks some important features for a programming language (because it's under construction).

## How does it work?

While a program runs, executed Composables can generate HTML code that that accends along the syntax tree allowing parent nodes to wrap it.

## What is currently supported

This framework and under heavy development and might significantly change in the future.
Currently, the following language features are implemented:

- Variables (automatically trigger recompositions on change)
- if statements
- for and while loops
- Value system (currently, there are ints, bools and strings)
- Composables
    - Row
    - Column
    - Button (with onclick event)
    - Text
- Expression Parsing (mathematical and comparison expression support as you would expect it for a programming language)
- Single line comments (//)

## Building from source

To build from source, clone this repo, build the solution and run the Buildtool project.
This will copy all files to /WebComposition/WebCompositionApp/. From there, you need to host them using a http server and open the index.html file in a browser.