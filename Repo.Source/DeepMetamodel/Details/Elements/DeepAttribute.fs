namespace Repo.DeepMetamodel.Details.Elements

open Repo.DeepMetamodel
open Repo.LanguageMetamodel

type DeepAttribute(attribute: ILanguageElement, pool: DeepPool, repo: ILanguageRepository, level: int, potency: int) =
    inherit DeepContext(level, potency)
    
    let deepMetamodel = repo.Model Repo.DeepMetamodel.Consts.deepMetamodel
    let singleValueMetatype = deepMetamodel.Node Repo.DeepMetamodel.Consts.attributeSingleValue
    let singleAssociation =
        (deepMetamodel.Node Repo.DeepMetamodel.Consts.attribute).OutgoingAssociation Repo.DeepMetamodel.Consts.attributeSingleRelationship
        
    do
        let isSingleDef = attribute.Model.InstantiateNode
                              (Repo.DeepMetamodel.Consts.attributeSingleRelationship + "::" + false.ToString())
                              singleValueMetatype
                              Map.empty
        attribute.Model.InstantiateAssociation attribute isSingleDef singleAssociation Map.empty |> ignore
        attribute.OutgoingAssociations |> ignore
        
    member this.UnderlyingAttribute = attribute
        
    override this.ToString () =
        attribute.ToString ()

    interface IDeepAttribute with
    
        member this.IsSingle
            with get () =
                (attribute.OutgoingAssociation Repo.DeepMetamodel.Consts.attributeSingleRelationship).Target
                |> (fun e -> (e :?> ILanguageNode).Name.Equals(Repo.DeepMetamodel.Consts.attributeSingleRelationship + "::" + true.ToString()))
            and set v =
                let oldRelationship = attribute.OutgoingAssociation Repo.DeepMetamodel.Consts.attributeSingleRelationship
                oldRelationship.Model.DeleteElement oldRelationship.Target
                let newNode = attribute.Model.CreateNode (Repo.DeepMetamodel.Consts.attributeSingleRelationship + "::" + v.ToString())
                attribute.Model.InstantiateAssociation attribute newNode singleAssociation Map.empty |> ignore

        member this.Type = (attribute.OutgoingAssociation Repo.DeepMetamodel.Consts.typeRelationship).Target
                           |> (fun e -> pool.Wrap e -1 -1)
                           
        member this.Name = (attribute :?> ILanguageNode).Name
