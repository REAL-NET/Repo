namespace Repo.DeepMetamodel.Details.Elements

open Repo.DeepMetamodel
open Repo.LanguageMetamodel

/// Implementation of wrapper factory.
type DeepFactory(repo: ILanguageRepository) =
    interface IDeepFactory with
        member this.CreateElement element level potency pool =
            match element with
            | :? ILanguageNode as n -> DeepNode(n, pool, repo, level, potency) :> _
            | :? ILanguageGeneralization as g -> DeepGeneralization(g, pool, repo, level, potency) :> _
            | :? ILanguageInstanceOf as i -> DeepInstanceOf(i, pool, repo, level, potency) :> _
            | _ -> failwith "Unknown subtype"
            
        member this.CreateAssociation element level potency sourceName minSource maxSource minTarget maxTarget pool =
            DeepAssociation(element, pool, repo, level, potency, sourceName, minSource, maxSource, minTarget, maxTarget) :> IDeepAssociation

        member this.CreateModel model pool =
            DeepModel(model, pool, repo) :> _

        member this.CreateAttribute attribute level potency pool =
            DeepAttribute(attribute, pool, repo, level, potency) :> IDeepAttribute

        member this.CreateSlot element level potency pool =
            DeepSlot(element, pool, repo, level, potency) :> IDeepSlot

