# Syntax

## Variable binding `let`, `:=`, `::`, `^`

```aml
let a := 5
```

### Type declaration `::`

When binding variables, a type declaration may be provided

```aml
let a :: Integer := 5
```

### Cases `|`

## Function definition `->`, `infix`, `^`

Functions may only take in a single value and return a single value.

```aml
a -> b # takes an arguement `a`, and returns the value contained in `b`
```

Functions can be chained

```aml
a -> b -> c
# evaluates to: (a -> (b -> (c)))
```

Functions are 1st class citizens, and can be bound to variables and passed
around like any other value

```aml
let return-42 := _ -> 42
let fourty-two := return-42 _ |> std.int.to-string # fourty-two is "42"

let operationThatCanFail
  :: Integer -> Integer -> Maybe[Float]
  := a -> b -> a / b
```

### Infix functions

To define an infix function, (ie, a function whose argument may be placed in
front of the function call), provide a type declaration for your function
preceded by the `infix` keyword:

```aml
let %
  :: infix Integer -> Integer -> Maybe[Integer]
  := a -> b ->
    (a / b)
    <&> std.float.to-int
    <&> (multiply b)
    <&> (subtract a)

6 % 3 # returns (Some 0)
```

As always, AML can automatically fill in the types

```aml
let **
  :: infix
  := x -> y -> match y
    | when y is 0
      => 1
    | when (y is Integer) and (y > 1)
      => (x + x) ** (y - 1)
```

## Structures `{}`, `,`, `as`, `...`, `.`, `:`, `[]`

```aml
let a := { b: 1, c: 2 }
let { b, c } := a # b is 1, c is 2
let { b as d } := a # partial destructuring is allowed, and aliasing is available
```

## Modules `interface`, `extends`

### Tuples `[]`

## Type declarations `type`

### Struct member accessor `.`

```aml
let a := { b: { c: 42 } }

a.b.c # 42
```

## Unit `_`, `()`

When specified as a function parameter, ignores this parameter.

When used in a destructuring operation, such as a `match` arm, ignores the
destructured value.

When used in a guard expression, represents the default case.

## Expression grouping `()`

## Comments `#`, `//`, `/* */`

### Documentation Comments `/** */`

## Macros `` expr`macro` ``, `${}`

```aml
let sql-query
  :: ^T -> SqlQuery
  := name -> sql`SELECT * FROM \`users\` WHERE 'name' = ${name}`

let sql-prepared-statement := sql-query "John"
```

## Primitive values: `"`, `'`, `0`, `0.0`, `true, false`, `{}`, `[]`

## Pattern matching: `match`, `|`, `=>`
