namespace AndreasCommon

open Microsoft.FSharp.Linq
open Microsoft.FSharp.Data.TypeProviders

//type Class1() = 
//    member this.X = "F#"


type DbSchema = SqlDataConnection<"Data Source=CARL2013\SQLEXPRESS;Initial Catalog=Andreas;User=Andreas;Password=password;">
    // member this.RestUrl() : string = "http://*:8080/Andreeas/"
//type Properties = SqlDataConnection<"Data Source=CARL2013\SQLEXPRESS;Initial Catalog=Andreas;Integrated Security=SSPI;"
module Properties = 
    
    type Properties() = 
        member this.RestUrl = "http://*:8080/Andreeas/" 
        static member RestUrl_ : string = "http://*:8080/Andreeas/" 