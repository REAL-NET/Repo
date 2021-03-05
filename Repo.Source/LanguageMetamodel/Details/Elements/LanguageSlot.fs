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

open Repo.LanguageMetamodel
open Repo.AttributeMetamodel

/// Implementation of Slot in Language metamodel.
type LanguageSlot(node: IAttributeElement, pool: LanguagePool, repo: IAttributeRepository) =
    
    let languageMetamodel = repo.Model Consts.languageMetamodel
    let valueAssociation = (languageMetamodel.Node Consts.slot).OutgoingAssociation Repo.LanguageMetamodel.Consts.valueEdge

    let unwrap (element: ILanguageElement) = (element :?> LanguageElement).UnderlyingElement
    
    interface ILanguageSlot with
        member this.Attribute =
            (node.OutgoingAssociation Repo.LanguageMetamodel.Consts.attributeEdge).Target
            |> pool.WrapAttribute

        /// Returns a node that represents type of an attribute.
        member this.Value
            with get () =
                (node.OutgoingAssociation Repo.LanguageMetamodel.Consts.valueEdge).Target
                |> pool.Wrap
            and set v =
                let oldValue = (node.OutgoingAssociation Repo.LanguageMetamodel.Consts.valueEdge).Target
                oldValue.Model.DeleteElement oldValue
                node.Model.InstantiateAssociation node (unwrap v) valueAssociation |> ignore

