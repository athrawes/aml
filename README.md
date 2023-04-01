# AML

AML is an in-progress toy language loosely inspired by ML family languages such
as F# and Ocaml, with a little bit of Haskell and Lisp thrown in for good
measure.

The main goals are:

* To embrace a highly FP style
* To have a minimum amount of syntax
* To be highly readable
* To have high type-safety
* While still being highly powerful and versatile

## Syntax

### Variable binding `=`, `:`, `'`, `@`

```aml
a = 5
```

Let bindings are expressions that evaluate to the value they are assigned to

```aml
a = 5
|> to-string # "5"; also, `a` is bound to an integer 5
```

#### Type declaration `:`

When binding variables, a type declaration may be provided

```aml
a: Integer = 5
```

To mark a type parameter, simply prepend with a `'` character:

```aml
my-function: 'a -> String
```

#### References & Lifetime annotations

```aml
# a function whose argument's type is inferred but is a reference with a
# lifetime of &a
my-function: &a -> String

# a function whose argument's type is 'a and is a reference with lifetime &a
other-callback: &a'a -> String

# a function whose argument's type is 'a and is a reference with an inferred
# lifetime
another-callback: &'a -> String
```

#### Type Bounds

```aml
my-function
  : 'a -> 'b -> 'c
    where 'a :> D, E
    where 'b :> Map
  = a b -> c
```

#### Cases `|`

#### Decorators `@`

It's possible to add decorators to any assignment binding.

For example, to add tail-recursion to a function which cannot be automatically
optimized, you can use the `@tail-recursive` decorator as in this pseudo-code:

```aml
module MyModule :> Mappable =
  @tail-recursive MyModule.Empty MyModule.id
  map = accumulator identity my-module callback ->
    # ... some code which is tail-recursive and behaves as `map` should ...
    # ... no hand-waving here, I swear 🤪 ...
```

This will cause AML to first send the name of the binding and the definition of
the binding to the decorator function (in this case, `@tail-recursive`). Further
arguments to the decorator function are simply specified inline, as is the case
here.

The result of the decorator will then be bound to the original name, so for this
example, the type of `MyModule.map` would be

```aml
map: MyModule 'a -> ('a -> 'b) -> MyModule 'b
```

and not

```aml
map: MyModule _ -> MyModule.id -> MyModule 'a -> ('a -> 'b) -> MyModule 'b
```

A simpler example might be as follows:

```aml
add-one
  : _ -> Number -> Number
  = _bindName bindValue -> bindValue + 1

# The @add-one decorator intercepts the binding, so the number 6 is what is
# actually bound to `result`.
@add-one
result = 5
```

Obviously, this can lead to easy to miss changes in bindings if misused, as
above. If not reading carefully, one _might_ have intuited that `result` should
have been bound to 5 instead of 6, so it is _highly_ advised that this technique
is used only in cases where doing so leads to an overall improvement in
readability.

### Function definition `->`, `( )`

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
  : Integer -> Integer -> Maybe Float
  = a b -> a / b
```

#### Infix functions

To define an infix function, (ie, a function whose argument may be placed in
front of the function call), place the name of the function in parenthesis:

```aml
(%): Integer -> Integer -> Maybe Integer
  = a b -> (a / b)
    >>= to-integer
    >>= (multiply b)
    >>= (subtract a)

6 % 3 # returns (Some 0)
```

As always, AML can automatically fill in the types

```aml
(+) = add # Integer -> Integer
```

### Scopes and expressions `( )`

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

#### Grouping expressions `( )`

Parenthesis may be used to group expressions

### Structures `{}`, `,`, `as`, `...`, `.`, `:`, `[]`, `:>` `module`, `_`

```aml
a = { b = 1, c = 2 }
{ b, c } = a   # b is 1, c is 2
{ b as d } = a # partial destructuring is allowed, and aliasing is available
{ _ as Name } = use "Some/Module" # collect all from structure/module as alias
```

Punning is allowed:

```aml
value -> { value } # 'a -> Map String 'a
```

#### Tuples `[]`

```aml
a = [1, 2, 3]
[b, c] = a
```

#### Struct member accessor `.`

```aml
a = { b = { c = 42 } }

a.b.c # 42
```

### Unit `_`

When specified as a function parameter, ignores this parameter.

When used in a destructuring operation, such as a `match` arm, ignores the
destructured value.

When used in a guard expression, represents the default case.

### Comments `#`

Comments immediately above modules and functions are interpreted as
documentation comments.

### Macros `` expr`macro` ``, `${}`

```aml
sql-query
  : 't -> SqlQuery
  = sql`SELECT * FROM \`users\` WHERE 'name' = ${name}`

sql-prepared-statement = sql-query "John"
```

### Interop `extern`

Functions and values from other languages can be imported by using the `extern`
keyword.

```aml
module File =
  type File

  fopen: String -> String -> Maybe File
    = filename mode ->
      f = String.toCharList filename
      m = String.toCharList mode
      extern fopen [f, m]
```

### Primitive values: `"`, `'`, `0`, `0.0`, `{}`, `[]`

### Pattern matching: `match`, `|`, `->`

### Modules

#### Importing modules

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

#### Declaring a module

Declaring a module is as simple as

```aml
module MyModule =
  # methods, constants, etc...
```

### I/O

Borrowing some concepts from Haskell, AML has fully managed effects. Most
noticeably, this means that most functions in the `IO` namespace don't
immediately perform an operation, and instead return an `IO 'a` computation
which must be passed to the `IO.run` function in order to be executed.

For example, to write "Hello, World!" to stdout, you'd write:

```aml
IO.stdout "Hello, World!"
|> IO.run
```

`IO 'a` has all of the usual FP goodies one would expect here, allowing users
to bind, map, apply, and compose IO operations. To be more specific, `IO 'a` is
an Effect, AKA Monad. This makes creating IO pipelines a breeze:

```aml
prompt =
  IO.stdin "please input an integer: "
  |> map String.to-integer
  |> then match
    | Ok value => match value
      | n when (n % 2 is 0) => IO.stdout "Number is even\n"
      | n => (IO.stdout "Number is odd\n"
    | _ =>
      IO.stderr "Invalid input, please try again.\n"

# IO has not actually occurred at this point, we've just created the
# computations to do so.

IO.run prompt # IO happens here
```

### AML code conventions

#### Casing

Type parameters should either be a single capital letter or a short descriptor
in PascalCase. For example, `'a` or `'MyParameter`

Lifetime parameters should be either a single lower case letter or a short
descriptor in kebab-case. For example, `&a` or `&program-lifetime`.

Type names should be in PascalCase.

Variable and function names should be in kebab-case.
