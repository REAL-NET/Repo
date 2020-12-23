namespace Repo.LanguageMetamodel.Elements.Tests

open NUnit.Framework
open FsUnitTyped
open Repo
open Repo.LanguageMetamodel

type ElementType = ILanguageElement

[<TestFixture>]
type LanguageElementTests() =
    
    let mutable repo = LanguageMetamodelRepoFactory.Create ()
    let mutable model = repo.InstantiateLanguageMetamodel "TestModel"
    
    let (~+) name = model.CreateNode name

    let (--->) (node1: ElementType) (node2: ElementType) =
        model.CreateAssociation node1 node2 "testEdge"

    let (--|>) (node1: ElementType) (node2: ElementType) =
        model.CreateGeneralization node1 node2 |> ignore
        
    [<SetUp>]
    member this.Setup () =
        repo <- LanguageMetamodelRepoFactory.Create ()
        model <- repo.InstantiateLanguageMetamodel "TestModel"        
    
    [<Test>]
    member this.OutgoingAssociationsTest () =
        let node1 = +"Node1"
        let node2 = +"Node2"
        let edge = node1 ---> node2
        node1.OutgoingAssociations |> shouldContain edge
        node1.OutgoingAssociations |> shouldHaveLength 1
        ()
        
    [<Test>]
    member this.AttributesTest () =
        let node = +"Node"
        let ``type`` = +"Type"
        node.AddAttribute "attribute" ``type`` 
        
        node.Attributes |> Seq.filter (fun a -> a.Name = "attribute") |> shouldHaveLength 1
        
    [<Test>]
    member this.AddingTwoAttributesWithTheSameNameAreNotAllowedTest () =
        let node = +"Node"
        let ``type`` = +"Type"
        
        node.AddAttribute "attribute" ``type``
        (fun () -> node.AddAttribute "attribute" ``type``) |> shouldFail<AmbiguousAttributesException>

    [<Test>]
    member this.AttributesRespectGeneralizationTest () =
        let parent = +"Parent"
        let ``type`` = +"Type"
            
        parent.AddAttribute "attribute" ``type``

        let child = +"Child"
        child --|> parent

        child.Attributes |> Seq.filter (fun a -> a.Name = "attribute") |> shouldHaveLength 1
        
    [<Test>]
    member this.SlotsTest () =
        let node = +"Node"
        let ``type`` = +"Type"
        
        node.AddAttribute "attribute" ``type``

        let instanceModel = repo.InstantiateModel "InstanceModel" model

        let value = instanceModel.InstantiateNode "Value" ``type`` Map.empty :> ElementType
        let instance = instanceModel.InstantiateNode "Instance" node (Map.empty.Add("attribute", value))
        
        instance.Slots |> shouldHaveLength 1

        (instance.Slot "attribute").Value |> shouldEqual value
        (instance.Slot "attribute").Attribute |> shouldEqual (Seq.head node.Attributes)
        
    [<Test>]    
    member this.EnumerationTest () =
        let valuesInsideEnum = Seq.ofList[ "1"; "2" ]
        let enumeration = model.CreateEnumeration "1 and 2" valuesInsideEnum
        enumeration.Name |> shouldEqual "1 and 2"
   
