namespace AML

module AST =
    type Identifier = string
    type InfixKeyword = string
    type VariableDeclaration = string
    type TypeBind = string
    type SymbolBind = string
    type TypeParameterOpener = string
    type FunctionOperator = string
    type Number = string
    type String = string
    type Boolean = string
    type StructOpen = string
    type StructClose = string
    type SplatOperator = string
    type StructEntryNameValueSeperator = string
    type GroupOpen = string
    type GroupClose = string
    type EndOfLine = string
    type FieldSeparator =
        | Comma of string
        | EndOfLine of EndOfLine

    type TypeParameter = TypeParameterOpener * Identifier

    type TypeSignature =
        | Concrete of Identifier * (FunctionOperator * TypeSignature) option
        | Generic of TypeParameter * Identifier * (FunctionOperator * TypeSignature) option

    type Function = Identifier * FunctionOperator * Expression

    and Expression =
        | AnonymousFunctionCall of GroupOpen * Function * GroupClose * Expression
        | Boolean of Boolean
        | Function of Function
        | Identifier of Identifier
        | LineExpression of Expression * EndOfLine
        | NamedFunctionCall of Identifier * Expression
        | NestedEnclosedExpression of GroupOpen * Expression * GroupClose
        | Number of Number
        | String of String
        | Structure of StructOpen * (Identifier * StructEntryNameValueSeperator * Expression * FieldSeparator) list option * (SplatOperator * FieldSeparator) option * StructClose

    and LetDeclaration =
        VariableDeclaration * Identifier * (TypeBind * InfixKeyword option * TypeSignature) option * SymbolBind * Expression
