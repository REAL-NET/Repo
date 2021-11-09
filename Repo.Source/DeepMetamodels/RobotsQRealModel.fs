namespace Repo.DeepMetamodels

open Repo.DeepMetamodel

type RobotsQRealModelsBuilder() =
    interface IDeepModelBuilder with
        member this.Build(repo: IDeepRepository): unit =
            let metamodel = repo.InstantiateDeepMetamodel "RobotsQRealMetamodel"

            let abstractNode = metamodel.CreateNode "Abstract node" 0 0

            let link = metamodel.CreateAssociation abstractNode abstractNode "link" 0 1 (-1) (-1) (-1) (-1)

            let initialNode = metamodel.CreateNode "Initial node" 0 1
            let initialNodePicture = initialNode.AddSimpleAttribute "Image" (-1) (-1)
            initialNode.AddSimpleSlot initialNodePicture "images/initialBlock.svg" (-1) (-1) |> ignore
            metamodel.CreateGeneralization abstractNode initialNode "Initial node gen" (-1) (-1) |> ignore

            let finalNode = metamodel.CreateNode "Final node" 0 1
            let finalNodePicture = finalNode.AddSimpleAttribute "Image" (-1) (-1)
            finalNode.AddSimpleSlot finalNodePicture "images/finalBlock.svg" (-1) (-1) |> ignore
            metamodel.CreateGeneralization abstractNode finalNode "Final node gen" (-1) (-1) |> ignore

            let abstractMotorsBlock = metamodel.CreateNode "Abstract motors block" 0 0
            let abstractMotorsBlockPorts = abstractMotorsBlock.AddSimpleAttribute "Ports" (-1) (-1)
            abstractMotorsBlock.AddSimpleSlot abstractMotorsBlockPorts "M3, M4" (-1) (-1) |> ignore
            metamodel.CreateGeneralization abstractNode abstractMotorsBlock "Abstract motors block gen" (-1) (-1) |> ignore

            let abstractMotorsPowerBlock = metamodel.CreateNode "Abstract motors power block" 0 0
            let abstractMotorsPowerBlockPower = abstractMotorsPowerBlock.AddSimpleAttribute "Power" (-1) (-1)
            abstractMotorsPowerBlock.AddSimpleSlot abstractMotorsPowerBlockPower "100" (-1) (-1) |> ignore
            metamodel.CreateGeneralization abstractMotorsBlock abstractMotorsPowerBlock "Abstract motors power block gen" (-1) (-1) |> ignore

            let motorsForward = metamodel.CreateNode "Motors forward" 0 1
            let motorsForwardPicture = motorsForward.AddSimpleAttribute "Image" (-1) (-1)
            motorsForward.AddSimpleSlot motorsForwardPicture "images/enginesForwardBlock.svg" (-1) (-1) |> ignore
            metamodel.CreateGeneralization abstractMotorsPowerBlock motorsForward "Motors forward gen" (-1) (-1) |> ignore

            let motorsBackward = metamodel.CreateNode "Motors backward" 0 1
            let motorsBackwardPicture = motorsBackward.AddSimpleAttribute "Image" (-1) (-1)
            motorsBackward.AddSimpleSlot motorsBackwardPicture "images/enginesBackwardBlock.svg" (-1) (-1) |> ignore
            metamodel.CreateGeneralization abstractMotorsPowerBlock motorsBackward "Motors backward gen" (-1) (-1) |> ignore

            let motorsStop = metamodel.CreateNode "Motors stop" 0 1
            let motorsStopPicture = motorsStop.AddSimpleAttribute "Image" (-1) (-1)
            motorsStop.AddSimpleSlot motorsStopPicture "images/enginesStopBlock.svg" (-1) (-1) |> ignore
            metamodel.CreateGeneralization abstractMotorsPowerBlock motorsStop "Motors stop gen" (-1) (-1) |> ignore

            let timer = metamodel.CreateNode "Timer" 0 1
            let timerPicture = timer.AddSimpleAttribute "Image" (-1) (-1)
            timer.AddSimpleSlot timerPicture "images/timerBlock.svg" (-1) (-1) |> ignore
            let timerDelay = timer.AddSimpleAttribute "Delay" (-1) (-1)
            timer.AddSimpleSlot timerDelay "1000" (-1) (-1) |> ignore
            metamodel.CreateGeneralization abstractNode timer "Motors stop gen" (-1) (-1) |> ignore

            let qRealModel = repo.InstantiateModel "RobotsQRealModel" metamodel

            let initialNodeInstance = qRealModel.InstantiateNode "Initial node" initialNode
            let finalNodeInstance = qRealModel.InstantiateNode "Final node" finalNode
            let motorsForwardInstance = qRealModel.InstantiateNode "Motors forward" motorsForward
            let timerInstance = qRealModel.InstantiateNode "Timer" timer
            qRealModel.InstantiateAssociation initialNodeInstance motorsForwardInstance "start forward" link |> ignore
            qRealModel.InstantiateAssociation motorsForwardInstance timerInstance "motors timer" link |> ignore
            qRealModel.InstantiateAssociation timerInstance finalNodeInstance "timer finish" link |> ignore

            ()
            
            

