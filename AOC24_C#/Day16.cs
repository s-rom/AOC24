using System.Diagnostics;
namespace Day16;


using NodeState = (GridVector position, GridVector direction);


class Maze: Grid<char>
{

    private static readonly char WALL = '#';
    private static readonly char START = 'S';
    private static readonly char END = 'E';


    private static  GridVector start;
    private static  GridVector end;
    

    

    public Maze(string inputFile)
    {
        using StreamReader sr = File.OpenText(inputFile);
        List<List<char>> data = [];

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            data.Add(line.ToCharArray().ToList());
        }

        this.Fill(data);

        foreach(var p in Positions())
        {
            if (ElementAt(p) == START) start = p;
            if (ElementAt(p) == END) end = p;
        }

       

    }


    public int GetCostToPosition(GridVector from, GridVector to, GridVector direction)
    {
        var nextDirection = to - from;
        if (nextDirection != direction)
        {
            return 1001;
        }
        else
        {
            return 1;
        }
    }


    public int DijkstraShortestPath()
    {
        var distance = new Dictionary<NodeState, int>();
        var parent = new Dictionary<NodeState, HashSet<NodeState>>();
        var queue = new PriorityQueue<NodeState, int>();

        var startState = (start, GridVector.RIGHT);
        distance[startState] = 0;
        parent[startState] = [];
        queue.Enqueue(startState, 0);

        while (queue.Count > 0)
        {
            var currentState = queue.Dequeue();
            var (currentPos, currentDir) = currentState;
            var currentCost = distance[currentState];

            foreach (var neighbour in FourNeighbours(currentPos).Where(x => !(ElementAt(x) == WALL)))
            {
                var nextDirection = neighbour - currentPos;
                var nextState = (neighbour, nextDirection);
                var newDistance = currentCost + GetCostToPosition(currentPos, neighbour, currentDir);

                // 1.- La nueva distancia es mejor
                    // Borrar el parent antiguo
                
                // 2.- La nueva distancia es igual
                    // Añadir como parent posible

                if (!distance.ContainsKey(nextState))
                {
                    // First time checking this state

                    distance[nextState] = newDistance;
                    
                    Debug.Assert(!parent.ContainsKey(nextState));
                    parent[nextState] = [];

                    parent[nextState].Add(currentState);
                    queue.Enqueue(nextState, newDistance);
                }
                else if (newDistance < distance[nextState])
                {
                    // Better path found for this state
                    distance[nextState] = newDistance;
                    parent[nextState].Clear();
                    parent[nextState] = [currentState];
                    queue.Enqueue(nextState, newDistance);
                }
                else if (newDistance == distance[nextState])
                {
                    // Alternative path found for this state
                    // Better path found for this state
                    distance[nextState] = newDistance;
                    parent[nextState].Add(currentState);
                    queue.Enqueue(nextState, newDistance);
                }


            }
        }

        var endStates = distance.Keys.Where(k => k.position == end);
        var bestEndState = endStates.OrderBy(k => distance[k]).First();



        var toCheck = new  Queue<NodeState>();
        toCheck.Enqueue(bestEndState);
        HashSet<GridVector> positionInBestPath = [];

        while (toCheck.Count > 0)
        {
            var nextState = toCheck.Dequeue();

            positionInBestPath.Add(nextState.position);

            foreach (var parentState in parent[nextState])
            {

                toCheck.Enqueue(parentState);
            }
        }

        Console.WriteLine($"bestPathPositions: {positionInBestPath.Count}");
        Console.WriteLine($"End states {endStates.Count()}");
    
        return distance[bestEndState];
    }


    
    public void HighlightPosition(GridVector position)
    {
        var result = "";
        char [,] renderData = new char[Rows, Columns];
        foreach(var pos in Positions())
        {
            renderData[pos.Row, pos.Column] = data[pos.Row, pos.Column];
        }

        renderData[position.Row, position.Column] = 'A';

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                result += renderData[r,c]; 
            }
            result += "\n";
        }
        Console.WriteLine(result);
    }

    public void PrintPath(List<GridVector> path)
    {
        var result = "";
        char [,] renderData = new char[Rows, Columns];
        foreach(var pos in Positions())
        {
            renderData[pos.Row, pos.Column] = data[pos.Row, pos.Column];
        }

        foreach (var pos in path)
        {
            renderData[pos.Row, pos.Column] = 'O';
        }

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                result += renderData[r,c]; 
            }
            result += "\n";
        }
        Console.WriteLine(result);
    }

}


class Day16
{
    
    public static int Part1And2()
    {
        Maze m = new(@"..\..\..\input_16.txt");
        return m.DijkstraShortestPath();
    }
 

    

}


