namespace Repo.DeepMetamodel.Details.Elements

open System.Collections.Generic
open Repo.DeepMetamodel
open Repo.LanguageMetamodel


type DeepPool(factory: IDeepFactory) =
    let elementsPool = Dictionary<ILanguageElement, IDeepElement>() :> IDictionary<_, _>
    let associationsPool = Dictionary<ILanguageAssociation, IDeepAssociation>() :> IDictionary<_, _>
    let modelsPool = Dictionary<ILanguageModel, IDeepModel>() :> IDictionary<_, _>
    let attributesPool = Dictionary<ILanguageAttribute, IDeepAttribute>() :> IDictionary<_, _>
    let slotsPool = Dictionary<ILanguageSlot, IDeepSlot>() :> IDictionary<_, _>

    let wrap (pool: IDictionary<'a, 'b>) (factory: 'a -> 'b) (element: 'a): 'b =
        if pool.ContainsKey element then
            pool.[element]
        else 
            let wrapper = factory element
            pool.Add(element, wrapper)
            wrapper

    let unregister (pool: IDictionary<'a, 'b>) (element: 'a) =
        if not <| pool.Remove element then 
            failwith "Removing non-existent element"

    /// Wraps given ILanguageElement element to DeepElement. Creates new wrapper if needed, otherwise returns cached copy.
    member this.Wrap (element: ILanguageElement) (level: int) (potency: int): IDeepElement =
        wrap elementsPool (fun e -> factory.CreateElement e level potency this) element

    /// Removes element from cache.
    member this.UnregisterElement (element: ILanguageElement): unit =
        unregister elementsPool element
        
    member this.WrapAssociation (edge: ILanguageAssociation) (level: int) (potency: int) (minSource: int) (maxSource: int) (minTarget: int) (maxTarget: int): IDeepAssociation =
        wrap associationsPool (fun e -> factory.CreateAssociation e level potency minSource maxSource minTarget maxTarget this) edge
        
    member this.UnregisterAssociation (edge: ILanguageAssociation): unit =
        unregister associationsPool edge

    /// Wraps given LanguageElement to DeepAttribute. Creates new wrapper if needed, otherwise returns cached copy.
    member this.WrapAttribute (attribute: ILanguageAttribute) (level: int) (potency: int): IDeepAttribute =
        wrap attributesPool (fun e -> factory.CreateAttribute e level potency this) attribute

    /// Removes attribute from cache.
    member this.UnregisterAttribute (element: ILanguageAttribute): unit =
        unregister attributesPool element

    /// Wraps given AttributeElement to LanguageSlot. Creates new wrapper if needed, otherwise returns cached copy.
    member this.WrapSlot (slot: ILanguageSlot) (level: int) (potency: int): IDeepSlot =
        wrap slotsPool (fun e -> factory.CreateSlot e level potency this) slot

    /// Removes slot from cache.
    member this.UnregisterSlot (element: ILanguageSlot): unit =
        unregister slotsPool element

    /// Wraps given node to LanguageModel. Creates new wrapper if needed, otherwise returns cached copy.
    member this.WrapModel (model: ILanguageModel): IDeepModel =
        wrap modelsPool (fun e -> factory.CreateModel e this) model

    /// Removes model from cache.
    member this.UnregisterModel (model: ILanguageModel): unit =
        unregister modelsPool model

    /// Clears cached values, invalidating all references to Language elements.
    member this.Clear () =
        elementsPool.Clear ()
        associationsPool.Clear ()
        modelsPool.Clear ()
        attributesPool.Clear ()
        slotsPool.Clear ()

/// Abstract factory that creates wrapper objects.
and IDeepFactory =
    /// Creates AttributeElement wrapper by given LanguageElement.
    abstract CreateElement: element: ILanguageElement -> level: int -> potency: int -> pool: DeepPool -> IDeepElement
    
    abstract CreateAssociation: edge: ILanguageAssociation
         -> level: int
         -> potency: int
         -> minSource: int
         -> maxSource: int
         -> minTarget: int
         -> maxTarget: int
         -> pool: DeepPool
         -> IDeepAssociation

    /// Creates AttributeModel wrapper by given LanguageModel.
    abstract CreateModel: model: ILanguageModel -> pool: DeepPool -> IDeepModel

    /// Creates AttributeAttribute wrapper by given LanguageElement.
    abstract CreateAttribute: attribute: ILanguageAttribute -> level: int -> potency: int -> pool: DeepPool -> IDeepAttribute

    /// Creates AttributeSlot wrapper by given LanguageElement.
    abstract CreateSlot: slot: ILanguageSlot -> level: int -> potency: int -> pool: DeepPool -> IDeepSlot
