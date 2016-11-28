namespace ComposableData

type ISource<'T> = interface end
type ISink<'T> = interface end
type ITransform<'TI,'TO> = 
  abstract member Apply: ISource<'TI> -> ISource<'TO>

type IBoundedSource<'T> = 
  abstract member Items: unit -> 'T seq
  inherit ISource<'T>

type IUnboundedSource<'T> = 
  abstract member OnNext: 'T -> unit
  inherit ISource<'T>

type IActionSink<'T> = 
  abstract member Action: 'T -> unit
  inherit ISink<'T>
    
type IRun = 
  abstract member Run: unit -> unit

type ListSource<'T>(list: 'T list) = 
  interface IBoundedSource<'T> with
    member this.Items() = list |> Seq.ofList

type ActionSink<'T>(action: 'T -> unit) =
  interface IActionSink<'T> with
    member this.Action(item) = action(item)

type IFlow<'T> =
  abstract member Map<'TO> : ('T -> 'TO) -> IFlow<'TO>
  abstract member FlatMap<'TO> : ('T -> seq<'TO>) -> IFlow<'TO>
  abstract member Filter : ('T -> bool) -> IFlow<'T>
  abstract member GroupBy<'TK when 'TK: equality> : ('T -> 'TK) -> IFlow<('TK * seq<'T>)>
  abstract member To: ISink<'T> -> IRun

[<RequireQualifiedAccess>]
module Flow =
  let map<'TI, 'TO> (f: 'TI -> 'TO) (flow:IFlow<'TI>) = flow.Map(f)
  let flatMap<'TI, 'TO> (f: 'TI -> seq<'TO>) (flow:IFlow<'TI>) = flow.FlatMap(f)
  let filter<'T> (f: 'T -> bool) (flow:IFlow<'T>) = flow.Filter(f)
  let groupBy<'TI, 'TK when 'TK: equality> (f: 'TI -> 'TK) (flow:IFlow<'TI>) = flow.GroupBy(f)
  let connectTo<'T> sink (flow:IFlow<'T>) = flow.To sink
  let run (r: IRun) = r.Run()

[<RequireQualifiedAccess>]
module Runner =
  type private MapTransform<'TI, 'TO>(map: 'TI -> 'TO) =
    interface ITransform<'TI, 'TO> with
      member this.Apply(source: ISource<'TI>): ISource<'TO> = 
        { new IBoundedSource<'TO> with
            member this.Items() = (source :?> IBoundedSource<'TI>).Items() |> Seq.map map
        } :> ISource<'TO>

  type private FlatMapTransform<'TI, 'TO>(map: 'TI -> seq<'TO>) =
    interface ITransform<'TI, 'TO> with
      member this.Apply(source: ISource<'TI>): ISource<'TO> = 
        { new IBoundedSource<'TO> with
            member this.Items() = (source :?> IBoundedSource<'TI>).Items() |> Seq.map map |> Seq.concat
        } :> ISource<'TO>

  type private GroupByTransform<'TI, 'TK when 'TK: equality>(getKey: 'TI -> 'TK) =
    interface ITransform<'TI, ('TK * seq<'TI>)> with
      member this.Apply(source: ISource<'TI>): ISource<('TK * seq<'TI>)> = 
        { new IBoundedSource<('TK * seq<'TI>)> with
            member this.Items() = 
              let a = (source :?> IBoundedSource<'TI>).Items() |> Seq.groupBy getKey
              a
        } :> ISource<('TK * seq<'TI>)>

  type private Block<'TI, 'TO>(source: ISource<'TI>, transform: ITransform<'TI, 'TO>, sink: ISink<'TO>) =
     member this.Source = source
     member this.Transform = transform
     member this.Sink = sink;

     interface ISink<'TI>
     interface ISource<'TO>
     interface IRun with
       member this.Run() =       
         match this.Sink with
         | :? IRun as r -> r.Run()
         | :? IActionSink<'TO> as action ->
           this.Transform.Apply(this.Source) :?> IBoundedSource<'TO>
           |> (fun x -> x.Items())
           |> Seq.iter (fun item -> action.Action(item))
         | _ -> failwith "Unknown Sink type"

  type private Flow<'T>(source: ISource<'T>, factory: ISink<'T> -> IRun) =
    interface IFlow<'T> with
      member this.Map<'TO>(map) =
        let transform = new MapTransform<'T, 'TO>(map) :> ITransform<'T, 'TO>
        let source2 = transform.Apply(source)
        new Flow<'TO>(source2, fun sink -> new Block<'T, 'TO>(source, transform, sink) :> IRun) :> IFlow<'TO>

      member this.FlatMap<'TO>(map) =
        let transform = new FlatMapTransform<'T, 'TO>(map) :> ITransform<'T, 'TO>
        let source2 = transform.Apply(source)
        new Flow<'TO>(source2, fun sink -> new Block<'T, 'TO>(source, transform, sink) :> IRun) :> IFlow<'TO>

      member this.Filter(predicate) = (this :> IFlow<'T>).FlatMap<'T>(fun x -> if predicate x then seq [x] else seq [])

      member this.GroupBy<'TK when 'TK: equality>(getKey) =
        let transform = new GroupByTransform<'T, 'TK>(getKey) :> ITransform<'T, ('TK * seq<'T>)>
        let source2 = transform.Apply(source)
        new Flow<'TK * seq<'T>>(source2, fun sink -> new Block<'T, ('TK * seq<'T>)>(source, transform, sink) :> IRun) :> IFlow<'TK * seq<'T>>

      member this.To(sink) = factory(sink)

  let from<'T> source =
    new Flow<'T>(source, fun sink -> new Block<'T, 'T>(source, new MapTransform<'T, 'T>(id), sink) :> IRun) :> IFlow<'T>

[<RequireQualifiedAccess>]
module Visualizer =
  type private Printer(log: string seq) =
    interface IRun with
      member this.Run() =
        log |> Seq.iter System.Console.WriteLine

  type private Visual<'T>(log: string seq) =
    interface IFlow<'T> with
      member this.Map<'TO>(map) =
        let s = sprintf "%s %s" typedefof<'T>.Name typedefof<'TO>.Name
        new Visual<'TO>([s] |> Seq.append log) :> IFlow<'TO>

      member this.FlatMap<'TO>(map) =
        let s = sprintf "%s %s" typedefof<'T>.Name typedefof<'TO>.Name
        new Visual<'TO>([s] |> Seq.append log) :> IFlow<'TO>

      member this.Filter(predicate) =
        let s = "filtering"
        new Visual<'T>([s] |> Seq.append log) :> IFlow<'T>

      member this.GroupBy<'TK when 'TK: equality>(getKey) =
        let s = sprintf "%s %s" typedefof<'T>.Name typedefof<'TK>.Name
        new Visual<'TK * seq<'T>>([s] |> Seq.append log) :> IFlow<'TK * seq<'T>>

      member this.To(sink) =
        let s = sprintf "-> %s" (sink.GetType().Name)
        new Printer([s] |> Seq.append log) :> IRun
  
  let from<'T> (source: ISource<'T>) =
    new Visual<'T>([source.GetType().Name + " ->"]) :> IFlow<'T>