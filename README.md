# AML

AML is an in-progress toy language loosely inspired by ML family languages such
as F# and Ocaml, with a little bit of Haskell and Lisp thrown in for good
measure.

The main goals are:

- To embrace a highly FP style
- To have a minimum amount of syntax
- To be highly readable
- To have high type-safety
- While still being highly powerful and versatile

## Syntax

### Variable binding `=`, `:`, `'`, `@`, `extends`

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
a : Integer = 5
```

To mark a type parameter, simply prepend with a `'` character:

```aml
my-function : 'a -> String
```

#### Type Bounds

```aml
my-function : 'a -> 'b -> 'c
  where 'a extends D, E
        | 'b extends Map
my-function = a b -> c
```

#### Cases `|`

#### Decorators `@`

It's possible to add decorators to any expression.

##### Simple Decorators

Simple runtime decorators can be added before an expression. Typically, this
would be used to compose functions with a simple syntax.

```aml
add5 = add 5

@add5
add15 = add 10

add15 5 # => 20
```

##### Macro Decorators

It's also possible to add a compile-time macro decorator to an expression.
For example, to add tail-recursion to a function which cannot be automatically
optimized, you can use the `@@tail-recursive` decorator as so:

```aml
@@tail-recursive 0 1
fibonacci = accumulator identity n ->
  match n
  | 0 => accumulator
  | _ => fibonacci (n - 1) identity (accumulator + identity)
```

This will cause AML to first send the name of the binding and the definition of
the binding to the decorator function (in this case, `tail-recursive`). Further
arguments to the decorator function are simply specified inline, as is the case
here.

The result of the decorator will then be bound to the original name, so for this
example, the type of `fibonacci` will be `Integer -> Integer`, and not
`Integer -> Integer -> Integer -> Integer` as is written.

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

A bit of syntax sugar for chained functions:

```aml
a b -> c
```

is strictly equivalent to

```aml
a -> b -> c
```

Do note that the precedence for this syntax sugar is very low; if used in a
larger expression, you may need parenthesis to disambiguate the order of
operations.

```aml
a = 1
b = 2
add-then-do = a -> b -> callback -> (a + b) |> callback

c = add-then-do a b d e -> d + e
# error: d is not defined, as it's not being considered as part of the argument
# list as intended

c = add-then-do a b (d e -> d + e)
# works!
```

Functions are 1st class citizens, and can be bound to variables and passed
around like any other value

```aml
return-42 = _ -> 42
forty-two = compose return-42 to-string
forty-two _ # forty-two is "42"

operationThatCanFail : Integer -> Integer -> Maybe<Float>
operationThatCanFail = a b -> a / b
```

#### Infix functions

To define an infix function, (ie, a function whose argument may be placed in
front of the function call), place the name of the function in parenthesis:

```aml
(%) : Integer -> Integer -> Maybe<Integer>
(%) = a b -> (a / b)
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
add-two : Integer -> Integer
add-two = arg ->
  one = 1
  two = add one one

  two + arg
```

#### Grouping expressions `( )`

Parenthesis may be used to group expressions

### Structures `{}`, `,`, `as`, `...`, `.`, `:`, `[]`, `extends`, `module`, `_`

```aml
a = { b = 1, c = 2 }
{ b, c } = a   # b is 1, c is 2
{ b as d } = a # partial destructuring is allowed, and aliasing is available
Name = use "Some/Module" # collect all from structure/module as alias
Name, { a } = use "Some/Module" # collect all from struct/module as alias, and also extract specific items
```

Punning is allowed:

```aml
value -> { value } # 'a -> Map<String, 'a>
```

#### Tuples `()`

```aml
a = (1, 2, 3)
(b, c) = a
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

### Primitive values: `"`, `'`, `0`, `0.0`, `{}`, `()`

### Pattern matching: `match`, `|`, `=>`, `with`, `,`

If a given pattern has only one variant (e.g., the `Identity` monad which only
has the value in the monad), then a `with` destructuring may be easier to use.

