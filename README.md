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

### Variable binding `==`, `:`, ``` ` ```

```aml
a = 5
```

Let bindings are expressions that evaluate to the value they are assigned to

```aml
a = 5
|> to_string # "5"; also, `a` is bound to an integer 5
```

#### Type declaration `:`, ``` ` ```

When binding variables, a type declaration may be provided

```aml
a : Integer = 5
```

To mark a type parameter, simply prepend with a ``` ` ``` character:

```aml
my_function : `a -> String
```

#### Type Bounds

```aml
my_function : `a -> `b -> `c
    where `a is Orderable
    where `a is Functor
    where `b is Map
my_function = a -> b -> c
```

#### Cases `|`

### Functions `->`, `( )`

Functions may only take in a single value and return a single value.

```aml
a -> b # takes an argument `a`, and returns the value contained in `b`
```

Function definitions can be chained

```aml
a -> b -> c
# evaluates to: (a -> (b -> (c)))
```

Functions are 1st class citizens, and can be bound to variables and passed
around like any other value

```aml
return_42 = _ -> 42
forty_two = compose return_42 to_string
forty_two _ # forty_two is "42"

operationThatCanFail : ℤ -> ℤ -> Maybe<Float>
operationThatCanFail = a -> b -> a / b
```

#### Infix functions

To define an infix function, (ie, a function whose argument may be placed in
front of the function call), place the name of the function in parenthesis:

```aml
(%) : ℤ -> ℤ -> Maybe<ℤ>
(%) = a -> b -> (a / b)
	>>= to_integer
	>>= (multiply b)
	>>= (subtract a)

6 % 3 # returns (Some 0)
```

As always, AML can automatically fill in the types

```aml
(+) = add # ℤ -> ℤ
```

#### Calling functions

To call a function, simply specify an argument after the function name:

```aml
# add_1 will add 1 to any integer
add_1 : ℤ -> ℤ

add_1 1
# 2

add_1 2
# 3
```

For infix functions, specify the arguments both before and after the function:

```aml
(%) : ℤ -> ℤ -> Maybe<ℤ>

6 % 3
# (Some 0)
```

### Scopes and expressions `( )`

AML is an expression-based language.
This applies to function definitions as well:

```aml
call_and_add_two = callback -> 2 + (callback 1)

add_one = add 1 # Number -> Number
|> call_and_add_two # 4; add_one is passed to call_and_add_two
```

Note the lack of indentation in this example; this indicates to AML that we
want to evaluate the second line as continuing at the same level as the first.
This is equivalent to:

```aml
call_and_add_two = callback -> 2 + (callback 1)

(add_one = add 1) |> call_and_add_two # 4
```

To indicate that subsequent lines should be considered as part of the scope of
the function definition, simply indent the subsequent lines:

```aml
add_two : ℤ -> ℤ
add_two = arg ->
	one = 1
	two = add one one

	two + arg
```

#### Grouping expressions `( )`

Parenthesis may be used to group expressions

### Structures `{}`, `,`, `as`, `...`, `.`, `:`, `[]`, `_`

```aml
a = { b: 1, c: 2 }
{ b, c } = a   # b is 1, c is 2
{ b as d } = a # partial destructuring is allowed, and aliasing is available
from "Some/Module" import * as Name
```

Punning is allowed:

```aml
value -> { value } # `a -> Map<String, `a>
```

#### Tuples `()`

```aml
a = (1, 2, 3)
(b, c) = a
```

#### Struct member accessor `.`

```aml
a = { b: { c: 42 } }

a.b.c # 42
```

### Unit `_`

When specified as a function parameter, ignores this parameter.

When used in a destructuring operation, such as a `case` arm, ignores the
destructured value.

When used in a guard expression, represents the default case.

### Comments `#`

Comments immediately above modules and functions are interpreted as
documentation comments.

### Primitive values: `"`, ``` ` ```, `0`, `0.0`, `{}`, `()`

