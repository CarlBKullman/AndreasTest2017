namespace Crud

open System
open System.Data
open System.Data.Linq
open System.Linq

 module Crud = 

    let db = DbConnection.DbSchema.GetDataContext()
//    let tStock = DbConnection.DbSchema.ServiceTypes.Stock

    let getAllStocks = 
        query { for row in db.Stock do
                 select row
        }    

    let getStock stockId = 
        query { for row in db.Stock do
                where ( row.StockID.Equals stockId )
                select row
                headOrDefault
        }

    let getHistories stockId = 
        query { 
            for row in db.History do 
            where ( row.StockID.Equals stockId )
            select row
        }


    let getAllStockIds = 
        query { for row in db.Stock do
                 select row.StockID
        }    
        |> Seq.toList
        |> Set.ofList


    let overwriteStock (newStockValue : DbConnection.DbSchema.ServiceTypes.Stock) = 
        let foundStockMayBe = query {
            for row in db.Stock do 
            where ( row.StockID.Equals newStockValue.StockID)
                    
            select (Some row)
            exactlyOneOrDefault
        }
        try
            match foundStockMayBe with
            | Some foundStock ->
                foundStock.Name <- newStockValue.Name
            | None -> 
                db.Stock.InsertOnSubmit(newStockValue)
            db.DataContext.SubmitChanges()
        with
        | exn -> printfn "Exception:\n%s" exn.Message

    let deleteStock stock = 
        try
            db.Stock.Attach(stock)
            db.Stock.DeleteOnSubmit(stock)
            db.DataContext.SubmitChanges()
            printfn "Successfully Delete Stock: %s %s" stock.StockID stock.Name
        with
        | exn -> printfn "DeleteStock Exception:\n%s" exn.Message

    let deleteStocks stocks = 
        try
            db.Stock.AttachAll(stocks)
            db.Stock.DeleteAllOnSubmit(stocks)
            db.DataContext.SubmitChanges()
            stocks|> Seq.iter (fun h -> printfn "Successfully Delete Stock: %s %s" h.StockID h.Name)
        with
        | exn -> printfn "DeleteStock Exception:\n%s" exn.Message
        

//History
    let overwriteHistory (newHistoryValue : DbConnection.DbSchema.ServiceTypes.History) = 
        let foundHistoryMayBe = query {
            for row in db.History do 
            where ( row.StockID.Equals newHistoryValue.StockID &&
                    row.BusinessDate.Equals newHistoryValue.BusinessDate )
            select (Some row)
            exactlyOneOrDefault
        }
        try
            match foundHistoryMayBe with
            | Some foundHistory ->
                foundHistory.OpenPrice <- newHistoryValue.OpenPrice
                foundHistory.HighPrice <- newHistoryValue.HighPrice
                foundHistory.LowPrice <- newHistoryValue.LowPrice
                foundHistory.ClosePrice <- newHistoryValue.ClosePrice
                foundHistory.CloseAdjPrice <- newHistoryValue.CloseAdjPrice
                foundHistory.Volume <- newHistoryValue.Volume
            | None -> 
                db.History.InsertOnSubmit(newHistoryValue)
            db.DataContext.SubmitChanges()
        with
        | exn -> printfn "Exception:\n%s" exn.Message

    let overwriteHistories  histories = 
        histories|> Seq.iter (fun h -> overwriteHistory h)

    let deleteHistories histories = 
        try
            db.History.AttachAll(histories)
            db.DataContext.SubmitChanges()
            histories|> Seq.iter (fun h -> printfn "Successfully DeleteHistories: %s %A %i" h.StockID h.BusinessDate h.Volume)
        with
        | exn -> printfn "DeleteHistories Exception:\n%s" exn.Message
        
 //test 
    let validate_insert_default_Stocks =
        printfn "validate default Stocks for test\n"
        let defaultRecords = [
            new DbConnection.DbSchema.ServiceTypes.Stock(StockID = "MSFT", Name = "Microsoft Corporation") ;
            new DbConnection.DbSchema.ServiceTypes.Stock(StockID = "INTC", Name = "Intel Corporation")
            ]
   
        let existingRecords = 
            query { for row in db.Stock do
                     select row.StockID
            }    
            |> Seq.toList
            |> Set.ofList

        let recordsToAdd = defaultRecords.Where( fun r -> not (Set.contains r.StockID existingRecords))
   
        db.Stock.InsertAllOnSubmit(recordsToAdd)
        try
            db.DataContext.SubmitChanges()
            recordsToAdd|> Seq.iter (fun stock -> printfn "Successfully inserted: %s %s" stock.StockID stock.Name)
        with
        | exn -> printfn "Exception:\n%s" exn.Message

    [<EntryPoint>]
    let main argv = 
    
        let getNumber msg =
            printf msg;
            try
                int32(System.Console.ReadLine())
            with
                | :? System.FormatException -> -1
                | :? System.OverflowException -> System.Int32.MinValue
                | :? System.ArgumentNullException -> 0


        // Enable the logging of database activity to the console.
        db.DataContext.Log <- System.Console.Out
        validate_insert_default_Stocks
        printfn "Database validated \n" 

///Stokcs    
        getAllStocks|> printfn "getAllStocks returns: %A"

        let microsoft = getStock ("MSFT")
        printfn "getStock returns: %s %s"  microsoft.StockID microsoft.Name

///history    
        let histories = [
            new DbConnection.DbSchema.ServiceTypes.History(StockID = "MSFT", BusinessDate = DateTime.Now) 
        ]
        
        overwriteHistories histories 
        printfn "insertHistories returns:"

        let oldhistories = getHistories "MSFT" 

        oldhistories |>  Seq.iter (fun h -> 
                printfn "reaadHistories: %s %A %i" h.StockID h.BusinessDate h.Volume)

       // let db = DbConnection.DbSchema.GetDataContext()
       

        printfn "Overvriew ============ Histories"
        //let oldDate = snd (System.DateTime.TryParse "1-1-2011")
        let _ , oldDate = System.DateTime.TryParse "1-1-2011"
        let histories333 = 
            new DbConnection.DbSchema.ServiceTypes.History(StockID = "MSFT", BusinessDate = oldDate, Volume = 333) 
        let histories444 = 
            new DbConnection.DbSchema.ServiceTypes.History(StockID = "INTC", BusinessDate = oldDate, Volume = 333) 
        overwriteHistory histories333
        printfn "overwriteHistory MSFT returns:"
        overwriteHistory histories444
        printfn "overwriteHistory INTC returns" 
        //match System.DateTime.TryParse "1-1-2011" with
        //| true, date -> printfn "Success: %A" date
        //| false, _ -> printfn "Failed!"

        let stock = 
            new DbConnection.DbSchema.ServiceTypes.Stock(StockID = "xxx", Name = "yyy") 
        //overwriteStock stock

        let MSFT = getStock ("MSFT")
        printfn "getStock returns: %A" MSFT   



        deleteStock stock
//        Console.ReadLine() |> ignore
        0 // return an integer exit code
