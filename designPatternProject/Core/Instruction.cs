namespace RobotFactory.Core
{
    public class Instruction
    {
        public string Type { get; set; }        // PRODUCING, GET_OUT_STOCK, ASSEMBLE, INSTALL, etc.
        public string[] Args { get; set; }      // Liste d'arguments

        public Instruction(string type, params string[] args)
        {
            Type = type;
            Args = args;
        }

        public override string ToString()
        {
            return $"{Type} {string.Join(" ", Args)}";
        }
    }
}
