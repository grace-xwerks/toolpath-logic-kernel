namespace TLK.Logic

module Domain =
    type TurnParameters = {
        Diameter: float
        EndZ: float
        Tool: string
    }

    type Operation =
        | InitializeMachine of modelName: string
        | TurnOD of TurnParameters
        | SyncWait of syncCode: int
        | Unknown of rawData: string

    type MachineState = {
        CurrentX: float
        CurrentZ: float
        ActiveTool: string
    }

module StateMachine =
    type ChannelState =
        | Idle
        | Machining of ToolNumber: int * Operation: string
        | WaitingForSync of SyncCode: string
        | ToolChange of TargetTool: int

    type MachineChannel =
        | Path1 
        | Path2 
        | Path3 

    type MachineContext = {
        Channel1State: ChannelState
        Channel2State: ChannelState
        MainSpindleRPM: int
        SubSpindleRPM: int
        IsClamped: bool
    }

    let evaluateSync (context: MachineContext) (requiredSync: string) =
        match context.Channel1State, context.Channel2State with
        | WaitingForSync s1, WaitingForSync s2 when s1 = requiredSync && s2 = requiredSync ->
            Ok "Sync Confirmed: Safe to proceed."
        | _ ->
            Error "Sync Fault: Channels are not aligned!"

    // The Execution Engine: Processes one operation and updates the machine's state
    let ProcessOperation (context: MachineContext) (op: Domain.Operation) =
        match op with
        | Domain.Operation.InitializeMachine model ->
            printfn "[F# Engine] Booting Kinematics for %s..." model
            context 

        | Domain.Operation.TurnOD p ->
            printfn "[F# Engine] Path 1 Active: Turning to Z%f" p.EndZ
            { context with Channel1State = Machining(0, "Turn") } 

        | Domain.Operation.SyncWait code ->
            printfn "[F# Engine] Path 1 HALTED: Requesting Sync !%d" code
            
            let waitingContext = { context with Channel1State = WaitingForSync(code.ToString()) }
            
            match evaluateSync waitingContext (code.ToString()) with
            | Ok msg -> printfn "   -> %s" msg
            | Error msg -> printfn "   -> %s (Channel 2 is not at this sync point!)" msg
            
            waitingContext

        | _ -> context