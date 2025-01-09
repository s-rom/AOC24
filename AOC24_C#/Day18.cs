using Day10;
using Raylib_cs;

namespace Day18;


class MemoryMap: Grid<char>
{

    private readonly static char EMPTY = '.';
    private readonly static char CORRUPTED = '#';

    private GridVector startingPos = new(0, 0);
    
    private GridVector targetPos;

    private Queue<GridVector> fallingBytes = [];


    public MemoryMap(string inputFile, int rows, int cols, int maxBytes = int.MaxValue)
        : base(rows, cols, EMPTY)
    {
        InitFromFile(inputFile, maxBytes);
        targetPos = new(row: Rows - 1, col: Columns - 1);
    }

    public void Empty()
    {
        this.Fill(EMPTY);
    }

    public GridVector? AddCorruptedByte()
    {
        if (fallingBytes.Count == 0) return null;

        var b = fallingBytes.Dequeue();
        SetValue(b, CORRUPTED);
        return b;
    }

    private void InitFromFile(string inputFile, int maxBytes = int.MaxValue)
    {
        using StreamReader sr  = File.OpenText(inputFile);
        string? line;

        List<List<char>> raw = [];

        int b = 0;
        while ((line = sr.ReadLine()) != null && b < maxBytes)
        {
            var bytePos = line.Split(",").Select(x => int.Parse(x)).ToList();
            GridVector pos = new(row: bytePos[1], col: bytePos[0]);
            fallingBytes.Enqueue(pos);
            this.SetValue(pos, CORRUPTED);
            b++;
        }
    }

    public int? DijkstraShortestPath()
    {
        PriorityQueue<GridVector, int> queue = new();
        Dictionary<GridVector, int> distance = [];
        Dictionary<GridVector, GridVector?> parent = [];
        
        foreach (var pos in this.Positions())
        {
            distance[pos] = int.MaxValue;
            parent[pos] = null;
        }

        distance[startingPos] =  0;
        queue.Enqueue(startingPos, 0);
        parent[startingPos] = null;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var neigh in this.InBoundsFourNeighbours(current))
            {
                if (this.ElementAt(neigh) == CORRUPTED) continue;
                
                var distToNeigh = distance[current] + 1;

                if (distToNeigh < distance[neigh])
                {
                    

                    distance[neigh] = distToNeigh;
                    queue.Enqueue(neigh, distToNeigh);
                    parent[neigh] = current;

                    if (neigh == targetPos) return distance[targetPos];
                }
            }
        }

        parent.TryGetValue(targetPos, out GridVector? targetPar);
        if (targetPar is null) return null;
        else return distance[this.targetPos];
    }
}


class Day18
{

    public static int Part1()
    {
        MemoryMap map = new(@"..\..\..\input_18.txt", 71, 71, 1024);
        return map.DijkstraShortestPath() ?? -1;
    }

    public static string Part2()
    {
        MemoryMap map = new(@"..\..\..\input_18.txt", 71, 71);
        map.Empty();

        for (int i = 0; i < 1024; i++)
        {
            map.AddCorruptedByte();
        }


        GridVector? nextByte;
        while((nextByte = map.AddCorruptedByte()) != null)
        {
            int? pathLength = map.DijkstraShortestPath();

            if (!pathLength.HasValue)
            {
                return nextByte.Value.Column + "," + nextByte.Value.Row;
            }
        }
        return "All bytes allow a path";
    }
}