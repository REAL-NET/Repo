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
        let a = model.CreateNode "a" 0 1
        let b = model.CreateNode "b" 0 1 
        model.Nodes |> shouldHaveLength 2
        model.Elements |> shouldHaveLength 2
        model.Elements |> shouldContain (a :> IDeepElement)
        model.Elements |> shouldContain (b :> IDeepElement)
        
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
        
    [<Test>]
    member this.CreateMetamodelTest () =
        let nodeMetatype = model.CreateNode "elem" 0 1
        let linkMetatype = model.CreateAssociation nodeMetatype nodeMetatype "targetName" 0 0 0 9 0 9
        let newModel = repo.InstantiateModel "newModel" model
        let newNodeA = newModel.InstantiateNode "elem1" nodeMetatype 0 1
        let newNodeB = newModel.InstantiateNode "elem2" nodeMetatype 0 1
        newModel.InstantiateAssociation newNodeA newNodeB "assoc1" linkMetatype 0 0 0 9 0 9 |> ignore
        newModel.Node "elem1" |> shouldEqual newNodeA
        (newModel.Node "elem1").Metatype |> shouldEqual (nodeMetatype :> IDeepElement)
        newModel.Relationships |> shouldHaveLength 1
        
    [<Test>]
    member this.AttributesTest () =
        let node = model.CreateNode "Node" 0 1
        let ``type`` = model.CreateNode "Type" 0 1
        let attribute = node.AddAttribute "attribute" ``type`` 0 1
        
        node.Attributes |> shouldHaveLength 1
        node.Attributes |> shouldContain attribute
        attribute.Type |> shouldEqual (``type`` :> IDeepElement)
        attribute.Name |> shouldEqual "attribute"
        
    [<Test>]
    member this.SlotsTest () =
        let node = model.CreateNode "Node" 0 1
        let value = model.CreateNode "Value" 0 1
        let ``type`` = model.CreateNode "Type" 0 1
        let attribute = node.AddAttribute "attribute" ``type`` 0 1
        let slot = node.AddSlot attribute value 0 1
        
        node.Attributes |> shouldHaveLength 1
        node.Attributes |> shouldContain attribute
        node.Slots |> shouldHaveLength 1
        node.Slots |> shouldContain slot
        slot.Attribute |> shouldEqual attribute
        slot.Value |> shouldEqual (value :> IDeepElement)
        
    [<Test>]
    member this.InstantiateModelTest () =
        let testModel = repo.InstantiateModel "TestModel" model
        repo.Models |> shouldContain testModel
            
        
