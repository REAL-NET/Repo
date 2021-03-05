﻿(* Copyright 2019 REAL.NET group
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License. *)

namespace Repo.LanguageMetamodel.Details.Elements

open Repo.LanguageMetamodel
open Repo.AttributeMetamodel

/// Implementation of Attribute.
type LanguageAttribute(attribute: IAttributeElement, pool: LanguagePool, repo: IAttributeRepository) =

    override this.ToString () =
        let this = this :> ILanguageAttribute
        this.Name + ": " + this.Type.ToString ()

    interface ILanguageAttribute with
        member this.Name = (attribute :?> IAttributeNode).Name

        member this.Type = (attribute.OutgoingAssociation Consts.typeEdge).Target |> pool.Wrap
