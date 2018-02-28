open System
open System.Data.Linq

open DbConnection
[<Literal>]



let validateDb = 
        "IF OBJECT_ID (N'dbo.Stock', N'U') IS NOT NULL\n\
        BEGIN \n\
        print 'table Stock exists';\n\
--          drop table dbo.Stock;\n\
      END \n\
    else \n\
      begin \n\
          print 'table Stock not found create new!' \n\
		  CREATE TABLE dbo.Stock (\n\
			StockID	     NVARCHAR(10)  NOT NULL,\n\b
			Name         NVARCHAR(4000)   NOT NULL,\n\
			PRIMARY KEY (StockID)  \n\
			)\n\
      end; \n\
    IF OBJECT_ID (N'dbo.History', N'U') IS NOT NULL\n\
        BEGIN \n\
        print 'table History exists';\n\
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
    end; \n\
    IF OBJECT_ID (N'dbo.Albums', N'U') IS NOT NULL\n\
        BEGIN \n\
        print 'table Albums exists';\n\
--          drop table dbo.Albums;\n\
      END \n\
    else \n\
      begin \n\
          print 'table Albums not found create new!' \n\
		  CREATE TABLE dbo.Albums ( \n\
                AlbumId INT NOT NULL,     \n\
                ArtistId INT NOT NULL,   \n\
                GenreId  INT NOT NULL,  \n\
                Title NVARCHAR(100) NOT NULL,   \n\
                Price DEC(19,6) NOT NULL DEFAULT 0, \n\
    			PRIMARY KEY (AlbumId) \n\
        ) \n\
			)\n\
    end; \n\
    IF OBJECT_ID (N'dbo.LastStockHistory', N'U') IS NOT NULL\n\
        BEGIN \n\
        print 'view exists';\n\
--          drop view dbo.LastStockHistory;\n\
      END \n\
    else \n\
      begin \n\
          print 'view LastStockHistory not found create new!' \n\
         CREATE VIEW LastStockHistory AS \n\
            SELECT  \n\
                Stock.StockID,  \n\
                    COALESCE(MAX(History.BusinessDate), DATEADD(DAY, -30, GETDATE())) as LastBusinessDay,  \n\
                    DATEDIFF(S,   \n\
			                    '1970-01-01',   \n\
			                    COALESCE(  \n\
				                    MAX(History.BusinessDate),   \n\
				                    DATEADD(DAY, -30, GETDATE())  \n\
			                    )  \n\
                    ) AS SecondsSinceEpoch  \n\
            FROM Stock  \n\
            LEFT OUTER JOIN History ON Stock.StockID = History.StockID  \n\
            GROUP BY Stock.StockID  \n\
      end;"
//LastStockHistory is default -30 days from now. chage view definition when needed. "DATEADD(DAY, -30, GETDATE()" above

[<EntryPoint>]
let main argv = 
    printfn "Ensure Database\n" 

    let db = DbConnection.DbSchema.GetDataContext()
    
    // Enable the logging of database activity to the console.
    db.DataContext.Log <- System.Console.Out
    
    //Execure database initialization SQl statement, create 1->M simple datamoder Stock -> History 
    //and view LastStockHistory with outer join to find latest dated history

    
    try 
        db.DataContext.ExecuteCommand(validateDb) |> ignore
    with
    | exn -> printfn "Exception:\n%s" exn.Message
    printfn "Database validated\n"
    //may be there is way in f# to do data maping and buid DB from this, I am missing here
    //Outer join view sonds more exlegant in SQL as in F# to code, this is why I use SQL view LastStockHistory
    0 
