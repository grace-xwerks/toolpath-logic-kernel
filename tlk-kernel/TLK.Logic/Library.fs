namespace TLK.Logic

module StateMachine =

    // Define the exact, immutable states a channel can be in
    type ChannelState =
        | Idle
        | Machining of ToolNumber: int * Operation: string
        | WaitingForSync of SyncCode: string
        | ToolChange of TargetTool: int

    // Define the physical channels of the machine
    type MachineChannel =
        | Path1 // Main Spindle
        | Path2 // Sub Spindle
        | Path3 // Optional Gang/3rd Path

    // The Master Context: A snapshot of the machine at any given millisecond
    type MachineContext = {
        Channel1State: ChannelState
        Channel2State: ChannelState
        MainSpindleRPM: int
        SubSpindleRPM: int
        IsClamped: bool
    }

    // A foundational function to evaluate if a sync is safe to execute
    let evaluateSync (context: MachineContext) (requiredSync: string) =
        match context.Channel1State, context.Channel2State with
        | WaitingForSync s1, WaitingForSync s2 when s1 = requiredSync && s2 = requiredSync ->
            // Both channels are ready and waiting for the exact same sync code
            Ok "Sync Confirmed: Safe to proceed."
        | _ ->
            // One of the channels is busy or waiting for a different code
            Error "Sync Fault: Channels are not aligned!"