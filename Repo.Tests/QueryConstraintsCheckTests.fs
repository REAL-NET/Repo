module QueryConstraintsCheckTests

open NUnit.Framework
open FsUnit

open Repo

let repo =
    RepoFactory.Create ()

[<Test>]
let ``Metamodel and model exist`` () =
    let metamodel = repo.Model "QueryMetamodel"
    let model = repo.Model "QueryModel"
    model.Metamodel |> should equal metamodel
