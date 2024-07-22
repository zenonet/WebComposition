# Other composables

## Text

Text is probably the most basic composable. It takes a string as it's only parameter and displays it as a span in HTML:
```wcp
Text("hah")
```

## Button

Buttons translate to normal HTML buttons. They take a lambda function as a parameter. This function is executed when the button is clicked.
The button also has a content block.
```
isClicked = init false
Button({
    // This is executed when the button is clicked
    isClicked = true
    // Changing a variable in here will automatically recompose the UI so that anything depending on the variable is updated.
}){
    // This is the buttons content
    if(isClicked){
        Text("Thank you")
    }else{
        Text("Pls click me")
    }
}
```

## TextField

Text fields can be used to let user type some text. They don't have a state, so you'll have to save it yourself.
A text field takes a string as its first parameter and a lambda function that is called when the text changes as its second parameter.
It is highly recommended to create a variable for the content of the text field and to update it in the lambda function to allow for a normal typing experience:

```wcp
txt = init ""

TextField(txt, {
    txt = it // The parameter it contains the current content of the text field. 
})
```

## Checkbox

Checkboxes are pretty similar to TextFields: They don't have a state themselves, and you need to save their value. Checkboxes have a bool value for their state
as their first parameter and a lambda function as their second parameter that is called when the state changes.
```wcp
isChecked = init false

CheckBox(isChecked, {
    isChecked = it // The parameter it contains the current state of the checkbox. 
})
```