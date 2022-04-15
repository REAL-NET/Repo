module QueryModelTests

open NUnit.Framework
open FsUnit

open Repo

let repo =
    RepoFactory.Create ()

[<Test>]
let ``Metamodel and model should exist`` () =
    let metamodel = repo.Model "QueryMetamodel"
    let model = repo.Model "QueryModel"
    model.Metamodel |> should equal metamodel

[<Test>]
let ``Model should have 7 nodes`` () =
    let model = repo.Model "QueryModel"
    model.Metamodel.Nodes |> Seq.length |> should equal 7

[<Test>]
let ``Model should have 4 edges`` () =
    let model = repo.Model "QueryModel"
    model.Metamodel.Edges |> Seq.length |> should equal 4
