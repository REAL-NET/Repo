namespace Repo.DeepMetamodel.Details.Elements

open Repo.DeepMetamodel
open Repo.LanguageMetamodel

type DeepSlot(node: ILanguageSlot, pool: DeepPool, repo: ILanguageRepository, level: int, potency: int) =
    inherit DeepContext(level, potency)
    
    interface IDeepSlot with
        member this.Attribute = pool.WrapAttribute node.Attribute 0 0

        member this.Value
            with get () = pool.Wrap node.Value 0 0 
            and set v = node.Value <- (v :?> DeepElement).UnderlyingElement

