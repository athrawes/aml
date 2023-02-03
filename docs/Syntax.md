# Syntax

## Variable binding `let`, `:=`, `::`, `^`

```aml
let a := 5
```

### Type declaration `^`

When binding variables, a type declaration may be provided

```aml
let a :: Integer := 5
```

### Cases `|`

## Function definition `->`

Functions may only take in a single value and return a single value.

```aml
a -> b # takes an arguement `a`, and returns the value contained in `b`
```

Functions can be chained

```aml
a -> b -> c
```

Functions are 1st class citizens, and can be bound to variables and passed
around like any other value

```aml
let return-42 := _ -> 42
let fourty-two := return-42 _ |> std.int.to-string # fourty-two is "42"

let operationThatCanFail :: Integer -> Integer -> Result[Float, _]
  := a -> b -> a / b
```

## Structures `{}`, `,`, `as`, `...`, `.`, `:`, `[]`

```aml
let a := { b: 1, c: 2 }
let { b, c } := a # b is 1, c is 2
let { b as d } := a # partial destructuring is allowed, and aliasing is available
```

### Tuples `[]`

## Type declarations `type`

### Struct member accessor `.`

```aml
let a := { b: { c: 42 } }

a.b.c # 42
```

## Unit `_`

When specified as a function parameter, ignores this parameter.

When used in a destructuring operation, such as a `match` arm, ignores the
destructured value.

When used in a guard expression, represents the default case.

## Expression grouping `()`

## Infix functions `\``

```aml
let `infixDivision` = a / b
6 infixDivision 3 # 2
```

## Comments `#`, `//`, `/* */`

### Documentation Comments `/** */`

## Primitive values: `"`, `'`, `0`, `0.0`, `true, false`, `{}`, `[]`
