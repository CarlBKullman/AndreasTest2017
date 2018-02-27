open System
open System.Windows.Forms
open FSharp.Charting
open Crud

[<EntryPoint>]
[<STAThread>]
let main argv = 
 
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault false
            
    let stockId = match argv with
        |  [|first|] -> first
        | _ -> "MSFT"

    printfn "Chart for stock '%s' " stockId 

    let hiostories = 
        Crud.getHistories stockId 
    
    let min = System.Math.Round ((hiostories |> Seq.map ( fun r -> r.LowPrice) |> Seq.min) * 0.98m)
    let max = System.Math.Round ((hiostories |> Seq.map ( fun r -> r.HighPrice)|> Seq.max) * 1.02m)
    
    let minVol = Convert.ToDouble (hiostories |> Seq.map ( fun r -> r.Volume) |> Seq.min) * 0.98 
    let maxVol = Convert.ToDouble (hiostories |> Seq.map ( fun r -> r.Volume) |> Seq.max) * 1.02

    printfn "Stock %s min %E max %E" stockId min max

    //adjust Column chart for volume
    //min = 20% from price span below
    let priceSpan = Convert.ToDouble ( max-min )
    let volumnSpan = Convert.ToDouble (maxVol-minVol)
    
    let adjMinVol = min - ( max-min ) /4m
    //transform volume   v = (v-minVol)*priceSpan/5/volSpan + min
    let volumeWithDates = hiostories |> Seq.map (fun h -> 
                                (h.BusinessDate.ToShortDateString(), 
                                    //h.Volume
                                    ((Convert.ToDouble h.Volume) - minVol)*priceSpan/ 5.0 / volumnSpan + (Convert.ToDouble min)
                                ))
    
    // Candlestick Charts
    let pricesWithDates = hiostories 
                            |> Seq.map (fun h -> 
                                (h.BusinessDate.ToShortDateString(), h.HighPrice, h.LowPrice, h.OpenPrice, h.ClosePrice))
    // Candlestick chart price range specified
    
    let chart = Chart.Combine([
                                Chart.Candlestick(pricesWithDates) 
                                Chart.Column(volumeWithDates,Name="Voulme") 
                                ]).WithYAxis(
                                    Min = Convert.ToDouble adjMinVol, 
                                    Max = Convert.ToDouble max)
    Application.Run (chart.ShowChart())
    0
