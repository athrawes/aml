# Modules

## Importing modules

Importing everything from a module

```aml
{ ... } = use "System/Collections"

# All items from the System.Collections namespace, including `List` and
# `Sequence`, have been imported into the current namespace

List.from [1, 2, 3] |> List.to-sequence |> map (el -> el * 2)
```

Importing multiple items from a module
Note: multi-item imports may span multiple lines

```aml
{ to-upper, to-lower } = use "System/String"

"Hello, World!" |> to-upper # "HELLO, WORLD!"
"Hello, World!" |> to-lower # "hello, world!"
```

Aliasing imports

```aml
{ map as list-map } = use "System/Collections/List"
{ map as seq-map } = use "System/Collections/Sequence"
```

## Declaring a module

Declaring a module is as simple as

```aml
module MyModule =
  # methods, constants, etc...
```
