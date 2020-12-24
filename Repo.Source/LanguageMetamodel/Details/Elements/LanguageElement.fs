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

open System
open Repo
open Repo.CoreMetamodel
open Repo.LanguageMetamodel
open Repo.AttributeMetamodel

/// Implementation of Element.
[<AbstractClass>]
type LanguageElement(element: IAttributeElement, pool: LanguagePool, repo: IAttributeRepository) =

    let languageMetamodel =
        repo.Model Repo.LanguageMetamodel.Consts.languageMetamodel

    let languageAssociation =
        languageMetamodel.Node Repo.LanguageMetamodel.Consts.association

    let attributeMetamodel = repo.Model Consts.attributeMetamodel

    let attributeMetatype = attributeMetamodel.Node Consts.attribute

    let attributesAssociationMetatype =
        attributeMetamodel.Association Consts.attributesEdge

    let slotsAssociationMetatype =
        attributeMetamodel.Association Consts.slotsEdge

    let typeAssociationMetatype =
        attributeMetamodel.Association Consts.typeEdge

    let slotsAssociationMetatype =
        attributeMetamodel.Association Consts.slotsEdge


    let wrap = pool.Wrap

    let model () = element.Model

    let mutable AttributeMap: Map<string, IAttributeElement> = Map.empty

    let unwrap (element: ILanguageElement) =
        (element :?> LanguageElement).UnderlyingElement

    /// Instantiation of association with given metatype.
    let (--->) source (target, metatype) =
        (model ()).InstantiateAssociation source target metatype
        |> ignore


    /// Returns underlying AttributeElement.
    member this.UnderlyingElement = element

    override this.ToString() =
        match element with
        | :? ILanguageNode as n -> n.Name
        | :? ILanguageAssociation as a -> a.TargetName
        | :? ILanguageGeneralization -> "generalization"
        | _ -> "unknown"

    interface ILanguageElement with
        member this.OutgoingEdges =
            element.OutgoingEdges
            |> Seq.map pool.Wrap
            |> Seq.cast<ILanguageEdge>

        member this.OutgoingAssociations =
            element.OutgoingAssociations
            |> Seq.map pool.Wrap
            |> Seq.cast<ILanguageAssociation>

        member this.OutgoingAssociation name =
            element.OutgoingAssociation name |> pool.Wrap :?> ILanguageAssociation

        member this.IncomingAssociations =
            element.IncomingAssociations
            |> Seq.map pool.Wrap
            |> Seq.cast<ILanguageAssociation>

        member this.DirectSupertypes =
            element.OutgoingEdges
            |> Seq.filter (fun e -> e :? IAttributeGeneralization)
            |> Seq.map (fun e -> e.Target)
            |> Seq.map wrap

        member this.Attributes =
            element.Attributes |> Seq.map (pool.WrapAttribute)

        member this.AddAttribute name ``type`` =
            element.AddAttribute name (unwrap ``type``)

        member this.Slots = element.Slots |> Seq.map pool.WrapSlot

        member this.Slot name =
            let slots = (this :> ILanguageElement).Slots
            slots
            |> Seq.filter (fun s -> s.Attribute.Name = name)
            |> Helpers.exactlyOneElement name

        member this.Model: ILanguageModel = pool.WrapModel element.Model

        member this.Metatype = pool.Wrap element.Metatype
