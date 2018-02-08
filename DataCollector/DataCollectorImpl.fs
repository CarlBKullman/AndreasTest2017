namespace DataCollector
open System
open System.Runtime.Serialization 
open FSharp.Data
open FSharp.Data.CsvExtensions
open Crud

type Stock = DbConnection.DbSchema.ServiceTypes.Stock
type History = DbConnection.DbSchema.ServiceTypes.Stock

module DataCollector = 
    open Crud.Crud

//    yahoo.urlpattern <- "%s/v7/finance/download/%s?period1=%i&period2=%i&interval=1d&events=history&crumb=%s" 
    let yahooUrl = "https://query1.finance.yahoo.com"
    let mockUrl = "http://127.0.0.1:8088"
           
    //let msft = CsvFile.Load("https://query1.finance.yahoo.com/v7/finance/download/MSFT?period1=1514635129&period2=1517313529&interval=1d&events=history&crumb=z7O4jAW0mv0").Cache()
    
    //let msft = CsvFile.Load("http://127.0.0.1:8088/v7/finance/download/INTC.csv").Cache()

    [<EntryPoint>]
    let main argv = 

        let msft = CsvFile.Load( (sprintf "%s/v7/finance/download/%s?period1=%i&period2=%i&interval=1d&events=history&crumb=%s" 
            mockUrl "MSFT" 1514635129 1517313529 "z7O4jAW0mv0") ).Cache()
        // Print the prices in the HLOC format

        //msft.Rows 
        //|> Seq.iter (fun row ->  
        //          printfn "HLOC: (%s, %s, %s)" (row.GetColumn "High") (row.GetColumn "Low") (row.GetColumn "Date")
        //    )
        //msft.Rows 
        //|> Seq.iter (fun row ->  
        //    printfn "HLOC: (%f, %M, %O)" 
        //        (row.["High"].AsFloat()) (row?Low.AsDecimal()) (row?Date.AsDateTime())
        //    )
        
        getStocksLastBusinessDay |> Seq.iter (fun row ->  
            printfn "stock %s %O " row.StockID row.LastBusinessDay
            )

        let db = DbConnection.DbSchema.GetDataContext()


        0
