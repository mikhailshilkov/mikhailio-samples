namespace Interaction

open System
open System.Windows.Forms
open WindowsInput

module Clicker =

  let simulator = new InputSimulator()
  let currentPosition () = 
    let mp = Control.MousePosition;
    (mp.X, mp.Y)

  let moveTo x y =
    let toX = 65535. * x / (Screen.PrimaryScreen.Bounds.Width |> float)
    let toY = 65535. * y / (Screen.PrimaryScreen.Bounds.Height |> float)
    simulator.Mouse.MoveMouseTo(toX, toY)

  let linearStep from until max i =
    from + (until - from) * i / max

  let sinStep (from:int) (until:int) (max:int) (index:int) =
    let fromf = from |> float
    let untilf = until |> float
    let maxf = max |> float
    let indexf = index |> float
    fromf + (untilf - fromf) * Math.Sin(Math.PI / 2. * indexf / maxf) |> int

  let moveToWorkflow step (toX, toY) = async {
    let (fromX, fromY) = currentPosition()
    let count = Math.Max(10, (Math.Abs (toX - fromX) + Math.Abs (toY - fromY)) / 20)
    for i = 0 to count do
      let x = step fromX toX count i |> float
      let y = step fromY toY count i |> float
      moveTo x y
      do! Async.Sleep 3
    }

  let clickButton (minX, minY, maxX, maxY) =
    let r = new Random()
    let p = (r.Next(minX, maxX), r.Next(minY, maxY))
    moveToWorkflow sinStep p |> Async.RunSynchronously
    simulator.Mouse.LeftButtonClick()