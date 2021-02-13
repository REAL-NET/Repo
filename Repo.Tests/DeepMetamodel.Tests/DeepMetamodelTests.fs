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
        model.Edges |> shouldHaveLength 1
        
    [<Test>]
    member this.GetNodesTest () =
        let node = model.CreateNode "a" 0 1
        model.Node "a" |> ignore
        
        
        
        
        
    

