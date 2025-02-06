namespace AOC24;

public class Util
{

    private static readonly Dictionary<int, string> spacesCache = [];


    public static string Indent(string str, int count)
    {
        
        if (!spacesCache.TryGetValue(count, out string? spaces))
        {
            spaces = "";
            for (int i = 0; i < count; i++)
                spaces += ' ';

            spacesCache.Add(count, spaces);
        }
        
        return spaces + str;
    }


    public static void PrintIndented(string str, int count)
    {
        Console.WriteLine(Indent(str, count));
    }
}



