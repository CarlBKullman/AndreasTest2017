namespace RESTful2.MusicStoreDb

open System
open System.Data
open System.Data.Linq
open System.Linq
open Microsoft.FSharp.Data.TypeProviders

//open DbConnection

module MusicStoreDb = 
    type Album = 
        { 
          AlbumId : int
          ArtistId : int
          GenreId : int
          Title : string
          Price : decimal}
    
    //type private Sql = SqlDataProvider< "Server=localhost;Database=SuaveMusicStore;Trusted_Connection=True;MultipleActiveResultSets=true;Integrated Security=SSPI;", DatabaseVendor=Common.DatabaseProviderTypes.MSSQLSERVER >
    let db = DbConnection.DbSchema.GetDataContext()

    //let DbContext = DbConnection.DbSchema.GetDataContext()
    type DbContext = DbConnection.DbSchema.ServiceTypes
 (*   
  CREATE TABLE Albums (
AlbumId INT NOT NULL,    
ArtistId INT NOT NULL,  
GenreId  INT NOT NULL, 
Title NVARCHAR(100) NOT NULL,  
Price DEC(19,6) NOT NULL DEFAULT 0
)
)
*)
    type AlbumEntity = DbContext.Albums
    
    //let private getContext() = Sql.GetDataContext()
    //type AlbumEntity = DbConnection.DbSchema.ServiceTypes.Albums
    let firstOrNone s = s |> Seq.tryFind (fun _ -> true)

    let mapToAlbum (albumEntity :  AlbumEntity) =
        {   
            AlbumId = albumEntity.AlbumId
            ArtistId = albumEntity.ArtistId
            GenreId = albumEntity.GenreId
            Title = albumEntity.Title
            Price = albumEntity.Price
        }

    let getAlbums () = 
        query { for row in db.Albums do
                 select row
        }    
        |> Seq.map mapToAlbum

    let getAlbumEntityById id = 
        query { 
        for album in db.Albums do
            where (album.AlbumId = id)
            select album
        } |> firstOrNone

    let getAlbumById id =
        getAlbumEntityById id 
        |> Option.map mapToAlbum

    let createAlbum album =
        let album = new AlbumEntity(AlbumId = album.AlbumId, ArtistId = album.ArtistId, GenreId = album.GenreId, Price = album.Price, Title = album.Title)
        db.Albums.InsertOnSubmit(album)
        db.DataContext.SubmitChanges()
        album |> mapToAlbum

    let updateAlbumById id album =
        let albumEntity = getAlbumEntityById album.AlbumId
        match albumEntity with
        | None -> None
        | Some a ->
            a.ArtistId <- album.AlbumId
            a.GenreId <- album.GenreId
            a.Price <- album.Price
            a.Title <- album.Title
            db.DataContext.SubmitChanges()
            Some album
        
    let updateAlbum album =
        updateAlbumById album.AlbumId album

    let deleteAlbum id =
        let albumEntity = getAlbumEntityById id
        match albumEntity with
        | None -> ()
        | Some a ->
            db.Albums.DeleteOnSubmit(a)
            db.DataContext.SubmitChanges()

    let isAlbumExists id =
        let albumEntity = getAlbumEntityById id
        match albumEntity with
        | None -> false
        | Some _ -> true
    