namespace designPatternProject
{
    public enum Category
    {
        G, // Généraliste
        D, // Domestique
        I, // Industriel
        M  // Militaire
    }

    /// <summary>
    /// Règles de compatibilité entre la catégorie du robot
    /// et celle d’une pièce ou d’un système qu’on veut y installer.
    /// </summary>
    public static class Compatibility
    {
        public static bool IsCompatible(Category robotCat, Category pieceCat, bool isSystem)
        {
            switch (robotCat)
            {
                case Category.D:
                    // Domestique : accepte G, D ou I.
                    return pieceCat == Category.G || pieceCat == Category.D || pieceCat == Category.I;

                case Category.I:
                    // Industriel : accepte G ou I.
                    return pieceCat == Category.G || pieceCat == Category.I;

                case Category.M:
                    // Militaire : règles différentes selon pièce ou système.
                    return isSystem
                        ? (pieceCat == Category.M || pieceCat == Category.G)   // systèmes M ou G
                        : (pieceCat == Category.M || pieceCat == Category.I);  // pièces   M ou I

                default: // Un robot ne peut jamais être G (généraliste)
                    return false;
            }
        }
    }
}
