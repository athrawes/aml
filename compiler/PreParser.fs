module PreParser
open Lexer

let transform (tokens: (Token * TokenPosition) seq): (Token * TokenPosition) seq =
    let rec transform' (tokens: (Token * TokenPosition) seq) = seq {
        match tokens |> Seq.tryHead with
        | None -> ()
        | Some (token, position) ->
            match token with
            | EOL | Indent _ ->
                yield! collapseWhitespace tokens
            | _ ->
                yield (token, position)
                yield! transform' (Seq.tail tokens)
    }
    and peek (tokens: (Token * TokenPosition) seq) (n: int) =
        match Seq.tryItem n tokens with
        | Some (token, _) -> Some token
        | None -> None
    and collapseWhitespace (tokens: (Token * TokenPosition) seq) = seq {
        let (token) = tokens |> Seq.tryHead
        let nextToken = tokens |> Seq.tryItem 1

        match (token, nextToken) with
        | (Some (EOL, _), Some (EOL, _)) -> ()
        | (Some (EOL, _), Some (Indent _, _)) when peek tokens 1 = Some EOL -> ()
        | (Some (EOL, position), _) -> yield (EOL, position)
        | (Some (Indent _, _), Some (EOL, _)) -> ()
        | (Some (Indent n, position), _) -> yield (Indent n, position)
        | (Some (DeIndent _, _), Some (EOL, _)) -> ()
        | (Some (DeIndent _, _), Some (Indent _, _)) -> ()
        | (Some (DeIndent _, _), Some (DeIndent _, _)) -> ()
        | (Some (DeIndent n, position), _) -> yield (DeIndent n, position)
        | (Some (t, position), _) -> yield (t, position)
        | (None, _) -> ()

        yield! transform' (Seq.tail tokens)
    }

    transform' tokens
