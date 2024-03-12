module Parser

open Lexer

type Program = Definition list

and Expression =
    | Definition of Definition
    | MatchExpression of MatchExpression
    | FunctionCall of FunctionCall
    | SubExpression of Expression
    | Value of Token

and MatchExpression =
    { t: string
      value: Expression
      cases: (Expression * Expression) list }

and FunctionCall = { t: string; name: Token; argument: Expression }

and Definition =
    { t: string
      name: Token
      typeDef: Expression
      value: Expression }

let rec parse (tokens: (Token * TokenPosition) seq) : Program =
  []
