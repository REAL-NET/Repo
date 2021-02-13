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
            failwith "Not implemented"

        member this.Metamodel =
            pool.WrapModel model.Metamodel

        member this.CreateNode name level potency =
            (this :> IDeepModel).InstantiateNode 
                name 
                ((pool.Wrap languageMetamodelNode level potency) :?> IDeepNode) 
                Map.empty
                level
                potency

        member this.CreateGeneralization source target level potency =
            wrap (model.CreateGeneralization (unwrap source) (unwrap target)) level potency :?> IDeepGeneralization

        member this.CreateAssociation source target targetName level potency minSource maxSource minTarget maxTarget =
            pool.WrapAssociation (model.CreateAssociation (unwrap source) (unwrap target) targetName) level potency source.Name minSource maxSource minTarget maxTarget

        member this.InstantiateNode name metatype attributeValues level potency =
            let node = model.InstantiateNode name (unwrap metatype :?> ILanguageNode) Map.empty
            let wrappedNode = pool.Wrap node level potency :?> IDeepNode
            wrappedNode.Name <- node.Name
            wrappedNode

        member this.InstantiateAssociation source target metatype attributeValues level potency minSource maxSource minTarget maxTarget =
            let edge = 
                model.InstantiateAssociation 
                    (unwrap source) 
                    (unwrap target) 
                    (unwrap metatype :?> ILanguageAssociation)
                    Map.empty
            pool.WrapAssociation edge level potency source.Name minSource maxSource minTarget maxTarget

        member this.Elements = model.Elements |> Seq.map (fun e -> wrap e 0 0) 

        member this.Nodes = model.Nodes
                            |> Seq.map (fun e -> wrap e 0 0)   
                            |> Seq.cast<IDeepNode>

        member this.Edges = model.Edges |> Seq.map wrap |> Seq.cast<IDeepRelationship>

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
            (this :> IDeepModel).Edges
            |> Seq.choose (fun e -> if e :? IDeepAssociation then Some (e :?> IDeepAssociation) else None)
            |> Seq.filter (fun e -> e.TargetName = name)
            |> Helpers.exactlyOneElement name

        member this.HasAssociation name =
            (this :> IDeepModel).Edges
            |> Seq.choose (fun e -> if e :? IDeepAssociation then Some (e :?> IDeepAssociation) else None)
            |> Seq.filter (fun e -> e.TargetName = name)
            |> Seq.isEmpty
            |> not

        member this.PrintContents () =
            printfn "Model: %s" model.Name
            ()


