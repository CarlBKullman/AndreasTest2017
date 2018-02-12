open System
open System.Net
open System.Drawing
open System.Windows.Forms
open System.Windows.Forms.DataVisualization.Charting
open FSharp.Charting
open FSharp.Charting
open Microsoft.FSharp.Control.WebExtensions
open System.Windows.Forms.DataVisualization.Charting
open System.Collections
open System.Drawing
open System.IO
open System.Windows.Forms
open System.Windows.Forms.DataVisualization.Charting
open System.Windows.Forms.DataVisualization.Charting.Utilities
open System
open FSharp.Charting
open Crud

[<EntryPoint>]
[<STAThread>]
let main argv = 
 
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault false

    let hiostories = 
        Crud.getHistories "MSFT" 
    
    let min = (hiostories |> Seq.map ( fun r -> r.LowPrice) |> Seq.min) * 0.95m 
    let max = (hiostories |> Seq.map ( fun r -> r.HighPrice)|> Seq.max) * 1.05m

    printfn "min %E max %E" min max
    //let line = 
    //    Crud.getHistories "MSFT" 
    //    |> Seq.map (fun h -> h.BusinessDate, h.ClosePrice)

    //let line = [ for x in 0 .. 10 -> x, x*x ]
    //let chart = Chart.Line(line)

    // Stock Charts
    //let prices = hiostories |> Seq.map (fun h -> h.HighPrice, h.LowPrice, h.OpenPrice, h.ClosePrice)
    //let chart = Chart.Stock(prices)
    //List.mapi

    // Candlestick Charts
    let pricesWithDates = hiostories 
                            |> Seq.map (fun h -> 
                                (h.BusinessDate.ToShortDateString(), h.HighPrice, h.LowPrice, h.OpenPrice, h.ClosePrice))
    // Candlestick chart price range specified
    let chart = Chart.Candlestick(pricesWithDates).WithYAxis(Min = Convert.ToDouble min, Max = Convert.ToDouble max)

    //let pricesWithDates = 
    //    prices |> 
    //    List.mapi (fun h -> 
    //        (h.B.ToShortDateString(), h.hi, lo, op, cl))


    Application.Run (chart.ShowChart())
    0
