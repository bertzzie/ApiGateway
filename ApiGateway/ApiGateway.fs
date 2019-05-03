module ApiGateway

open Rails
open Profile

open Suave
open Suave.Successful
open Suave.Operators
open Suave.RequestErrors

open Newtonsoft.Json
open Newtonsoft.Json.Serialization

let JSON v =
    let settings = new JsonSerializerSettings()
    settings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
    
    JsonConvert.SerializeObject(v, settings)
    |> OK
    >=> Writers.setMimeType "application/json; charset=utf-8"
    
let getProfile userName (httpContext: HttpContext) = async {
    let! profile = getProfile userName
    
    match profile with
    | Success p -> return! JSON p httpContext
    | Failure _ ->
        let message = sprintf "Username %s not found" userName
        return! NOT_FOUND message httpContext
}
