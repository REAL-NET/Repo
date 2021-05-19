namespace Repo.DeepMetamodels

open Repo.DeepMetamodel

type IDeepModelBuilder =
    interface
        /// Builds a model inside given repository.
        abstract Build: repo: IDeepRepository -> unit
    end

