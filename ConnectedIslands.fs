module Seq =
  let crossproduct seq1 seq2 = seq { 
    for el1 in seq1 do
      for el2 in seq2 -> el1, el2 
  }

type Graph<'a> = {
  Nodes: seq<'a>
  Neighbours: 'a -> seq<'a>
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Graph =
  let findConnectedComponents graph = 

    let rec visitNode accumulator visited node =
      let newAccumulator = Set.add node accumulator
      let newVisited = Set.add node visited

      graph.Neighbours node
      |> Seq.filter (fun n -> Set.contains n newVisited |> not)
      |> Seq.fold (fun (acc, vis) n -> visitNode acc vis n) (newAccumulator, newVisited)

    let visitComponent (sets, visited) node =
      if Set.contains node visited then sets, visited
      else
        let a, b = visitNode Set.empty visited node
        a :: sets, b

    graph.Nodes
    |> Seq.fold visitComponent ([], Set.empty)
    |> fst

type Matrix2D = int[,]

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Matrix2D =
  let allCells (mx: int[][]) = seq {
    for x in [0 .. mx.Length - 1] do
      for y in [0 .. mx.[x].Length - 1] -> x, y
  }

  let neighbours (mx: int[][]) (x,y) =
    Seq.crossproduct [x-1 .. x+1] [y-1 .. y+1]
    |> Seq.filter (fun (i, j) -> i >= 0 && j >= 0 && i < mx.Length && j < mx.[i].Length)
    |> Seq.filter (fun (i, j) -> i <> x || j <> y)    

[<EntryPoint>]
let main argv = 
  let mat = [| [|1; 1; 0; 0; 0|];
               [|0; 1; 0; 0; 1|];
               [|1; 0; 0; 1; 1|];
               [|0; 0; 0; 0; 0|];
               [|1; 0; 1; 0; 1|]
            |]
  let isNode (x, y) = mat.[x].[y] = 1

  let graph = {
    Nodes = Matrix2D.allCells mat |> Seq.filter isNode
    Neighbours = Matrix2D.neighbours mat >> Seq.filter isNode
  }

  let result = Graph.findConnectedComponents graph |> List.length
  printfn "%i" result

  System.Console.ReadKey() |> ignore
  0 // return an integer exit code