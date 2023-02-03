# Syntax

## Variable binding `let`, `=`, `:`, `^`

```aml
let a = 5
```

Let bindings are expressions that evaluate to the value they are assigned to

```aml
let a = 5
|> to-string # "5"; also, `a` is bound to an integer 5
```

### Type declaration `:`, `^`

When binding variables, a type declaration may be provided

```aml
let a : Integer = 5
```

In cases where the type is intended to be generic, placing a `^` character
indicates such a type parameter:

```aml
type my-function : ^T -> String
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
let return-42 = _ -> 42
let fourty-two = compose return-42 to-string
fourty-two () # fourty-two is "42"

let operationThatCanFail
  : Integer -> Integer -> Maybe[Float]
  = a -> b -> a / b
```

### Infix functions

To define an infix function, (ie, a function whose argument may be placed in
front of the function call), provide a type declaration for your function
preceded by the `infix` keyword:

```aml
let %
  : infix Integer -> Integer -> Maybe[Integer]
  = a -> b ->
    (a / b)
    |> map to-integer
    |> map (multiply b)
    |> map (subtract a)

6 % 3 # returns (Some 0)
```

As always, AML can automatically fill in the types

```aml
let + : infix = add # :: Integer -> Integer
```

## Scopes and expressions `( )`

AML is an expression-based language; there are no statements, as such. As
previously noted, binding variables using `let` expressions are expressions
which evaluate to the value of the bound variable.

This applies to function definitions as well:

```aml
let call-and-add-two = fn -> 2 + (fn 1)

let add-one = add 1 # Number -> Number
|> call-and-add-two # 4; add-one is passed to call-and-add-two
```

Note the lack of indentation in this example; this indicates to AML that we want
to evaluate the second line as continuing at the same level as the first. This
is equivelant to:

```aml
let call-and-add-two = fn -> 2 + (fn 1)

(let add-one = add 1) |> call-and-add-two # 4
```

To indicate that subsequent lines should be considered as part of the scope of
the function definition, simply indent the subsequent lines:

```aml
let add-two
  : Integer -> Integer
  = arg ->
    let one = 1
    let two = add one one

    two + arg
```

### Grouping expressions `( )`

Parenthesis may be used to group expressions

## Structures `{}`, `,`, `as`, `...`, `.`, `:`, `[]`, `extends` `module`

```aml
let a = { b: 1, c: 2 }
let { b, c } = a   # b is 1, c is 2
let { b as d } = a # partial destructuring is allowed, and aliasing is available
```

Punning is allowed:

```aml
value -> { value } # ^T -> Struct[^T]
```

### Tuples `[]`

### Struct member accessor `.`

```aml
let a = { b: { c: 42 } }

a.b.c # 42
```

## Type declarations `type`

## Unit `_`, `()`

When specified as a function parameter, ignores this parameter.

When used in a destructuring operation, such as a `match` arm, ignores the
destructured value.

When used in a guard expression, represents the default case.

## Comments `#`, `//`, `/* */`

### Documentation Comments `/** */`

## Macros `` expr`macro` ``, `${}`

```aml
let sql-query
  : ^T -> SqlQuery
  = name -> sql`SELECT * FROM \`users\` WHERE 'name' = ${name}`

let sql-prepared-statement = sql-query "John"
```

## Primitive values: `"`, `'`, `0`, `0.0`, `true, false`, `{}`, `[]`

## Pattern matching: `match`, `|`, `=>`
