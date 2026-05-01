# tlk-schemas

The shared JSON contract between the [front-end](../tlk-frontend/), the [kernel](../tlk-kernel/), and the AI bridge. Data only — no build step.

## Files

| File | Role |
|---|---|
| [`active_program.json`](active_program.json) | The canonical handoff file. The front-end writes it on export; the kernel reads it on run. Committed copy is a worked example (Machine → TurnCycle → Sync) that produces [`../tlk-kernel/citizen_output.nc`](../tlk-kernel/citizen_output.nc). |
| [`ai_schema.json`](ai_schema.json) | The strict JSON Schema an LLM must conform to when generating a graph. Any text-to-G-code path bottoms out here. |

## Shape

A program is a `nodes` dictionary plus a `connections` array.

```json
{
  "nodes": {
    "node_1": {
      "type_": "MachineNode",
      "custom": { "machine_model": "Citizen_L20" }
    },
    "node_2": {
      "type_": "TurnCycleNode",
      "custom": { "tool_call": "T0303", "target_dia": "0.125", "end_z": "-0.850" }
    },
    "node_3": {
      "type_": "SyncNode",
      "custom": { "sync_number": "5" }
    }
  },
  "connections": [
    { "out": ["node_1", "Channel 1 State"], "in": ["node_2", "State In"] },
    { "out": ["node_2", "State Out"],       "in": ["node_3", "Ch 1 In"] }
  ]
}
```

- `type_` is the node-class discriminator. Today's supported values: `MachineNode`, `TurnCycleNode`, `SyncNode` (the kernel falls back to `Domain.Operation.Unknown` for anything else).
- `custom` carries node-specific parameters as **strings** (the front-end's text inputs serialize this way; the kernel parses to numeric types where needed).
- `connections` are bipartite pin references — `[node_id, pin_name]` on each side.

## Evolving the schema

Any change here ripples to the front-end (node definitions in `app.py`), the kernel (parsing in `Program.cs`, F# `Domain.Operation` cases, `PostProcessor` emission), and the AI schema (the LLM contract). Touch all four when adding a new node type — there's no central registry generating the others from this folder.

The `tlk-quoting-engine` repo also consumes this contract — see [`grace-xwerks/tlk-quoting-engine`](https://github.com/grace-xwerks/tlk-quoting-engine) before making breaking changes.
