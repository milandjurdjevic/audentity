namespace Audentity

open System
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

    let collectionEntryObjectToReference (collectionEntry: CollectionEntry) (object: obj) =
        let links =
            collectionEntry.Metadata.TargetEntityType.GetProperties()
            |> Seq.filter (_.IsPrimaryKey())
            |> Seq.map (objectPropertyToLink object)
            |> Array.ofSeq

        { Name = collectionEntry.Metadata.Name
          Target = collectionEntry.Metadata.TargetEntityType.Name
          Links = links }

    let private collectionEntryToReferences (collectionEntry: CollectionEntry) =
        collectionEntry.CurrentValue
        |> Seq.cast<obj>
        |> Seq.map (collectionEntryObjectToReference collectionEntry)

    let private referenceEntryToReference (entry: ReferenceEntry) =
        let links =
            entry.TargetEntry.Properties
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

    let OfEntries (entries: EntityEntry seq) = entries |> Seq.map entityEntryToEntity
