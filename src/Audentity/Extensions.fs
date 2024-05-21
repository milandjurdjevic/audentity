namespace Audentity

open System.Runtime.CompilerServices
open Microsoft.EntityFrameworkCore.ChangeTracking

type Extensions() =

    [<Extension>]
    static member Collect(entries: EntityEntry seq) = Collect.Traces entries

    [<Extension>]
    static member Merge(traces: Trace seq) = Merge.Traces traces
