# WebComposition

WebComposition is a composition based UI Framework for the web. It is based on and deeply integrated with the WebComposition Programming language.
The Programming Language is an interpreted language and the interpreter is written in C# meaning it can run nearly everywhere but most importantly:
in the Web as WASM.

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
Currently the following language features are implemented:

- Variables (automatically trigger recompositions on change)
- if statements
- Value system (currently, there are ints, bools and strings)
- Composables
    - Row
    - Column
    - Button (with onclick event)
    - Text
- Expression Parsing (mathematical expression support as you would expect it for a programming language)
