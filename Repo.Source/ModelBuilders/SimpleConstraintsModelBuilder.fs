namespace Repo.Metametamodels

open Repo.DataLayer
open Repo.CoreSemanticLayer
open Repo.InfrastructureSemanticLayer
open System.Runtime.InteropServices

/// Initializes repository with test model conforming to Robots Metamodel, actual program that can be written by end-user.
type SimpleConstraintsModelBuilder() =
   interface IModelBuilder with
       member this.Build(repo: IRepo): unit =
           let infrastructure = InfrastructureSemantic(repo)
           let metamodel = Repo.findModel repo "ConstraintsMetamodel"
           let infrastructureMetamodel = infrastructure.Metamodel.Model

           let metamodelAbstractNode = Model.findNode metamodel "AbstractNode"
           let metamodelInitialNode = Model.findNode metamodel "InitialNode"
           let metamodelFinalNode = Model.findNode metamodel "FinalNode"
           let metamodelMotorsForward = Model.findNode metamodel "MotorsForward"
           let metamodelTimer = Model.findNode metamodel "Timer"

           let metamodelAll = Model.findNode metamodel "AllNodes"
           //let metamodelAny = Model.findNode metamodel "AnyNodes"
           
           let metamodelOr = Model.findNode metamodel "OrNode"
           let metamodelNot = Model.findNode metamodel "NotNode"
           let metamodelNone = Model.findNode metamodel "NoNodes"


           let link = Model.findAssociationWithSource metamodelAbstractNode "target"

           let model = repo.CreateModel("SimpleConstraintsModel", metamodel)
           model.Properties <- model.Properties.Add ("IsVisible", false.ToString())

           let finalNode = infrastructure.Instantiate model metamodelFinalNode
           let noNode = infrastructure.Instantiate model metamodelNone

           let (-->) (src: IElement) dst =
               let aLink = infrastructure.Instantiate model link :?> IAssociation
               aLink.Source <- Some src
               aLink.Target <- Some dst
               dst

           
           finalNode --> noNode |> ignore
           ()