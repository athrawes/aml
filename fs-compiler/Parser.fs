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

let rec parse (tokens: (Token * TokenPosition) seq) : Definition list =
  let rec parseDefinition (tokens: (Token * TokenPosition) seq) : Definition * (Token * TokenPosition) seq =
    let name = tokens |> Seq.head |> fst
    let (typeDef, tokens) = tokens |> Seq.skip 1 |> Seq.toList |> parseExpression
    let (value, tokens) = tokens |> Seq.toList |> parseExpression

    ({ name = name; typeDef = typeDef :: []; value = value :: [] }, tokens)

  and parseExpression (tokens: (Token * TokenPosition) list) : Expression * (Token * TokenPosition) seq =
    match tokens with
    | [] -> (Value (EOF), [])
    | [tkn] -> (Value (fst tkn), [])
    | tkn :: tail ->
      match fst tkn with
      | GroupOpen ->
        let (expression, tokens) = parseExpression tail
        (SubExpression (expression :: []), tokens)
      | GroupClose -> (Value (fst tkn), Seq.ofList tail)
      | _ -> (Value (fst tkn), Seq.ofList tail)

  match Seq.isEmpty tokens with
  | true -> []
  | _ ->
    let (definition, tokens) = tokens |> parseDefinition
    definition :: parse tokens
