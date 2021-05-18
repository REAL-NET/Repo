namespace Repo.DeepMetamodel

open System


type IDeepContext =
    interface
        abstract Level: int with get, set
        abstract Potency: int with get, set
    end

/// Element, most general thing that can be in a model.
and IDeepElement =
    interface
        inherit IDeepContext
        
        abstract Name: string with get, set
        
         /// Outgoing edges (all of possible kinds) for that element.
        abstract OutgoingEdges: IDeepRelationship seq with get
        
        /// Outgoing associations for that element.
        abstract OutgoingAssociations: IDeepAssociation seq with get

        /// Incoming associations for that element.
        abstract IncomingAssociations: IDeepAssociation seq with get

        /// Direct parents of this element in generalization hierarchy.
        abstract DirectSupertypes : IDeepElement seq with get

        /// A list of all attributes available for an element.
        abstract Attributes: IDeepAttribute seq with get
        
        /// Adds an attribute to element
        abstract AddAttribute: name: string -> ``type``: IDeepElement -> level: int -> potency: int -> IDeepAttribute
        
        /// A list of all slots for an element.
        abstract Slots: IDeepSlot seq with get
        
        /// Adds a slot to element
        abstract AddSlot: attribute: IDeepAttribute -> value: IDeepElement -> level: int -> potency: int -> IDeepSlot
        
        /// Returns all element that could be possible the values for given attribute
        abstract GetValuesForAttribute: attribute: IDeepAttribute -> seq<IDeepElement>

        /// Returns a model to which this element belongs.
        abstract Model: IDeepModel with get

        /// False when metatype of an element can not be represented in terms of Attribute Metamodel.
        /// InstanceOf edges always have this property set to false, to avoid infinite recursion.
        abstract HasMetatype: bool with get

        /// Returns an element that this element is an instance of (target of an "instanceOf" association).
        abstract Metatype: IDeepElement with get
    end

/// Attribute is like field in a class --- describes possible values of a field in instances. Has type 
/// (a set of possible values) and name.
and IDeepAttribute =
    interface
        inherit IDeepContext
        
        /// Is attribute single or dual
        abstract IsSingle: bool with get, set
        
        /// A type of an attribute. Restricts a set of possible values for corresponding slot.
        abstract Type: IDeepElement with get

        /// A name of an attribute.
        abstract Name: string with get
    end

/// An instance of attribute. Contains actual value.
and IDeepSlot =
    interface
        inherit IDeepContext

        /// Attribute that this slot is an instance of.
        abstract Attribute: IDeepAttribute with get

        /// Value of a slot.
        abstract Value: IDeepElement with get, set
    end

/// Node is a kind of element which can connect only to edge, corresponds to node of the model graph.
and IDeepNode =
    interface
        inherit IDeepElement
    end

/// Edge is a kind of element which can connect to everything.
and IDeepRelationship =
    interface
        inherit IDeepElement
        /// Element at the beginning of an edge, may be None if edge is not connected.
        abstract Source: IDeepElement with get

        /// Element at the ending of an edge, may be None if edge is not connected.
        abstract Target: IDeepElement with get
    end

/// Generalization is a kind of edge which has special semantic in metamodel (allows to inherit associations).
and IDeepGeneralization =
    interface
        inherit IDeepRelationship
    end

/// Association is a general kind of edge, has string attribute describing target of an edge.
and IDeepAssociation =
    interface
        inherit IDeepRelationship
        
        abstract MinSource: int with get
        abstract MaxSource: int with get
        abstract MinTarget: int with get
        abstract MaxTarget: int with get 
                
    end

/// InstanceOf is a kind of edge which has special semantic in entire metamodel stack. Means that source is an instance
/// of target. Every element shall have at least one such outgoing edge (except InstanceOf itself, for it we assume
/// that it is always an instance of InstanceOf node of a corresponding metamodel). Can cross metamodel boundaries. 
/// There can be several InstanceOf relations from the same element (for example, linguistic InstanceOf and ontological
/// InstanceOf), to differentiate between them subsequent metalevels can add attributes to this edge.
///
/// InstanceOf type is determined by metalevel of edge source and is governed by its linguistic metamodel (or just
/// metamodel, if linguistic and ontological metamodels are not differentiated on that metalevel). Note that
/// all linguistic metamodels shall have InstanceOf node and fully support InstanceOf semantics.
and IDeepInstanceOf =
    interface
        inherit IDeepRelationship
    end

