namespace Repo.DeepMetamodels

open Repo.DeepMetamodel
open Repo.Facade

type RobotsSubroutineModelsBuilder() =
    interface IDeepModelBuilder with
        member this.Build(repo: IDeepRepository): unit =
            let metamodel = repo.InstantiateDeepMetamodel "RobotsMetamodel"
            
            let element = metamodel.CreateNode "Element" 0 0
            let move = metamodel.CreateNode "Move" 0 1
            let sing = metamodel.CreateNode "Sing" 0 1
            let subroutine = metamodel.CreateNode "Subroutine" 0 2
            let flow = metamodel.CreateAssociation element element "flow" 0 1 (-1) (-1) (-1) (-1)
            metamodel.CreateGeneralization element move "Move gen" (-1) (-1) |> ignore
            metamodel.CreateGeneralization element sing "Sing gen" (-1) (-1) |> ignore
            metamodel.CreateGeneralization element subroutine "Subroutine gen" (-1) (-1) |> ignore

            let subroutinesModel = repo.InstantiateModel "RobotsSubroutines" metamodel
            
            let phantomOfOpera = subroutinesModel.InstantiateNode "Phantom of Opera" subroutine
            let danceMove = subroutinesModel.InstantiateNode "Dance" move
            let overture = subroutinesModel.InstantiateNode "Overture" sing
            subroutinesModel.InstantiateAssociation phantomOfOpera danceMove "to dance" flow |> ignore
            subroutinesModel.InstantiateAssociation danceMove overture "to sing overture" flow |> ignore

            let notreDame = subroutinesModel.InstantiateNode "Notre-Dame" subroutine
            let runMove = subroutinesModel.InstantiateNode "Run" move
            let belle = subroutinesModel.InstantiateNode "Belle" sing
            subroutinesModel.InstantiateAssociation notreDame belle "to sing Belle" flow |> ignore
            subroutinesModel.InstantiateAssociation belle runMove "to run" flow |> ignore

            repo.InstantiateModel "RobotsModel" subroutinesModel |> ignore
            
            ()
            
            

