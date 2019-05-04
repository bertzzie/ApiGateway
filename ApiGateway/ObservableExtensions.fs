module ObservableExtensions

open System
open Rails
open FSharp.Control.Reactive

// flatmap multiple data from stream, and merges the result
let multiFlatmap f observable =
    observable
    |> Observable.flatmap (Array.map f >> Observable.mergeArray)
    |> Observable.toArray
    
module Rails =
    let private fmHelper source mapper =
        match source with
        | Success s -> mapper s
        | Failure f -> Observable.single (Failure f)
    
    let flatmap (map: 's -> IObservable<Result<'r>>) (source: IObservable<Result<'s>>): IObservable<Result<'r>> =
        source
        |> Observable.flatmap (fun s -> fmHelper s map)
        
    
    let toArray (source: IObservable<Result<'b>>): IObservable<Result<'b[]>> =
        let mapper (a: Result<'b>): Result<'b[]> =
            match a with
            | Success s -> Success [|s|]
            | Failure f -> Failure f
        
        let accumulator a b =
            match a, b with
            | Success s1, Success s2 -> Success(Array.append s1 s2)
            | Failure f1, Success _  -> Failure f1
            | Success  _, Failure f2 -> Failure f2
            | Failure f1, Failure f2 -> Failure(sprintf "%s;%s" f1 f2)
    
        source
        |> Observable.map mapper
        |> Observable.reduce accumulator
    
    let multiFlatmap (f: 'a -> IObservable<Result<'b>>) (observable: IObservable<Result<'a[]>>): IObservable<Result<'b[]>> =
        observable
        |> flatmap (Array.map f >> Observable.mergeArray)
        |> toArray
