open System
open System.IO
open System.Threading
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Writers

[<EntryPoint>]
let main argv = 
/// mock for https://query1.finance.yahoo.com/v7/finance/download/MSFT?period1=1514635129&period2=1517313529&interval=1d&events=history&crumb=z7O4jAW0mv0
/// MSFT.csv and INTC.csv

  let app : WebPart =
      choose [
        GET >=> path "/" >=> Files.file "index.html"
        GET >=> Files.browseHome
        RequestErrors.NOT_FOUND "Page not found." 
      ]
  let mimeTypes =
    defaultMimeTypesMap
        @@ (function | ".csv" -> createMimeType " text/csv" false | _ -> None)
  let local = Suave.Http.HttpBinding.createSimple HTTP "127.0.0.1" 8088
  let config =
    { defaultConfig with homeFolder = Some (Path.GetFullPath "../../") ; mimeTypesMap = mimeTypes; bindings = [local] }
  startWebServer config app
 
  printfn "Make requests now"
  Console.ReadKey true |> ignore
    
  0 // return an integer exit code
