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
   
   let mutable myMinSource = minSource
   let mutable myMaxSource = maxSource
   let mutable myMinTarget = minTarget
   let mutable myMaxTarget = maxTarget
   
   interface IDeepAssociation with                       
        member this.MinSource
            with get() = myMinSource
            and set v = myMinSource <- v
            
        member this.MaxSource
            with get() = myMaxSource
            and set v = myMaxSource <- v
            
        member this.MinTarget
            with get() = myMinTarget
            and set v = myMinTarget <- v
            
        member this.MaxTarget
            with get() = myMaxTarget
            and set v = myMaxTarget <- v
