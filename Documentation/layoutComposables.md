# Layout composables

As of now, there are tree layout composables:

- Box
  - Just a pure HTML div
- Column
  - Lays out items below each other
  - Uses a flexbox div in HTML
- Row
  - Lays out items next to each other
  - Uses a flexbox div in HTML

All of them don't take parameters but need a content block:

```wcp

Box{
    Text("hehe")
}
Column{
    Text("up")
    Text("down")
}

Row{
    Text("left")
    Text("right")
}
```