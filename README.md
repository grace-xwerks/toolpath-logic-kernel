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