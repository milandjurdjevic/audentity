module Audentity.Collect

open Microsoft.EntityFrameworkCore.ChangeTracking
open Microsoft.EntityFrameworkCore.Metadata
open System.Collections.Frozen

let Traces (entries: EntityEntry seq) =

    // Map links using the object primary keys.
    let mapLinks (primaryKeys: IProperty seq) (object: obj) =
        let mapLinkValue (key: IReadOnlyPropertyBase) =
            key.PropertyInfo
            |> Option.ofObj
            |> Option.map (_.GetValue(object))
            |> Option.map Option.ofObj
            |> Option.flatten
            |> Option.map (_.ToString())

        let mapLink (key: IReadOnlyPropertyBase) =
            { Name = key.Name
              Value = mapLinkValue key }

        primaryKeys |> Seq.map mapLink

    // Map link from property entry.
    let mapLink (property: PropertyEntry) =
        { Name = property.Metadata.Name
          Value = property.CurrentValue |> Option.ofObj |> Option.map (_.ToString()) }

    // Map entry reference.
    let mapReference (entry: NavigationEntry) links =
        { Name = entry.Metadata.Name
          Type = entry.Metadata.TargetEntityType.ClrType
          Links = links |> Seq.toArray }

    // Map navigation entry references.
    let mapReferences (entry: NavigationEntry) =
        match entry with
        // Map reference entry.
        | :? ReferenceEntry as referenceEntry when not (isNull referenceEntry.TargetEntry) ->
            referenceEntry.TargetEntry.Properties
            |> Seq.filter (_.Metadata.IsPrimaryKey())
            |> Seq.map mapLink
            |> mapReference entry
            |> Seq.singleton

        // Map collection entry.
        | :? CollectionEntry as collectionEntry when not (isNull collectionEntry.CurrentValue) ->
            // Cache primary keys.
            let primaryKeys =
                collectionEntry.Metadata.TargetEntityType.GetProperties()
                |> Seq.filter (_.IsPrimaryKey())
                |> _.ToFrozenSet()

            collectionEntry.CurrentValue
            |> Seq.cast<obj>
            |> Seq.map (mapLinks primaryKeys)
            |> Seq.map (mapReference entry)

        // Map empty reference entry.
        | _ -> mapReference entry Array.empty |> Seq.singleton

    // Map property from property entry.
    let mapProperty (entry: PropertyEntry) =
        { Name = entry.Metadata.Name
          CurrentValue = entry.CurrentValue |> Option.ofObj |> Option.map (_.ToString())
          OriginalValue = entry.OriginalValue |> Option.ofObj |> Option.map (_.ToString())
          IsPrimaryKey = entry.Metadata.IsPrimaryKey() }

    // Map trace from entity entry.
    let mapTrace (entry: EntityEntry) =
        { Type = entry.Metadata.ClrType
          State = entry.State
          Properties = entry.Properties |> Seq.map mapProperty |> Seq.toArray
          References = entry.References |> Seq.collect mapReferences |> Seq.toArray
          IsOwned = entry.Metadata.IsOwned() }

    entries |> Seq.map mapTrace
