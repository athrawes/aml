# Thenable

## `then`

```aml
then: 'm<'a> -> ('a -> 'm<'b>) -> 'm<'b>
```

A method which allows chaining functions which return instances of the
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
(>>=): 'm<'a> -> ('a -> 'm<'b>) -> 'm<'b>
```

An infix alias for `then`

```aml
(Maybe.from 42)
>>= (value -> value / 2) # Maybe<Float> (21.0)
>>= (value -> value / 5) # Maybe<Float> (4.2)
```

## `=<<`

```aml
(=<<): ('a -> 'm<'b>) -> 'm<'a> -> 'm<'b>
```

An infix alias for `then`, but with the arguments reversed

```aml
(value -> value / 2) =<< (Maybe.from 42) # Maybe<Float> (21)
```

## `chain`

```aml
chain: ('a -> 'm<'b>) -> ('b -> 'm<'c>) -> 'a -> 'm<'c>
```

Composes two functions by chaining.

```aml
{ decodeAsync } = use "System/Text/JSON"
{ GET } = use "System/Web/Http"

fetch-json
  : URL -> Async<Result<JsonValue, _>>
  = chain GET (response -> decodeAsync response.Body)
```

## `>=>`

```aml
(>=>): ('a -> 'm<'b>) -> ('b -> 'm<'c>) -> 'a -> 'm<'c>
```

An infix alias for `chain`

```aml
{ decodeAsync } = use "System/Text/JSON"
{ GET } = use "System/Web/Http"

fetch-json = GET >=> (response -> decodeAsync response.Body)
```

## `<=<`

```aml
(<=<): ('b -> 'm<'c>) -> ('a -> 'm<'b>) -> 'a -> 'm<'c>
```

An infix alias for `chain`, but with the arguments reversed

```aml
{ decodeAsync } = use "System/Text/JSON"
{ GET } = use "System/Web/Http"

fetch-json = (response -> decodeAsync response.Body) <=< GET
```

## `flatten`

```aml
flatten: 'm<'m<'a>> -> 'm<'a>
```

Flattens two levels of wrapping into a single level

```aml
nested-value = Some (Some 5) // Maybe<Maybe<Integer>>
optional-value = flatten nested-value // Maybe<Integer>
```