```aml
i = Identity 42

with i (value) => value`
```

To be clear, this is syntactic sugar for a `match` statement that only has a
single arm:

```aml
i = Identity 42

# These two code blocks are equivalent
# ---

match i
| Identity (value) => value * 2

# ---

with i (value) => value * 2
```

### Modules

#### Importing modules

Importing everything from a module

```aml
{ ... } = use "Collections"

# All items from the System.Collections namespace, including `List` and
# `Sequence`, have been imported into the current namespace

List.from (1, 2, 3) |> List.to-sequence |> map (el -> el * 2)
```

Importing multiple items from a module
Note: multi-item imports may span multiple lines

```aml
{ to-upper, to-lower } = use "String"

"Hello, World!" |> to-upper # "HELLO, WORLD!"
"Hello, World!" |> to-lower # "hello, world!"
```

Aliasing imports

```aml
{ map as list-map } = use "Collections/List"
{ map as seq-map } = use "Collections/Sequence"
```

#### Declaring a module

By default, every file declares a module with the same name as the file. So a
a file in the root of the project `/Foo.aml` will define a module named `Foo`.
Files in subfolders will declare namespaced modules; so a file `/Foo/Bar.aml`
will create a module `Foo/Bar`, and so on.

These module definitions are implicit, and do not need to be stated if the
module definition does not require any modification.

If a module definition needs to be modified, for example to extend the module
from another, a `module` statement can be added as the first statement in the
file:

```aml
module MyModule
# methods, constants, etc...
```

To extend the current module with the methods from other modules, add the `extends`
operator and a list of the modules to extend from:

```aml
module Foo extends Bar Baz
```

### I/O

Borrowing some concepts from Haskell, AML has fully managed effects. Most
noticeably, this means that most functions in the `IO` namespace don't
immediately perform an operation, and instead return an `IO<'a>` computation
which must be passed to the `IO.run` function in order to be executed.

For example, to write "Hello, World!" to stdout, you'd write:

```aml
IO.stdout "Hello, World!"
|> IO.run
```

`IO<'a>` has all of the usual FP goodies one would expect here, allowing users
to bind, map, apply, and compose IO operations. To be more specific, `IO<'a>` is
an Effect, AKA Monad. This makes creating IO pipelines a breeze:

```aml
prompt =
  IO.stdin "please input an integer: "
  |> map String.to-integer
  |> then
    match
    | Ok (value) =>
      match value
      | n when (n % 2 is 0) => IO.stdout "Number is even\n"
      | n => IO.stdout "Number is odd\n"
    | _ =>
      IO.stderr "Invalid input, please try again.\n"

# IO has not actually occurred at this point, we've just created the
# computations to do so.

IO.run prompt # IO happens here
```

## AML code conventions

### Operator precedence

Precedence in AML is as follows:

1. Grouped expressions are evaluated first
2. Expressions are evaluated strictly right to left
3. Functions are greedy and will attempt to take as many arguments as possible

Some examples:

1. Grouped expressions

    ```aml
    => (1 + 2) * (3 + 4)
    // ------- | -------
    //       3 *       7
    //       -----------
    //                21
    ```

2. Left to right evaluation

    ```aml
    => 2 + 4 * 3
    //  ---- | |
    //     6 * 3
    //     -----
    //        18
    ```

    ```aml
    => 2.0 |> divide  4.0
    // --------------   |
    // <fn>divide 2.0   |
    // ------------------
    // <fn>divide 2.0 4.0
    // ------------------
    //                0.5
    ```

3. Greedy evaluation

    ```aml
    let do-things = f a b c -> (f (f (f a) b) c)
    let add2 = a + 2

    => do-things add2 1 2 3
    // ((((do-things add2) 1) 2) 3)
    ```

### Casing

Type parameters should either be a single lowercase letter or a short
descriptor in camelCase. For example, `'a` or `'myParameter`

Type names should be in PascalCase.

Variable and function names should be in kebab-case.

### Naming

Functions which return a `Boolean` value should end in a question mark for
readability, e.g. `is-int?`, `is-string?`, and so on.
