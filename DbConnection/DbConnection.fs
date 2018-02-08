namespace DbConnection
open Microsoft.FSharp.Data.TypeProviders
open System.Data.Linq

//DbSchema is used for accessing MS SQL database default database  "Initial Catalog=Andreas" must be created and 
//login User=Andreas;Password=password must have dbo level access to create tables and manipulate data

type DbSchema = SqlDataConnection<"Data Source=CARL2013\SQLEXPRESS;Initial Catalog=Andreas;User=Andreas;Password=password;">

//other constatans

//Properties.MockUrl = "127.0.0.1"
//Properties.Mockport = 8088
    






