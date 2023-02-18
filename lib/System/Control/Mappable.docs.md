# Mappable

Defines things that can be 'mapped' over; that is, things which, when given
a function from type 'a to type 'b, can use that function to transform the
values within in a well-defined way.

## `map`

```aml
map: 'm<'a> -> ('a -> 'b) -> 'm<'b>
```

A function to apply a given mapping function to the current effect.

```aml
List.from [1, 2, 3]
|> map (multiply 2) # List<Integer> (2, 4, 6)
|> map (multiply 2) # List<Integer> (4, 6, 8)
```

## `<$>`

An infix alias for `map`

```aml
<$> : 'm<'a> -> ('a -> 'b) -> 'm<'b>
```

```aml
List.from [1, 2, 3]
<$> (multiply 2) # List<Integer> (2, 4, 6)
<$> (multiply 2) # List<Integer> (4, 6, 8)
```

## `<&>`

An infix alias for `map`, but with the arguments for this function reversed.

```aml
<&> : ('a -> 'b) -> 'm<'a> -> 'm<'b>
```

```aml
(multiply 2) <&> (List.from [1, 2, 3])
```
