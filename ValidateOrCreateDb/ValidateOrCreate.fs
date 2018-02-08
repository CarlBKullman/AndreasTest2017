open System
open System.Data
open System.Data.Linq
open System.Linq

//open Microsoft.FSharp.Data.TypeProviders
//open Microsoft.FSharp.Linq
open AndreasCommon
[<Literal>]
let validateDb = 
        "IF OBJECT_ID (N'dbo.Stock', N'U') IS NOT NULL\n\
        BEGIN \n\
        print 'table History exists';\n\
--          drop table dbo.Stock;\n\
      END \n\
    else \n\
      begin \n\
          print 'table Stock not found create new!' \n\
		  CREATE TABLE dbo.Stock (\n\
			StockID	     NVARCHAR(10)  NOT NULL,\n\
			Name         NVARCHAR(4000)   NOT NULL,\n\
			PRIMARY KEY (StockID)  \n\
			)\n\
      end; \n\
    IF OBJECT_ID (N'dbo.History', N'U') IS NOT NULL\n\
        BEGIN \n\
        print 'table exists';\n\
--          drop table dbo.History;\n\
      END \n\
    else \n\
      begin \n\
          print 'table History not found create new!' \n\
		  CREATE TABLE dbo.History ( \n\
			StockID	     NVARCHAR(10)  NOT NULL, \n\
			BusinessDate DATE      NOT NULL, \n\
			OpenPrice	 DEC(19,6) NOT NULL DEFAULT 0, \n\
			HighPrice	 DEC(19,6) NOT NULL DEFAULT 0, \n\
			LowPrice	 DEC(19,6) NOT NULL DEFAULT 0, \n\
			ClosePrice   DEC(19,6) NOT NULL DEFAULT 0, \n\
            CloseAdjPrice   DEC(19,6) NOT NULL DEFAULT 0, \n\
			Volume       INT       NOT NULL DEFAULT 0, \n\
			PRIMARY KEY (StockID, BusinessDate) \n\
            --,CONSTRAINT FK_StockID FOREIGN KEY (StockID) \n\
            --    REFERENCES dbo.Stock (StockID) \n\
            --    ON DELETE CASCADE \n\
            --    ON UPDATE CASCADE \n\
			)\n\
      end;"

[<EntryPoint>]
let main argv = 
    printfn "Ensure Database\n" 

    let db = AndreasCommon.DbSchema.GetDataContext()
    // Enable the logging of database activity to the console.
    db.DataContext.Log <- System.Console.Out

    try 
        db.DataContext.ExecuteCommand(validateDb) |> ignore
    with
    | exn -> printfn "Exception:\n%s" exn.Message
    printfn "Database validated\n"

    // Console.ReadLine() |> ignore
    0 // return an integer exit code
