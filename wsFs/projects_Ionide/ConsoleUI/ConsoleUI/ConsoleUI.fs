module ConsoleUI

[<EntryPoint>]
let main argv =
    printfn "Hello World!"
    System.Console.ReadKey() |> ignore
    0 // return an integer exit code
