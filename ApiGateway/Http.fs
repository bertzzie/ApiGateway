module Http

open Hopac
open Rails
open HttpFs.Client
open FSharp.Control.Reactive

type HttpResponse =
    | OK of string
    | Error of int
    
let private extractHttpResponse response =
    match response.statusCode with
    | 200 -> response |> Response.readBodyAsString |> Job.map Success
    | _   -> response.statusCode.ToString() |> Failure |> Job.result

let getResponseAsync url = async {
    let! response =
        Request.create Get (new System.Uri(url))
        |> Request.setHeader (UserAgent "FSharpRx")
        |> getResponse
        |> Alt.afterJob extractHttpResponse
        |> Alt.toAsync
        
    return response
 }

let asyncResponseToObservable = getResponseAsync >> Observable.ofAsync
