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

    and CalledFunction =
        | NamedFunctionCall of Identifier * Identifier
        | AnonymousFunctionCall of GroupOpen * Function * GroupClose * Identifier

    and SubExpression =
        | Identifier of Identifier
        | Primitive of Primitive
        | Function of Function

    and Expression =
        | SimpleEnclosedExpression of GroupOpen * SubExpression * GroupClose
        | NestedEnclosedExpression of GroupOpen * Expression * GroupClose
        | LineExpression of Expression * EndOfLine
        | CalledFunction of CalledFunction

    and Statement =
        | LetDeclaration of LetDeclaration
        | CalledFunction of CalledFunction

    and LetDeclaration =
        VariableDeclaration * Identifier * (TypeBind * InfixKeyword option * TypeSignature) option * SymbolBind * Expression

    and Primitive =
        | Number of Number
        | String of String
        | Boolean of Boolean
        | Structure of Structure

    and Structure =
        StructOpen * (Identifier * StructEntryNameValueSeperator * Expression * FieldSeparator) list option * (SplatOperator * FieldSeparator) option * StructClose
