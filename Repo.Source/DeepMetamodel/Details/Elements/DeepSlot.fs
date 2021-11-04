namespace Repo.DeepMetamodel.Details.Elements

open System
open Repo.DeepMetamodel
open Repo.LanguageMetamodel

type DeepSlot(node: ILanguageElement, pool: DeepPool, repo: ILanguageRepository, level: int, potency: int) =
    inherit DeepContext(node, repo, level, potency)
    
    let deepMetamodel = repo.Model Repo.DeepMetamodel.Consts.deepMetamodel
    let valueAssociation = (deepMetamodel.Node Repo.DeepMetamodel.Consts.slot).OutgoingAssociation Repo.DeepMetamodel.Consts.valueRelationship
    let simpleAttributeType = pool.Wrap (deepMetamodel.Node Repo.DeepMetamodel.Consts.simpleAttributeType) -1 -1
    let slotsAssociationMetatype = deepMetamodel.Association Repo.DeepMetamodel.Consts.slotsRelationship

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
        
        member this.IsSimple
            with get () = (this :> IDeepSlot).Value.Metatype.Equals(simpleAttributeType)
                
        member this.SimpleValue
            with get () =
                let name = (this :> IDeepSlot).Value.Name
                if (this :> IDeepSlot).IsSimple then
                    let startIndex = name.IndexOf(":->:")
                    (this :> IDeepSlot).Value.Name.Substring(startIndex + 4)
                else
                    name
            and set value =
                let name = "SimpleValue." + Guid.NewGuid().ToString() + ":->:" + value
                let origNode = node.IncomingAssociations
                                |> Seq.find (fun e -> e.Metatype.Equals(slotsAssociationMetatype))
                                |> (fun e -> e.Source)
                                |> (fun e -> pool.Wrap e -1 -1)
                let slotValue = origNode.Model.InstantiateNode name (simpleAttributeType:?> IDeepNode)
                (this :> IDeepSlot).Value <- slotValue            

