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
            let metamodelMaterializationPlank = Model.findNode metamodel "MaterializationPlank"
            let metamodelSort = Model.findNode metamodel "Sort"
            let metamodelAggregate = Model.findNode metamodel "Aggregate"
            let metamodelJoin = Model.findNode metamodel "Join"
            let metamodelFilter = Model.findNode metamodel "Filter"
            let metamodelRead = Model.findNode metamodel "Read"
            let metamodelOperatorInternals = Model.findNode metamodel "OperatorInternals"

            let link = Model.findAssociationWithSource metamodelAbstractQueryBlock "target"

            let model = repo.CreateModel("QueryTestModel", metamodel)

            let materializationPlank = infrastructure.Instantiate model metamodelMaterializationPlank

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

            sort --> aggregate --> join |> ignore

            join --> read2 |> ignore

            filter --> read3 |> ignore

            ()
