# I/O

Borrowing some concepts from Haskell, AML has fully managed effects. Most
noticibly, this means that most functions in the `IO` namespace don't
immediately perform an operation, and instead return an `IO<A>` computation
which must be passed to the `IO.run` function in order to be executed.

For example, to write "Hello, World!" to stdout, you'd write:

```aml
IO.stdout "Hello, World!"
|> IO.run
```

`IO<A>` has all of the usual FP goodies one would expect here, allowing users
to bind, map, apply, and compose IO operations. To be more specific, `IO<A>` is
an [Effect, AKA Monad](./Monads.md). This makes creating IO piplines a breeze:

```aml
prompt =
  IO.stdin "please input an integer: "
  |> map String.to-integer
  |> then match
    | Ok { value } =>
      if (value % 2 is 0)
        (IO.stdout "Number is even\n")
        (IO.stdout "Number is odd\n")
    | _ =>
      IO.stderr "Invalid input, please try again.\n"

# IO has not actually occurred at this point, we've just created the
# computations to do so.

IO.run prompt # IO happens here
```
