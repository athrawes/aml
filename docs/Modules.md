# Modules

## Importing modules

Importing everything from a module

```aml
let { ... } := use "std.collections"

# All items from the std.collections namespace, including `list` and `seq`, have
# been imported into the current namespace

list.fill 3 (el -> el + 1) |> list.to-seq |> seq.map (el -> el * 2)
```

Importing multiple items from a module
Note: multi-item imports may span multiple lines

```aml
let { to-string, to-lower } := use "std.string"

"Hello, World!" |> to-upper # "HELLO, WORLD!"
"Hello, World!" |> to-lower # "hello, world!"
```

Aliasing imports

```aml
let { map as list-map } := use "std.collections.list"
let { map as seq-map } := use "std.collections.seq"
```
