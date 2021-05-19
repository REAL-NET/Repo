namespace Repo.DeepMetamodels

open Repo
open Repo.DeepMetamodel


type AtkinsonModelBuilder() =
    interface IDeepModelBuilder with
        member this.Build(repo: IDeepRepository): unit =
            let metametamodel = repo.InstantiateDeepMetamodel "AtkinsonMetameta" 
            
            let stringType = metametamodel.CreateNode "String" (-1) (-1)
            
            let ``component`` = metametamodel.CreateNode "Component" 0 2
            (``component``.AddAttribute "author" stringType 0 1).IsSingle <- true
            (``component``.AddAttribute "status" stringType 0 2).IsSingle <- true
            let node = metametamodel.CreateNode "Node" 0 2
            let description = node.AddAttribute "description" stringType 0 2
            let nodeKind = metametamodel.InstantiateNode "NodeKind" stringType
            node.AddSlot description nodeKind 0 0 |> ignore
            let residesOn = metametamodel.CreateAssociation ``component`` node "resides on" 0 2 (-1) (-1) (-1) (-1) 
            
            let metamodel = repo.InstantiateModel "AtkinsonMeta" metametamodel
            let c = metamodel.InstantiateNode "C" ``component``
            let c_author = Seq.find (fun (e: IDeepAttribute) -> e.Name = "author") c.Attributes
            let billBlogs = metamodel.InstantiateNode "Bill Bloggs" stringType
            c.AddSlot c_author billBlogs 1 0 |> ignore
            let n = metamodel.InstantiateNode "N" node
            let nodeType = metamodel.InstantiateNode "NodeType" stringType
            let n_description = Seq.find (fun (e: IDeepAttribute) -> e.Name = "description") n.Attributes
            n.AddSlot n_description nodeType 1 0 |> ignore
            let residesOn1 = metamodel.InstantiateAssociation c n "resides on" residesOn
            
            let model = repo.InstantiateModel "AtkinsonModel" metamodel
            let ci = model.InstantiateNode "CI" c
            let ci_status = Seq.find (fun (e: IDeepAttribute) -> e.Name = "status") ci.Attributes
            let active = model.InstantiateNode "Active" stringType
            ci.AddSlot ci_status active 2 0 |> ignore
            let ni = model.InstantiateNode "NI" n
            let aNode = model.InstantiateNode "aNode" stringType
            let ni_description = Seq.find (fun (e: IDeepAttribute) -> e.Name = "description") ni.Attributes
            ni.AddSlot ni_description aNode 2 0 |> ignore
            model.InstantiateAssociation ci ni "resides on" residesOn1 |> ignore

            ()
            

