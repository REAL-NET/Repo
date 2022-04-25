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
type QueryTestModelBuilder() =
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
            let metamodelAggregates = Model.findNodes metamodel "Aggregate"
            let metamodelAggregate = findNodeWithAttribute metamodelAggregates "type" "tuple"
            let metamodelJoins = Model.findNodes metamodel "Join"
            let metamodelJoin = findNodeWithAttribute metamodelJoins "type" "positional"
            let metamodelFilters = Model.findNodes metamodel "Filter"
            let metamodelFilter = findNodeWithAttribute metamodelFilters "type" "positional"
            let metamodelDSs = Model.findNodes metamodel "DS"
            let metamodelDS = findNodeWithAttribute metamodelDSs "type" "tuple"
            let metamodelRead = Model.findNode metamodel "Read"
            let metamodelOperatorInternals = Model.findNode metamodel "OperatorInternals"

            let link = Model.findAssociationWithSource metamodelAbstractQueryBlock "target"

            let model = repo.CreateModel("QueryTestModel", metamodel)

            let materializationLine = infrastructure.Instantiate model metamodelMaterializationLine

            let sort = infrastructure.Instantiate model metamodelSort

            let operatorInternals1 = infrastructure.Instantiate model metamodelOperatorInternals

            let aggregate = infrastructure.Instantiate model metamodelAggregate

            let operatorInternals2 = infrastructure.Instantiate model metamodelOperatorInternals

            let join = infrastructure.Instantiate model metamodelJoin

            let read1 = infrastructure.Instantiate model metamodelRead
            infrastructure.Element.SetAttributeValue read1 "argument" "d_datekey"

            let read2 = infrastructure.Instantiate model metamodelRead
            infrastructure.Element.SetAttributeValue read2 "argument" "lo_orderdate"

            let filter = infrastructure.Instantiate model metamodelFilter

            let ds = infrastructure.Instantiate model metamodelDS

            let read3 = infrastructure.Instantiate model metamodelRead
            infrastructure.Element.SetAttributeValue read3 "argument" "p_category"

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
            
            sort --> aggregate |> ignore
            aggregate --> join |> ignore
            join --> read2 |> ignore
            filter --> read3 |> ignore
            join --> ds |> ignore

            ()
