namespace Repo.DeepMetamodel.Details.Elements

open Repo.DeepMetamodel
open Repo.LanguageMetamodel

type DeepSlot(node: ILanguageElement, pool: DeepPool, repo: ILanguageRepository, level: int, potency: int) =
    inherit DeepContext(level, potency)
    
    let deepMetamodel = repo.Model Repo.DeepMetamodel.Consts.deepMetamodel
    let valueAssociation = (deepMetamodel.Node Repo.DeepMetamodel.Consts.slot).OutgoingAssociation Repo.DeepMetamodel.Consts.valueRelationship

    let unwrap (element: IDeepElement) = (element :?> DeepElement).UnderlyingElement
    
    interface IDeepSlot with
        member this.Attribute =
            (node.OutgoingAssociation Repo.DeepMetamodel.Consts.attributeRelationship).Target
            |> (fun e -> pool.WrapAttribute e -1 -1)

        /// Returns a node that represents type of an attribute.
        member this.Value
            with get () =
                (node.OutgoingAssociation Repo.DeepMetamodel.Consts.valueRelationship).Target
                |> (fun e -> pool.Wrap e -1 -1)
            and set v =
                let oldValue = (node.OutgoingAssociation Repo.DeepMetamodel.Consts.valueRelationship).Target
                oldValue.Model.DeleteElement oldValue
                node.Model.InstantiateAssociation node (unwrap v) valueAssociation Map.empty |> ignore

