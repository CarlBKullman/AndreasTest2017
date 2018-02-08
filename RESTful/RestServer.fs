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
open Suave.Utils.Collections

open System.ServiceModel 
open System.Runtime.Serialization


open System.Collections.Generic
open Suave.Utils
open Suave.Utils.Collections
open Suave.Http
open Suave.Web
//open System
//open System.Net.Http
//open System.Text

open Suave.Operators
open Suave.Filters
open Suave.Json
//open Suave.Http.warbler
//open Suave.Types.context 
//open Suave.Types.request

open Suave.Writers
open Crud
//{"stockID":"MSFT","name":"Microsoft Corporation"}
[<DataContract>]
type Stock = AndreasCommon.DbSchema.ServiceTypes.Stock
   //{ 
   //   [<field: DataMember(Name = "stockID")>]
   //   stockID : string;
   //   [<field: DataMember(Name = "name")>]
   //   name : string;
   //}

[<DataContract>]
type ResultStock = //AndreasCommon.DbSchema.ServiceTypes.Stock
   { 
      [<field: DataMember(Name = "resultStock")>]
      resultStock  : AndreasCommon.DbSchema.ServiceTypes.Stock;
   }

[<DataContract>]
type ResultCompany =
   { 
      [<field: DataMember(Name = "result")>]
      result : AndreasCommon.DbSchema.ServiceTypes.Stock;
   }

[<DataContract>]
type Calc =
   { 
      [<field: DataMember(Name = "stockID")>]
      stockID : string;
      [<field: DataMember(Name = "name")>]
      name : string;
   }
 
[<DataContract>]
type Result =
   { 
      [<field: DataMember(Name = "result")>]
      result : int;
   }

[<AutoOpen>]
type RestResource<'a> = {
             GetAll : unit -> 'a seq
   }


[<EntryPoint>]
let main argv = 
//web server config
    let local = Suave.Http.HttpBinding.createSimple HTTP "127.0.0.1" 8080
    let config =
        { defaultConfig with bindings = [local] }


//Json tansformer
    let jsonSerializerSettings = new JsonSerializerSettings()

    //let JSON_ v =
    //    jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
    //    JsonConvert.SerializeObject(v, jsonSerializerSettings) 
    //    |> OK
    //    >=> Writers.setMimeType "application/json; charset=utf-8"

    let JSON v =
        match v with
        | null -> NOT_FOUND  "Not found"
        | _ ->
            jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
            JsonConvert.SerializeObject(v, jsonSerializerSettings) 
            |> OK
            >=> Writers.setMimeType "application/json; charset=utf-8"
    (* *)
    
    //let getResourceFromReq<'a> (req : HttpRequest) =
    //    let getString rawForm =
    //        System.Text.Encoding.UTF8.GetString(rawForm)
    //    req.rawForm |> getString |> fromJson<'a>



    //build web  INTC
    let app : WebPart =
       choose
            [                
              GET >=> //http://127.0.0.1:8080/stock
                path "/stock" >=> JSON (Crud.getAllStocks) 
              GET >=> //http://127.0.0.1:8080/stock/MSFT
                pathScan "/stock/%s" (fun (stockId) -> JSON (Crud.getStock stockId)) 
              GET >=> //http://127.0.0.1:8080/history/MSFT 
                pathScan "/history/%s" (fun (stockId) -> JSON (Crud.getHistories stockId)) 
              POST >=> 
                path "/addstock" >=> (mapJson (fun (stock:Stock) -> { resultStock = (Crud.getStock stock.StockID) }))
            ] >=> Writers.setMimeType "application/json; charset=utf-8"
    let MSFT = Crud.getStock "MSFT"
//    startWebServer defaultConfig (mapJson (fun (calc:Calc) -> { result = calc.a + calc.b }))
//    startWebServer defaultConfig (mapJson (fun (stock:Stock) -> { resultStock = MSFT}))
//    startWebServer defaultConfig (mapJson (fun (calc:Calc) -> { resultStock = (Crud.getStock "MSFT")}))
    startWebServer defaultConfig (mapJson (fun (stock:Stock) -> { resultStock = (Crud.getStock stock.StockID )}))


//    startWebServer config app
            (* *)
    
    
    //let personWebPart = rest "people" {
    //    GetAll = Db.getPeople
    //}

(*
    //transform SQL data to Json:

    let MSFT = Crud.getStock ("MSFT")
    printfn "getStock returns: %A"  MSFT 

    startWebServer config (mapJson (fun (calc:Calc) -> { stock = MSFT }))
*)

(*
    let jsonSerializerSettings = new JsonSerializerSettings()

    let JSON v =
        jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
        JsonConvert.SerializeObject(v, jsonSerializerSettings) 
        |> OK
        >=> Writers.setMimeType "application/json; charset=utf-8"

    //let getAll = "asd"
    let rest resourceName resource =
        let resourcePath = "/" + resourceName
        let gellAll = resource.GetAll () |> JSON
        path resourcePath >=> GET // >=> getAll


    //let Get resourceName resource =
    //    let resourcePath = "/" + resourceName
    //    let get = warbler (fun _ -> resource () |> JSON)
    //    GET >>= path resourcePath >>= get


    (*
    

    let Get resourceName resource =
        let resourcePath = "/" + resourceName
        let get = warbler (fun _ -> resource () |> JSON)

        GET >>= path resourcePath >>= get


    let PlayerRoutes = choose [
                        Get "players" (Player.GetAll model)
                        //GetById "players" (Player.GetItem model)
                        //Post "players" (Player.Create model)
                        //Delete "players" (Player.DeleteItem model)
                        //Put "players" (Player.Update model)
                        //PutById "players" (Player.UpdateById model)
                        ]
                        *)
    
    let PlayerRoutes = choose [
                        GET >=> path "/p" >=> OK "hi PlayerRoutes" 
                        //Get "games" (Game.GetAll model)
                        //GetById "games" (Game.GetItem model)
                        //Post "games" (Game.Create model)
                        //Delete "games" (Game.DeleteItem model)
                        ]
    
    let GameRoutes = choose [ 
                        GET >=> path "/g" >=> OK "hi GameRoutes" 
                    ]
                        //Get "games" (Game.GetAll model)
                        ////GetById "games" (Game.GetItem model)
                        ////Post "games" (Game.Create model)
                        ////Delete "games" (Game.DeleteItem model)
                        //]

    let routes = choose [
                            PlayerRoutes
                            GameRoutes
                            (NOT_FOUND "Huh?")
                        ]
    startWebServer config routes
*)
    printfn "Make requests now"
    Console.ReadKey true |> ignore
    
    0 // return an integer exit code

