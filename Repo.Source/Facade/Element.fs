(* Copyright 2017 Yurii Litvinov
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

namespace Repo.Facade

open Repo
open Repo.InfrastructureMetamodel
open Repo.InfrastructureMetamodel.Details.Elements

/// Implementation of an element wrapper.
[<AbstractClass>]
type Element(element: IInfrastructureElement, pool: FacadePool, repo: IInfrastructureRepository) =

    //let infrastructureMetamodel = repo.Model InfrastructureMetamodel.Consts.infrastructureMetamodel
    //let infrastructureMetamodelHelper = 
    //    InfrastructureMetamodel.Semantics.InfrastructureMetamodelHelper infrastructureMetamodel
    //let elementSemantics = Repo.AttributeMetamodel.Semantics.ElementSemantics(infrastructureMetamodel)

    //let findMetatype (element : DataLayer.IDataElement) =
    //    if instantiationSemantics.Metamodel.IsNode element then
    //        Metatype.Node
    //    elif instantiationSemantics.Metamodel.IsEdge element then
    //        Metatype.Edge
    //    else
    //        raise (InvalidSemanticOperationException
    //            "Trying to get a metatype of an element that is not instance of the Element. Model is malformed.")

    //let attributeType (kind: AttributeKind) =
    //    match kind with
    //    | AttributeKind.Boolean -> infrastructureMetamodel.Node "Boolean"
    //    | AttributeKind.Double -> infrastructureMetamodel.Node "Double"
    //    | AttributeKind.Int -> infrastructureMetamodel.Node "Int"
    //    | AttributeKind.String -> infrastructureMetamodel.Node "String"
    //    |_ -> raise (new System.NotSupportedException ())

    /// Returns corresponding element from data repository.
    member this.UnderlyingElement = element

    interface IElement with
        member this.Name
            with get (): string =
                match element with
                | :? InfrastructureNode as n -> (n :> IInfrastructureNode).Name 
                | :? InfrastructureEdge as e -> invalidOp "Edges don't have names."
                | _ -> invalidOp "Unknown type, it's no edge and node either."

            and set (v: string): unit =
                match element with
                | :? InfrastructureNode as n -> (n :> IInfrastructureNode).Name <- v
                | :? InfrastructureEdge as e -> invalidOp "Edges don't have names."
                | _ -> invalidOp "Unknown type, it's no edge and node either."

        member this.Attributes = 
            failwith "not implemented"

        member this.Class =
            failwith "Not implemented"
            //repository.GetElement element.LinguisticType

        member this.IsAbstract =
            failwith "Not implemented"
            //if infrastructureMetamodelHelper.IsElement element then
            //    if infrastructureMetamodelHelper.IsGeneralization element then
            //        true
            //    else
            //        match elementSemantics.StringSlotValue "isAbstract" element with
            //        | "true" -> true
            //        | "false" -> false
            //        | _ -> failwith "Incorrect isAbstract attribute value"
            //else
            //    true

        member this.Shape = 
            failwith "Not implemented"
            //elementSemantics.StringSlotValue "shape" element

        member this.Metatype = 
            failwith "Not implemented"
            //findMetatype element

        member this.InstanceMetatype =
            failwith "Not implemented"
            //match elementSemantics.StringSlotValue "instanceMetatype" element with
            //| "Metatype.Node" -> Metatype.Node
            //| "Metatype.Edge" -> Metatype.Edge
            //| _ -> failwith "Incorrect instanceMetatype attribute value"

        member this.AddAttribute (name, kind, defaultValue) =
            failwith "Not implemented"
            //let attributeType = attributeType kind
            //let defaultValueNode = 
            //    instantiationSemantics.InstantiateElement element.Model defaultValue attributeType Map.empty 
            //    :?> DataLayer.IDataNode
            //instantiationSemantics.Element.AddAttribute element name attributeType defaultValueNode
            //()
