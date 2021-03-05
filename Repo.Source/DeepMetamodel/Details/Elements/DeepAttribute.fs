namespace Repo.DeepMetamodel.Details.Elements

open Repo.LanguageMetamodel
open Repo.DeepMetamodel

type DeepAttribute(attribute: ILanguageElement, pool: DeepPool, repo: ILanguageRepository, level: int, potency: int) =
    inherit DeepContext(level, potency)
    
    /// Returns underlying Attribute element for this attribute.
    member this.UnderlyingAttribute = attribute

    override this.ToString () =
        attribute.ToString ()

    interface IDeepAttribute with
        member this.Type = (attribute.OutgoingAssociation Consts.typeRelationship).Target
                           |> (fun e -> pool.Wrap e 0 0)
                           
        member this.Name = (attribute :?> ILanguageNode).Name


