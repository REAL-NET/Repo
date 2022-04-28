(* Copyright 2017-2018 REAL.NET group
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

namespace Repo.Metametamodels

open Repo
open Repo.DataLayer
open Repo.CoreSemanticLayer
open Repo.InfrastructureSemanticLayer

/// Initializes repository with test model conforming to Query Metamodel, actual program that can be written by end-user.
type QueryModelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =
            let infrastructure = InfrastructureSemantic(repo)
            let metamodel = Repo.findModel repo "QueryMetamodel"

            let metamodelAbstractQueryBlock = Model.findNode metamodel "AbstractQueryBlock"
            let metamodelMaterializationLine = Model.findNode metamodel "MaterializationLine"

            let findNodeWithAttribute nodes attributeName attributeValue =
                let node = nodes |> Seq.filter (fun m -> infrastructure.Element.AttributeValue m attributeName = attributeValue)
                if Seq.isEmpty node then
                    raise (InvalidSemanticOperationException <| sprintf "Node with attribute %s value %s not found" attributeName attributeValue)
                elif Seq.length node <> 1 then
                    raise (InvalidSemanticOperationException
                        <| sprintf "Node with attribute %s value %s appears more than once" attributeName attributeValue)
                else
                    Seq.head node

            let metamodelSorts = Model.findNodes metamodel "Sort"
            let metamodelSort = findNodeWithAttribute metamodelSorts "type" "tuple"
            let metamodelAggregate = Model.findNode metamodel "Aggregate"
            let metamodelJoins = Model.findNodes metamodel "Join"
            let metamodelJoin = findNodeWithAttribute metamodelJoins "type" "positional"
            let metamodelRead = Model.findNode metamodel "Read"
            let metamodelOperatorInternals = Model.findNode metamodel "OperatorInternals"

            let link = Model.findAssociationWithSource metamodelAbstractQueryBlock "target"

            let model = repo.CreateModel("QueryModel", metamodel)

            let materializationLine = infrastructure.Instantiate model metamodelMaterializationLine
            infrastructure.Element.SetAttributeValue materializationLine "xCoordinate" "100"
            infrastructure.Element.SetAttributeValue materializationLine "yCoordinate" "220"

            let sort = infrastructure.Instantiate model metamodelSort
            infrastructure.Element.SetAttributeValue sort "xCoordinate" "235"
            infrastructure.Element.SetAttributeValue sort "yCoordinate" "70"

            let operatorInternals1 = infrastructure.Instantiate model metamodelOperatorInternals
            infrastructure.Element.SetAttributeValue operatorInternals1 "xCoordinate" "100"
            infrastructure.Element.SetAttributeValue operatorInternals1 "yCoordinate" "120"

            let aggregate = infrastructure.Instantiate model metamodelAggregate
            infrastructure.Element.SetAttributeValue aggregate "xCoordinate" "135"
            infrastructure.Element.SetAttributeValue aggregate "yCoordinate" "25"

            let operatorInternals2 = infrastructure.Instantiate model metamodelOperatorInternals
            infrastructure.Element.SetAttributeValue operatorInternals2 "xCoordinate" "100"
            infrastructure.Element.SetAttributeValue operatorInternals2 "yCoordinate" "240"

            let join = infrastructure.Instantiate model metamodelJoin
            infrastructure.Element.SetAttributeValue join "xCoordinate" "135"
            infrastructure.Element.SetAttributeValue join "yCoordinate" "25"

            let read1 = infrastructure.Instantiate model metamodelRead
            infrastructure.Element.SetAttributeValue read1 "xCoordinate" "30"
            infrastructure.Element.SetAttributeValue read1 "yCoordinate" "25"
            infrastructure.Element.SetAttributeValue read1 "argument" "d_datekey"

            let read2 = infrastructure.Instantiate model metamodelRead
            infrastructure.Element.SetAttributeValue read2 "xCoordinate" "240"
            infrastructure.Element.SetAttributeValue read2 "yCoordinate" "25"
            infrastructure.Element.SetAttributeValue read2 "argument" "lo_orderdate"

            let (-->) (src: IElement) dst =
                let aLink = infrastructure.Instantiate model link :?> IAssociation
                aLink.Source <- Some src
                aLink.Target <- Some dst
                aLink

            let edgeToAggregate = operatorInternals1 --> aggregate 
            infrastructure.Element.SetAttributeValue edgeToAggregate "type" "internals"

            let edgeToJoin = operatorInternals2 --> join
            infrastructure.Element.SetAttributeValue edgeToJoin "type" "internals"

            let edgeToRead1 = operatorInternals2 --> read1
            infrastructure.Element.SetAttributeValue edgeToRead1 "type" "internals"

            let edgeToRead2 = operatorInternals2 --> read2
            infrastructure.Element.SetAttributeValue edgeToRead2 "type" "internals"

            let joinToRead1 = join --> read1
            infrastructure.Element.SetAttributeValue joinToRead1 "type" "remote"
            infrastructure.Element.SetAttributeValue joinToRead1 "sourcePort" "left"
            infrastructure.Element.SetAttributeValue joinToRead1 "targetPort" "right"

            sort --> aggregate |> ignore
            aggregate --> join |> ignore

            let joinToRead2 = join --> read2
            infrastructure.Element.SetAttributeValue joinToRead2 "sourcePort" "right"
            infrastructure.Element.SetAttributeValue joinToRead2 "targetPort" "left"

            ()
