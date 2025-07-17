using RobotFactory.Core;
using System;
using System.Collections.Generic;

namespace RobotFactory.Services
{
    public class InstructionBuilder
    {
        private readonly TemplateManager _templateManager;

        public InstructionBuilder(TemplateManager templateManager)
        {
            _templateManager = templateManager;
        }

        public List<Instruction> BuildInstructions(string robotName)
        {
            var instructions = new List<Instruction>();

            var template = _templateManager.GetTemplate(robotName);
            if (template == null) return instructions;

            // Début
            instructions.Add(new Instruction("PRODUCING", robotName));

            // GET_OUT_STOCK pour chaque pièce
            foreach (var piece in template.RequiredPieces)
            {
                instructions.Add(new Instruction("GET_OUT_STOCK", "1", piece));

                // INSTALL si c’est un Core => détection simple par nom
                if (piece.StartsWith("Core_"))
                {
                    // On suppose que tous les Core utilisent System_SB1
                    instructions.Add(new Instruction("INSTALL", "System_SB1", piece));
                }
            }

            // Assemblage fictif (simplifié)
            instructions.Add(new Instruction("ASSEMBLE", $"TMP_{robotName}", string.Join(" ", template.RequiredPieces)));

            // Fin
            instructions.Add(new Instruction("FINISHED", robotName));

            return instructions;
        }

        public List<Instruction> BuildMultiple(Dictionary<string, int> robotArgs)
        {
            var all = new List<Instruction>();

            foreach (var kv in robotArgs)
            {
                for (int i = 0; i < kv.Value; i++)
                {
                    all.AddRange(BuildInstructions(kv.Key));
                }
            }

            return all;
        }
    }
}
