namespace Repo.DeepMetamodel

open Repo
open Repo.DeepMetamodel.Details
open Repo.DeepMetamodel.Details.Elements

/// Factory that creates language metamodel repository.
[<AbstractClass; Sealed>]
type DeepMetamodelRepoFactory =
    /// Method that returns repository with Core Metamodel.
    static member Create() = 
        let langRepo = LanguageMetamodel.LanguageMetamodelRepoFactory.Create ()
        let factory = DeepFactory(langRepo)
        let pool = DeepPool(factory)
        let repo = DeepRepository(pool, langRepo)
        DeepMetamodelCreator.createIn repo
        repo :> IDeepRepository