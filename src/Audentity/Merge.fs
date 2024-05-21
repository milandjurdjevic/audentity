module Audentity.Merge

open System.Collections.Frozen

let Traces (traces: Trace seq) =
    let all = traces |> _.ToFrozenSet()
    let owned = all |> Seq.filter _.IsOwned |> _.ToFrozenSet()

    // Check whether the trace is referenced by the reference.
    let isReferencing (reference: Reference) (trace: Trace) =
        let keys = trace.Properties |> Seq.filter (_.IsPrimaryKey) |> Seq.toArray

        let contains (link: Link) =
            keys
            |> Seq.exists (fun key -> key.Name = link.Name && key.CurrentValue = link.Value)

        trace.Type = reference.Type && reference.Links |> Seq.forall contains

    // Collect referenced properties, from references.
    let rec collectReferenceProperties (references: Reference seq) =
        seq {
            for reference in references do

                let toReferenced (property: Property) =
                    { property with
                        Name = $"{reference.Name}/{property.Name}" }

                let trace = all |> Seq.tryFind (isReferencing reference)

                if Option.isSome trace then
                    yield! trace.Value.Properties |> Seq.map toReferenced

                    yield! collectReferenceProperties trace.Value.References |> Seq.map toReferenced
        }

    let collectTraceReferences (trace: Trace) =
        trace
        |> _.References
        |> Seq.filter (fun reference -> owned |> Seq.exists (isReferencing reference))
        |> Seq.toArray

    let mapTrace (trace: Trace) =
        let references = collectTraceReferences trace

        { trace with
            References = references
            Properties =
                references
                |> collectReferenceProperties
                |> Seq.filter (fun prop -> not prop.IsPrimaryKey)
                |> Seq.append trace.Properties
                |> Seq.toArray }

    all |> Seq.filter (fun trace -> not trace.IsOwned) |> Seq.map mapTrace
