using System.Numerics;


namespace Day17;

class MathUtil
{

    public static T  Mod<T>(T a, T b) where T: INumber<T> 
    {
        var r = a % b;
        if (r < T.Zero)
        {
            return r + b; 
        }
        else
        {
            return r;
        }
    }
}
