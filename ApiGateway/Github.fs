module Github

open Rails
open FSharp.Data

type GithubUser = JsonProvider<"user.json">
type GithubUserRepos = JsonProvider<"repos.json">

type Profile = {
    Name: string
    AvatarUrl: string
    PopularRepositories: Repository seq
} and Repository = {
    Name: string
    Stars: int
    Languages: string[]
}

let parseUser = GithubUser.Parse
let parseUserRepos = GithubUserRepos.Parse
let parseLanguages languagesJson =
    languagesJson
    |> JsonValue.Parse
    |> JsonExtensions.Properties
    |> Array.map fst

let host = "https://api.github.com"
let userUrl = sprintf "%s/users/%s" host
let reposUrl = sprintf "%s/users/%s/repos" host
let languagesUrl repoName userName =
    sprintf "%s/repos/%s/%s/languages" host userName repoName

let popularRepos (repos: GithubUserRepos.Root []) =
    let ownRepos = repos |> Array.filter (fun repo -> not repo.Fork)
    let takeCount = if ownRepos.Length > 3 then 3 else repos.Length
    
    ownRepos
    |> Array.sortBy (fun r -> -r.StargazersCount)
    |> Array.take takeCount

let reposResponseToPopularRepos = function
    | Success(r) -> r |> parseUserRepos |> popularRepos |> Success
    | _          -> Failure "Error Processing Popular Repo"
    
let languageResponseToRepoWithLanguages (repo: GithubUserRepos.Root) = function
    | Success(l) -> { Name = repo.Name; Stars = repo.StargazersCount; Languages = (parseLanguages l) } |> Success
    | _          -> Failure "Error Processing Language"
    
let toProfile = function
    | Success(u), Success(repos) ->
        let user = parseUser u
        { Name = user.Name; PopularRepositories = repos; AvatarUrl = user.AvatarUrl } |> Success
        
    | _ -> Failure "Error Processing Profile"