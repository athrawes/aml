module PreParser
open Lexer

let transform (tokens: (Token * TokenPosition) seq): Token seq =
    let mutable currentIndent = 0
    let rec transform' (tokens: (Token * TokenPosition) seq) = seq {
        match tokens |> Seq.tryHead with
        | Some (token, position) ->
            match token with
            | EOL | Indent _ ->
                yield! collapseWhitespace (tokens |> Seq.skip 1)
            | _ ->
                yield token
                yield! transform' (tokens |> Seq.skip 1)
        | None -> ()
    }
    and peek (tokens: (Token * TokenPosition) seq) (n: int) =
        match Seq.tryItem n tokens with
        | Some (token, _) -> Some token
        | None -> None
    and collapseWhitespace (tokens: (Token * TokenPosition) seq) = seq {
        yield! Seq.map fst tokens
        // let mutable lastToken = None
        // for token, position in tokens do
        //     match (lastToken, token) with
        //     | (Some (EOL), EOL) -> ()
        //     | (Some (EOL), Indent _) when peek tokens 1 = Some EOL -> ()
        //     | (Some (EOL), _) -> yield token
        //     | (Some (Indent _), EOL) -> ()
        //     | (Some (Indent _), _) -> yield token
        //     | (Some (DeIndent _), EOL) -> ()
        //     | (Some (DeIndent _), Indent _) -> ()
        //     | (Some (DeIndent _), DeIndent _) -> ()
        //     | (Some (DeIndent _), _) -> yield token
        //     | (Some (_), _) -> yield token
        //     | (None, _) -> yield token
        //     lastToken <- Some (token)
    }

    transform' tokens
