namespace AndreasCommon

open Microsoft.FSharp.Data.TypeProviders

type DbSchema = SqlDataConnection<"Data Source=CARL2013\SQLEXPRESS;Initial Catalog=Andreas;User=Andreas;Password=password;">

// member this.RestUrl() : string = "http://*:8080/Andreeas/"
//type Properties = SqlDataConnection<"Data Source=CARL2013\SQLEXPRESS;Initial Catalog=Andreas;Integrated Security=SSPI;"
module Properties = 
    type Properties() = 
        member this.RestUrl = "http://*:8080/Andreeas/" 
        static member RestUrl_ : string = "http://*:8080/Andreeas/" 

(*

example using propery text file:
        type ConnectionStringValue = ConnectionStringValue of string
    let readConnectionString ():ConnectionStringValue =
        let json = System.IO.File.ReadAllText("config.json");
        let config = JsonValue.Parse(json);
        ConnectionStringValue(sprintf "server=%s;Integrated Security=SSPI;database=%s;pooling=true" (config?server.AsString()) (config?database.AsString()))


    let conn:string = readConnectionString().ToString();
    type database = SqlDataProvider<
                        ConnectionString = conn,
                        DatabaseVendor = Common.DatabaseProviderTypes.MSSQLSERVER>
Hello, for the connection string, the static parameter MUST be a literal string known at compile time (this can be a [<Literal>] 
But may work for web server....
*)