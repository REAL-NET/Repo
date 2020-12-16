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

namespace Repo.InfrastructureMetamodel.Details.Elements

open Repo.InfrastructureMetamodel
open Repo.LanguageMetamodel

/// Implementation of Element.
[<AbstractClass>]
type InfrastructureElement(element: ILanguageElement, pool: InfrastructurePool, repo: ILanguageRepository) =

    let languageMetamodel =
        repo.Model Repo.InfrastructureMetamodel.Consts.infrastructureMetamodel
    
    let attributesAssociationMetatype =
        languageMetamodel.Association Consts.attributesEdge
        
    let slotsAssociationMetatype =
        languageMetamodel.Association Consts.slotsEdge
    
    let wrap = pool.Wrap 
    
    /// Returns underlying CoreElement.
    member this.UnderlyingElement = element

    interface IInfrastructureElement with
        
        member this.OutgoingEdges =
            element.OutgoingEdges
            |> Seq.map pool.Wrap
            |> Seq.cast<IInfrastructureEdge>
        
        member this.OutgoingAssociations = 
            element.OutgoingAssociations
            |> Seq.map pool.Wrap
            |> Seq.cast<IInfrastructureAssociation>

        member this.IncomingAssociations =
            element.IncomingAssociations
            |> Seq.map pool.Wrap
            |> Seq.cast<IInfrastructureAssociation>

        member this.DirectSupertypes =
            element.OutgoingEdges
            |> Seq.filter (fun e -> e :? IInfrastructureGeneralization)
            |> Seq.map (fun e -> e.Target)
            |> Seq.map wrap
          

        member this.Attributes =
             element.Attributes
             |> Seq.map pool.WrapAttribute

        member this.Slots =
            element.Slots
            |> Seq.map pool.WrapSlot

        member this.Model: IInfrastructureModel =
            pool.WrapModel element.Model

        member this.Metatype =
            pool.Wrap element.Metatype