/// Model is a set of nodes and edges, corresponds to one diagram (or one palette) in editor.
and IDeepModel =
    interface
        /// Model can have descriptive name (must be unique).
        abstract Name: string with get, set

        /// Returns true if metamodel of this model is representable in Attribute Metamodel semantics.
        abstract HasMetamodel: bool with get

        /// Metamodel is a model whose elements are types of elements for this model.
        /// Model can be a metamodel for itself.
        abstract Metamodel: IDeepModel with get

        /// Creates a new node in a model by instantiating Attribute Metamodel "Node".
        abstract CreateNode: name: string -> level: int -> potency: int -> IDeepNode

        /// Creates new Generalization edge with given source and target by instantiating 
        /// Attribute Metamodel "Generalization"
        abstract CreateGeneralization: 
            source: IDeepElement 
            -> target: IDeepElement
            -> name: string
            -> level: int
            -> potency: int
            -> IDeepGeneralization

        /// Creates new Association edge with given source and target by instantiating 
        /// Attribute Metamodel "Association".
        abstract CreateAssociation:
            source: IDeepElement
            -> target: IDeepElement
            -> name: string
            -> level: int
            -> potency: int
            -> minSource: int
            -> maxSource: int
            -> minTarget: int
            -> maxTarget: int
            -> IDeepAssociation

        /// Creates a new node in a model by instantiating given node from metamodel, supplying given values
        /// to its slots. Slots without values receive values from DefaultValue property of corresponding attribute.
        abstract InstantiateNode:
            name: string
            -> metatype: IDeepNode
            -> IDeepNode

        /// Creates a new association in a model by instantiating given association from metamodel.
        abstract InstantiateAssociation: 
            source: IDeepElement 
            -> target: IDeepElement
            -> name: string
            -> metatype: IDeepAssociation 
            -> IDeepAssociation

        /// Returns all elements in a model.
        abstract Elements: IDeepElement seq with get

        /// Returns all nodes in a model.
        abstract Nodes: IDeepNode seq with get

        /// Returns all edges in a model.
        abstract Relationships: IDeepRelationship seq with get

        /// Deletes element from a model and unconnects related elements if needed. Removes "hanging" edges.
        /// Nodes without connections are not removed automatically.
        abstract DeleteElement: element : IDeepElement -> unit

        /// Searches node in a model. If there are none or more than one node with given name, throws an exception.
        abstract Node: name: string -> IDeepNode

        /// Returns true if a node with given name exists in a model.
        abstract HasNode: name: string -> bool

        /// Searches association with given traget name in a model. If there are none or more than one association 
        /// with given name, throws an exception.
        abstract Association: name: string -> IDeepAssociation

        /// Returns true if an association with given target name exists in a model.
        abstract HasAssociation: name: string -> bool

        /// Prints model contents on a console.
        abstract PrintContents: unit -> unit
    end

/// Repository is a collection of models.
type IDeepRepository =
    interface
        /// All models in a repository.
        abstract Models: IDeepModel seq with get

        /// Creates and returns a new model in repository based on Deep Metamodel.
        abstract InstantiateDeepMetamodel: name: string -> IDeepModel

        /// Creates and returns a new model in repository based on a given metamodel.
        abstract InstantiateModel: name: string -> metamodel: IDeepModel -> IDeepModel

        /// Searches model in repository. 
        /// If there are none or more than one model with given name, throws an exception.
        abstract Model: name: string -> IDeepModel

        /// Deletes given model and all its elements from repository.
        abstract DeleteModel: model : IDeepModel -> unit

        /// Clears repository contents.
        abstract Clear : unit -> unit
    end
