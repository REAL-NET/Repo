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

/// Initializes repository with test model conforming to Greenhouse Metamodel, actual program that can be written by end-user.
type SHModelForGraphEditorBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =

            let infrastructure = InfrastructureSemantic(repo)
            let metamodel = Repo.findModel repo "SHMetamodelForGraphEditor"

            let metamodelAbstractNode = Model.findNode metamodel "AbstractNode"
            let metamodelLightOn = Model.findNode metamodel "LightOn"
            let metamodelAndOperation = Model.findNode metamodel "AndOperation"
            let metamodelDarkCondition = Model.findNode metamodel "DarkCondition"
            let metamodelEntered = Model.findNode metamodel "Entered"

            let link = Model.findAssociationWithSource metamodelAbstractNode "target"

            let model = repo.CreateModel("SHModelForGraphEditor", metamodel)
            
            /// Example with "AND" operation
            let darkCondition = infrastructure.Instantiate model metamodelDarkCondition
            let entered = infrastructure.Instantiate model metamodelEntered
            let andOperation = infrastructure.Instantiate model metamodelAndOperation
            let lightOn = infrastructure.Instantiate model metamodelLightOn

            let (-->) (src: IElement) dst =
                let aLink = infrastructure.Instantiate model link :?> IAssociation
                aLink.Source <- Some src
                aLink.Target <- Some dst
                dst

            darkCondition --> andOperation |> ignore
            entered --> andOperation |> ignore
            
            andOperation --> lightOn |> ignore

            ()