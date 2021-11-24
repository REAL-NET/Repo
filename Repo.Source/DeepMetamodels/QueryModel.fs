namespace Repo.DeepMetamodels

open Repo.DeepMetamodel

type QueryModelBuilder() =
    interface IDeepModelBuilder with
        member this.Build(repo: IDeepRepository): unit =
            let metamodel = repo.InstantiateDeepMetamodel "QueryMetamodel"

            let abstractQueryBlock = metamodel.CreateNode "Abstract node" (-1) (-1)
            let abstractQueryBlockXCoordinate = abstractQueryBlock.AddSimpleAttribute "xCoordinate" (-1) (-1)
            abstractQueryBlock.AddSimpleSlot abstractQueryBlockXCoordinate "" (-1) (-1) |> ignore
            let abstractQueryBlockYCoordinate = abstractQueryBlock.AddSimpleAttribute "yCoordinate" (-1) (-1)
            abstractQueryBlock.AddSimpleSlot abstractQueryBlockYCoordinate "" (-1) (-1) |> ignore

            let link = metamodel.CreateAssociation abstractQueryBlock abstractQueryBlock "link" (-1) (-1) (-1) (-1) (-1) (-1)
            let linkConnectionType = link.AddSimpleAttribute "connection type" (-1) (-1)
            link.AddSimpleSlot linkConnectionType "local" (-1) (-1) |> ignore

            let abstractRedBlock = metamodel.InstantiateNode "Red block" abstractQueryBlock
            let abstractRedBlockParent = abstractRedBlock.AddSimpleAttribute "parent" (-1) (-1)
            abstractRedBlock.AddSimpleSlot abstractRedBlockParent "" (-1) (-1) |> ignore
            let abstractRedBlockChildren = abstractRedBlock.AddSimpleAttribute "children" (-1) (-1)
            abstractRedBlock.AddSimpleSlot abstractRedBlockChildren "" (-1) (-1) |> ignore
            //let abstractRedBlockConnectionType = abstractRedBlock.AddSimpleAttribute "connection type" (-1) (-1)
            //abstractRedBlock.AddSimpleSlot abstractRedBlockConnectionType "local" (-1) (-1) |> ignore
            //metamodel.CreateGeneralization abstractQueryBlock abstractRedBlock "Abstract red block gen" (-1) (-1) |> ignore

            let abstractYellowBlock = metamodel.InstantiateNode "Yellow block" abstractQueryBlock
            let abstractYellowBlockParent = abstractYellowBlock.AddSimpleAttribute "parent" (-1) (-1)
            abstractYellowBlock.AddSimpleSlot abstractYellowBlockParent "" (-1) (-1) |> ignore
            //let abstractYellowBlockConnectionType = abstractYellowBlock.AddSimpleAttribute "connection type" (-1) (-1)
            //abstractYellowBlock.AddSimpleSlot abstractYellowBlockConnectionType "local" (-1) (-1) |> ignore
            let abstractYellowBlockArgument = abstractYellowBlock.AddSimpleAttribute "argument" (-1) (-1)
            abstractYellowBlock.AddSimpleSlot abstractYellowBlockArgument "" (-1) (-1) |> ignore
            //metamodel.CreateGeneralization abstractQueryBlock abstractYellowBlock "Abstract yellow block gen" (-1) (-1) |> ignore

            let abstractBlueBlock = metamodel.InstantiateNode "Blue block" abstractQueryBlock
            let abstractBlueBlockContents = abstractBlueBlock.AddSimpleAttribute "contents" (-1) (-1)
            abstractBlueBlock.AddSimpleSlot abstractBlueBlockContents "" (-1) (-1) |> ignore
            //metamodel.CreateGeneralization abstractQueryBlock abstractBlueBlock "Abstract blue block gen" (-1) (-1) |> ignore

            let aggregate = metamodel.InstantiateNode "Aggregate" abstractRedBlock
            let aggregateImage = aggregate.AddSimpleAttribute "image" (-1) (-1)
            aggregate.AddSimpleSlot aggregateImage "images/aggregate.png" (-1) (-1) |> ignore
            //metamodel.CreateGeneralization abstractRedBlock aggregate "Aggregate gen" (-1) (-1) |> ignore

            let ds = metamodel.InstantiateNode "DS" abstractRedBlock
            let dsImage = ds.AddSimpleAttribute "image" (-1) (-1)
            ds.AddSimpleSlot dsImage "images/ds.png" (-1) (-1) |> ignore
            //metamodel.CreateGeneralization abstractRedBlock ds "DS gen" (-1) (-1) |> ignore

            let holds = metamodel.InstantiateNode "HOLDS" abstractRedBlock
            let holdsImage = holds.AddSimpleAttribute "image" (-1) (-1)
            holds.AddSimpleSlot holdsImage "images/holds.png" (-1) (-1) |> ignore
            //metamodel.CreateGeneralization abstractRedBlock holds "Holds gen" (-1) (-1) |> ignore

            let materialize = metamodel.InstantiateNode "Materialize" abstractRedBlock
            let materializeImage = materialize.AddSimpleAttribute "image" (-1) (-1)
            materialize.AddSimpleSlot materializeImage "images/materialize.png" (-1) (-1) |> ignore
            //metamodel.CreateGeneralization abstractRedBlock materialize "Materialize gen" (-1) (-1) |> ignore

            let read = metamodel.InstantiateNode "Read" abstractRedBlock
            let readImage = read.AddSimpleAttribute "image" (-1) (-1)
            read.AddSimpleSlot readImage "images/read.png" (-1) (-1) |> ignore
            //metamodel.CreateGeneralization abstractRedBlock read "Read gen" (-1) (-1) |> ignore

            let queryModel = repo.InstantiateModel "QueryModel" metamodel

            let holdsInstance = queryModel.InstantiateNode "HOLDS_1" holds
            let holdsInstanceXCoordinate = Seq.find (fun (e: IDeepSlot) -> e.Attribute.Name = "xCoordinate") holdsInstance.Slots
            holdsInstanceXCoordinate.SimpleValue <- "100"
            let holdsInstanceYCoordinate = Seq.find (fun (e: IDeepSlot) -> e.Attribute.Name = "yCoordinate") holdsInstance.Slots
            holdsInstanceYCoordinate.SimpleValue <- "100"
            let holdsInstanceChildren = Seq.find (fun (e: IDeepSlot) -> e.Attribute.Name = "children") holdsInstance.Slots
            holdsInstanceChildren.SimpleValue <- "Materialize_1"

            let materializeInstance = queryModel.InstantiateNode "Materialize_1" materialize
            let materializeInstanceXCoordinate = Seq.find (fun (e: IDeepSlot) -> e.Attribute.Name = "xCoordinate") materializeInstance.Slots
            materializeInstanceXCoordinate.SimpleValue <- "100"
            let materializeInstanceYCoordinate = Seq.find (fun (e: IDeepSlot) -> e.Attribute.Name = "yCoordinate") materializeInstance.Slots
            materializeInstanceYCoordinate.SimpleValue <- "200"
            let materializeInstanceParent = Seq.find (fun (e: IDeepSlot) -> e.Attribute.Name = "parent") materializeInstance.Slots
            materializeInstanceParent.SimpleValue <- "HOLDS_1"
            let materializeInstanceChildren = Seq.find (fun (e: IDeepSlot) -> e.Attribute.Name = "children") materializeInstance.Slots
            materializeInstanceChildren.SimpleValue <- "DS_1"

            let dsInstance = queryModel.InstantiateNode "DS_1" ds
            let dsInstanceXCoordinate = Seq.find (fun (e: IDeepSlot) -> e.Attribute.Name = "xCoordinate") dsInstance.Slots
            dsInstanceXCoordinate.SimpleValue <- "100"
            let dsInstanceYCoordinate = Seq.find (fun (e: IDeepSlot) -> e.Attribute.Name = "yCoordinate") dsInstance.Slots
            dsInstanceYCoordinate.SimpleValue <- "300"
            let dsInstanceParent = Seq.find (fun (e: IDeepSlot) -> e.Attribute.Name = "parent") dsInstance.Slots
            dsInstanceParent.SimpleValue <- "Materialize_1"

            queryModel.InstantiateAssociation holdsInstance materializeInstance "HOLDS_Materialize_1" link |> ignore
            queryModel.InstantiateAssociation materializeInstance dsInstance "Materialize_DS_1" link |> ignore

            ()
            
            
