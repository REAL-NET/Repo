namespace Repo.DeepMetamodel.Details.Elements

open Repo
open Repo.DeepMetamodel
open Repo.LanguageMetamodel

type DeepModel(model: ILanguageModel, pool: DeepPool, repo: ILanguageRepository) =

    let unwrap (element: IDeepElement) = (element :?> DeepElement).UnderlyingElement
    let wrap element level potency = pool.Wrap element level potency
    
    let languageMetamodel = repo.Model LanguageMetamodel.Consts.languageMetamodel
    let deepModel = repo.Model DeepMetamodel.Consts.deepMetamodel
    
    let languageMetamodelNode = languageMetamodel.Node LanguageMetamodel.Consts.node


   /// Returns underlying BasicNode that is a root node for model.
    member this.UnderlyingModel = model

    interface IDeepModel with

        member this.Name 
            with get () = model.Name
            and set v = model.Name <- v

        member this.HasMetamodel =
            true

        member this.Metamodel =
            pool.WrapModel model.Metamodel

        member this.CreateNode name level potency =
            (this :> IDeepModel).InstantiateNode 
                name 
                ((pool.Wrap languageMetamodelNode level potency) :?> IDeepNode) 
                level
                potency

        member this.CreateGeneralization source target level potency =
            let generalization = model.CreateGeneralization (unwrap source) (unwrap target)
            let wrappedGeneralization = wrap generalization level potency :?> IDeepGeneralization
            wrappedGeneralization.Name <- "generalization"
            wrappedGeneralization

        member this.CreateAssociation source target name level potency minSource maxSource minTarget maxTarget =
            let association = model.CreateAssociation (unwrap source) (unwrap target) target.Name
            let wrappedAssociation = pool.WrapAssociation association level potency minSource maxSource minTarget maxTarget
            wrappedAssociation.Name <- name
            wrappedAssociation

        member this.InstantiateNode name metatype level potency =
            let node = model.InstantiateNode name (unwrap metatype :?> ILanguageNode) Map.empty
            let wrappedNode = pool.Wrap node level potency :?> IDeepNode
            wrappedNode.Name <- node.Name
            wrappedNode

        member this.InstantiateAssociation source target name metatype level potency minSource maxSource minTarget maxTarget =
            let edge = model.InstantiateAssociation 
                        (unwrap source) 
                        (unwrap target) 
                        (unwrap metatype :?> ILanguageAssociation)
                        Map.empty
            let wrappedAssociation = pool.WrapAssociation edge level potency minSource maxSource minTarget maxTarget
            wrappedAssociation.Name <- name
            wrappedAssociation

        member this.Nodes = model.Nodes
                            |> Seq.map (fun e -> wrap e 0 0)   
                            |> Seq.cast<IDeepNode>

        member this.Relationships = model.Edges
                                    |> Seq.map (fun e ->
                                        (if e :? ILanguageAssociation
                                         then pool.WrapAssociation (e :?> ILanguageAssociation) 0 0 0 0 0 0 :> IDeepRelationship
                                         else wrap e 0 0 :?> IDeepRelationship))
                                    |> Seq.cast<IDeepRelationship>
                                    
        member this.Elements =
            let castedModel = this :> IDeepModel
            let a = (Seq.map (fun e -> e :> IDeepElement) castedModel.Nodes)
            let b = (Seq.map (fun e -> e :> IDeepElement) castedModel.Relationships)
            Seq.append a b

        member this.DeleteElement element =
            model.DeleteElement (unwrap element)

        member this.Node name =
            (this :> IDeepModel).Nodes
            |> Seq.filter (fun n -> n.Name = name)
            |> Helpers.exactlyOneElement name

        member this.HasNode name =
            (this :> IDeepModel).Nodes
            |> Seq.filter (fun n -> n.Name = name)
            |> Seq.isEmpty
            |> not

        member this.Association name =
            (this :> IDeepModel).Relationships
            |> Seq.choose (fun e -> if e :? IDeepAssociation then Some (e :?> IDeepAssociation) else None)
            |> Seq.map (fun e -> printf "%s" e.Name; e)
            |> Seq.filter (fun e -> e.Name = name)
            |> Helpers.exactlyOneElement name

        member this.HasAssociation name =
            (this :> IDeepModel).Relationships
            |> Seq.choose (fun e -> if e :? IDeepAssociation then Some (e :?> IDeepAssociation) else None)
            |> Seq.filter (fun e -> e.Name = name)
            |> Seq.isEmpty
            |> not

        member this.PrintContents () =
            printfn "Model: %s" model.Name
            ()


