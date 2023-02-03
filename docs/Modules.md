# Modules

## Importing modules

Importing everything from a module

```aml
let { ... } = use "System.Collections"

# All items from the System.Collections namespace, including `List` and
# `Sequence`, have been imported into the current namespace

List.fill 3 (el -> el + 1) |> List.to-sequence |> map (el -> el * 2)
```

Importing multiple items from a module
Note: multi-item imports may span multiple lines

```aml
let { to-string, to-lower } = use "System.String"

"Hello, World!" |> to-upper # "HELLO, WORLD!"
"Hello, World!" |> to-lower # "hello, world!"
```

Aliasing imports

```aml
let { map as list-map } = use "System.Collections.List"
let { map as seq-map } = use "System.Collections.Sequence"
```
