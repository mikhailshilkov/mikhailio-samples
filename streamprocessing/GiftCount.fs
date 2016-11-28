open System 
open System.Collections.Generic

type Gift = string

let tokenize (wishlist: string) =
  wishlist.ToLowerInvariant().Split([|","; ", "|], StringSplitOptions.RemoveEmptyEntries)
  |> List.ofArray
  |> List.map (fun x -> x.Trim())
  |> List.filter (fun x -> x.Length > 0)

let print (gift, count) = sprintf "%i %s" count gift

type BoundedSource<'T> = unit -> 'T seq
type UnboundedSource<'T> = ('T -> unit) -> unit

type Source<'T> = 
  | Bounded of BoundedSource<'T>
  | Unbounded of UnboundedSource<'T>

type Sink<'T> = | Action of ('T -> unit)

type Triggered<'T>() = 
  let subscribers = new List<'T -> unit>()
  member this.DoNext x =
    subscribers.ForEach(fun s -> s x)
  member this.Subscribe = subscribers.Add

type Runnable = unit -> unit

type IFlow<'a> =
  abstract member Map<'b> : ('a -> 'b) -> IFlow<'b>
  abstract member Collect<'b> : ('a -> 'b list) -> IFlow<'b>
  abstract member CountBy<'b when 'b: equality> : ('a -> 'b) -> IFlow<'b * int>
  abstract member To: Sink<'a> -> Runnable

module Flow =
  let map<'TI, 'TO> (f: 'TI -> 'TO) (flow: IFlow<'TI>) = flow.Map f
  let collect<'TI, 'TO> (f: 'TI -> 'TO list) (flow: IFlow<'TI>) = flow.Collect f
  let countBy<'T, 'TK when 'TK: equality> (f: 'T -> 'TK) (flow: IFlow<'T>) = flow.CountBy f
  let connectTo<'T> sink (flow: IFlow<'T>) = flow.To sink
  let run (r: Runnable) = r()

module Runner =
  let private mapTransform map = function
    | Bounded bs -> bs >> Seq.map map |> Bounded
    | Unbounded us ->
      fun subscriber -> map >> subscriber |> us
      |> Unbounded

  let private collectTransform mapToMany = function
    | Bounded bs -> bs >> Seq.map mapToMany >> Seq.concat |> Bounded
    | Unbounded us ->
      fun subscriber -> mapToMany >> Seq.iter subscriber |> us
      |> Unbounded

  let private countByTransform<'a, 'b when 'b: equality> (getKey: 'a -> 'b) source =
    let state = new Dictionary<'b, int>()
    let addItem i = 
      let key = getKey i
      let value = if state.ContainsKey key then state.[key] else 0
      let newValue = value + 1
      state.[key] <- newValue
      (key, newValue)

    match source with
    | Bounded bs -> bs >> Seq.countBy getKey |> Bounded
    | Unbounded us -> (fun s -> addItem >> s) >> us |> Unbounded

  let private stage source transform sink () =
    match transform source, sink with
    | Bounded bs, Action a -> bs() |> Seq.iter a
    | Unbounded us, Action a -> us a

  type private Flow<'a>(source: Source<'a>, connect: Sink<'a> -> Runnable) =
    member this.Apply<'b> t = new Flow<'b>(t source, stage source t) :> IFlow<'b>

    interface IFlow<'a> with
      member this.Map<'b> map = this.Apply<'b> (mapTransform map)

      member this.Collect<'b> map = this.Apply<'b> (collectTransform map)

      member this.CountBy<'b when 'b: equality>(getKey) = 
        this.Apply<'b * int> (countByTransform<'a, 'b> getKey)

      member this.To(sink) = connect(sink)

  let from<'a> source =
    new Flow<'a>(source, stage source (mapTransform id)) :> IFlow<'a>

let consoleSource = new Triggered<string>()
let unboundedSource = consoleSource.Subscribe |> Unbounded
let consoleSink = (fun (s: string) -> Console.WriteLine s) |> Action

unboundedSource                // e.g. same console source as before
|> Runner.from                 // create Runner implementation of IFlow
|> Flow.collect tokenize
|> Flow.countBy id
|> Flow.map print
|> Flow.connectTo consoleSink  // connect to the Sink
|> Flow.run                    // start listening for events

Seq.initInfinite (fun _ -> Console.ReadLine())
|> Seq.takeWhile ((<>) "q")
|> Seq.iter consoleSource.DoNext