### Pattern matching: `case`, `|`, `=>`, `with`, `,`

### Modules

#### Importing modules

Importing everything from a module

```aml
from "Collections" import *

# All items from the System.Collections namespace, including `List` and
# `Sequence`, have been imported into the current namespace

List.new (1, 2, 3) |> List.to_sequence |> map (el -> el * 2)
```

Importing multiple items from a module
Note: multi-item imports may span multiple lines

```aml
from "String" import { to_upper, to_lower }

"Hello, World!" |> to_upper # "HELLO, WORLD!"
"Hello, World!" |> to_lower # "hello, world!"
```

Aliasing imports

```aml
from "Collections/List" import { map as list_map }
from "Collections/Sequence" import { map as seq_map }
```

### Traits

TODO: fill this out. Basically, Rust traits.

### I/O

Borrowing some concepts from Haskell, AML has fully managed effects. Most
noticeably, this means that most functions in the `IO` namespace don't
immediately perform an operation, and instead return an ```IO<`a>``` computation
which must be passed to the `IO.run` function in order to be executed.

For example, to write "Hello, World!" to stdout, you'd write:

```aml
IO.stdout "Hello, World!"
|> IO.run
```

```IO<`a>``` has all of the usual FP goodies one would expect here, allowing users
to bind, map, apply, and compose IO operations. To be more specific, ```IO<`a>``` is
an Effect, AKA Monad. This makes creating IO pipelines a breeze:

```aml
prompt =
	IO.stdin "please input an integer: "
	|> map String.to_integer
	|> then
		case
		| Ok (value) =>
			case value
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
2. Lines implicitly group all expressions on that line
3. Infix functions implicitly create a group expression with their immediately
   left and right neighbor expressions
4. Expressions are evaluated strictly left to right. Does _not_ respect
   PEMDAS/BODMAS or other mathematical conventions.
5. Functions are greedy and will attempt to take as many arguments as possible,
   taking into account the types of any following expressions.

Some examples:

1. Grouped expressions

    ```aml
    # (1 + 2) * (3 + 4)
    # ------- | -------
    #       3 *       7
    #       -----------
    #                21
    ```

2. Grouping by line

    ```aml
    # 1 +
    # 3 / 4
    # -----
    # (1 +) (3 / 4)
    # ----- -------
    # fn +1    0.75
    # -------------
    #          1.75
    ```

3. Infix functions

   ```aml
   # pair 1  5 * 6
   # --------------
   # pair 1 (5 * 6)
   #    | | -------
   # pair 1 30
   # ---------
   # Pair(1, 30)
   ```

4. Left to right evaluation

    ```aml
    # 2 + 4 * 3
    # ----- | |
    #     6 * 3
    #     -----
    #        18
    ```

    ```aml
    # 2.0 |> divide   4.0
    # -------------------
    # (2.0 |> divide) 4.0
    # ---------------   |
    # fn divide 2.0 $   |
    # ---------------   |
    # fn divide 2.0   4.0
    # -------------------
    #                 0.5
    ```

5. Greedy evaluation

    ```aml
    do_things = f -> a -> b -> c -> (f (f (f a 1) b) c)

    # do_things multiply 1 2 3
    # ------------------------
    # f          -> a -> b -> c -> (f (f (f a 1) b) c)
    # multiply      1    2    3
    # --------------------------------------------
    # (multiply (multiply (multiply 1 1) 2) 3)
    #         |         | -------------- |  |
    #         | (multiply              1 2) |
    #         | --------------------------- |
    # (multiply                           2 3)
    # ----------------------------------------
    #                                        6
    ```

### Casing

Type parameters should either be a single lowercase letter or a short
descriptor in camelCase. For example, ``` `a ``` or ``` `myParameter ```

Type names should be in PascalCase.

Variable and function names should be in snake_case.

### Naming

Functions which return a `Boolean` value should end in a question mark for
readability, e.g. `is_int?`, `is_string?`, and so on.
