namespace SuavRESTful2

module App =   
    open Suave.Web
    open RESTful2.Rest
    open RESTful2.Db
    open RESTful2.MusicStoreDb
    open Suave

    type AudienceDto = {
        AudienceId : string
        Name : string  
    }    

    [<EntryPoint>]
    let main argv =    

        let personWebPart = rest "people" {
            GetAll = Db.getPeople
            GetById = Db.getPerson
            Create = Db.createPerson
            Update = Db.updatePerson
            UpdateById = Db.updatePersonById
            Delete = Db.deletePerson
            IsExists = Db.isPersonExists
        }

        let albumWebPart = rest "albums" {
            GetAll = MusicStoreDb.getAlbums
            GetById = MusicStoreDb.getAlbumById
            Create = MusicStoreDb.createAlbum
            Update = MusicStoreDb.updateAlbum
            UpdateById = MusicStoreDb.updateAlbumById
            Delete = MusicStoreDb.deleteAlbum
            IsExists = MusicStoreDb.isAlbumExists
        }       

       
        let app = choose[personWebPart;albumWebPart]                    

        startWebServer defaultConfig app
            
        0

        
