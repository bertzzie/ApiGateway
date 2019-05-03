module Rails

type Result<'Entity> =
    | Success of 'Entity
    | Failure of string
    
let bind switch input =
    match input with
    | Success s -> switch s
    | Failure f -> Failure f
    
let map input =
    bind (input >> Success)
    
let plus addSuccess addFailure switch1 switch2 x =
    match (switch1 x), (switch2 x) with
    | Success s1, Success s2 -> Success(addSuccess s1 s2)
    | Failure f1, Success _  -> Failure f1
    | Success  _, Failure f2 -> Failure f2
    | Failure f1, Failure f2 -> Failure(addFailure f1 f2)
    
let (&&&) v1 v2 =
    let addSuccess s1 s2 = [|s1; s2|]
    let addFailure f1 f2 = sprintf "%s;%s" f1 f2
    
    plus addSuccess addFailure v1 v2
    
let (>>=) input switch =
    bind switch input

