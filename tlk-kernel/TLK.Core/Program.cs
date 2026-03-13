using TLK.Logic;

using System;
using System.IO;
using System.Text.Json;

namespace TLK.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- TLK Kinematic Engine Initializing ---");

           // 1. Locate the exported JSON from the UI
            // AppContext.BaseDirectory puts us in bin/Debug/net10.0/. We need to go up 5 levels.
            string jsonPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "tlk-schemas", "active_program.json"));

            if (!File.Exists(jsonPath))
            {
                Console.WriteLine($"[ERROR] Could not find active program at: {jsonPath}");
                return;
            }
            // 2. Read and parse the JSON document
            string jsonString = File.ReadAllText(jsonPath);
            using JsonDocument doc = JsonDocument.Parse(jsonString);
            JsonElement root = doc.RootElement;

            Console.WriteLine($"SUCCESS: Loaded {jsonPath}\n");
            Console.WriteLine("--- Digesting Nodes ---");

            // 3. Dig into the "nodes" dictionary and map to strict F# Types
            Console.WriteLine("--- Digesting Nodes into F# Types ---");
            
            // Look closely: The dictionary now holds F# Operations, not strings!
            Dictionary<string, Domain.Operation> nodeDictionary = new Dictionary<string, Domain.Operation>();

            if (root.TryGetProperty("nodes", out JsonElement nodesElement))
            {
                foreach (JsonProperty node in nodesElement.EnumerateObject())
                {
                    string nodeId = node.Name; 
                    JsonElement nodeData = node.Value;
                    
                    if (nodeData.TryGetProperty("type_", out JsonElement typeElement))
                    {
                        string cleanType = (typeElement.GetString() ?? "").Split('.')[^1];

                        if (nodeData.TryGetProperty("custom", out JsonElement props))
                        {
                            if (cleanType == "MachineNode")
                            {
                                string model = props.GetProperty("machine_model").GetString() ?? "Unknown";
                                nodeDictionary[nodeId] = Domain.Operation.NewInitializeMachine(model);
                                Console.WriteLine($"> F# Type Created: InitializeMachine ({model})");
                            }
                            else if (cleanType == "TurnCycleNode")
                            {
                                string tool = props.GetProperty("tool_call").GetString() ?? "Unknown";
                                double dia = double.Parse(props.GetProperty("target_dia").GetString() ?? "0.0");
                                double endZ = double.Parse(props.GetProperty("end_z").GetString() ?? "0.0");

                                // Create the F# TurnParameters record
                                var turnParams = new Domain.TurnParameters(dia, endZ, tool);
                                
                                // Wrap it in the F# Operation union
                                nodeDictionary[nodeId] = Domain.Operation.NewTurnOD(turnParams);
                                Console.WriteLine($"> F# Type Created: TurnOD (Z{endZ})");
                            }
                            else if (cleanType == "SyncNode")
                            {
                                int syncNum = int.Parse(props.GetProperty("sync_number").GetString() ?? "0");
                                nodeDictionary[nodeId] = Domain.Operation.NewSyncWait(syncNum);
                                Console.WriteLine($"> F# Type Created: SyncWait (!{syncNum})");
                            }
                        }
                    }
                }
            }

            Console.WriteLine("\n--- Tracing Logic Flow ---");

            // 4. Parse the "connections" array to map the wires
            if (root.TryGetProperty("connections", out JsonElement connectionsElement))
            {
                foreach (JsonElement conn in connectionsElement.EnumerateArray())
                {
                    // "out" is an array: ["NodeID", "PinName"]
                    JsonElement outPin = conn.GetProperty("out");
                    string fromId = outPin[0].GetString() ?? "";
                    string fromPort = outPin[1].GetString() ?? "";

                    // "in" is an array: ["NodeID", "PinName"]
                    JsonElement inPin = conn.GetProperty("in");
                    string toId = inPin[0].GetString() ?? "";
                    string toPort = inPin[1].GetString() ?? "";

                    // Look up our friendly names using the dictionary
                    // Look up our friendly names using the dictionary
                    string fromName = nodeDictionary.ContainsKey(fromId) ? nodeDictionary[fromId].ToString() : "Unknown";
                    string toName = nodeDictionary.ContainsKey(toId) ? nodeDictionary[toId].ToString() : "Unknown";

                    Console.WriteLine($"[Wire] {fromName} ({fromPort})  --->  {toName} ({toPort})");
                }
            }
            
            Console.WriteLine("\n--- F# Kinematic Processing ---");
            
            // 1. Create a blank starting state for the machine (both channels Idle)
            var initialState = new StateMachine.MachineContext(
                StateMachine.ChannelState.Idle,
                StateMachine.ChannelState.Idle,
                0, 0, false
            );

            // 2. Feed the operations into the F# brain one by one
            var currentState = initialState;
            foreach (var node in nodeDictionary.Values)
            {
                currentState = StateMachine.ProcessOperation(currentState, node);
            }
            // 5. Hand the validated timeline over to the G-Code Compiler
            PostProcessor.GenerateCitizenGCode(nodeDictionary.Values);
        }
    }
}