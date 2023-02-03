# Thenable

## `then`

```aml
then: M<A> -> (A -> M<B>) -> M<B>
```

A method which allowes chaining functions which return instances of the
current type.

```aml
(Maybe.from 42)
|> then (value -> value / 2) # Maybe<Float> (21.0)
|> then (value -> value / 5) # Maybe<Float> (4.2)
```

An alias for `then`

An alias for `then`

## `>>=`

```aml
(>>=): M<A> -> (A -> M<B>) -> M<B>
```

An infix alias for `then`

```aml
(Maybe.from 42)
>>= (value -> value / 2) # Maybe<Float> (21.0)
>>= (value -> value / 5) # Maybe<Float> (4.2)
```

## `=<<`

```aml
(=<<): (A -> M<B>) -> M<A> -> M<B>
```

An infix alias for `then`, but with the arguments reversed

```aml
(value -> value / 2) =<< (Maybe.from 42) # Maybe<Float> (21)
```

## `chain`

```aml
chain: (A -> M<B>) -> (B -> M<C>) -> A -> M<C>
```

Composes two functions by chaining.

```aml
{ decodeAsync } = use "System/Text/JSON"
{ GET } = use "System/Web/Http"

fetch-json
  : URL -> Async<Result<JsonValue, _>>
  = chain
    GET
    (response -> decodeAsync response.Body)
```

## `>=>`

```aml
(>=>): (A -> M<B>) -> (B -> M<C>) -> A -> M<C>
```

An infix alias for `chain`

```aml
{ decodeAsync } = use "System/Text/JSON"
{ GET } = use "System/Web/Http"

fetch-json = GET >=> (response -> decodeAsync response.Body)
```

## `<=<`

```aml
(<=<): (B -> M<C>) -> (A -> M<B>) -> A -> M<C>
```

An infix alias for `chain`, but with the arguments reversed

```aml
{ decodeAsync } = use "System/Text/JSON"
{ GET } = use "System/Web/Http"

fetch-json = (response -> decodeAsync response.Body) <=< GET
```
