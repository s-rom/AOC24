
namespace Day25;

class Day25
{



    public static List<int> ParseLock(List<char[]> schematic)
    {
        List<int> lockDepth = [];
    
        for(int col = 0; col < schematic[0].Length; col++)
        {
            int row;
            for (row = 0; row < schematic.Count; row++)
            {
                if (schematic[row][col] == '.') break;
            }

            lockDepth.Add(row -  1);
        }
        return lockDepth;
    }

    public static List<int> ParseKey(List<char[]> schematic)
    {
        List<int> lockDepth = [];
    
        for(int col = 0; col < schematic[0].Length; col++)
        {
            int row;
            for (row = 0; row < schematic.Count; row++)
            {
                if (schematic[row][col] == '#') break;
            }

            lockDepth.Add(7 - row -  1);
        }
        return lockDepth;
    }

    public static void ParseInput(string inputFile, out List<List<int>> locks, out List<List<int>> keys)
    {
        using StreamReader sr = File.OpenText(inputFile);
        string? line;

        locks = [];
        keys = [];

        List<char[]> schematic = [];        

        while ((line = sr.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                if (schematic[0][0] == '#')
                {
                    locks.Add(ParseLock(schematic));
                }
                else
                {
                    keys.Add(ParseKey(schematic));
                }
                schematic.Clear();
                continue;
            }

            schematic.Add(line.ToCharArray());
        } 

        if (schematic[0][0] == '#')
        {
            locks.Add(ParseLock(schematic));
        }
        else
        {
            keys.Add(ParseKey(schematic));
        }

    }


    public static bool KeyLockFit(List<int> keyHeight, List<int> lockHeight)
    {

        for (int i = 0; i < keyHeight.Count; i++)
        {
            if (keyHeight[i] + lockHeight[i] > 5) return false;
        }

        return true;
    }


    public static ulong Part1()
    {
        ParseInput(@"..\..\..\input_25.txt",
            out List<List<int>> locks,
            out List<List<int>> keys
        );

        ulong total = 0L;
        foreach (var lockHeight in locks)
        {
            foreach (var keyHeight in keys)
            {
                if (KeyLockFit(keyHeight, lockHeight)) total++; 
            }
        }
    
        return total;
    }

}