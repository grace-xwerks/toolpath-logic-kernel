# tlk-kernel

The back-end of [TLK](../README.md): ingest the JSON exported by the front-end, validate the multi-channel timeline in F#, and emit shop-ready G-code for the target machine.

## Layout

```
tlk-kernel/
├── TLK_Kernel.slnx          # Solution file referencing the two projects
├── TLK.Core/                # C# — ingestion, orchestration, G-code emission
│   ├── Program.cs           #   Entry point: reads active_program.json, runs the pipeline
│   └── PostProcessor.cs     #   Emits Citizen L20 G-code from the validated timeline
├── TLK.Logic/               # F# — domain types + state machine (the "brain")
│   └── Library.fs           #   Operation union, MachineContext, ProcessOperation
└── citizen_output.nc        # Last G-code output, committed as a worked example
```

The C# project references the F# project; together they target the modern .NET SDK (see each `.csproj` / `.fsproj` for the specific TFM).

## Pipeline

1. **Read JSON** — `Program.Main` loads [`../tlk-schemas/active_program.json`](../tlk-schemas/active_program.json).
2. **Materialize as F# types** — each node becomes a `Domain.Operation` discriminated union case (`InitializeMachine | TurnOD | SyncWait | Unknown`), keyed by node id.
3. **Trace connections** — the JSON `connections` array is walked to print the wire flow (sequential timeline construction).
4. **Run the state machine** — `StateMachine.ProcessOperation` folds operations over a `MachineContext` (per-channel state, spindle RPMs, clamp state). `evaluateSync` enforces multi-channel barrier alignment.
5. **Emit G-code** — `PostProcessor.GenerateCitizenGCode` walks the validated operations and writes Citizen L20 two-channel output, with `!1L<n>` sync codes.

## Running

```bash
dotnet build TLK_Kernel.slnx
dotnet run --project TLK.Core
```

Reads `../tlk-schemas/active_program.json`; writes `citizen_output.nc` next to the project (the path is resolved relative to the build output dir).

A clean run prints node digestion, wire tracing, F# state-machine output, and the final G-code to stdout — see the committed [`citizen_output.nc`](citizen_output.nc) for what a successful end-to-end pass looks like.

## Where things live

- **Adding a new operation type** — define it in `Library.fs` (`Domain.Operation`), pattern-match it in `StateMachine.ProcessOperation`, then handle it in `Program.cs` (JSON → F# type) and `PostProcessor.cs` (F# type → G-code).
- **Targeting a different control** — `PostProcessor` is currently Citizen-L20-specific (`!1L<n>` sync codes, two-channel layout, `M30`/`%` framing). A second post would live alongside it; the F# layer is control-agnostic.
- **Validation rules** — `evaluateSync` is the only crash-prevention check today. Axis-limit and interference-zone checks belong here, on `MachineContext`.
