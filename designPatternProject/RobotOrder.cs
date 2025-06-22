namespace designPatternProject
{
    public record RobotOrder(string RobotName,
                             int    Qty,
                             List<ModItem> Mods);
}
