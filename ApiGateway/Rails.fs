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
    
let (>>=) input switch =
    bind switch input

