module Repo.DeepMetamodel.Details.DeepMetamodelCreator

open Repo.BasicMetamodel
open Repo.DeepMetamodel
open Repo.DeepMetamodel.Details.Elements

let createIn(repo: IDeepRepository): unit =
    let repo = (repo :?> DeepRepository).UnderlyingRepo
    
    let model = repo.InstantiateLanguageMetamodel Consts.deepMetamodel
    
    let (--->) source (target, name) = model.CreateAssociation source target name |> ignore
    let (~+) name = model.CreateNode name
    let (--|>) source target = model.CreateGeneralization source target |> ignore
    
    let deepContext = +Consts.deepContext
    let model = +Consts.model
    let repository = +Consts.repository
    let element = +Consts.element
    let node = +Consts.node
    let relationship = +Consts.relationships
    let generalization = +Consts.generalization
    let association = +Consts.association
    let attribute = +Consts.attribute
    let slot = +Consts.slot
    let stringNode = +Consts.string
    
    element --|> deepContext
    attribute --|> deepContext
    slot --|> deepContext
    node --|> element
    relationship --|> element
    generalization --|> relationship
    association --|> relationship
    
    repository ---> (model, Consts.modelsRelationship)
    model ---> (model, Consts.metamodelRelationship)
    model ---> (element, Consts.elementsRelationship)
    model ---> (stringNode, Consts.nameRelationship)
    element ---> (model, Consts.modelsRelationship)
    relationship ---> (element, Consts.sourceRelationship)
    relationship ---> (element, Consts.targetRelationship)
    node ---> (stringNode, Consts.nameRelationship)
    association ---> (stringNode, Consts.targetNameRelationship)
    
    element ---> (attribute, Consts.attributesRelationship)
    element ---> (slot, Consts.slotsRelationship)
    slot ---> (attribute, Consts.attributeRelationship)
    slot ---> (node, Consts.valueRelationship)
    attribute ---> (stringNode, Consts.nameRelationship)
    attribute ---> (node, Consts.typeRelationship)
    
    let attributeSingleValue = +Consts.attributeSingleValue
    attribute ---> (attributeSingleValue, Consts.attributeSingleRelationship)
    let contextLevelValue = +Consts.contextLevelValue
    deepContext ---> (contextLevelValue, Consts.contextLevelRelationship)
    let contextPotencyValue = +Consts.contextPotencyValue
    deepContext ---> (contextPotencyValue, Consts.contextPotencyRelationship)
    
    let simpleAttributeType = +Consts.simpleAttributeType
    simpleAttributeType --|> element

    
    