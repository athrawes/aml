open Lexer
open PreParser
open Parser
open System

[<EntryPoint>]
let main argv =
    let lines = IO.File.ReadLines argv[0]

    let tokenStream = tokenize (String.concat "\n" lines)

    // for token in tokenStream do
    //     printfn "%A" token

    let tokens = transform tokenStream

    // for token in tokens do
    //     printfn "%A" token

    let ast = parse tokens

    for definition in ast do
        printfn "%A" definition

    0
