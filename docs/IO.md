# I/O

Borrowing some concepts from Haskell, AML has fully managed effects. Most
noticibly, this means that most functions in the `io` namespace don't
immediately perform an operation, and instead return an `IO ^T` computation
which must be passed to the `io.run` function in order to be executed.

For example, to write "Hello, World!" to stdout, you'd write:

```aml
"Hello, World!"
|> io.bind
|> posix.io.stdout
|> io.run
```

`IO ^T` has all of the usual FP goodies one would expect here, allowing users
to bind, map, apply, and compose IO operations. To be more specific, `IO ^T`
is a [Monad](./Monads.md). This makes creating IO piplines a breeze:

```aml
let prompt :=
  posix.io.stdin "please input an integer: " # IO String
  |> io.map std.string.to-integer # IO (Result [Integer, _])
  |> io.map result -> # IO _
    match result
    | Ok(value) :: when (value % 2) is 0 =>
      posix.io.stdout "Number is even"
    | Ok(value) :: when (value % 2) is 1 =>
      posix.io.stdout "Number is odd"
    | _ =>
      posix.io.stderr "Invalid input, please try again."

# IO has not actually occurred at this point, we've just created the
# computations to do so.

io.run prompt # IO happens here
```
