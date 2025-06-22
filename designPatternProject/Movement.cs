namespace designPatternProject
{
    public record Movement(DateTime Timestamp,
                           string   Instruction,  // la ligne utilisateur complète
                           Dictionary<string,int> Delta); // <Nom, + / – quantité>
}
