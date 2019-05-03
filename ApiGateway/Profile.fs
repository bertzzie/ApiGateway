module Profile

open FSharp.Control.Reactive
open System.Reactive.Threading.Tasks

open Http
open Rails
open Github
open ObservableExtensions

let getProfile userName = async {
    
    let userStream =
        userName
        |> userUrl
        |> asyncResponseToObservable
        
    let toRepoWithLanguagesStream (repo: GithubUserRepos.Root) =
        userName
        |> languagesUrl repo.Name
        |> asyncResponseToObservable
        |> Observable.map (languageResponseToRepoWithLanguages repo)
        
    let popularReposStream =
        userName
        |> reposUrl
        |> asyncResponseToObservable
        |> Observable.map reposResponseToPopularRepos
        |> ObservableExtensions.Rails.multiFlatmap toRepoWithLanguagesStream
        
    return! popularReposStream
        |> Observable.zip userStream
        |> Observable.map toProfile
        |> TaskObservableExtensions.ToTask
        |> Async.AwaitTask
}


