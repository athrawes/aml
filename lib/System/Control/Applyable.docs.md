# Applyable

## `apply`

```aml
apply : ^A[^T -> ^U] -> ^A[^T] -> ^A[^U]
```

Applies functions within an effect to values within the same effect.

```aml
let function-list = List.from [(add 2), (add 10)]
let values-list = List.from [1, 2, 3]

List.apply function-list values-list # List[Integer] (3, 4, 5, 11, 12, 13)
```

## `<*>`

```aml
<*> : infix ^A[^T -> ^U] -> ^A[^T] -> ^A[^U]
```

An infix alias of `apply`.

## `<**>`

```aml
<**> : infix ^A[^T] -> ^A[^T -> ^U] -> ^A[^U]
```

An infix alias of `apply`, but with the arguments reversed.
