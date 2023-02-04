using Unity.Mathematics;

public static class BoolExtensions
{
    public static bool Any(this bool2 b)
    {
        return b.x || b.y;
    }
    
    public static bool All(this bool2 b)
    {
        return b.x && b.y;
    }
}
