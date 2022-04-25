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

/// Initializes repository with Query Metamodel
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

            let (~+) (name, isAbstract) =
                let node = infrastructure.Instantiate model metamodelNode :?> INode
                node.Name <- name
                infrastructure.Element.SetAttributeValue node "isAbstract" (if isAbstract then "true" else "false")
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

                infrastructure.Element.AddAttribute edge "type" "AttributeKind.String" ""
                infrastructure.Element.SetAttributeValue edge "type" "local"
                infrastructure.Element.SetAttributeValue edge "name" linkName

                edge

            let abstractQueryBlock  = +("AbstractQueryBlock", true)

            let operator = +("Operator", true)
            infrastructure.Element.AddAttribute operator "type" "AttributeKind.String" ""
            infrastructure.Element.AddAttribute operator "kind" "AttributeKind.String" "operator"
            infrastructure.Element.AddAttribute operator "width" "AttributeKind.Int" "80"
            infrastructure.Element.AddAttribute operator "height" "AttributeKind.Int" "30"

            let operatorInternals = +("OperatorInternals", false)
            infrastructure.Element.AddAttribute operatorInternals "kind" "AttributeKind.String" "operatorInternals"
            infrastructure.Element.AddAttribute operatorInternals "width" "AttributeKind.Int" "350"
            infrastructure.Element.AddAttribute operatorInternals "height" "AttributeKind.Int" "80"

            let reader = +("Reader", true)
            infrastructure.Element.AddAttribute reader "argument" "AttributeKind.String" ""
            infrastructure.Element.AddAttribute reader "kind" "AttributeKind.String" "reader"
            infrastructure.Element.AddAttribute reader "width" "AttributeKind.Int" "80"
            infrastructure.Element.AddAttribute reader "height" "AttributeKind.Int" "30"

            let materializationLine = +("MaterializationLine", false)
            infrastructure.Element.AddAttribute materializationLine "kind" "AttributeKind.String" "materializationLine"
            infrastructure.Element.AddAttribute materializationLine "width" "AttributeKind.Int" "350"

            let ds = +("DS", false)
            infrastructure.Element.AddAttribute ds "argument" "AttributeKind.String" ""
            let sortPositional = +("Sort", false)
            let sortTuple = +("Sort", false)
            let joinPositional = +("Join", false)
            let joinTuple = +("Join", false)
            let aggregatePositional = +("Aggregate", false)
            let aggregateTuple = +("Aggregate", false)
            let filterPositional = +("Filter", false)
            let filterTuple = +("Filter", false)
            //let materialize = +("Materialize", false)
            let posAND = +("PosAND", false)
            let posOR = +("PosOR", false)
            let posNOT = +("PosNOT", false)
            let read = +("Read", false)

            let link = abstractQueryBlock ---> (abstractQueryBlock, "target", "link")

            ds --|> operator
            sortPositional --|> operator
            sortTuple --|> operator
            joinPositional --|> operator
            joinTuple --|> operator
            aggregatePositional --|> operator
            aggregateTuple --|> operator
            filterPositional --|> operator
            filterTuple --|> operator
            //materialize --|> operator
            posAND --|> operator
            posOR --|> operator
            posNOT --|> operator
            operator --|> abstractQueryBlock
            read --|> reader
            reader --|> abstractQueryBlock
            materializationLine --|> abstractQueryBlock
            operatorInternals --|> abstractQueryBlock

            infrastructure.Element.SetAttributeValue ds "type" "positional"
            infrastructure.Element.SetAttributeValue sortPositional "type" "positional"
            infrastructure.Element.SetAttributeValue sortTuple "type" "tuple"
            infrastructure.Element.SetAttributeValue joinPositional "type" "positional"
            infrastructure.Element.SetAttributeValue joinTuple "type" "tuple"
            infrastructure.Element.SetAttributeValue aggregatePositional "type" "positional"
            infrastructure.Element.SetAttributeValue aggregateTuple "type" "tuple"
            infrastructure.Element.SetAttributeValue filterPositional "type" "positional"
            infrastructure.Element.SetAttributeValue filterTuple "type" "tuple"
            infrastructure.Element.SetAttributeValue posAND "type" "positional"
            infrastructure.Element.SetAttributeValue posOR "type" "positional"
            infrastructure.Element.SetAttributeValue posNOT "type" "positional"

            ()