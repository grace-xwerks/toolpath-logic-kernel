# tlk-frontend

The visual node-graph layer of [TLK](../README.md). Build a CNC program by wiring nodes on a canvas, then export the graph as JSON for the kernel to consume.

## What's here

A single Python entry point: [`app.py`](app.py).

It launches a Qt window (PySide6) hosting a [NodeGraphQt](https://github.com/jchanvfx/NodeGraphQt) canvas with three node types defined inline:

| Node | Role | Key parameters |
|---|---|---|
| `MachineNode` | The "Environment" — machine config and channel outputs | `machine_model`, `is_swiss` |
| `TurnCycleNode` | An OD turning operation | `tool_call`, `target_dia`, `end_z` |
| `SyncNode` | Multi-channel wait/sync barrier | `sync_number` |

A "Compile & Export to JSON" button serializes the current graph (nodes + connections) and writes it to [`../tlk-schemas/active_program.json`](../tlk-schemas/active_program.json) — the same file the kernel ingests. **This overwrites the committed worked example**, so don't expect it to round-trip cleanly with the sample.

The canvas is auto-populated with a Machine → TurnCycle → Sync chain on launch so you have something to experiment with immediately.

## Running

```bash
pip install PySide6 NodeGraphQt
python app.py
```

There's no pinned manifest — versions float to whatever pip resolves today. If the UI behaves oddly, pinning known-good versions is the first thing to try.

## What it doesn't do (yet)

- Only three node types are wired into the registry. The schema (see [`../tlk-schemas/ai_schema.json`](../tlk-schemas/ai_schema.json)) anticipates more (drilling, etc.); add a corresponding `BaseNode` subclass and `graph.register_node(...)` call to expose them.
- No save/load of the graph back from JSON — export is one-way.
- No machine-parameter validation in the UI; that lives in the kernel's F# state machine.
