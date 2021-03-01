namespace Repo.DeepMetamodel.Details.Elements

open Repo.DeepMetamodel
open Repo.LanguageMetamodel

type DeepRelationship(edge: ILanguageEdge, pool: DeepPool, repo: ILanguageRepository, level: int, potency: int) =
    inherit DeepElement(edge, pool, repo, level, potency)
    
    interface IDeepRelationship with
        member this.Source = pool.Wrap edge.Source (this :> IDeepElement).Level (this :> IDeepElement).Potency

        member this.Target = pool.Wrap edge.Target (this :> IDeepElement).Level (this :> IDeepElement).Potency    

