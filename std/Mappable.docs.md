# Mappable

Defines things that can be 'mapped' over; that is, things which, when given
a function from type ^A to type ^B, can use that function to transform the
values within in a well-defined way.

## `map`

```aml
map :: ^F[^A] -> (^A -> ^B) -> ^F[^B]
```

A function to apply a given mapping function to the current effect.

```aml
List.from [1, 2, 3]
|> map (value -> value * 2) # List[Integer] (2, 4, 6)
|> map (value -> value / 2) # List[Float] (1.0, 2.0, 3.0)
```

## `<$>`

An infix alias for `then`

```aml
<$> :: ^F[^A] -> (^A -> ^B) -> ^F[^B]
```

```aml
(List.from [1, 2, 3]) <$> (value -> value 2) # List[Integer] (2, 4, 6)
```

## `<&>`

An infix alias for `then`, but with the arguments for this function reversed.
This makes pipelines easier to write.

```aml
<&> :: (^A -> ^B) -> ^F[^A] -> ^F[^B]
```

```aml
(List.from [1, 2, 3])
<&> (value -> value * 2) # List[Integer] (2, 4, 6)
<&> (value -> value / 2) # List[Float] (1.0, 2.0, 3.0)
<&> Integer.fromFloat # List[Integer] (1, 2, 3)
```
