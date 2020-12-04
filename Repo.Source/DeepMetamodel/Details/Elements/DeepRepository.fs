namespace Repo.DeepMetamodel.Details.Elements

open Repo.DeepMetamodel
open Repo.LanguageMetamodel

type DeepRepository(pool: DeepPool, repo: ILanguageRepository) =

    // Recalculated because repo is created empty and then filled with models.
    let languageMetamodel () = repo.Model Consts.languageMetamodel

    let wrap model = pool.WrapModel model
    let unwrap (model: IDeepModel) = (model :?> DeepModel).UnderlyingModel

    /// Returns underlying BasicRepository object.
    member this.UnderlyingRepo = repo

    interface IDeepRepository with

        member this.Models = 
            repo.Models |> Seq.map wrap

        member this.Model name = 
            repo.Model name |> wrap

        member this.InstantiateDeepMetamodel name =
            (this :> IDeepRepository).InstantiateModel name (pool.WrapModel (languageMetamodel ()))

        member this.InstantiateModel name metamodel = 
            repo.InstantiateModel name (unwrap metamodel) |> wrap

        member this.DeleteModel model =
            repo.DeleteModel (unwrap model)
            pool.UnregisterModel (unwrap model)

        member this.Clear () =
            pool.Clear ()
            repo.Clear ()
            ()


