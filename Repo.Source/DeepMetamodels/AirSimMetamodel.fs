namespace Repo.DeepMetamodels

open Repo.DeepMetamodel

type AirSimMetamodelBuilder() =
    interface IDeepModelBuilder with
        member this.Build(repo: IDeepRepository): unit =
            let metamodel = repo.InstantiateDeepMetamodel("AirSimMetamodel")

            let (~+) name =
                metamodel.CreateNode name (-1) (-1)

            let (--|>) (source: IDeepElement) (target: IDeepElement) name =
                metamodel.CreateGeneralization source target name (-1) (-1)

            let (--->) (source: IDeepElement) (target: IDeepElement) name =
                metamodel.CreateAssociation source target name (-1) (-1) (-1) (-1) (-1) (-1)
                
            let abstractNode = +"AbstractNode"
            let initialNode = +"InitialNode"
            let finalNode = +"FinalNode"

            let takeoff = +"Takeoff"
            let landing = +"Land"
            let move = +"Move"
            let hover = +"Hover"
            let timer = +"Timer"
            let ifNode = +"IfNode"
            
            (--->) abstractNode abstractNode "Link" |> ignore
            (--->) abstractNode abstractNode "If Link" |> ignore

            (--|>) initialNode abstractNode "init gen" |> ignore
            (--|>) finalNode abstractNode "final gen" |> ignore
            (--|>) takeoff abstractNode "takeoff gen" |> ignore
            (--|>) landing abstractNode "landing gen" |> ignore
            (--|>) move abstractNode "move gen" |> ignore
            (--|>) hover abstractNode "hover gen" |> ignore
            (--|>) timer abstractNode "timer gen" |> ignore
            (--|>) ifNode abstractNode "ifNode gen" |> ignore

            ()