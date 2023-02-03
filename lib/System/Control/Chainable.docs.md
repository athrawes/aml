# Chainable

## `then`

```aml
then : (^A -> ^M[^B]) -> ^M[^A] -> ^M[^B]
```

A method which allowes chaining functions which return instances of the
current type.

```aml
(Maybe.from 42)
|> Maybe.then (value -> value / 2) # Maybe[Float] (21.0)
|> Maybe.then (value -> value / 5) # Maybe[Float] (4.2)
```

## `chain`

```aml
chain : (^A -> ^M[^B]) -> ^M[^A] -> ^M[^B]
```

An alias for `then`

## `bind`

```aml
bind : (^A -> ^M[^B]) -> ^M[^A] -> ^M[^B]
```

An alias for `then`

## `=<<`

```aml
=<< : infix (^A -> ^M[^B]) -> ^M[^A] -> ^M[^B]
```

An infix alias for `then`

```aml
(value -> value / 2) =<< (Maybe.from 42) # Maybe[Float] (21)
```

## `>>=`

```aml
>>= : infix ^M[^A] -> (^A -> ^M[^B]) -> ^M[^B]
```

An infix alias for `then`, but with the arguments reversed to allow for
left to right composition order.

```aml
(Maybe.from 42)
>>= (value -> value / 2) # Maybe[Float] (21.0)
>>= (value -> value / 5) # Maybe[Float] (4.2)
```

## `chain-compose`

```aml
chain-compose : (^A -> ^M[^B]) -> (^B -> ^M[^C]) -> ^A -> ^M[^C]
```

Composes two functions by chaining.

```aml
let sleep-then-return
    : Integer -> Async[String]
    = chain-compose
        Async.sleep # (Integer -> Async[Unit])
        (_ -> Async.resolve "Finished Sleeping") # (Unit -> Async[String])
```

## `>=>`

```aml
>=> : infix (^A -> ^M[^B]) -> (^B -> ^M[^C]) -> ^A -> ^M[^C]
```

An infix alias for `chain-compose`

```aml
Async.sleep >=> (_ -> Async.resolve "Finished Sleeping")
```