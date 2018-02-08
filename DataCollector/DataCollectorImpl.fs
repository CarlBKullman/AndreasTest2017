namespace DataCollector
open System
open System.Runtime.Serialization 
open FSharp.Data
open FSharp.Data.CsvExtensions
open Crud

type Stock = DbConnection.DbSchema.ServiceTypes.Stock
type History = DbConnection.DbSchema.ServiceTypes.History

module DataCollector = 
    open Crud.Crud

//    yahoo.urlpattern <- "%s/v7/finance/download/%s?period1=%i&period2=%i&interval=1d&events=history&crumb=%s" 
    let yahooUrl = "https://query1.finance.yahoo.com"
    let mockUrl = "http://127.0.0.1:8088"
           
    //let msft = CsvFile.Load("https://query1.finance.yahoo.com/v7/finance/download/MSFT?period1=1514635129&period2=1517313529&interval=1d&events=history&crumb=z7O4jAW0mv0").Cache()
    
    //let msft = CsvFile.Load("http://127.0.0.1:8088/v7/finance/download/INTC.csv").Cache()
    let getCsv (stockId:string) (firstDate:int) (lastDate:int64)= 
        CsvFile.Load( (sprintf "%s/v7/finance/download/%s?period1=%i&period2=%i&interval=1d&events=history&crumb=%s" 
        mockUrl stockId firstDate lastDate "z7O4jAW0mv0") ).Cache().Rows

    [<EntryPoint>]
    let main argv = 

        let msft = CsvFile.Load( (sprintf "%s/v7/finance/download/%s?period1=%i&period2=%i&interval=1d&events=history&crumb=%s" 
            mockUrl "MSFT" 1514635129 1517313529 "z7O4jAW0mv0") ).Cache()
        let unixTimeSeconds = (DateTimeOffset(DateTime.Now).ToUnixTimeSeconds())

        getStocksLastBusinessDay |> Seq.iter (fun row ->  
            printfn "uppdate stock %s after %O " row.StockID row.LastBusinessDay;
            getCsv row.StockID (row.SecondsSinceEpoch.GetValueOrDefault()) unixTimeSeconds 
            |> Seq.iter (fun history ->  
                        printfn "HLOC: (%f, %M, %O)" (history.["High"].AsFloat()) (history?Low.AsDecimal()) (history?Date.AsDateTime())
                    )
            //insert into DB                
            getCsv row.StockID (row.SecondsSinceEpoch.GetValueOrDefault()) unixTimeSeconds
            |> Seq.map (fun h -> new History(
                                    StockID = row.StockID, 
                                    BusinessDate = (h?Date.AsDateTime()),
                                    OpenPrice = h?Open.AsDecimal(),
                                    HighPrice = h?High.AsDecimal(),
                                    LowPrice = h?Low.AsDecimal(),
                                    ClosePrice = h?Close.AsDecimal(),
                                    CloseAdjPrice = h.["Adj Close"].AsDecimal(),
                                    Volume = h?Volume.AsInteger()
                                )
                )
            |> Seq.toList
            |> overwriteHistories
            )
        0
