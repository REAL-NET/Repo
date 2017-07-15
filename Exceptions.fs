﻿(* Copyright 2017 Yurii Litvinov
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

namespace RepoExperimental

/// Thrown if trying to delete a model that is a metamodel for some another model in repository.
exception DeletingUsedModel of modelName: string

/// Thrown if we are trying to do something wrong with a model. Most probably means incorrect model or
/// internal error in repository.
exception InvalidSemanticOperationException of errorMessage: string

/// Thrown when some semantic operation can not be completed due to missing elements of Core Metamodel.
/// Core Metamodel forms a core of a tool, so it always shall be present and all modifications inside it shall be
/// reflected in the code (basically, it is a model of repository data layer itself).
exception MalformedCoreMetamodelException of errorMessage: string

/// Thrown when some semantic operation can not be completed due to missing or incorrect elements of 
/// Infrastructure Metamodel. Infrastructure Metamodel provides a way for an editor and other tools to work
/// with models, so if it is malformed, all high-level tools based on a repository will not work.
exception MalformedInfrastructureMetamodelException of errorMessage: string
