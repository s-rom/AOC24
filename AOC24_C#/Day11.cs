using System.Diagnostics.SymbolStore;

namespace Day11;


class Day11
{

    private static readonly string inputFile = @"..\..\..\input_11.txt";

    private static LinkedList<long> parseInput()
    {
        using StreamReader sr = File.OpenText(inputFile);
        return new(sr.ReadToEnd().TrimEnd().Split(' ').Select(x => long.Parse(x)).ToList());
    }

    private static bool EvenDigits(long num)
    {
        return (num.ToString().Length) % 2 == 0;
    }


    private static (long, long) SplitDigits(long num)
    {
        string repr = num.ToString();

        return new(
            long.Parse(repr.Substring(0, repr.Length/2)),
            long.Parse(repr.Substring(repr.Length/2, repr.Length/2))
        );
    }

    private static void AddStone(Dictionary<long,long> stones, long stone, long qty = 1)
    {
        if (!stones.ContainsKey(stone))
        {
            stones.Add(stone, 0);
        }

        stones[stone] += qty;
    }


    private static void RemoveStone(Dictionary<long,long> stones, long stone, long qty = 1)
    {
        stones[stone] -= qty;

        if (stones[stone] == 0)
        {
            stones.Remove(stone);
        }

    }


    private static void FastBlink(Dictionary<long,long> stones)
    {
        Dictionary<long, long> copy = new(stones);

        foreach (var stone in copy.Keys)
        {
            
            if (stone == 0)
            {
                RemoveStone(stones, 0, copy[stone]);
                AddStone(stones, 1, copy[stone]);
                continue;
            }

            if (EvenDigits(stone))
            {
            
                var (left, right) = SplitDigits(stone);
                
                RemoveStone(stones, stone, copy[stone]);
                AddStone(stones, left, copy[stone]);
                AddStone(stones, right, copy[stone]);
                
                continue;
            }


            RemoveStone(stones, stone, copy[stone]);
            AddStone(stones, stone * 2024, copy[stone]);
        }
    }

    private static void Blink(LinkedList<long> list)
    {
        LinkedListNode<long>? node = list.First;
       
        while (node != null)
        {

            if (node.Value == 0)
            {
                node.Value = 1;
                node = node.Next;
                continue;
            }

            if (EvenDigits(node.Value))
            {
                var (left, right) = SplitDigits(node.Value);
                node.Value = left;
                var newNode = list.AddAfter(node, right);
                node = newNode.Next;
                continue;
            }


            node.Value *= 2024;
            node = node.Next;
        }

    }


    public static int Part1()
    {
        var list = parseInput();

        foreach (var i in Enumerable.Range(0, 25))
        {
            Blink(list);
        }

        return list.Count;
    }

    public static long Part2()
    {
        Dictionary<long, long> stones = [];
        var list = parseInput().ToList();

        foreach (var i in list)
        {
            stones.Add(i, 1);
        }


        foreach (var _ in Enumerable.Range(0, 75))
        {
            FastBlink(stones);
        }

        long total = 0;
        foreach (var stone in stones.Keys)
        {
            total += stones[stone];
        }

        return total;
    }



}