
namespace Day19;

class Day19
{

    private static List<string> availablePatterns;
    private static List<string> targetPatterns = [];
    private static Dictionary<string, bool> canBeCreatedCache = [];
    private static Dictionary<string, long> combinationsCache = [];


    private static void Parse(string inputFile)
    {
        using StreamReader sr  = File.OpenText(inputFile);
        string? line = sr.ReadLine(); // first line
        availablePatterns = line!.Split(", ").Select(x=>x.TrimEnd()).ToList();
    
        sr.ReadLine();

        while ((line = sr.ReadLine()) != null)
        {
            targetPatterns.Add(line.TrimEnd());
        }

        foreach (var towel in availablePatterns)
        {
            canBeCreatedCache.Add(towel, true);
        }

    }

    private static bool CanBeCreated(string pattern)
    {

        foreach (var towel in availablePatterns)
        {
            if (pattern.StartsWith(towel))
            {
                var remainder = pattern[towel.Length..];

                if (!canBeCreatedCache.TryGetValue(remainder, out bool canBeCreated))
                {
                    canBeCreatedCache[remainder] = CanBeCreated(remainder);
                }

                if (canBeCreatedCache[remainder]) return true;
            }
        }

        return false;
    }

    
    private static long TotalCombinations(string pattern)
    {
        long total = 0;

        if (string.IsNullOrEmpty(pattern)) return 1;

        foreach (var towel in availablePatterns)
        {
            if (!pattern.StartsWith(towel)) continue;
            
            var remainder = pattern[towel.Length..];

            if (!combinationsCache.TryGetValue(remainder, out long combinations))
            {
                combinationsCache[remainder] = TotalCombinations(remainder);
            }

            total += combinationsCache[remainder];
        }

        return total;
    }


    public static int Part1()
    {
        Parse(@"..\..\..\input_19.txt");
        return targetPatterns.Aggregate(0, (x, pattern) => CanBeCreated(pattern)? x + 1: x);
    }
    
    public static long Part2()
    {
        Parse(@"..\..\..\input_19.txt");

        foreach (var towel in availablePatterns)
        {
            if (!combinationsCache.ContainsKey(towel))
            {
                combinationsCache[towel] = TotalCombinations(towel);
            }
        }


        long total = 0;
        foreach (var towel in targetPatterns)
        {
            total += TotalCombinations(towel);
        }
        return total;
    }
}