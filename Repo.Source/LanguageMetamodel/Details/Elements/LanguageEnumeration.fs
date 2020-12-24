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

namespace Repo.LanguageMetamodel.Details.Elements

open Repo
open Repo.LanguageMetamodel
open Repo.AttributeMetamodel

/// Implementation of Enumeration.
type LanguageEnumeration(node: IAttributeNode, pool: LanguagePool, repo: IAttributeRepository) =
    inherit LanguageElement(node, pool, repo)
    
        
    let model = node.Model
    
    let languageMetamodel =
        repo.Model Consts.languageMetamodel
        
    let enumMetatype = languageMetamodel.Node Consts.enumeration

    let enumElementsAssociation =
        (languageMetamodel.Node LanguageMetamodel.Consts.enumeration).OutgoingAssociation
            LanguageMetamodel.Consts.elementsEdge

    interface ILanguageEnumeration with
        member this.Name
            with get () = node.Name
            and set v = node.Name <- v

        member this.AddElement name =
            let attributeNode = model.InstantiateNode name enumMetatype Map.empty     
            let association = model.InstantiateAssociation node attributeNode enumElementsAssociation Map.empty
            attributeNode |> pool.Wrap |> ignore
            association |> pool.Wrap |> ignore
            
        member this.Elements =
            node.OutgoingAssociations
            |> Seq.filter (fun a -> a.Metatype = (enumElementsAssociation :> IAttributeElement))
            |> Seq.map (fun a -> a.Target)
            |> Seq.map pool.Wrap
            |> Seq.cast<ILanguageNode>
