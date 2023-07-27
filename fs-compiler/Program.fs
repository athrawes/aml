open Lexar
open System

[<EntryPoint>]
let main argv =
    let lines = IO.File.ReadLines argv[0]
    let tokens = tokenize (String.concat "\n" lines)
    for token in tokens do
        printfn "%A" token
    0
