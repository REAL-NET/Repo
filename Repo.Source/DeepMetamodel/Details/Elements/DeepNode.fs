namespace Repo.DeepMetamodel.Details.Elements

open Repo.DeepMetamodel
open Repo.LanguageMetamodel

type DeepNode(element: ILanguageNode, pool: DeepPool, repo: ILanguageRepository, level: int, potency: int) =
    inherit DeepElement(element, pool, repo, level, potency)
    interface IDeepNode

