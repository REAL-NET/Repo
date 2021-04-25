namespace Repo.DeepMetamodels

open Repo.DeepMetamodel

type IModelBuilder =
    interface
        /// Builds a model inside given repository.
        abstract Build: repo: IDeepRepository -> unit
    end

