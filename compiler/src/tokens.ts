type FilePosition = {
    lineStart: number,
    lineEnd: number,
    startColumn: number,
    endColumn: number
}

type VariableBinding = FilePosition & { value: string, kind: 'binding' };
type TypeBinding = FilePosition & { value: string, kind: 'type' };
type Function = FilePosition & { value: string, kind: 'function' };
type String = FilePosition & { value: string, kind: 'string' };
type Whitespace = FilePosition & { value: string, kind: 'whitespace' };
type Identifier = FilePosition & { value: string, kind: 'identifier' };
type NewLine = FilePosition & { value: string, kind: 'newline' };
type Indent = FilePosition & { value: string, kind: 'indent' };
type Infer = FilePosition & { value: string, kind: 'infer' };
type Comment = FilePosition & { value: string, kind: 'comment' };
type RecordOpen = FilePosition & { value: string, kind: 'recordOpen' };
type RecordClose = FilePosition & { value: string, kind: 'recordClose' };
type GenericOpen = FilePosition & { value: string, kind: 'genericOpen' };
type GenericClose = FilePosition & { value: string, kind: 'genericClose' };
type GroupOpen = FilePosition & { value: string, kind: 'groupOpen' };
type GroupClose = FilePosition & { value: string, kind: 'groupClose' };
type TupleOpen = FilePosition & { value: string, kind: 'tupleOpen' };
type TupleClose = FilePosition & { value: string, kind: 'tupleClose' };
type ValueSeparator = FilePosition & { value: string, kind: 'valueSeparator' };
type CaseArm = FilePosition & { value: string, kind: 'caseArm' };
type Guard = FilePosition & { value: string, kind: 'guard' };
type Decorator = FilePosition & { value: string, kind: 'decorator' };
type ExpressionSeparator = FilePosition & { value: string, kind: 'expressionSeparator' };

export type Token =
    | VariableBinding
    | TypeBinding
    | Function
    | String
    | Whitespace
    | Identifier
    | NewLine
    | Indent
    | Comment
    | Infer
    | RecordClose
    | RecordOpen
    | GenericClose
    | GenericOpen
    | GroupClose
    | GroupOpen
    | TupleClose
    | TupleOpen
    | ValueSeparator
    | CaseArm
    | Guard
    | Decorator
    | ExpressionSeparator
