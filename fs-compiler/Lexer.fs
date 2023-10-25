module Lexer

type Position = { line: int; column: int; index: int }

type Lexer = { source: string; position: Position }

type TokenPosition = { start: Position; finish: Position }

type Token =
    // Keywords
    | KeywordAlias // as
    | KeywordGuard // when
    | KeywordMatch // match
    | KeywordModule // module
    | KeywordOf // of
    | KeywordType // type
    | KeywordWith // with
    // Operators
    | Arm // => (arm separator)
    | Binding // =
    | Decorator // @
    | FieldSeparator // ,
    | Function // ->
    | MacroDecorator // @@
    | Pipe // |
    | Splat // ...
    | StructMemberAccessor // .
    | TypeBinding // :
    | TypeExtension // extends
    | Unit // _
    | EOL // \n, \r, \r\n
    // Paired operators
    | GroupOpen // (
    | GroupClose // )
    | StructOpen // {
    | StructClose // }
    | GenericOpen // <
    | GenericClose // >
    | TupleOpen // [
    | TupleClose // ]
    // Literals
    | Comment of string // # ...
    | Identifier of string
    | String of string // "..."
    | TypeParameter of string // '...
    | Indent of int // leading whitespace

let rec readToken lexer : Lexer * Token * TokenPosition =
    let emit = emitSimple lexer

    match peek lexer 0 with
    | None -> failwith "Unexpected end of input"
    | Some c ->
        match c with
        | ' '
        | '\t' ->
            if lexer.position.column = 1 then
                let indent = countIndent lexer
                emit (Indent indent) indent
            else
                let newLexer = advance lexer 1
                readToken newLexer
        | '\n' ->
            (advance lexer 1,
             EOL,
             { start = lexer.position
               finish = lexer.position })
        | '|' -> emit Pipe 1
        | ',' -> emit FieldSeparator 1
        | '_' -> emit Unit 1
        | '(' -> emit GroupOpen 1
        | ')' -> emit GroupClose 1
        | '{' -> emit StructOpen 1
        | '}' -> emit StructClose 1
        | '[' -> emit TupleOpen 1
        | ']' -> emit TupleClose 1
        | '<' -> emit GenericOpen 1
        | '>' -> emit GenericClose 1
        | '=' when peek lexer 1 = Some '>' -> emit Arm 2
        | '=' -> emit Binding 1
        | '-' when peek lexer 1 = Some '>' -> emit Function 2
        | ':' -> emit TypeBinding 1
        | '"' ->
            let (str, position) = readString lexer
            let next = advance lexer (position.finish.column - lexer.position.column + 1)

            (next, String str, position)
        | '#' ->
            let (comment, position) = readComment lexer
            let next = advance lexer (position.finish.column - lexer.position.column + 1)

            (next, Comment comment, position)
        | '\'' ->
            let (identifier, position) = readIdentifier (advance lexer 1)

            // fix starting position to sort off-by-one error
            let position =
                { start = { position.start with column = position.start.column - 1 }
                  finish = position.finish }

            let next = advance lexer (position.finish.column - lexer.position.column + 1)

            (next, TypeParameter identifier, position)
        | _ ->
            let (identifier, position) = readIdentifier lexer
            let next = advance lexer (position.finish.column - lexer.position.column + 1)

            let token =
                match identifier with
                | "as" -> KeywordAlias
                | "when" -> KeywordGuard
                | "match" -> KeywordMatch
                | "module" -> KeywordModule
                | "of" -> KeywordOf
                | "type" -> KeywordType
                | "extends" -> TypeExtension
                | "with" -> KeywordWith
                | _ -> Identifier identifier

            (next, token, position)

and emitSimple lexer token consumed =
    let next = advance lexer consumed

    let finish =
        { line = next.position.line
          index = next.position.index - 1
          column = next.position.column - 1 }

    let tokenPosition =
        { start = lexer.position
          finish = finish }

    (next, token, tokenPosition)

and countIndent lexer =
    let mutable next = lexer
    let mutable indent = 0

    while peek next 0 = Some '\t' || peek next 0 = Some ' ' do
        next <- advance next 1
        indent <- indent + 1

    indent

and readString lexer =
    let str =
        Seq.unfold
            (function
            | idx when (peek lexer idx) <> Some '"' ->
                let c = peek lexer idx
                Some(c, idx + 1)
            | _ -> None)
            1
        |> Seq.choose id
        |> Seq.map (fun c -> c.ToString())
        |> String.concat ""

    (str,
     { start = lexer.position
       finish = (advance lexer (str.Length + 1)).position })

and readIdentifier lexer =
    let mutable next = lexer

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
     { start = lexer.position
       finish = nextPosition })

and readComment lexer =
    let lexer = advance lexer 1 // Skip the '#'

    let chunk =
        Seq.unfold
            (function
            | idx when (peek lexer idx) <> Some '\n' ->
                let c = peek lexer idx
                Some(c, idx + 1)
            | _ -> None)
            0
        |> Seq.choose id
        |> Seq.map (fun c -> c.ToString())
        |> String.concat ""

    (chunk,
     { start = lexer.position
       finish =
         { line = lexer.position.line
           index = lexer.position.index + chunk.Length
           column = lexer.position.column + chunk.Length } })

and advance lexer n : Lexer =
    let stringPosition = lexer.position.index - 1
    let chunk = lexer.source.[stringPosition .. stringPosition + n - 1]
    let newlines = Seq.filter (fun c -> c = '\n') chunk |> Seq.length

    let newPosition =
        { line = lexer.position.line + newlines
          index = lexer.position.index + n
          column =
            if newlines > 0 then
                chunk.Length - chunk.LastIndexOf('\n')
            else
                lexer.position.column + n }

    { source = lexer.source
      position = newPosition }

and peek lexer n =
    let stringPosition = lexer.position.index - 1

    if stringPosition + n < lexer.source.Length then
        Some lexer.source.[stringPosition + n]
    else
        None

and isIdentifierCharacter =
    function
    | c when System.Char.IsWhiteSpace c -> false
    | c when c = '(' || c = ')' -> false // Grouping operators
    | c when c = '{' || c = '}' -> false // Record operators
    | c when c = '<' || c = '>' -> false // Generic operators
    | c when c = '[' || c = ']' -> false // Tuple operators
    | c when c = '"' -> false // String literal
    | c when c = '#' -> false // Comment
    | c when c = '=' -> false // Binding
    | c when c = '|' -> false // Pipe
    | c when c = ',' -> false // Field separator
    | _ -> true

let tokenize (input: string) : (Token * TokenPosition) seq =
    Seq.unfold
        (fun (lexer) ->
            if lexer.position.index > lexer.source.Length then
                None
            else
                let (newLexer, token, position) = readToken lexer
                Some((token, position), newLexer))
        { source = input
          position = { line = 1; index = 1; column = 1 } }
