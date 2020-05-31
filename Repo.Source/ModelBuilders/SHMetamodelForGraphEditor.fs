(* Copyright 2017-2018 REAL.NET group
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. *)
 
namespace Repo.Metametamodels

open Repo
open Repo.DataLayer

/// Initializes repository with Smart Home Metamodel (Graph editor)
type SHMetamodelForGraphEditorBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =
            let infrastructure = InfrastructureSemanticLayer.InfrastructureSemantic(repo)
            let metamodel = infrastructure.Metamodel.Model

            let find name = CoreSemanticLayer.Model.findNode metamodel name

            let metamodelElement = find "Element"
            let metamodelNode = find "Node"
            let metamodelGeneralization = find "Generalization"
            let metamodelAssociation = find "Association"

            let model = repo.CreateModel("SHMetamodelForGraphEditor", metamodel)

            let (~+) (name, shape, isAbstract) =
                let node = infrastructure.Instantiate model metamodelNode :?> INode
                node.Name <- name
                infrastructure.Element.SetAttributeValue node "shape" shape
                infrastructure.Element.SetAttributeValue node "isAbstract" (if isAbstract then "true" else "false")
                infrastructure.Element.SetAttributeValue node "instanceMetatype" "Metatype.Node"

                node

            let (--|>) (source: IElement) target =
                model.CreateGeneralization(metamodelGeneralization, source, target) |> ignore

            let (--->) (source: IElement) (target, targetName, linkName) =
                let edge = infrastructure.Instantiate model metamodelAssociation :?> IAssociation
                edge.Source <- Some source
                edge.Target <- Some target
                edge.TargetName <- targetName

                //infrastructure.Element.SetAttributeValue edge "image" "View/Pictures/Greenhouse/Edge.png"
                infrastructure.Element.SetAttributeValue edge "isAbstract" "false"
                infrastructure.Element.SetAttributeValue edge "instanceMetatype" "Metatype.Edge"
                infrastructure.Element.SetAttributeValue edge "name" linkName

                edge

            let abstractNode = +("AbstractNode", "", true)

            let andOperation = +("AndOperation", "SmartHome/andOperation.svg", false)

            let valueRequiredCondition = +("ValueRequiredCondition", "", true)
            
            let minValueRequiredCondition = +("MinValueRequiredCondition", "", true)
            let lightCondition = +("LightCondition", "SmartHome/lightCondition.svg", false)
            let hotCondition = +("HotCondition", "SmartHome/hotCondition.svg", false)

            let maxValueRequiredCondition = +("MaxValueRequiredCondition", "", true)
            let darkCondition = +("DarkCondition", "SmartHome/darkCondition.svg", false)
            let coldCondition = +("ColdCondition", "SmartHome/coldCondition.svg", false)
            
            let simpleCondition = +("SimpleCondition", "", true)
            let entered = +("Entered", "SmartHome/entered.svg", false)
            let exited = +("Exited", "SmartHome/exited.svg", false)
            
            let turnOn = +("TurnOn", "", true)
            let lightOn = +("LightOn", "SmartHome/lightOn.svg", false)
            let heatingOn = +("HeatingOn", "SmartHome/heatingOn.svg", false)
            let kettleOn = +("KettleOn", "SmartHome/kettleOn.svg", false)
            
            let turnOff = +("TurnOff", "", true)
            let lightOff = +("LightOff", "SmartHome/lightOff.svg", false)
            let heatingOff = +("HeatingOff", "SmartHome/heatingOff.svg", false)
            let electricityOff = +("ElectricityOff", "SmartHome/electricityOff.svg", false)
            let tvOff = +("TvOff", "SmartHome/tvOff.svg", false)
            
            
            infrastructure.Element.AddAttribute valueRequiredCondition "value" "AttributeKind.Int" "0"
            infrastructure.Element.AddAttribute turnOn "value" "AttributeKind.Int" "0"

            let link = abstractNode ---> (abstractNode, "target", "Link")
            infrastructure.Element.AddAttribute link "guard" "AttributeKind.String" ""

            andOperation --|> abstractNode
            valueRequiredCondition --|> abstractNode
            simpleCondition --|> abstractNode
            turnOn --|> abstractNode
            turnOff --|> abstractNode
            
            minValueRequiredCondition --|> valueRequiredCondition
            maxValueRequiredCondition --|> valueRequiredCondition
            
            lightCondition --|> minValueRequiredCondition
            hotCondition --|> minValueRequiredCondition
            
            darkCondition --|> maxValueRequiredCondition
            coldCondition --|> maxValueRequiredCondition
            
            entered --|> simpleCondition
            exited --|> simpleCondition
            
            heatingOn --|> turnOn
            kettleOn --|> turnOn
            lightOn --|> turnOn
            
            electricityOff --|> turnOff
            heatingOff --|> turnOff
            lightOff --|> turnOff
            tvOff --|> turnOff
            
            ()



