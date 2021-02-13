namespace Repo.Tests.DeepMetamodel.Tests

open NUnit.Framework
open FsUnitTyped
open Repo.DeepMetamodel

[<TestFixture>]
type DeepMetamodelTests() =

    let mutable repo = DeepMetamodelRepoFactory.Create ()
    let mutable model = repo.InstantiateDeepMetamodel "TestModel"
    
    [<SetUp>]
    member this.SetUp () =
        repo <- DeepMetamodelRepoFactory.Create ()
        model <- repo.InstantiateDeepMetamodel "TestModel"

    [<Test>]
    member this.CreateNodeTest () =
        model.CreateNode "a" 0 1 |> ignore
        model.CreateNode "b" 0 1 |> ignore
        model.Nodes |> shouldHaveLength 2
        model.Elements |> shouldHaveLength 2
        
    [<Test>]
    member this.CreateAssociationTest () =
        let aElement = model.CreateNode "a" 0 1 
        let bElement = model.CreateNode "b" 0 1
        model.CreateAssociation aElement bElement "targetName" 0 0 0 9 0 9 |> ignore
        model.Relationships |> shouldHaveLength 1
        
    [<Test>]
    member this.GetNodesTest () =
        model.CreateNode "a" 0 1 |> ignore
        model.Node "a" |> ignore
        
    [<Test>]
    member this.HasNodeTest () =
        model.CreateNode "a" 0 1 |> ignore
        model.HasNode "a" |> shouldEqual true
        model.HasNode "b" |> shouldEqual false
        
    [<Test>]
    member this.AssociationTest () =
        let nodeA = model.CreateNode "a" 0 1
        let nodeB = model.CreateNode "b" 0 1
        let association = model.CreateAssociation nodeA nodeB "targetName" 0 0 0 9 0 9
        model.Relationships |> shouldHaveLength 1
        association.Source |> shouldEqual (nodeA :> IDeepElement)
        association.Target |> shouldEqual (nodeB :> IDeepElement)

    [<Test>]
    member this.GetAssociationTest () =
        let nodeA = model.CreateNode "a" 0 1
        let nodeB = model.CreateNode "b" 0 1
        let association = model.CreateAssociation nodeA nodeB "targetName" 0 0 0 9 0 9
        model.Association "targetName" |> shouldEqual association
