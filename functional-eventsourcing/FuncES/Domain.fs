module Domain

open System
open NodaTime

type Location = {
  Latitude: float
  Longitude: float
}

type Schedule = {
  Origin: Location
  Destination: Location
  Departure: Instant
  Arrival: Instant
}

type Vehicle = {
  LicensePlate: string
}

type TripEventBody = 
  | NewTripScheduled of Guid * Schedule
  | AssignedToVehicle of Vehicle
  | PositionReceived of Location
  | ArrivedAtOrigin
  | DepartedFromOrigin
  | ArrivalExpectedAt of Instant
  | ArrivedAtDestination

type TripEvent = {
  Moment: Instant
  Event: TripEventBody
}

let eventNow b = { Moment = SystemClock.Instance.Now; Event = b }

type TripStatus = NotActive | HeadingToOrigin | AtOrigin | OnRoute | ArrivedToDestination
 
type Trip = {
  Id: Guid
  Plan: Schedule
  Status: TripStatus
  AssignedTo: Vehicle option
  EstimatedArrival: Instant option
} 

let init id plan =
  { Id = id
    Plan = plan
    Status = NotActive
    AssignedTo = None
    EstimatedArrival = None } 

let apply trip event = 
  match event.Event with
  | NewTripScheduled _ -> failwith "NewTripScheduled can't be applied to existing trip"
  | AssignedToVehicle v -> { trip with AssignedTo = Some v }
  | PositionReceived p when trip.Status = NotActive -> { trip with Status = HeadingToOrigin }
  | PositionReceived _ -> trip
  | ArrivedAtOrigin -> { trip with Status = AtOrigin }
  | DepartedFromOrigin -> { trip with Status = OnRoute }
  | ArrivalExpectedAt t -> { trip with EstimatedArrival = Some t }
  | ArrivedAtDestination -> { trip with Status = ArrivedToDestination; EstimatedArrival = Some event.Moment }

let project = function
  | [{ Event = NewTripScheduled(id, plan) } :: otherEvents] -> 
    (init id plan, otherEvents) ||> List.fold apply 
  | [] -> failwith "Impossible to fold an empty list of events"
  | _ -> failwith "NewTripScheduled should be the first event in the list"

type NewTripCommand = { 
  Plan: Schedule
  Vehicle: Vehicle option
}

type Position = {
  Moment: Instant
  Vehicle: Vehicle
  Location: Location
}

let createTrip command = seq {
  yield NewTripScheduled(Guid.NewGuid(), command.Plan) |> eventNow
  match command.Vehicle with
  | Some v -> yield AssignedToVehicle v |> eventNow
  | None -> ()
}

[<Measure>]type m
type Distance = int<m>

let recordPosition distanceTo timeTo trip command = seq {
  let isInside x = distanceTo command.Location x < 1000<m>
  let makeEvent e = { Moment = command.Moment; Event = e }

  yield PositionReceived(command.Location) |> makeEvent 

  match trip.Status with
  | HeadingToOrigin -> 
    if isInside trip.Plan.Origin then 
      yield makeEvent ArrivedAtOrigin
  | AtOrigin -> 
    if isInside trip.Plan.Origin |> not then 
      yield makeEvent DepartedFromOrigin
  | OnRoute ->
    if isInside trip.Plan.Destination then 
      yield makeEvent ArrivedAtDestination
    else
      let remainingTime = timeTo command.Location trip.Plan.Destination
      yield ArrivalExpectedAt(command.Moment.Plus(remainingTime)) |> makeEvent
  | _ -> ()
}