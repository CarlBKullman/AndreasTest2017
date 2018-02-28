open System
open System.IO

open Newtonsoft.Json
open Newtonsoft.Json.Serialization


open Suave
open Suave.Filters
open Suave.Operators
open Suave.Writers
open Suave.Successful
open Suave.RequestErrors
open Suave.Json
open System.Runtime.Serialization

open Suave.Http
open Suave.Web

open Suave.Operators
open Suave.Filters
open Suave.Json

open Suave.Writers
open Crud

[<DataContract>]
type Stock = DbConnection.DbSchema.ServiceTypes.Stock

[<DataContract>]
type ResultStock = //DbConnection.DbSchema.ServiceTypes.Stock
   { 
      [<field: DataMember(Name = "resultStock")>]
      resultStock  : DbConnection.DbSchema.ServiceTypes.Stock;
   }

[<DataContract>]
type ResultCompany =
   { 
      [<field: DataMember(Name = "result")>]
      result : DbConnection.DbSchema.ServiceTypes.Stock;
   }

[<EntryPoint>]
let main argv = 
//web server config
    let local = Suave.Http.HttpBinding.createSimple HTTP "127.0.0.1" 8080
    let config =
        { defaultConfig with bindings = [local] }


//Json tansformer
    let jsonSerializerSettings = new JsonSerializerSettings()

    let JSON v =
        match v with
        | null -> NOT_FOUND  "Not found"
        | _ ->
            jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
            JsonConvert.SerializeObject(v, jsonSerializerSettings) 
            |> OK
            >=> Writers.setMimeType "application/json; charset=utf-8"

    //build web  INTC
    let app : WebPart =
       choose
            [                
              GET >=> //http://127.0.0.1:8080/stock
                path "/stock" >=> JSON (Crud.getAllStocks) 
              GET >=> //http://127.0.0.1:8080/stock/MSFT
                pathScan "/stock/%s" (fun (stockId) -> JSON (Crud.getStock stockId)) 
              DELETE >=> //http://127.0.0.1:8080/stock/MSFT
                pathScan "/stock/%s" (fun (stockId) -> Crud.deleteStock (Crud.getStock stockId); OK "") 
              POST >=> 
                path "/stock" >=> (mapJson (fun (stock:Stock) -> { resultStock = (Crud.getStock stock.StockID) }))
              //TODO PUT >=> 
              //  pathScan "/stock/%s" (fun (stockId) -> 
              //      mapJson (fun (stock:Stock) -> { resultStock = (Crud.getStock stock.StockID) })
              //      OK ""
              //  )
              GET >=> //http://127.0.0.1:8080/history/MSFT 
                pathScan "/history/%s" (fun (stockId) -> JSON (Crud.getHistories stockId)) 

            ] >=> Writers.setMimeType "application/json; charset=utf-8"


    startWebServer defaultConfig (mapJson (fun (stock:Stock) -> { resultStock = (Crud.getStock stock.StockID )}))


    printfn "Make requests now"
    Console.ReadKey true |> ignore
    
    0 // return an integer exit code

