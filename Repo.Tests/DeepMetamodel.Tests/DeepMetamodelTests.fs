namespace Repo.Tests.DeepMetamodel.Tests

open NUnit.Framework
open FsUnitTyped
open Repo
open Repo.DeepMetamodel
open Repo.DeepMetamodels

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
        model.Elements |> shouldContain (association :> IDeepElement)
        
    [<Test>]
    member this.CreateMetamodelTest () =
        let nodeMetatype = model.CreateNode "elem" 0 1
        let linkMetatype = model.CreateAssociation nodeMetatype nodeMetatype "targetName" 0 1 0 9 0 9
        let newModel = repo.InstantiateModel "newModel" model
        let newNodeA = newModel.InstantiateNode "elem1" nodeMetatype
        let newNodeB = newModel.InstantiateNode "elem2" nodeMetatype
        newModel.InstantiateAssociation newNodeA newNodeB "assoc1" linkMetatype |> ignore
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
        let ``type`` = model.CreateNode "Type" 0 1
        let value = model.InstantiateNode "Value" ``type``
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
        
    [<Test>]
    member this.InstantiateThroughTwoModelsTest () =
        let metaNode = model.CreateNode "metaNode" 0 1
        let testModel1 = repo.InstantiateModel "Model1" model
        let testModel2 = repo.InstantiateModel "Model2" testModel1
        let node = testModel2.InstantiateNode "node" metaNode 
        testModel1.Nodes |> shouldBeEmpty
        testModel2.Nodes |> shouldContain node
        
    [<Test>]
    member this.InstantiateAssociationTest () =
        let metaNode = model.CreateNode "metaNode" 0 1
        let metaAssociation = model.CreateAssociation metaNode metaNode "metaLink" 0 1 0 0 0 0
        let testModel = repo.InstantiateModel "Model1" model
        let testNode1 = testModel.InstantiateNode "node1" metaNode 
        let testNode2 = testModel.InstantiateNode "node2" metaNode 
        let testAssociation = testModel.InstantiateAssociation testNode1 testNode2 "link" metaAssociation 
        testModel.Relationships |> shouldContain (testAssociation :> IDeepRelationship)
        
    [<Test>]
    member this.CheckLevelPotencySimpleInstantiation () =
        let metaNode = model.CreateNode "metaNode" 2 3
        let testModel = repo.InstantiateModel "Model1" model
        let testNode = testModel.InstantiateNode "node1" metaNode
        testNode.Level |> shouldEqual 3
        testNode.Potency |> shouldEqual 2
        
    [<Test>]
    member this.CheckLevelPotencyTwoModelsInstantiation () =
        let metaNode = model.CreateNode "metaNode" 2 3
        let testModel1 = repo.InstantiateModel "Model1" model
        let testModel2 = repo.InstantiateModel "Model2" testModel1
        let testNode = testModel2.InstantiateNode "node1" metaNode
        testNode.Level |> shouldEqual 4
        testNode.Potency |> shouldEqual 2
        
    [<Test>]
    member this.PotencyExceptionIsThrown () =
        let metaNode = model.CreateNode "metaNode" 2 0
        let testModel = repo.InstantiateModel "Model1" model
        Assert.Throws<InstantiationNotAllowedByPotencyException>(fun () -> testModel.InstantiateNode "node1" metaNode |> ignore)
            |> ignore
            
    [<Test>]
    member this.InfiniteLevelPotency () =
        let metaNode = model.CreateNode "metaNode" (-1) (-1)
        let testModel = repo.InstantiateModel "Model1" model
        let testNode = testModel.InstantiateNode "node1" metaNode
        testNode.Level |> shouldEqual (-1)
        testNode.Potency |> shouldEqual (-1)
        
    [<Test>]
    member this.AssociationLevelPotency () =
        let metaNode = model.CreateNode "metaNode" 0 1
        let metaAssociation = model.CreateAssociation metaNode metaNode "metaLink" 0 1 (-1) (-1) (-1) (-1)
        let testModel = repo.InstantiateModel "Model1" model
        let testNode1 = testModel.InstantiateNode "node1" metaNode 
        let testNode2 = testModel.InstantiateNode "node2" metaNode 
        let testAssociation = testModel.InstantiateAssociation testNode1 testNode2 "link" metaAssociation
        testAssociation.Level |> shouldEqual 1
        testAssociation.Potency |> shouldEqual 0
        
    [<Test>]
    member this.AttributesInstantiation () =
        let node = model.CreateNode "Node" 0 1
        let ``type`` = model.CreateNode "Type" 0 1
        node.AddAttribute "attribute" ``type`` 0 1 |> ignore
        
        let newModel = repo.InstantiateModel "newModel" model
        let newNode = newModel.InstantiateNode "newNode" node
        newNode.Attributes |> shouldHaveLength 1
        
    [<Test>]
    member this.IncorrectValueTypeForAttributeIsThrown () =
        let node = model.CreateNode "Node" 0 1
        let ``type`` = model.CreateNode "Type" 0 1
        let value = model.CreateNode "Value" 0 1
        let attribute = node.AddAttribute "attribute" ``type`` 0 1
        Assert.Throws<IncorrectValueTypeForAttribute>(fun () -> node.AddSlot attribute value 0 1 |> ignore)
            |> ignore
            
    [<Test>]
    member this.AttributesSlotsNotElements () =
        let node = model.CreateNode "Node" 0 1
        let ``type`` = model.CreateNode "Type" 0 1
        let value = model.InstantiateNode "Value" ``type``
        let attribute = node.AddAttribute "attribute" ``type`` 0 1
        let slot = node.AddSlot attribute value 0 1
        model.Elements |> shouldHaveLength 3
        
    [<Test>]
    member this.AvailableValues () =
        let node = model.CreateNode "Node" 0 1
        let type1 = model.CreateNode "Type" 0 1
        let type2 = model.CreateNode "Type2" 0 1
        let attribute = node.AddAttribute "attr" type1 0 1
        
        let newModel = repo.InstantiateModel "newModel" model
        let newNode = newModel.InstantiateNode "newNode" node
        let value1 = newModel.InstantiateNode "value1" type1
        let value2 = newModel.InstantiateNode "value1" type2
        
        let newAttribute = Seq.find (fun e -> (e :> IDeepAttribute).Name = attribute.Name) newNode.Attributes
        let availableAttributes = newNode.GetValuesForAttribute newAttribute
        availableAttributes |> shouldContain (value1 :> IDeepElement)
        availableAttributes |> shouldNotContain (value2 :> IDeepElement)
        
    [<Test>]
    member this.SingleAttributeException () =
        let node = model.CreateNode "Node" 0 1
        let ``type`` = model.CreateNode "Type" 0 1
        let value = model.InstantiateNode "Value" ``type``
        let attribute = node.AddAttribute "attribute" ``type`` 0 1
        attribute.IsSingle <- true
        Assert.Throws<SingleAttributeSlotCreatingException>(fun () -> node.AddSlot attribute value 0 1 |> ignore)
            |> ignore
        
    [<Test>]
    member this.BuildAtkinson () =
        let atkinsonModelBuilder = AtkinsonModelBuilder () :> IDeepModelBuilder
        atkinsonModelBuilder.Build repo
        ()
        
    [<Test>]
    member this.BuildRobots () =
        let robotsModelBuilder = RobotsSubroutineModelsBuilder () :> IDeepModelBuilder
        robotsModelBuilder.Build repo
        ()
        
 