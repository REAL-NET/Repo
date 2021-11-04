namespace Repo.DeepMetamodel.Details.Elements

open System
open Repo.DeepMetamodel
open Repo.LanguageMetamodel

type DeepContext(node: ILanguageElement, repo: ILanguageRepository, level: int, potency: int) =
    
    let deepMetamodel = repo.Model Repo.DeepMetamodel.Consts.deepMetamodel
    
    let potencyValueMetatype = deepMetamodel.Node Repo.DeepMetamodel.Consts.contextPotencyValue
    let potencyAssociation =
        (deepMetamodel.Node Repo.DeepMetamodel.Consts.deepContext).OutgoingAssociation Repo.DeepMetamodel.Consts.contextPotencyRelationship
        
    let levelValueMetatype = deepMetamodel.Node Repo.DeepMetamodel.Consts.contextLevelValue
    let levelAssociation =
        (deepMetamodel.Node Repo.DeepMetamodel.Consts.deepContext).OutgoingAssociation Repo.DeepMetamodel.Consts.contextLevelRelationship
        
    
    let mutable _cache_level: Nullable<int> = System.Nullable()
    let mutable _cache_potency: Nullable<int> = System.Nullable()
    
    let hasNoLevelPotency =
        node.Metatype.Equals(potencyValueMetatype) ||
        node.Metatype.Equals(potencyAssociation) ||
        node.Metatype.Equals(levelValueMetatype) ||
        node.Metatype.Equals(levelAssociation)
    
    do
        if hasNoLevelPotency then () else  
        let oldLevelRelationships = Seq.filter
                                     (fun e -> (e :> ILanguageAssociation).TargetName = Repo.DeepMetamodel.Consts.contextLevelRelationship)
                                     node.OutgoingAssociations
        if ((Seq.length oldLevelRelationships) = 0)
        then
            let levelDef = node.Model.InstantiateNode
                                  (Repo.DeepMetamodel.Consts.contextLevelRelationship + "::" + level.ToString())
                                  levelValueMetatype
                                  Map.empty
            node.Model.InstantiateAssociation node levelDef levelAssociation Map.empty |> ignore
            _cache_level <- Nullable(level)
            
        let oldPotencyRelationships = Seq.filter
                                       (fun e -> (e :> ILanguageAssociation).TargetName = Repo.DeepMetamodel.Consts.contextPotencyRelationship)
                                       node.OutgoingAssociations
        if (Seq.length oldPotencyRelationships = 0)
        then 
            let potencyDef = node.Model.InstantiateNode
                                  (Repo.DeepMetamodel.Consts.contextPotencyRelationship + "::" + potency.ToString())
                                  potencyValueMetatype
                                  Map.empty
            node.Model.InstantiateAssociation node potencyDef potencyAssociation Map.empty |> ignore
            _cache_potency <- Nullable(potency)
        
            
    
    interface IDeepContext with
        
        member this.Level
            with get() =
                if hasNoLevelPotency then -1 else
                if (_cache_level.HasValue) then _cache_level.Value else 
                (node.OutgoingAssociation Repo.DeepMetamodel.Consts.contextLevelRelationship).Target
                |> (fun e -> (e :?> ILanguageNode).Name.Substring(Repo.DeepMetamodel.Consts.contextLevelRelationship.Length + 2))
                |> int
            and set value =
                let oldRelationships = Seq.filter
                                            (fun e -> (e :> ILanguageAssociation).TargetName = Repo.DeepMetamodel.Consts.contextLevelRelationship)
                                            node.OutgoingAssociations
                if (Seq.length oldRelationships > 0)
                then
                    let oldRelationship = node.OutgoingAssociation Repo.DeepMetamodel.Consts.contextLevelRelationship
                    oldRelationship.Model.DeleteElement oldRelationship.Target
                let newNode = node.Model.InstantiateNode
                                  (Repo.DeepMetamodel.Consts.contextLevelRelationship + "::" + value.ToString())
                                  levelValueMetatype
                                  Map.empty
                node.Model.InstantiateAssociation node newNode levelAssociation Map.empty |> ignore
                _cache_level <- Nullable(value)

            
        member this.Potency
            with get() =
                if hasNoLevelPotency then -1 else
                if (_cache_potency.HasValue) then _cache_potency.Value else 
                (node.OutgoingAssociation Repo.DeepMetamodel.Consts.contextPotencyRelationship).Target
                |> (fun e -> (e :?> ILanguageNode).Name.Substring(Repo.DeepMetamodel.Consts.contextPotencyRelationship.Length + 2))
                |> int
            and set value =
                let oldRelationships = Seq.filter
                                        (fun e -> (e :> ILanguageAssociation).TargetName = Repo.DeepMetamodel.Consts.contextPotencyRelationship)
                                        node.OutgoingAssociations
                if (Seq.length oldRelationships > 0)
                then
                    let oldRelationship = node.OutgoingAssociation Repo.DeepMetamodel.Consts.contextPotencyRelationship
                    oldRelationship.Model.DeleteElement oldRelationship.Target
                let newNode = node.Model.InstantiateNode
                                  (Repo.DeepMetamodel.Consts.contextPotencyRelationship + "::" + value.ToString())
                                  potencyValueMetatype
                                  Map.empty
                node.Model.InstantiateAssociation node newNode potencyAssociation Map.empty |> ignore
                _cache_potency <- Nullable(value)

