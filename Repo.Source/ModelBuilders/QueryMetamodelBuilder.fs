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
open Repo.InfrastructureSemanticLayer

/// Initializes repository with AirSim Metamodel
type QueryMetamodelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =
            let infrastructure = InfrastructureSemanticLayer.InfrastructureSemantic(repo)
            let metamodel = infrastructure.Metamodel.Model

            let find name = CoreSemanticLayer.Model.findNode metamodel name

            let metamodelNode = find "Node"
            let metamodelGeneralization = find "Generalization"
            let metamodelAssociation = find "Association"

            let model = repo.CreateModel("QueryMetamodel", metamodel)

            let (~+) (name) =
                let node = infrastructure.Instantiate model metamodelNode :?> INode
                node.Name <- name
                infrastructure.Element.AddAttribute node "xCoordinate" "AttributeKind.Int" "0"
                infrastructure.Element.AddAttribute node "yCoordinate" "AttributeKind.Int" "0"

                node

            let (--|>) (source: IElement) target =
                model.CreateGeneralization(metamodelGeneralization, source, target) |> ignore

            let (--->) (source: IElement) (target, targetName, linkName) =
                let edge = infrastructure.Instantiate model metamodelAssociation :?> IAssociation
                edge.Source <- Some source
                edge.Target <- Some target
                edge.TargetName <- targetName

                infrastructure.Element.AddAttribute edge "connectionType" "AttributeKind.String" ""
                infrastructure.Element.SetAttributeValue edge "connectionType" "local"
                infrastructure.Element.SetAttributeValue edge "name" linkName

                edge

            let abstractQueryBlock  = +("AbstractQueryBlock")

            let operator = +("Operator")
            infrastructure.Element.AddAttribute operator "children" "AttributeKind.String" ""
            infrastructure.Element.AddAttribute operator "parent" "AttributeKind.String" ""
            infrastructure.Element.AddAttribute operator "connectionType" "AttributeKind.String" ""

            let operatorInternals = +("OperatorInternals")
            infrastructure.Element.AddAttribute operatorInternals "contents" "AttributeKind.String" ""

            let reader = +("Reader")
            infrastructure.Element.AddAttribute reader "parent" "AttributeKind.String" ""
            infrastructure.Element.AddAttribute reader "connectionType" "AttributeKind.String" ""
            infrastructure.Element.AddAttribute reader "argument" "AttributeKind.String" ""
            
            let ds = +("DS")
            infrastructure.Element.AddAttribute ds "argument" "AttributeKind.String" ""
            let sort = +("Sort")
            let join = +("Join")
            let aggregate = +("Aggregate")
            let filter = +("Filter")
            let materialize = +("Materialize")
            let read = +("Read")

            let link = abstractQueryBlock ---> (abstractQueryBlock, "target", "Link")

            ds --|> operator
            sort --|> operator
            join --|> operator
            aggregate --|> operator
            filter --|> operator
            materialize --|> operator
            operator --|> abstractQueryBlock
            read --|> reader
            reader --|> abstractQueryBlock
            operatorInternals --|> abstractQueryBlock

            ()