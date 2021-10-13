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
    
    let getPotencyForContext (metatype: IDeepContext) =
        if (metatype.Potency.Equals(-1)) then -1 else metatype.Potency - 1
    
    let getPotency (metatype: IDeepElement) =
        let metaPotency = metatype.Potency
        if (metaPotency.Equals(0)) then raise (InstantiationNotAllowedByPotencyException metatype.Name)
        getPotencyForContext metatype
        
    let getLevel (metatype: IDeepElement) (model: IDeepModel) =
        if (metatype.Model.Equals(model)) then metatype.Level else 
        if (metatype.Level.Equals(-1))
        then -1
            else 
                let mutable levelDifference = 1
                let mutable current = model
                while ((Seq.contains metatype (current.Metamodel.Elements)) |> not) do
                    current <- model.Metamodel
                    levelDifference <- levelDifference + 1
                metatype.Level + levelDifference


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
            let newNode = (this :> IDeepModel).InstantiateNode 
                            name 
                            ((pool.Wrap languageMetamodelNode (-1) (-1)) :?> IDeepNode)
            newNode.Level <- level
            newNode.Potency <- potency
            newNode

        member this.CreateGeneralization source target name level potency =
            let generalization = model.CreateGeneralization (unwrap source) (unwrap target)
            let wrappedGeneralization = wrap generalization level potency :?> IDeepGeneralization
            wrappedGeneralization.Name <- name
            wrappedGeneralization

        member this.CreateAssociation source target name level potency minSource maxSource minTarget maxTarget =
            let association = model.CreateAssociation (unwrap source) (unwrap target) target.Name
            let wrappedAssociation = pool.WrapAssociation association level potency minSource maxSource minTarget maxTarget
            wrappedAssociation.Name <- name
            wrappedAssociation

        member this.InstantiateNode name metatype =
            let potency = getPotency metatype
            let level = getLevel metatype this     
            let node = model.InstantiateNode name (unwrap metatype :?> ILanguageNode) Map.empty
            let wrappedNode = pool.Wrap node level potency :?> IDeepNode
            wrappedNode.Name <- node.Name
            for attr in metatype.Attributes do
                if attr.Potency <> 0 then
                    wrappedNode.AddAttribute attr.Name attr.Type level (getPotencyForContext attr) |> ignore
            for slot in metatype.Slots do
                if slot.Potency <> 0 then
                    let attr = Seq.find (fun e -> (e :> IDeepAttribute).Name.Equals(slot.Attribute.Name)) wrappedNode.Attributes
                    if (not attr.IsSingle) then
                        wrappedNode.AddSlot attr slot.Value level (getPotencyForContext slot) |> ignore
            wrappedNode

        member this.InstantiateAssociation source target name metatype =
            let edge = model.InstantiateAssociation 
                        (unwrap source) 
                        (unwrap target) 
                        (unwrap metatype :?> ILanguageAssociation)
                        Map.empty
            let potency = getPotency metatype
            let level = getLevel metatype this
            let wrappedAssociation = pool.WrapAssociation edge level potency metatype.MinSource metatype.MaxSource metatype.MinTarget metatype.MaxTarget
            wrappedAssociation.Name <- name
            for attr in metatype.Attributes do
                if attr.Potency <> 0 then
                    wrappedAssociation.AddAttribute attr.Name attr.Type level (getPotencyForContext attr) |> ignore
            for slot in metatype.Slots do
                if slot.Potency <> 0 then
                    let attr = Seq.find (fun e -> (e :> IDeepAttribute).Name.Equals(slot.Attribute.Name)) wrappedAssociation.Attributes
                    if (not attr.IsSingle) then
                        wrappedAssociation.AddSlot attr slot.Value level (getPotencyForContext slot) |> ignore
            wrappedAssociation

        member this.Nodes = model.Nodes
                            |> Seq.map (fun e -> wrap e -1 -1)   
                            |> Seq.cast<IDeepNode>
                            // Do not return attributes
                            |> Seq.filter (fun e -> not (e.Metatype.Name.Equals(DeepMetamodel.Consts.attribute)))
                            // Do not return slots
                            |> Seq.filter (fun e -> not (e.Metatype.Name.Equals(DeepMetamodel.Consts.slot)))
                            // Do not return "is single" attribute values
                            |> Seq.filter (fun e -> not (e.Metatype.Name.Equals(DeepMetamodel.Consts.attributeSingleValue)))                        

        member this.Relationships = model.Edges
                                    |> Seq.map (fun e ->
                                        (if e :? ILanguageAssociation
                                         then pool.WrapAssociation (e :?> ILanguageAssociation) -1 -1 -1 -1 -1 -1 :> IDeepRelationship
                                         else wrap e -1 -1 :?> IDeepRelationship))
                                    |> Seq.cast<IDeepRelationship>
                                    // Do not return attribute relationships
                                    |> Seq.filter (fun e -> (not (e.Metatype.Name.Equals(DeepMetamodel.Consts.attributesRelationship))) &&
                                                            (not (e.Metatype.Name.Equals(DeepMetamodel.Consts.typeRelationship))) &&
                                                            (not (e.Metatype.Name.Equals(DeepMetamodel.Consts.attributeSingleRelationship))))
                                    // Do not return slot relationships
                                    |> Seq.filter (fun e -> (not (e.Metatype.Name.Equals(DeepMetamodel.Consts.slotsRelationship))) &&
                                                            (not (e.Metatype.Name.Equals(DeepMetamodel.Consts.attributeRelationship))) &&
                                                            (not (e.Metatype.Name.Equals(DeepMetamodel.Consts.valueRelationship)))) 
                                    
        member this.Elements =
            let castedModel = this :> IDeepModel
            let nodes = (Seq.map (fun e -> e :> IDeepElement) castedModel.Nodes)
            let relationships = (Seq.map (fun e -> e :> IDeepElement) castedModel.Relationships)
            Seq.append nodes relationships


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


