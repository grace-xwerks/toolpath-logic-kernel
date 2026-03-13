using System;
using System.Text;
using System.Collections.Generic;
using TLK.Logic;

namespace TLK.Core
{
    public static class PostProcessor
    {
        public static void GenerateCitizenGCode(IEnumerable<Domain.Operation> validatedTimeline)
        {
            Console.WriteLine("\n--- Compiling Citizen L20 G-Code ---");
            
            StringBuilder program = new StringBuilder();
            program.AppendLine("%");
            program.AppendLine("O1234 (TLK ALPHA GENERATED)");
            program.AppendLine("G99 G40 (Standard Prep)");
            program.AppendLine();

            foreach (var op in validatedTimeline)
            {
                // The F# compiler automatically generates these "Is..." properties for us!
                if (op.IsInitializeMachine)
                {
                    program.AppendLine("(--- START MAIN SPINDLE ---)");
                }
                else if (op.IsTurnOD)
                {
                    // Cast the operation to access the nested data
                    var turnData = (Domain.Operation.TurnOD)op;
                    string tool = turnData.Item.Tool;
                    double endZ = turnData.Item.EndZ;
                    
                    program.AppendLine($"{tool} (TURN OD)");
                    program.AppendLine("G0 X[CLEARANCE] Z0.1");
                    program.AppendLine($"G1 Z{endZ:F4} F0.002");
                }
                else if (op.IsSyncWait)
                {
                    var syncData = (Domain.Operation.SyncWait)op;
                    int code = syncData.syncCode; // Grab it by its actual F# name!
                    program.AppendLine($"!1L{code} (WAIT FOR CH2)");
                }
            }

            program.AppendLine("M30");
            program.AppendLine("%");

            Console.WriteLine(program.ToString());
        }
    }
}