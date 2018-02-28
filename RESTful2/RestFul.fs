namespace RESTful2.Rest

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave
open Suave.Operators
open Suave.Http
open Suave.Successful
open System.Collections
open System.Collections.Generic

[<AutoOpen>]
module RestFul =    
    open Suave.RequestErrors
    open Suave.Filters
    

    // 'a -> WebPart
    let JSON v =     
        let jsonSerializerSettings = new JsonSerializerSettings()
        jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
    
        JsonConvert.SerializeObject(v, jsonSerializerSettings)
        |> OK 
        >=> Writers.setMimeType "application/json; charset=utf-8"

    let fromJson<'a> json =
        JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a    

    let getResourceFromReq<'a> (req : HttpRequest) = 
        let getString rawForm = System.Text.Encoding.UTF8.GetString(rawForm)
        req.rawForm |> getString |> fromJson<'a>
    
    type Object = {
        node : Newtonsoft.Json.Linq.JToken
        ``type`` : string
    }
    //let data = JsonConvert.DeserializeObject<Object[]>(jsonStr)

    let fromJsonList<'a> json =
          JsonConvert.DeserializeObject<seq<'a[]>>(json) :?> seq<'a>

    let getResourceListFromReq<'a> (req : HttpRequest) = 
        let getString rawForm = System.Text.Encoding.UTF8.GetString(rawForm)
        req.rawForm |> getString |> fromJsonList 

    type RestResource<'a> = {
        GetAll : unit -> 'a seq
        GetById : int -> 'a option
        IsExists : int -> bool
        Create : 'a -> 'a
//        CreateList : 'a seq -> 'a seq
        Update : 'a -> 'a option
        UpdateById : int -> 'a -> 'a option
        Delete : int -> unit
        DeleteAll : unit -> unit
    }

    let rest resourceName resource =
       
        let resourcePath = "/" + resourceName
        let resourceIdPath = new PrintfFormat<(int -> string),unit,string,string,int>(resourcePath + "/%d")
        
        let badRequest = BAD_REQUEST "Resource not found"

        let handleResource requestError = function
            | Some r -> r |> JSON
            | _ -> requestError

        let getAll= warbler (fun _ -> resource.GetAll () |> JSON)

        let deleteAll = 
            printf "delete all"
            warbler (fun _ -> resource.DeleteAll) |> ignore
            NO_CONTENT

        let getResourceById = 
            resource.GetById >> handleResource (NOT_FOUND "Resource not found")
        let updateResourceById id =
            request (getResourceFromReq >> (resource.UpdateById id) >> handleResource badRequest)

        let deleteResourceById id =
            resource.Delete id
            NO_CONTENT

        let isResourceExists id =
            if resource.IsExists id then OK "" else NOT_FOUND ""
        
        (*
            Collection, such as https://api.example.com/resources : 

            GET	    --> List the URIs and perhaps other details of the collection's members.
            PUT	    --> Replacethe entire collection with another collection.
            PATCH	--> Not generally used
            POST	--> Create a new entry in the collection. 
                        The new entry's URI is assigned automatically and is usually returned by the operation.
            DELETE  --> Delete the entire collection.


            Element, such as https://api.example.com/resources/item17

            GET	    --> Retrieve a representation of the addressed member of the collection, 
                        expressed in an appropriate Internet media type.
            PUT	    --> Replacethe addressed member of the collection, or if it does not exist, create it.
            PATCH	--> Update the addressed member of the collection.
            POST	--> Not generally used. Treat the addressed member as a collection in its own right and create a new entry within it.
            DELETE  --> Delete the addressed member of the collection.
            HEAD    --> exists or not 

        *)

        choose [
            path resourcePath >=> choose [
                GET >=> getAll
                POST >=> request (getResourceFromReq >> resource.Create >> JSON)  //single record
//                POST >=> request (getResourceListFromReq >> resource.CreateList >> JSON) //collection 
                PUT >=> request (getResourceFromReq >> resource.Update >> handleResource badRequest)
                DELETE >=> deleteAll 
//                PATCH  >=> request (getResourceFromReq >> resource.Update >> handleResource badRequest)
            ]
            DELETE >=> pathScan resourceIdPath deleteResourceById
            GET >=> pathScan resourceIdPath getResourceById
            PUT >=> pathScan resourceIdPath updateResourceById
            HEAD >=> pathScan resourceIdPath isResourceExists
            //POST >=> pathScan resourceIdPath updateResourceById ---- not generally used
            //PATCH  >=> pathScan resourceIdPath updateResourceById --- update memeber 
        ]