# TLK Development Roadmap

## Milestone 1: The Alpha Core (Ingestion & UI)
- [x] Stand up Python UI canvas (NodeGraphQt).
- [x] Create Machine, Cycle, and Sync nodes.
- [x] Export visual graph to structured JSON.
- [x] Build C# ingestion engine to read node parameters.
- [x] Parse the `"connections"` array in C# to trace the wires and build the sequential timeline.

## Milestone 2: The F# State Machine (The Brain)
- [x] Pass the C# sequential timeline into the F# class library.
- [ ] Build the Channel 1 vs. Channel 2 validation loop.
- [ ] Implement the "Wait/Sync" halt logic (ensuring Path 1 waits for Path 2).
- [ ] Return a validated, crash-free timeline back to C#.

## Milestone 3: The G-Code Post (The Output)
- [ ] Map the validated nodes to specific string formats (e.g., `TurnCycleNode` -> `G1 Z-0.500`).
- [ ] Output a basic, two-channel text file formatted for the Citizen L20.
- [ ] Run a test compile to visually verify the `!1L1` sync codes line up perfectly.

## Milestone 4: The AI Bridge (The End Goal)
- [ ] Create a rigid JSON schema template for the AI to follow.
- [ ] Prompt an LLM to generate a simple turning operation using our schema.
- [ ] Feed the AI-generated JSON directly into the C# engine to prove we can generate crash-free G-code entirely from text.