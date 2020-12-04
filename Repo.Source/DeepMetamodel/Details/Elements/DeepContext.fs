namespace Repo.DeepMetamodel.Details.Elements

open Repo.DeepMetamodel

type DeepContext(level: int, potency: int) =
    
    let mutable myLevel: int = level
    let mutable myPotency: int = potency

    interface IDeepContext with
        member this.Level
            with get() = myLevel
            and set value = myLevel <- value
            
        member this.Potency
            with get() = myPotency
            and set value = myPotency <- value

