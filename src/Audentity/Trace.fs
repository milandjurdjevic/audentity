namespace Audentity

open System
open System.Collections.Generic
open Microsoft.EntityFrameworkCore

type Link = { Name: string; Value: string option }

type Reference =
    { Name: string
      Type: Type
      Links: IReadOnlyCollection<Link> }

type Property =
    { Name: string
      CurrentValue: string option
      OriginalValue: string option
      IsPrimaryKey: bool }

type Trace =
    { Type: Type
      State: EntityState
      Properties: IReadOnlyCollection<Property>
      References: IReadOnlyCollection<Reference>
      IsOwned: bool }
