# Toolpath Logic Kernel (TLK)

TLK is an open-source, node-based CAM interface and kinematic math engine. It fundamentally changes how CNC programming is visualized by replacing bloated 3D CAD environments with a visual, state-machine dataflow (similar to LabVIEW). 

It is designed natively for AI integration, utilizing structured JSON as a bridge between conversational intent and deterministic machine code.

## Core Architecture

The architecture is strictly separated into three distinct layers to prevent logic hallucination and ensure mathematically perfect toolpaths:

1. **The Front-End (Visual UI):** A node-graph canvas where users construct programs by wiring "Machine Nodes" and "Cycle SubVIs" together. This layer manages the visual state-machine and handles multi-channel synchronization (e.g., Swiss turning). It outputs purely structured JSON.
2. **The Back-End (Kinematic Engine):** A high-speed math engine built to ingest the JSON output, calculate tool vectors against immutable machine parameters, and generate crash-free, shop-ready G-code.
3. **The AI Layer (The Bridge):** A text-to-JSON translation layer allowing Large Language Models to generate node graphs via natural language prompts, bypassing the need for the AI to perform spatial math.

## Project Structure

* `/tlk-frontend`: Python-based visual node graph.
* `/tlk-kernel`: C#/F# core engine for processing JSON into G-code.
* `/tlk-schemas`: Standardized JSON structures for machine definitions and toolpath cycles.

## Development Philosophy

TLK separates the "Environment" from the "Action." Machine nodes dictate the immutable physical realities (axis limits, interference zones), while Cycle SubVIs execute standardized cutting logic referencing those master limits.

## Status

_Last updated 2026-05-01._

All four roadmap milestones are marked complete in [`TODO.md`](TODO.md):

| Milestone | What it proves | Status |
|---|---|---|
| 1. Alpha core | Python UI exports a node graph to JSON; C# ingests it into a sequential timeline | ✅ |
| 2. F# state machine | Two-channel validation with wait/sync halt logic returns a crash-free timeline | ✅ |
| 3. G-code post | Validated timeline maps to Citizen L20 two-channel output (`!1L1` sync codes line up) | ✅ |
| 4. AI bridge | LLM-authored JSON (against [`tlk-schemas/ai_schema.json`](tlk-schemas/ai_schema.json)) feeds the same engine and yields crash-free G-code | ✅ |

A worked example is committed: [`tlk-schemas/active_program.json`](tlk-schemas/active_program.json) is the input that produced [`tlk-kernel/citizen_output.nc`](tlk-kernel/citizen_output.nc).

## Getting started

Each layer runs independently; they communicate through JSON files matching the schemas in `tlk-schemas/`.

### Front-end (Python node graph)

```bash
cd tlk-frontend
pip install PySide6 NodeGraphQt   # no pinned manifest yet — see TODO
python app.py
```

Build a graph (Machine → Cycles → Sync), then export to JSON.

### Kernel (C# ingestion + F# state machine + G-code post)

```bash
cd tlk-kernel
dotnet build TLK_Kernel.slnx
dotnet run --project TLK.Core            # ingests JSON, runs F# validator, writes citizen_output.nc
```

Requires the .NET SDK (project targets the modern toolchain via the C# / F# project files).

### Schemas

`tlk-schemas/` is data-only — no build step. `ai_schema.json` is the contract for LLM-generated graphs; `active_program.json` is the canonical worked example.

## Contributing

Issues and PRs welcome at [`grace-xwerks/toolpath-logic-kernel`](https://github.com/grace-xwerks/toolpath-logic-kernel). For integrations that consume TLK output (e.g. quoting, scheduling), the canonical contract is in `tlk-schemas/` — please open an issue before evolving the JSON shape.

## License

No LICENSE file is committed yet. The README describes TLK as open-source; pin the actual license before external contributions land.