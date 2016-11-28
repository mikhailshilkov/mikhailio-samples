open System
open System.Drawing
open FSharp.Charting
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Core.Operators
open FSharp.Charting.ChartTypes
open System.Windows.Forms

[<EntryPoint>]
let main argv = 

    printf "Charting..."

    let reads next color =
      let data = 
        [1..1000]
        |> List.mapFold (fun s i -> ((i, s), s + next())) 0
        |> fst

      data 
      |> Chart.Line 
      |> Chart.WithStyling (Color = color) 
      |> Chart.WithXAxis (MajorGrid = ChartTypes.Grid(Enabled=false), Min = 0.0)
      |> Chart.WithYAxis (MajorGrid = ChartTypes.Grid(Enabled=false))

    let save filename (c:GenericChart) =
        use cc = new ChartControl(c)
        cc.Dock <- DockStyle.Fill
        use f = new Form()
        f.Size <- System.Drawing.Size(778, 500)
        f.Controls.Add cc
        f.Load |> Event.add (fun _ -> c.SaveChartAs(filename, ChartImageFormat.Png); f.Close()) // yay
        Application.Run f


    let r = new Random()
    let max = 10
    let constant () = 1
    let next () = 1 + r.Next (max - 1)
    let mutable i = 5
    let nextFaulty () = r.Next max |> (fun x -> if x = 5 then i <- i + 1; i else x)

    let mutable j = 0
    let single () = j <- j + 1; j

    save "disk-space.png" (reads constant Color.Blue)
    save "reads-low.png" (Chart.Combine [reads constant Color.Blue; reads next Color.Green])
    save "reads-outlier.png" (Chart.Combine [reads constant Color.Blue; reads next Color.Green; reads nextFaulty Color.Orange])
    save "reads-single.png" (Chart.Combine [reads constant Color.Blue; reads next Color.Green; reads nextFaulty Color.Orange; reads single Color.Red])
    printfn "Done"

//    let source = new ListSource<string>([
//      "Lorem Ipsum is simply dummy text of the printing and typesetting industry"
//      "Lorem Ipsum has been the industry's standard dummy text ever since the 1500s"
//      "when an unknown printer took a galley of type and scrambled it to make a type" 
//      "specimen book It has survived not only five centuries, but also the leap into" 
//      "electronic typesetting remaining essentially unchanged It was popularised in" 
//      "the 1960s with the release of Letraset sheets containing Lorem Ipsum passages" 
//      "and more recently with desktop publishing software like Aldus PageMaker including" 
//      "versions of Lorem Ipsum"]);
//    let sink = new ActionSink<string>(Console.WriteLine);
//
//    let start = Visualizer.from
//
//    let pipeline =
//      start source
//      |> Flow.flatMap (fun s -> s.Split ' ' |> Seq.ofArray)
//      |> Flow.groupBy id
//      |> Flow.map (fun (w, g) -> (w, Seq.length g))
//      |> Flow.filter (fun (w, c) -> w.Length > 3 && c > 1)    
//      |> Flow.map (fun (w, c) -> sprintf "%i %s" c w)
//      |> Flow.connectTo sink
//    
//    pipeline |> Flow.run

//    Console.ReadKey() |> ignore
    0 // return an integer exit code
