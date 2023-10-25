module Parser

open Lexer

type Expression =
    | Definition of Definition
    | MatchExpression of MatchExpression
    | FunctionCall of FunctionCall
    | SubExpression of Expression list
    | Value of Token

and MatchExpression =
    { value: Expression
      cases: (Expression * Expression list) list }

and FunctionCall = { name: Token; argument: Expression }

and Definition =
    { name: Token
      typeDef: Expression list
      value: Expression list }

type AST = { definitions: Definition list }

let rec parse (tokens: (Token * TokenPosition) seq) : AST = { definitions = [] }
