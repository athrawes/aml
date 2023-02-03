# Applyable

## `apply`

```aml
apply: M<A -> B> -> M<A> -> M<B>
```

Applies functions within an effect to values within the same effect.

```aml
function-list = List.from [(add 2), (add 10)]
values-list = List.from [1, 2, 3]

apply function-list values-list # List<Integer> (3, 4, 5, 11, 12, 13)
```

## `<*>`

```aml
<*> : infix M<A -> B> -> M<A> -> M<B>
```

An infix alias of `apply`.

## `<**>`

```aml
<**> : infix M<A> -> M<A -> B> -> M<B>
```

An infix alias of `apply`, but with the arguments reversed.
