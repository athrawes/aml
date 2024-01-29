open Lexer
open PreParser
open Parser
open System

[<EntryPoint>]
let main argv =
    let lines = IO.File.ReadLines argv[0]
    let tokenStream = tokenize (String.concat "\n" lines)
    let tokens = transform tokenStream
    for token in tokens do
        printfn "%A" token
    0
