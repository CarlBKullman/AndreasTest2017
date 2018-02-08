namespace SuaveRestApi.Rest

open SuaveRestApi.Rest
open SuaveRestApi.Db
open Suave.Web
open Suave.Http.Successful


[<AutoOpen>]
module RestFul =
     type RestResource<'a> = {
            GetAll : unit -> 'a seq
     }
     // 'a -> WebPart
     let JSON v =
        let jsonSerializerSettings = new JsonSerializerSettings()
        jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()

        JsonConvert.SerializeObject(v, jsonSerializerSettings)
        |> OK
        >=> Writers.setMimeType "application/json; charset=utf-8"

     // string -> RestResource<'a> -> WebPart
     let rest resourceName resource =
        let resourcePath = "/" + resourceName
        let gellAll = warbler (fun _ -> resource.GetAll () |> JSON)
        path resourcePath >=> GET >=> getAll