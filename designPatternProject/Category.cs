namespace designPatternProject
{
    public enum Category
    {
        G,
        D,
        I,
        M 
    }
    public enum ModOp { WITH, WITHOUT, REPLACE }
    public record ModItem(ModOp Op,
                      string PieceName,
                      int    Qty, 
                      string? NewPiece = null); 

    public static class Compatibility
    {
        public static bool IsCompatible(Category robotCat, Category pieceCat, bool isSystem)
        {
            switch (robotCat)
            {
                case Category.D:
                    return pieceCat == Category.G || pieceCat == Category.D || pieceCat == Category.I;

                case Category.I:
                    return pieceCat == Category.G || pieceCat == Category.I;

                case Category.M:

                    return isSystem
                        ? (pieceCat == Category.M || pieceCat == Category.G)
                        : (pieceCat == Category.M || pieceCat == Category.I);

                default:
                    return false;
            }
        }
    }
}
