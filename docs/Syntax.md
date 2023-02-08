# Syntax

## Variable binding `=`, `:`, `'`

```aml
a = 5
```

Let bindings are expressions that evaluate to the value they are assigned to

```aml
a = 5
|> to-string # "5"; also, `a` is bound to an integer 5
```

### Type declaration `:`

When binding variables, a type declaration may be provided

```aml
a: Integer = 5
```

If the symbol provided cannot be resolved to a known type, AML interprets the
parameter as a generic type parameter:

```aml
my-function: A -> String
```

### References & Lifetime annotations

```aml
# a function whose argument's type is inferred but is a reference with a
# lifetime of 'a
my-function: &'a -> String

# a function whose argument's type is A and is a reference with a lifetime of 'a
other-callback: &A'a -> String

# a function whose argument's type is A and is a reference with an inferred
# lifetime
another-callback: &A -> String
```

### Type Bounds

```aml
my-function
  : A -> B -> C
    where A :> D, E
    where B :> Map
  = a -> b -> c
```

### Cases `|`

## Function definition `->`, `( )`

Functions may only take in a single value and return a single value.

```aml
a -> b # takes an argument `a`, and returns the value contained in `b`
```

Functions can be chained

```aml
a -> b -> c
# evaluates to: (a -> (b -> (c)))
```

Functions are 1st class citizens, and can be bound to variables and passed
around like any other value

```aml
return-42 = _ -> 42
forty-two = compose return-42 to-string
forty-two _ # forty-two is "42"

operationThatCanFail
  : Integer -> Integer -> Maybe<Float>
  = a -> b -> a / b
```

### Infix functions

To define an infix function, (ie, a function whose argument may be placed in
front of the function call), place the name of the function in parenthesis:

```aml
(%): Integer -> Integer -> Maybe<Integer>
  = a -> b ->
    (a / b)
    >>= to-integer
    >>= (multiply b)
    >>= (subtract a)

6 % 3 # returns (Some 0)
```

As always, AML can automatically fill in the types

```aml
(+) = add # Integer -> Integer
```

## Scopes and expressions `( )`

AML is an expression-based language.
This applies to function definitions as well:

```aml
call-and-add-two = callback -> 2 + (callback 1)

add-one = add 1 # Number -> Number
|> call-and-add-two # 4; add-one is passed to call-and-add-two
```

Note the lack of indentation in this example; this indicates to AML that we
want to evaluate the second line as continuing at the same level as the first.
This is equivalent to:

```aml
call-and-add-two = callback -> 2 + (callback 1)

(add-one = add 1) |> call-and-add-two # 4
```

To indicate that subsequent lines should be considered as part of the scope of
the function definition, simply indent the subsequent lines:

```aml
add-two
  : Integer -> Integer
  = arg ->
    one = 1
    two = add one one

    two + arg
```

### Grouping expressions `( )`

Parenthesis may be used to group expressions

## Structures `{}`, `,`, `as`, `...`, `.`, `:`, `[]`, `:>` `module`, `_`

```aml
a = { b = 1, c = 2 }
{ b, c } = a   # b is 1, c is 2
{ b as d } = a # partial destructuring is allowed, and aliasing is available
{ _ as Name } = use "Some/Module" # collect all from structure/module as alias
```

Punning is allowed:

```aml
value -> { value } # A -> Map<String, A>
```

### Tuples `[]`

```aml
a = [1, 2, 3]
[b, c] = a
```

### Struct member accessor `.`

```aml
a = { b = { c = 42 } }

a.b.c # 42
```

## Unit `_`

When specified as a function parameter, ignores this parameter.

When used in a destructuring operation, such as a `match` arm, ignores the
destructured value.

When used in a guard expression, represents the default case.

## Comments `#`

Comments immediately above modules and functions are interpreted as
documentation comments.

## Macros `` expr`macro` ``, `${}`

```aml
sql-query
  : T -> SqlQuery
  = sql`SELECT * FROM \`users\` WHERE 'name' = ${name}`

sql-prepared-statement = sql-query "John"
```

## Primitive values: `"`, `'`, `0`, `0.0`, `{}`, `[]`

## Pattern matching: `match`, `|`, `->`
