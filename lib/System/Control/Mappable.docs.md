# Mappable

Defines things that can be 'mapped' over; that is, things which, when given
a function from type ^A to type ^B, can use that function to transform the
values within in a well-defined way.

## `map`

```aml
map : ^F[^A] -> (^A -> ^B) -> ^F[^B]
```

A function to apply a given mapping function to the current effect.

```aml
List.from [1, 2, 3]
|> map (multiply 2) # List[Integer] (2, 4, 6)
|> map (multiply 2) # List[Integer] (4, 6, 8)
```

## `fmap`

An alias for `map`

```aml
fmap : ^F[^A] -> (^A -> ^B) -> ^F[^B]
```

## `<$>`

An infix alias for `map`

```aml
<$> : ^F[^A] -> (^A -> ^B) -> ^F[^B]
```

```aml
List.from [1, 2, 3]
<$> (multiply 2) # List[Integer] (2, 4, 6)
<$> (multiply 2) # List[Integer] (4, 6, 8)
```

## `<&>`

An infix alias for `map`, but with the arguments for this function reversed.

```aml
<&> : (^A -> ^B) -> ^F[^A] -> ^F[^B]
```

```aml
(multiply 2) <&> (List.from [1, 2, 3])
```
