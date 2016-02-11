namespace Recognition

open System
open System.Drawing

module StringRecognition =
  type BW = B | W
  type CharPattern = {
    Char: char
    Pattern: BW list list
  }

  let numberPatterns = [|  
    { Char = '0'; Pattern = [[B;W;W;W;W;W;W;B];[W;B;B;B;B;B;B;W];[W;B;B;B;B;B;B;W];[W;B;B;B;B;B;B;W];[B;W;W;W;W;W;W;B]] }
    { Char = '1'; Pattern = [[B;W;B;B;B;B;B;W];[W;W;W;W;W;W;W;W];[B;B;B;B;B;B;B;W]] }
    { Char = '2'; Pattern = [[B;W;B;B;B;B;W;W];[W;B;B;B;B;W;B;W];[W;B;B;B;W;B;B;W];[W;B;B;W;B;B;B;W];[B;W;W;B;B;B;B;W]] }
    { Char = '3'; Pattern = [[B;W;B;B;B;B;W;B];[W;B;B;B;B;B;B;W];[W;B;B;W;B;B;B;W];[W;B;B;W;B;B;B;W];[B;W;W;B;W;W;W;B]] }
    { Char = '4'; Pattern = [[B;B;B;W;W;B;B;B];[B;B;W;B;W;B;B;B];[B;W;B;B;W;B;B;B];[W;W;W;W;W;W;W;W];[B;B;B;B;W;B;B;B]] }
    { Char = '5'; Pattern = [[W;W;W;W;B;B;W;B];[W;B;B;W;B;B;B;W];[W;B;B;W;B;B;B;W];[W;B;B;W;B;B;B;W];[W;B;B;B;W;W;W;B]] }
    { Char = '6'; Pattern = [[B;B;W;W;W;W;W;B];[B;W;B;W;B;B;B;W];[W;B;B;W;B;B;B;W];[W;B;B;W;B;B;B;W];[B;B;B;B;W;W;W;B]] }
    { Char = '7'; Pattern = [[W;B;B;B;B;B;B;B];[W;B;B;B;B;B;W;W];[W;B;B;B;W;W;B;B];[W;B;W;W;B;B;B;B];[W;W;B;B;B;B;B;B]] }
    { Char = '8'; Pattern = [[B;W;W;B;W;W;W;B];[W;B;B;W;B;B;B;W];[W;B;B;W;B;B;B;W];[W;B;B;W;B;B;B;W];[B;W;W;B;W;W;W;B]] }
    { Char = '9'; Pattern = [[B;W;W;W;B;B;B;B];[W;B;B;B;W;B;B;W];[W;B;B;B;W;B;B;W];[W;B;B;B;W;B;W;B];[B;W;W;W;W;W;B;B]] }
    { Char = ','; Pattern = [[B;B;B;B;B;B;W;W]] }
  |]

  let buttonPatterns = [|  
    { Char = 'F'; Pattern = [[B;W;W;W;W;W;W;W;W];[B;W;W;W;W;W;W;W;W];[B;W;B;B;W;B;B;B;B];[B;W;B;B;W;B;B;B;B];[B;W;B;B;W;B;B;B;B];[B;W;B;B;W;B;B;B;B]] }
    { Char = 'o'; Pattern = [[B;B;B;B;W;W;W;W;B];[B;B;B;W;W;W;W;W;W];[B;B;B;W;B;B;B;B;W];[B;B;B;W;B;B;B;B;W];[B;B;B;W;W;W;W;W;W];[B;B;B;B;W;W;W;W;B]] }
    { Char = 'l'; Pattern = [[W;W;W;W;W;W;W;W;W];[W;W;W;W;W;W;W;W;W]] }
    { Char = 'd'; Pattern = [[B;B;B;B;W;W;W;W;B];[B;B;B;W;W;W;W;W;W];[B;B;B;W;B;B;B;B;W];[B;B;B;W;B;B;B;B;W];[W;W;W;W;W;W;W;W;W];[W;W;W;W;W;W;W;W;W]] }
    { Char = 'C'; Pattern = [[B;B;B;W;W;W;W;B;B];[B;B;W;W;W;W;W;W;B];[B;W;W;B;B;B;B;W;W];[B;W;B;B;B;B;B;B;W];[B;W;B;B;B;B;B;B;W];[B;W;B;B;B;B;B;B;W];[B;W;B;B;B;B;B;B;W]] }
    { Char = 'a'; Pattern = [[B;B;B;B;B;B;W;W;B];[B;B;B;W;B;W;W;W;W];[B;B;B;W;B;W;B;B;W];[B;B;B;W;B;W;B;B;W];[B;B;B;W;W;W;W;W;W];[B;B;B;B;W;W;W;W;W]] }
    { Char = 'R'; Pattern = [[W;W;W;W;W;W;W;W];[W;W;W;W;W;W;W;W];[W;B;B;B;W;B;B;B];[W;B;B;B;W;W;B;B];[W;W;W;W;W;W;W;B];[B;W;W;W;B;B;W;W];[B;B;B;B;B;B;B;W]] }    
    { Char = 'a'; Pattern = [[B;B;B;B;B;W;W;B];[B;B;W;B;W;W;W;W];[B;B;W;B;W;B;B;W];[B;B;W;B;W;B;B;W];[B;B;W;W;W;W;W;W];[B;B;B;W;W;W;W;W]] }
    { Char = 'i'; Pattern = [[W;B;W;W;W;W;W;W];[W;B;W;W;W;W;W;W]] }
    { Char = 's'; Pattern = [[B;B;B;W;W;B;B;W];[B;B;W;W;W;B;B;W];[B;B;W;B;W;W;B;W];[B;B;W;B;B;W;W;W];[B;B;W;B;B;W;W;B]] }
    { Char = 'e'; Pattern = [[B;B;B;W;W;W;W;B];[B;B;W;W;W;W;W;W];[B;B;W;B;W;B;B;W];[B;B;W;B;W;B;B;W];[B;B;W;W;W;B;W;W];[B;B;B;W;W;B;W;B]] }
    { Char = 'T'; Pattern = [[W;B;B;B;B;B;B;B];[W;B;B;B;B;B;B;B];[W;W;W;W;W;W;W;W];[W;W;W;W;W;W;W;W];[W;B;B;B;B;B;B;B];[W;B;B;B;B;B;B;B]] }
    { Char = 'o'; Pattern = [[B;B;B;W;W;W;W;B];[B;B;W;W;W;W;W;W];[B;B;W;B;B;B;B;W];[B;B;W;B;B;B;B;W];[B;B;W;W;W;W;W;W];[B;B;B;W;W;W;W;B]] }
    { Char = 'h'; Pattern = [[W;W;W;W;W;W;W;W;W];[W;W;W;W;W;W;W;W;W];[B;B;B;W;B;B;B;B;B];[B;B;B;W;B;B;B;B;B];[B;B;B;W;W;W;W;W;W];[B;B;B;B;W;W;W;W;W]] }
    { Char = 'e'; Pattern = [[B;B;B;B;W;W;W;W;B];[B;B;B;W;W;W;W;W;W];[B;B;B;W;B;W;B;B;W];[B;B;B;W;B;W;B;B;W];[B;B;B;W;W;W;B;W;W];[B;B;B;B;W;W;B;W;B]] }
    { Char = 'c'; Pattern = [[B;B;B;B;W;W;W;W;B];[B;B;B;W;W;W;W;W;W];[B;B;B;W;B;B;B;B;W];[B;B;B;W;B;B;B;B;W];[B;B;B;W;B;B;B;B;W]] }
    { Char = 'k'; Pattern = [[W;W;W;W;W;W;W;W;W];[W;W;W;W;W;W;W;W;W];[B;B;B;B;W;W;W;B;B];[B;B;B;W;W;B;W;W;B];[B;B;B;W;B;B;B;W;W];[B;B;B;B;B;B;B;B;W]] }
    { Char = 'A'; Pattern = [[B;B;B;B;B;B;W;W;W];[B;B;B;W;W;W;W;W;W];[B;W;W;W;W;W;W;B;B];[B;W;W;B;B;B;W;B;B];[B;W;W;W;W;W;W;B;B];[B;B;B;W;W;W;W;W;W];[B;B;B;B;B;B;W;W;W]] }
    { Char = 'I'; Pattern = [[B;W;B;B;B;B;B;B;W];[B;W;W;W;W;W;W;W;W];[B;W;W;W;W;W;W;W;W];[B;W;B;B;B;B;B;B;W]] }
    { Char = 'n'; Pattern = [[B;B;B;W;W;W;W;W;W];[B;B;B;W;W;W;W;W;W];[B;B;B;W;B;B;B;B;B];[B;B;B;W;B;B;B;B;B];[B;B;B;W;W;W;W;W;W];[B;B;B;B;W;W;W;W;W]] }
  |]

  let getChar patterns bws =
    let samePatterns h p =
      Seq.zip h p
      |> Seq.forall (fun (v1, v2) -> v1 = v2)
    let matchingPattern = 
      patterns 
        |> Array.filter (fun p -> List.length p.Pattern = List.length bws)
        |> Array.filter (fun p -> samePatterns bws p.Pattern)
        |> Array.tryHead
    defaultArg (Option.map (fun p -> p.Char) matchingPattern) '?'


  let lessThanWhite t seq =
    (Seq.filter ((=) W) seq |> Seq.length) >= t

  let removePadding pixels =
      let maxWidth = Array2D.length1 pixels - 1
      let maxHeight = Array2D.length2 pixels - 1
      let firstX = [0..maxWidth] |> Seq.tryFindIndex (fun y -> lessThanWhite 1 pixels.[y, 0..maxHeight])
      let lastX = [0..maxWidth] |> Seq.tryFindIndexBack (fun y -> lessThanWhite 1 pixels.[y, 0..maxHeight])
      let firstY = [0..maxHeight] |> Seq.tryFindIndex (fun x -> lessThanWhite 2 pixels.[0..maxWidth, x])
      let lastY = [0..maxHeight] |> Seq.tryFindIndexBack (fun x -> lessThanWhite 2 pixels.[0..maxWidth, x])

      match (firstX, lastX, firstY, lastY) with
      | (Some fx, Some lx, Some fy, Some ly) -> pixels.[fx..lx, fy..ly]
      | _ -> Array2D.init 0 0 (fun _ _ -> B)

  let isWhite (c : Color) =
    if c.B > 127uy && c.G > 127uy && c.R > 127uy then W
    else B

  let recognizeString (matchSymbol: BW list list -> char) getPixel width height =
    let isSeparator (e : list<BW>) = List.forall ((=) B) e

    let splitIntoSymbols (e : BW list) (state: BW list list list) = 
      match state with
      | cur::rest ->
          if isSeparator e then
            match cur with
            | _::_ -> []::state // add new list
            | _ -> state        // skip if we already have empty item
          else (e::cur)::rest   // add e to current list
      | _ -> [[e]]

    let pixels = 
      Array2D.init width height (fun x y -> isWhite (getPixel x y))
      |> removePadding

    let pixelColumns =
      [0..Array2D.length1 pixels - 1] 
      |> Seq.map (fun x -> pixels.[x, 0..Array2D.length2 pixels - 1] |> List.ofArray)      

    Seq.foldBack splitIntoSymbols pixelColumns []
    |> List.map matchSymbol
    |> Array.ofSeq
    |> String.Concat

  let recognizeNumber x =
    recognizeString (getChar numberPatterns) x

  let recognizeButton x y z =
    let b = recognizeString (getChar buttonPatterns) x y z
    if b <> "?" then b else null

  let parsePattern getPixel width height =
    seq { for x in 0 .. width - 1 do
            yield seq { for y in 0 .. height - 1 do yield isWhite (getPixel x y)} 
        }
    |> Seq.map (fun y -> "[" + (y|> Seq.map (fun x -> if x = B then "B" else "W") |> String.concat ";") + "]")
    |> String.concat ";"
