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
    let flatmap (map: 's -> IObservable<Result<'r>>) (source: IObservable<Result<'s>>): IObservable<Result<'r>> =
        ___
    
    let toArray (source: IObservable<Result<'b>>): IObservable<Result<'b[]>> =
        source
        |> Observable.merge (fun (a, b) -> a &&& b)
    
    let multiFlatmap (f: 'a -> IObservable<Result<'b>>) (observable: IObservable<Result<'a[]>>): IObservable<Result<'b[]>> =
        observable
        |> flatmap (Array.map f >> Observable.mergeArray)
        |> toArray
