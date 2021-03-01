namespace Repo.DeepMetamodel.Details.Elements

open Repo.DeepMetamodel
open Repo.LanguageMetamodel

type DeepAssociation(edge: ILanguageAssociation,
                     pool: DeepPool,
                     repo: ILanguageRepository,
                     level: int,
                     potency: int,
                     minSource: int,
                     maxSource: int,
                     minTarget: int,
                     maxTarget: int) =
   inherit DeepRelationship(edge, pool, repo, level, potency)
   
   interface IDeepAssociation with                       
        member this.MinSource = minSource
            
        member this.MaxSource = maxSource
            
        member this.MinTarget = minTarget
            
        member this.MaxTarget = maxTarget
