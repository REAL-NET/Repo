namespace Repo.DeepMetamodel.Details.Elements

open Repo.DeepMetamodel
open Repo.LanguageMetamodel

type DeepGeneralization(edge: ILanguageEdge, pool: DeepPool, repo: ILanguageRepository, level: int, potency: int) =
   inherit DeepRelationship(edge, pool, repo, level, potency)
   interface IDeepGeneralization