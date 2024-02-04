namespace Audentity

open System
open System.Collections.Generic
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.ChangeTracking
open Microsoft.EntityFrameworkCore.Metadata

type Link = { Name: string; Value: string }

type Property =
    { Name: string
      OriginalValue: string
      CurrentValue: string }

type Reference =
    { Name: string
      Target: string
      Links: Link array }

type Entity =
    { Name: string
      State: EntityState
      Properties: Property array
      References: Reference array }

module Tracker =
    let private tryNullable object =
        if isNull object then None else Some object

    let private objectToString (object: obj) =
        if isNull object then String.Empty else object.ToString()

    let private propertyEntryToProperty (entry: PropertyEntry) =
        { Name = entry.Metadata.Name
          CurrentValue = objectToString entry.CurrentValue
          OriginalValue = objectToString entry.OriginalValue }

    let private propertyEntryToLink (entry: PropertyEntry) =
        { Name = entry.Metadata.Name
          Value = objectToString entry.CurrentValue }

    let private objectPropertyToLink (object: obj) (property: IProperty) =
        let value =
            match tryNullable property.PropertyInfo with
            | None -> String.Empty
            | Some some -> some.GetValue(object) |> objectToString

        { Name = property.Name; Value = value }

    let navigationEntryToLinkLessReference (entry: NavigationEntry) =
        { Name = entry.Metadata.Name
          Target = entry.Metadata.TargetEntityType.Name
          Links = Array.empty }

    let collectionEntryToReference (metadata: INavigationBase) (object: obj) =
        let links =
            metadata.TargetEntityType.GetProperties()
            |> Seq.filter (_.IsPrimaryKey())
            |> Seq.map (objectPropertyToLink object)
            |> Array.ofSeq

        { Name = metadata.Name
          Target = metadata.TargetEntityType.Name
          Links = links }

    let private collectionEntryToReferences (collectionEntry: CollectionEntry) =
        collectionEntry.CurrentValue
        |> Seq.cast<obj>
        |> Seq.map (collectionEntryToReference collectionEntry.Metadata)

    let private referenceEntryToReference (entry: ReferenceEntry) =
        let links =
            match tryNullable entry.TargetEntry with
            | None -> Array.empty
            | Some some ->
                some.Properties
                |> Seq.filter (_.Metadata.IsPrimaryKey())
                |> Seq.map propertyEntryToLink
                |> Array.ofSeq

        { Name = entry.Metadata.Name
          Target = entry.Metadata.TargetEntityType.Name
          Links = links }

    let private navigationEntryToReferences (entry: NavigationEntry) =
        seq {
            match entry with
            | :? ReferenceEntry as referenceEntry -> yield referenceEntryToReference referenceEntry
            | :? CollectionEntry as collectionEntry -> yield! collectionEntryToReferences collectionEntry
            | _ -> yield navigationEntryToLinkLessReference entry
        }

    let private entityEntryToEntity (entry: EntityEntry) =
        { Name = entry.Metadata.Name
          State = entry.State
          Properties = entry.Properties |> Seq.map propertyEntryToProperty |> Array.ofSeq
          References = entry.Navigations |> Seq.collect navigationEntryToReferences |> Array.ofSeq }

    let private entryIsNotShadow (entry: EntityEntry) =
        not (entry.Metadata.ClrType = typeof<Dictionary<string, obj>>)

    let OfEntries (entries: EntityEntry seq) =
        entries |> Seq.filter entryIsNotShadow |> Seq.map entityEntryToEntity
