namespace Repo.DeepMetamodel.Details.Elements

open Repo.DeepMetamodel
open Repo.LanguageMetamodel

type DeepSlot(node: ILanguageElement, pool: DeepPool, repo: ILanguageRepository, level: int, potency: int) =
    inherit DeepContext(level, potency)
    
    let languageMetamodel = repo.Model Consts.languageMetamodel
    let valueAssociation = (languageMetamodel.Node Consts.slot).OutgoingAssociation Consts.valueRelationship

    let unwrap (element: IDeepElement) = (element :?> DeepElement).UnderlyingElement
    
    interface IDeepSlot with
        member this.Attribute =
            (node.OutgoingAssociation Consts.attributeEdge).Target
            |> (fun e -> pool.WrapAttribute e 0 0)

        /// Returns a node that represents type of an attribute.
        member this.Value
            with get () =
                (node.OutgoingAssociation Consts.valueRelationship).Target
                |> (fun e -> pool.Wrap e 0 0)
            and set v =
                let oldValue = (node.OutgoingAssociation Consts.valueRelationship).Target
                oldValue.Model.DeleteElement oldValue
                node.Model.InstantiateAssociation node (unwrap v) valueAssociation |> ignore

