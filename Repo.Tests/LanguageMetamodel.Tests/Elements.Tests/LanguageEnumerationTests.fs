(* Copyright 2019 REAL.NET group
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License. *)

namespace Repo.AttributeMetamodel.Elements.Tests

open Repo
open Repo.LanguageMetamodel

open NUnit.Framework
open FsUnitTyped
open Repo.LanguageMetamodel.Details.Elements

[<TestFixture>]
type LanguageEnumerationTests() =
    

    let mutable repo = LanguageMetamodelRepoFactory.Create ()
    let mutable model = repo.InstantiateLanguageMetamodel "TestModel"
    
    let (~+) name = model.CreateNode name

    let (--->) (node1: ILanguageElement) (node2: ILanguageElement) =
        model.CreateAssociation node1 node2 "testEdge"

    let (--|>) (node1: ILanguageElement) (node2: ILanguageElement) =
        model.CreateGeneralization node1 node2 |> ignore

    [<SetUp>]
    member this.Setup () =
        repo <- LanguageMetamodelRepoFactory.Create ()
        model <- repo.InstantiateLanguageMetamodel "TestModel"

    [<Test>]
    member this.EnumerationElementsTest () =
        let enum = model.CreateEnumeration "TestEnum" ["true"; "false"]
        enum.Elements |> shouldHaveLength 2
        enum.Elements |> Seq.filter (fun n -> n.Name = "true") |> shouldHaveLength 1
        enum.Elements |> Seq.filter (fun n -> n.Name = "false") |> shouldHaveLength 1
        
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
        let element = (node :?> LanguageElement).UnderlyingElement
        element.Model.Nodes |> shouldHaveLength 3
        
        element.OutgoingAssociations |> shouldHaveLength 1

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
