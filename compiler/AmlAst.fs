namespace AML

module AST =
    type Identifier = string
    type InfixOperator = string
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
    type FieldSeparator = string
    type StructEntryNameValueSeperator = string
    type GroupOpen = string
    type GroupClose = string
    type EndOfLine = string

    type keyword =
        | VariableDeclaration
        | TypeDeclaration
        | StructFieldAliasOperator

    type VariableIdentifier =
        | Identifier
        | InfixIdentifier of InfixOperator * Identifier * InfixOperator

    type TypeParameter = TypeParameterOpener * Identifier

    type TypeSignature =
        | Concrete of Identifier * (FunctionOperator * TypeSignature) option
        | Generic of TypeParameter * Identifier * (FunctionOperator * TypeSignature) option

    type Function = Identifier * FunctionOperator * Expression
    and SubExpression =
        | Identifier
        | Primitive
        | Function
    and Expression =
        | SimpleEnclosedExpression of GroupOpen * SubExpression * GroupClose
        | NestedEnclosedExpression of GroupOpen * Expression * GroupClose
        | LineExpression of Expression * EndOfLine
    and Structure =
        StructOpen *
        ( SplatOperator * FieldSeparator option ) option *
        ( Identifier * StructEntryNameValueSeperator * Expression * FieldSeparator option  ) list option *
        StructClose
    and Primitive =
        | Number
        | String
        | Boolean
        | Structure

    type letDeclaration =
        VariableDeclaration *
        VariableIdentifier *
        (TypeBind * TypeSignature) option *
        SymbolBind *
        Expression
