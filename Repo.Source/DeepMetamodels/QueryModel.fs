namespace Repo.DeepMetamodels

open Repo.DeepMetamodel

type QueryModelBuilder() =
    interface IDeepModelBuilder with
        member this.Build(repo: IDeepRepository): unit =
            let metametametamodel = repo.InstantiateDeepMetamodel "QueryMetametametamodel"

            let abstractQueryBlock = metametametamodel.CreateNode "Abstract node" 0 3
            let abstractQueryBlockXCoordinate = abstractQueryBlock.AddSimpleAttribute "xCoordinate" 0 0
            abstractQueryBlock.AddSimpleSlot abstractQueryBlockXCoordinate "" 0 0 |> ignore
            let abstractQueryBlockYCoordinate = abstractQueryBlock.AddSimpleAttribute "yCoordinate" 0 0
            abstractQueryBlock.AddSimpleSlot abstractQueryBlockYCoordinate "" 0 0 |> ignore

            let link = metametametamodel.CreateAssociation abstractQueryBlock abstractQueryBlock "link" 0 1 (-1) (-1) (-1) (-1)
            let linkConnectionType = link.AddSimpleAttribute "connection type" 0 1
            link.AddSimpleSlot linkConnectionType "local" 0 0 |> ignore

            let metametamodel = repo.InstantiateModel "QueryMetametamodel" metametametamodel

            let operatorBlock = metametamodel.InstantiateNode "Operator block" abstractQueryBlock
            let abstractRedBlockParent = operatorBlock.AddSimpleAttribute "parent" 1 0
            operatorBlock.AddSimpleSlot abstractRedBlockParent "" 0 0|> ignore
            let abstractRedBlockChildren = operatorBlock.AddSimpleAttribute "children" 1 0
            operatorBlock.AddSimpleSlot abstractRedBlockChildren "" 0 0 |> ignore
            //let abstractRedBlockConnectionType = abstractRedBlock.AddSimpleAttribute "connection type" (-1) (-1)
            //abstractRedBlock.AddSimpleSlot abstractRedBlockConnectionType "local" (-1) (-1) |> ignore
            //metamodel.CreateGeneralization abstractQueryBlock abstractRedBlock "Abstract red block gen" (-1) (-1) |> ignore

            let readerBlock = metametamodel.InstantiateNode "Reader block" abstractQueryBlock
            let abstractYellowBlockParent = readerBlock.AddSimpleAttribute "parent" 1 0
            readerBlock.AddSimpleSlot abstractYellowBlockParent "" 0 0 |> ignore
            //let abstractYellowBlockConnectionType = abstractYellowBlock.AddSimpleAttribute "connection type" (-1) (-1)
            //abstractYellowBlock.AddSimpleSlot abstractYellowBlockConnectionType "local" (-1) (-1) |> ignore
            let abstractYellowBlockArgument = readerBlock.AddSimpleAttribute "argument"  1 0
            readerBlock.AddSimpleSlot abstractYellowBlockArgument "" 0 0 |> ignore
            //metamodel.CreateGeneralization abstractQueryBlock abstractYellowBlock "Abstract yellow block gen" (-1) (-1) |> ignore

            let operatorInternalsBlock = metametamodel.InstantiateNode "Operator internals block" abstractQueryBlock
            let abstractBlueBlockContents = operatorInternalsBlock.AddSimpleAttribute "contents" 1 0
            operatorInternalsBlock.AddSimpleSlot abstractBlueBlockContents "" 0 0 |> ignore
            //metamodel.CreateGeneralization abstractQueryBlock abstractBlueBlock "Abstract blue block gen" (-1) (-1) |> ignore

            let metamodel = repo.InstantiateModel "QueryMetamodel" metametamodel

            let aggregate = metamodel.InstantiateNode "Aggregate" operatorBlock
            let aggregateImage = aggregate.AddSimpleAttribute "image" 2 0
            aggregate.AddSimpleSlot aggregateImage "images/aggregate.png" 0 0 |> ignore
            //metamodel.CreateGeneralization abstractRedBlock aggregate "Aggregate gen" (-1) (-1) |> ignore

            let ds = metamodel.InstantiateNode "DS" operatorBlock
            let dsImage = ds.AddSimpleAttribute "image" 2 0
            ds.AddSimpleSlot dsImage "images/ds.png" 0 0 |> ignore
            //metamodel.CreateGeneralization abstractRedBlock ds "DS gen" (-1) (-1) |> ignore

            let holds = metamodel.InstantiateNode "HOLDS" operatorBlock
            let holdsImage = holds.AddSimpleAttribute "image" 2 0
            holds.AddSimpleSlot holdsImage "images/holds.png" 0 0 |> ignore
            //metamodel.CreateGeneralization abstractRedBlock holds "Holds gen" (-1) (-1) |> ignore

            let materialize = metamodel.InstantiateNode "Materialize" operatorBlock
            let materializeImage = materialize.AddSimpleAttribute "image" 2 0
            materialize.AddSimpleSlot materializeImage "images/materialize.png" 0 0 |> ignore
            //metamodel.CreateGeneralization abstractRedBlock materialize "Materialize gen" (-1) (-1) |> ignore

            let read = metamodel.InstantiateNode "Read" operatorBlock
            let readImage = read.AddSimpleAttribute "image" 2 0
            read.AddSimpleSlot readImage "images/read.png" 0 0 |> ignore
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
