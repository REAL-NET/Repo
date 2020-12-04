namespace Repo.DeepMetamodel.Details.Elements

open Repo.LanguageMetamodel
open Repo.DeepMetamodel

[<AbstractClass>]
type DeepElement(element: ILanguageElement, pool: DeepPool, repo: ILanguageRepository, level: int, potency: int) =
    inherit DeepContext(level, potency)
    
    let languageMetamodel =
        repo.Model Consts.deepMetametamodel
    
    let attributesAssociationMetatype =
        languageMetamodel.Association Consts.attributesRelationship
        
    let slotsAssociationMetatype =
        languageMetamodel.Association Consts.slotsEdge

    
    let wrap = pool.Wrap
    
    let mutable myName: string = "name"
    
    member this.UnderlyingElement = element
        
    interface IDeepElement with
        member this.Name
            with get() = myName
            and set value = myName <- value
        
        member this.OutgoingEdges =
            element.OutgoingEdges
            |> Seq.map pool.Wrap
            |> Seq.cast<IDeepRelationship>
        
        member this.OutgoingAssociations =
            element.OutgoingAssociations
            |> Seq.map (fun e -> pool.WrapAssociation e 0 0 "" 0 0 0 0)    
            |> Seq.cast<IDeepAssociation>

        member this.IncomingAssociations =
            element.IncomingAssociations
            |> Seq.map (fun e -> pool.WrapAssociation e 0 0 "" 0 0 0 0)
            |> Seq.cast<IDeepAssociation>

        member this.DirectSupertypes =
            element.OutgoingEdges
            |> Seq.filter (fun e -> e :? IDeepGeneralization)
            |> Seq.map (fun e -> e.Target)
            |> Seq.map (fun e -> wrap e 0 0)

        member this.Attributes =
             let selfAttributes =
                element.OutgoingAssociations
                |> Seq.filter (fun a -> a.Metatype = (attributesAssociationMetatype :> ILanguageElement))
                |> Seq.map (fun a -> a.Target)
                |> Seq.map wrap
                |> Seq.cast<IDeepAttribute>
                
             (this :> IDeepElement).DirectSupertypes
            |> Seq.map (fun e -> e.Attributes)
            |> Seq.concat
            |> Seq.append selfAttributes    

        member this.Slots =
            element.OutgoingAssociations
            |> Seq.filter (fun a -> a.Metatype = (slotsAssociationMetatype :> ILanguageElement))
            |> Seq.map (fun a -> a.Target)
            |> Seq.cast<ILanguageSlot>
            |> Seq.map (fun e -> pool.WrapSlot e 0 0 )

        member this.Model: IDeepModel =
            pool.WrapModel element.Model

        member this.HasMetatype =
            failwith "Not implemented"

        member this.Metatype =
            pool.Wrap element.Metatype 0 0 
    
