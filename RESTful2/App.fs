namespace SuavRESTful2

module App =   
    open System
    open System.IO

    open Suave.Web
    open RESTful2.Rest
    open RESTful2.Db
    open RESTful2.MusicStoreDb
    open Suave
    open Newtonsoft.Json
    open Newtonsoft.Json.Serialization

    type AudienceDto = {
        AudienceId : string
        Name : string  
    }    

    [<EntryPoint>]
    let main argv =    

        //let personWebPart = rest "people" {
        //    GetAll = Db.getPeople
        //    GetById = Db.getPerson
        //    Create = Db.createPerson
        //    Update = Db.updatePerson
        //    UpdateById = Db.updatePersonById
        //    Delete = Db.deletePerson
        //    IsExists = Db.isPersonExists
        //}

        let albumWebPart = rest "albums" {
            GetAll = MusicStoreDb.getAlbums
            GetById = MusicStoreDb.getAlbumById
            Create = MusicStoreDb.createAlbum
//            CreateList = MusicStoreDb.CreateAlbumList
            Update = MusicStoreDb.updateAlbum
            UpdateById = MusicStoreDb.updateAlbumById
            Delete = MusicStoreDb.deleteAlbum
            IsExists = MusicStoreDb.isAlbumExists
            DeleteAll = MusicStoreDb.delete
        }       


        //let stockWebPart = rest "stock" {
        //    GetAll = MusicStoreDb.getAlbums
        //    GetById = MusicStoreDb.getAlbumById
        //    Create = MusicStoreDb.createAlbum
        //    Update = MusicStoreDb.updateAlbum
        //    UpdateById = MusicStoreDb.updateAlbumById
        //    Delete = MusicStoreDb.deleteAlbum
        //    IsExists = MusicStoreDb.isAlbumExists
        //    DeleteAll = MusicStoreDb.delete
        //}       

        let app = choose[albumWebPart] //;stockWebPart]                    

        startWebServer defaultConfig app
(*         
        let jso = """
            [{"albumId":2,"artistId":2,"genreId":2,"title":"floys","price":1.250000},{"albumId":3,"artistId":1,"genreId":1,"title":"abba","price":12.500000}]"""  
        printf "injison %s" jso
        //MusicStoreDb.Album 
        //let s : MusicStoreDb.Album  = 
        let fromJsonList json = JsonConvert.DeserializeObject<MusicStoreDb.Album[]>(json) 
        let back = fromJsonList jso
        printf "injison back %A" back

        Console.ReadKey true |> ignore
*)
        0

        
