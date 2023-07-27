module Lexar

type Position = { line: int; column: int; index: int }

type Lexar = { source: string; position: Position }

type TokenPosition = { start: Position; finish: Position }

type Token =
    // Keywords
    | KeywordAlias // as
    | KeywordGuard // when
    | KeywordMatch // match
    | KeywordModule // module
    | KeywordOf // of
    | KeywordType // type
    // Operators
    | Arm // => (arm separator)
    | Binding // =
    | Decorator // @
    | ExpressionSeparator of int // \n\n
    | FieldSeparator // ,
    | Function // ->
    | MacroDecorator // @@
    | Pipe // |
    | Splat // ...
    | StructMemberAccessor // .
    | TypeBinding // :
    | TypeExtension // :>
    | Unit // _
    | EOL of int
    // Paired operators
    | GroupOpen // (
    | GroupClose // )
    | StructOpen // {
    | StructClose // }
    | GenericOpen // <
    | GenericClose // >
    // Literals
    | Comment of string // # ...
    | Identifier of string
    | String of string // "..."
    | TypeParameter of string // '...

let rec readToken lexar : Lexar * Token * TokenPosition =
    let emit = emitSimple lexar

    match peek lexar 0 with
    | None -> failwith "Unexpected end of input"
    | Some c ->
        match c with
        | ' '
        | '\t' ->
            let newLexar = advance lexar 1
            readToken newLexar
        | '\n' when peek lexar 1 = Some '\n' ->
            let next = advance lexar 2
            let indent = countIndent next

            (next,
             ExpressionSeparator indent,
             { start = lexar.position
               finish =
                 { line = lexar.position.line + 1
                   index = next.position.index - 1
                   column = 1 } })
        | '\n' ->
            let next = advance lexar 1
            let indent = countIndent next

            (next,
             EOL indent,
             { start = lexar.position
               finish = lexar.position })
        | '|' -> emit Pipe 1
        | ',' -> emit FieldSeparator 1
        | '_' -> emit Unit 1
        | '(' -> emit GroupOpen 1
        | ')' -> emit GroupClose 1
        | '{' -> emit StructOpen 1
        | '}' -> emit StructClose 1
        | '<' -> emit GenericOpen 1
        | '>' -> emit GenericClose 1
        | '=' when peek lexar 1 = Some '>' -> emit Arm 2
        | '=' -> emit Binding 1
        | '-' when peek lexar 1 = Some '>' -> emit Function 2
        | ':' when peek lexar 1 = Some '>' -> emit TypeExtension 2
        | ':' -> emit TypeBinding 1
        | '"' ->
            let (str, position) = readString lexar
            let next = advance lexar (position.finish.column - lexar.position.column + 1)

            (next, String str, position)
        | '#' ->
            let (comment, position) = readComment lexar
            let next = advance lexar (position.finish.column - lexar.position.column + 1)

            (next, Comment comment, position)
        | '\'' ->
            let (identifier, position) = readIdentifier (advance lexar 1)

            // fix starting position to sort off-by-one error
            let position =
                { start = { position.start with column = position.start.column - 1 }
                  finish = position.finish }

            let next = advance lexar (position.finish.column - lexar.position.column + 1)

            (next, TypeParameter identifier, position)
        | _ ->
            let (identifier, position) = readIdentifier lexar
            let next = advance lexar (position.finish.column - lexar.position.column + 1)

            let token =
                match identifier with
                | "as" -> KeywordAlias
                | "when" -> KeywordGuard
                | "match" -> KeywordMatch
                | "module" -> KeywordModule
                | "of" -> KeywordOf
                | "type" -> KeywordType
                | _ -> Identifier identifier

            (next, token, position)

and emitSimple lexar token consumed =
    let next = advance lexar consumed

    let finish =
        { line = next.position.line
          index = next.position.index - 1
          column = next.position.column - 1 }

    let tokenPosition =
        { start = lexar.position
          finish = finish }

    (next, token, tokenPosition)

and countIndent lexar =
    let mutable next = lexar
    let mutable indent = 0

    while peek next 0 = Some '\t' || peek next 0 = Some ' ' do
        next <- advance next 1
        indent <- indent + 1

    indent

and readString lexar =
    let str =
        Seq.unfold
            (function
            | idx when (peek lexar idx) <> Some '"' ->
                let c = peek lexar idx
                Some(c, idx + 1)
            | _ -> None)
            1
        |> Seq.choose id
        |> Seq.map (fun c -> c.ToString())
        |> String.concat ""

    (str,
     { start = lexar.position
       finish = (advance lexar (str.Length + 1)).position })

and readIdentifier lexar =
    let mutable next = lexar

    let identifier =
        seq {
            while peek next 0
                  |> Option.defaultValue ' '
                  |> isIdentifierCharacter do
                yield peek next 0
                next <- advance next 1

        }
        |> Seq.choose id
        |> Seq.map (fun c -> c.ToString())
        |> String.concat ""

    let nextPosition =
        { line = next.position.line
          index = next.position.index - 1
          column = next.position.column - 1 }

    (identifier,
     { start = lexar.position
       finish = nextPosition })

and readComment lexar =
    let lexar = advance lexar 1 // Skip the '#'

    let chunk =
        Seq.unfold
            (function
            | idx when (peek lexar idx) <> Some '\n' ->
                let c = peek lexar idx
                Some(c, idx + 1)
            | _ -> None)
            0
        |> Seq.choose id
        |> Seq.map (fun c -> c.ToString())
        |> String.concat ""

    (chunk,
     { start = lexar.position
       finish =
         { line = lexar.position.line
           index = lexar.position.index + chunk.Length
           column = lexar.position.column + chunk.Length } })

and advance lexar n : Lexar =
    let stringPosition = lexar.position.index - 1
    let chunk = lexar.source.[stringPosition .. stringPosition + n - 1]
    let newlines = Seq.filter (fun c -> c = '\n') chunk |> Seq.length

    let newPosition =
        { line = lexar.position.line + newlines
          index = lexar.position.index + n
          column =
            if newlines > 0 then
                chunk.Length - chunk.LastIndexOf('\n')
            else
                lexar.position.column + n }

    { source = lexar.source
      position = newPosition }

and peek lexar n =
    let stringPosition = lexar.position.index - 1

    if stringPosition + n < lexar.source.Length then
        Some lexar.source.[stringPosition + n]
    else
        None

and isOperatorCharacter =
    function
    | '='
    | '>'
    | '<'
    | ')'
    | '('
    | '{'
    | '}'
    | '['
    | ']'
    | ','
    | ':'
    | '|'
    | '\'' -> true
    | _ -> false

and isIdentifierCharacter =
    function
    | c when System.Char.IsWhiteSpace c -> false
    | c when isOperatorCharacter c -> false
    | _ -> true

let tokenize (input: string) : (Token * TokenPosition) seq =
    Seq.unfold
        (fun (lexar) ->
            if lexar.position.index > lexar.source.Length then
                None
            else
                let (newLexar, token, position) = readToken lexar
                Some((token, position), newLexar))
        { source = input
          position = { line = 1; index = 1; column = 1 } }
