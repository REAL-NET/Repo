namespace Repo.Tests.DeepMetamodel.Tests

open NUnit.Framework
open FsUnitTyped
open Repo.DeepMetamodel
open Repo.DeepMetamodels

[<TestFixture>]
type QueryMetamodelTests() =

    let mutable repo = DeepMetamodelRepoFactory.Create ()
    let mutable metamodel = repo.InstantiateDeepMetamodel "TestMetamodel"

    [<SetUp>]
    member this.SetUp () =
        repo <- DeepMetamodelRepoFactory.Create ()
        let queryModelBuilder = QueryModelBuilder () :> IDeepModelBuilder
        queryModelBuilder.Build repo
        metamodel <- repo.Model "QueryMetamodel"

    [<Test>]
    member this.Build () =
        let model = repo.Model "QueryModel"
        ()

    [<Test>]
    member this.NodesExist () =
        let model = repo.Model "QueryModel"
        model.Nodes |> shouldHaveLength 3
        ()

    [<Test>]
    member this.RelationshipsExist () =
        let model = repo.Model "QueryModel"
        model.Relationships |> shouldHaveLength 2
        ()

    [<Test>]
    member this.AttributesExist () =
        let model = repo.Model "QueryModel"
        let holdsNode = model.Node "HOLDS_1"
        let materializeNode = model.Node "Materialize_1"
        let dsNode = model.Node "DS_1"
        holdsNode.Attributes |> shouldHaveLength 5
        materializeNode.Attributes |> shouldHaveLength 5
        dsNode.Attributes |> shouldHaveLength 5
        ()

    [<Test>]
    member this.SlotsExist () =
        let model = repo.Model "QueryModel"
        let holdsNode = model.Node "HOLDS_1"
        let materializeNode = model.Node "Materialize_1"
        let dsNode = model.Node "DS_1"
        holdsNode.Slots |> shouldHaveLength 5
        materializeNode.Slots |> shouldHaveLength 5
        dsNode.Slots |> shouldHaveLength 5
        ()