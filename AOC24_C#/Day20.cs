namespace Day20;



class RaceGrid: Grid<char>
{
    private static readonly char WALL = '#';
    private static readonly char EMPTY = '.';
    
    private GridVector start;
    private GridVector end;
    

    public RaceGrid(string inputFile)
        : base(inputFile, x => x)
    {
        start = GridVector.ZERO;
        end = GridVector.ZERO;

        var s =  this.FirstPositionOf('S');
        if (s.HasValue)
        {
            start = s.Value;
        }
        var e =  this.FirstPositionOf('E');
        if (e.HasValue)
        {
            end = e.Value;
        }
    }


    public int Find2PicosendsCheat(int minSaved = 0)
    {
        Dictionary<GridVector, int> distance = [];
        var originalPath = FindPath(distance);


        Dictionary<int, int> cheats = [];

        foreach (var pos in originalPath)
        {
            var currentDistance = distance[pos];


            // Find possible cheats from this position
            foreach (var n in this.InBoundsFourNeighbours(pos))
            {
                if (ElementAt(n) == WALL)
                {
                    // Check direction to this pos
                    var dir = n - pos;

                    var cheatedPos = pos + dir * 2;

                    // Position after cheating must be at an original path position
                    if (IsInBounds(cheatedPos) && distance.ContainsKey(cheatedPos))
                    {

                        var saved = distance[cheatedPos] - currentDistance - 2;

                        if (saved > 0)
                        {
                            if (!cheats.ContainsKey(saved))
                                cheats[saved] = 0;
                            cheats[saved] ++;    
                        }
                    }
                }
            }

        }

        int result = 0;

        foreach (var cheat in cheats.Keys)
        {
            if (cheat >= minSaved)
            {
                var count = cheats[cheat];
                result += count;
            }
        }

        return result;

    }


    
    public int FindNPicosendsCheat(int picosends, int minSaved = 0)
    {
        Dictionary<GridVector, int> distance = [];
        var originalPath = FindPath(distance);


        Dictionary<int, int> cheats = [];

        foreach (var pos in originalPath)
        {
            var currentDistance = distance[pos];


            // Find possible cheats from this position
            foreach (var n in this.InBoundsFourNeighbours(pos))
            {
                if (ElementAt(n) == WALL)
                {
                    // Check direction to this pos
                    var dir = n - pos;

                    var cheatedPos = pos + dir * 2;

                    // Position after cheating must be at an original path position
                    if (IsInBounds(cheatedPos) && distance.ContainsKey(cheatedPos))
                    {

                        var saved = distance[cheatedPos] - currentDistance - 2;

                        if (saved > 0)
                        {
                            if (!cheats.ContainsKey(saved))
                                cheats[saved] = 0;
                            cheats[saved] ++;    
                        }
                    }
                }
            }

        }

        int result = 0;

        foreach (var cheat in cheats.Keys)
        {
            if (cheat >= minSaved)
            {
                var count = cheats[cheat];
                result += count;
            }
        }

        return result;

    }



    public List<GridVector> FindPath(Dictionary<GridVector, int> distance)
    {
        List<GridVector> path = [];

        Queue<GridVector> queue = [];
        
        distance[start] = 0;
        queue.Enqueue(start);
        path.Add(start);

        bool foundEnd = false;

        while (queue.Count > 0 && !foundEnd)
        {
            var current = queue.Dequeue();
            foreach (var n in this.InBoundsFourNeighbours(current))
            {

                if (this.ElementAt(n) == WALL) continue;

                // if visited, skip (to avoid going back)
                if (distance.ContainsKey(n)) continue;

                distance[n] = distance[current] + 1;
                queue.Enqueue(n);
                path.Add(n);

                if (n == end) 
                {
                    foundEnd = true;
                    break;
                }
            }
        }
    
        return path;
    }



}


class Day20
{
    public static int Part1()
    {
        var grid = new RaceGrid(@"..\..\..\input_20.txt");
        return grid.Find2PicosendsCheat(100);

    }

    public static int Part2()
    {
        var grid = new RaceGrid(@"..\..\..\input_20_test.txt");

        return 0;
    }
}