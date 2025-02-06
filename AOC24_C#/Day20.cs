using System.Diagnostics;
using System.Dynamic;
using System.Runtime.InteropServices;

namespace Day20;



class RaceGrid: Grid<char>
{
    private static readonly char WALL = '#';
    private static readonly char EMPTY = '.';
    
    public GridVector StartPosition { get; private set;}
    public GridVector EndPosition { get; private set; }
    
    private HashSet<(GridVector start, GridVector end)> cheats;

    public RaceGrid(string inputFile)
        : base(inputFile, x => x)
    {
        cheats = [];
        StartPosition = GridVector.ZERO;
        EndPosition = GridVector.ZERO;

        var s =  this.FirstPositionOf('S');
        if (s.HasValue)
        {
            StartPosition = s.Value;
        }
        var e =  this.FirstPositionOf('E');
        if (e.HasValue)
        {
            EndPosition = e.Value;
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

    
    /*
        20 ps max

        "Any cheat time not used is lost; 
            it can't be saved for another cheat later."

        => A cheat can only start when breaking into a wall
            -> A cheat ends when player is on the track again

        => If two cheats (c1, c2) start at pos A and end pos B 
            c1 = c2
    
    */
    public void FindExtendedCheats(Dictionary<int, int> cheatStats, int maxPs = 20, int minSaved = 100)
    {
        HashSet<(GridVector start, GridVector end)> visitedCheats = [];

        Dictionary<GridVector, int> bestPathDistances = [];
        var bestPath = FindPath(bestPathDistances);

        for (int startIdx = 0;  startIdx < bestPath.Count; startIdx++)
        {
            var cheatStart = bestPath[startIdx];

            for (int endIdx = startIdx + 1; endIdx < bestPath.Count; endIdx++)
            {
                var cheatEnd = bestPath[endIdx];
                
                if (cheatEnd == cheatStart) continue;

                int dist = GridVector.ManhattanDistance(cheatStart, cheatEnd);
                if (dist > maxPs) continue;

                var cheat = (cheatStart, cheatEnd);
                if (visitedCheats.Contains(cheat)) continue;
                visitedCheats.Add(cheat);
                
                var saved = bestPathDistances[cheatEnd] - bestPathDistances[cheatStart] - dist;


                if (saved >= minSaved)
                {
                    if (!cheatStats.ContainsKey(saved)) cheatStats.Add(saved, 0);
                    cheatStats[saved] ++;
                }

            }
        }

    }


    // Dijsktra shortest path from StartPosition to DistancePosition
    public List<GridVector> FindPath(Dictionary<GridVector, int> distance)
    {
        List<GridVector> path = [];

        Queue<GridVector> queue = [];
        
        distance[StartPosition] = 0;
        queue.Enqueue(StartPosition);
        path.Add(StartPosition);

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

                if (n == EndPosition) 
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
        var grid = new RaceGrid(@"..\..\..\input_20.txt");

        Dictionary<int, int> cheatStats = [];
        grid.FindExtendedCheats(cheatStats);

        int result = 0;
        foreach (var saved in cheatStats.Keys)
        {
            result += cheatStats[saved];
        }

        return result;
    }
}