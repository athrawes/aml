# Chainable

## `then`

```aml
then :: (^A -> ^M[^B]) -> ^M[^A] -> ^M[^B]
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
chain :: (^A -> ^M[^B]) -> ^M[^A] -> ^M[^B]
```

An alias for `then`

## `=<<`

An infix alias for `then`

```aml
=<< :: infix (^A -> ^M[^B]) -> ^M[^A] -> ^M[^B]
```

```aml
(value -> value / 2) =<< (Maybe.from 42) # Maybe[Float] (21)
```

## `>>=`

An infix alias for `then`, but with the arguments reversed to allow for
left to right composition order.

```aml
>>= :: infix ^M[^A] -> (^A -> ^M[^B]) -> ^M[^B]
```

```aml
(Maybe.from 42)
>>= (value -> value / 2) # Maybe[Float] (21.0)
>>= (value -> value / 5) # Maybe[Float] (4.2)
```
