using AOC24;
namespace Day21;



class NumpadRobot : Robot
{
    public NumpadRobot() : base(4, 3)
    {
        this.InitButtons(
            [
                ['7', '8', '9'],
                ['4', '5', '6'],
                ['1', '2', '3'],
                [EMPTY, '0', 'A']
            ]
        );
        PrecomputePaths();
    }
}


class DirectionalRobot: Robot
{
    public DirectionalRobot(): base(2,3)
    {
        this.InitButtons(
            [
                [EMPTY, '^',  'A'],
                ['<',   'v',  '>'],
            ] 
        );

    }

}




abstract class Robot(int rows, int columns)
{
    protected static readonly char EMPTY = ' ';

    public GridVector Position { get; protected set; }

    protected Dictionary<char, GridVector> buttonPositions = [];

    public Dictionary<(char, char), List<string>> ShortestPathsCache {get; set;} = [];

    protected Grid<char> buttons = new(rows, columns);  

    public void PrecomputePaths()
    {
        foreach (var from in this.buttons.Values())
        {
            foreach  (var to in this.buttons.Values())
            {
                if (from == EMPTY || to == EMPTY) continue;

                this.MoveArm(from);
                AllShortestPathsToButton(to);
            }
        }

        this.MoveArm('A');
    }

    protected void InitButtons(List<List<char>> newButtons)
    {
        buttons.Fill(newButtons);
        
        buttonPositions = [];
        for (int r = 0; r < buttons.Rows; r++)
        {
            for (int c = 0; c < buttons.Columns; c++)
            {
                var pos = new GridVector(r, c);
                buttonPositions.Add(buttons.ElementAt(pos), pos);
            }
        }
        
        var buttonAPosition = buttons.FirstPositionOf('A');
        if (buttonAPosition.HasValue)
        {
            Position = buttonAPosition.Value;
        }
        else
        {
            throw new Exception("A button not found");
        }
    }

    public void MoveArm(char button)
    {
        if (buttonPositions.TryGetValue(button, out GridVector pos))
        {
            MoveArm(pos);
        }
        else
        {
            Console.Error.WriteLine("Invalid button: " + button);
        }
    }

    public void MoveArm(GridVector position)
    {
        if (buttons.IsInBounds(position) && !(buttons.ElementAt(position) == EMPTY))
        {
            this.Position = position;
        }
        else 
        {
            Console.Error.WriteLine("Trying to move robot arm to invalid position");
            Console.Error.WriteLine(position);
        }
    }

    public static char InputChar(GridVector input)
    {
        char output = '_';

        if (input == GridVector.DOWN) output = 'v';
        if (input == GridVector.LEFT) output = '<';
        if (input == GridVector.RIGHT) output = '>';
        if (input == GridVector.UP) output = '^';

        return output;
    }


    private string PathToStringReversed(List<GridVector> path)
    {
        string inputs = "";
        path.Reverse();
        GridVector currentPos = this.Position;
        foreach (var p in path)
        {
            GridVector dir = p - currentPos;
            inputs += InputChar(dir);
            currentPos += dir;
        }

        return inputs;
    }

    public static string InputsString(List<GridVector> inputs)
    {
        string output = "";
        foreach (var input in inputs) output += InputChar(input);
        return output;
    }

    public List<string> AllShortestPathsToButton(char number)
    {
        GridVector target = buttonPositions[number];


        if (Position == target)
        {
            return ["A"];
        }


        char currentButton = buttons.ElementAt(Position);
        char targetButton = number;

        var combination = (currentButton, targetButton);

        if (ShortestPathsCache.TryGetValue(combination, out List<string>? cachedPaths))
        {
            return cachedPaths;
        }


        HashSet<GridVector> visited = [];
        Queue<GridVector> queue = [];
        Dictionary<GridVector, List<GridVector>> parent = [];
        Dictionary<GridVector, int> distance= [];

        foreach (var button in buttons.Positions())
        {
            distance[button] = int.MaxValue;
            parent[button] = [];
        }


        // Add current position     
        queue.Enqueue(Position);
        distance[Position] = 0;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            visited.Add(current);


            foreach (var node in AdjacentNodes(current))
            {
                if (visited.Contains(node)) continue;
                var distToNeigh = distance[current] + 1;


                if (distToNeigh < distance[node])
                {
                    distance[node] = distToNeigh;
                    queue.Enqueue(node);
                    parent[node].Clear();
                    parent[node].Add(current);
                }
                else
                {
                    parent[node].Add(current);
                }
            }
        }


        // Find all paths
        List<string> paths = [];

        Stack<List<GridVector>> pathQueue = new();
        GridVector start = this.Position;
        pathQueue.Push([buttonPositions[number]]);
        while (pathQueue.Count != 0)
        {
            var currentPath = pathQueue.Pop();
            var currentNode = currentPath.Last(); 

            if (currentNode == start) 
            {
                // found path
                currentPath.RemoveAt(currentPath.Count - 1);
                var stringPath = PathToStringReversed(currentPath);
                paths.Add(stringPath+"A");
                continue;
            }
            
            foreach (var parentNode in parent[currentNode])
            {
                var newPath = new List<GridVector>(currentPath);
                newPath.Add(parentNode);
                pathQueue.Push(newPath);
            }
        }
   
        PruneHighTurnsPaths(ref paths);
        ShortestPathsCache.Add(combination, paths);
        return paths;
   
    }


    private static int CountTurnsInPath(string path)
    {
        char last = ' ';
        int turns = 0;

        foreach (var input in path)
        {
            if (input != 'A' && input != last) turns++;
            last = input;
        }

        return turns;
    }

    private static void PruneHighTurnsPaths(ref List<string> paths)
    {
        if (paths.Count < 2) return;


        List<int> turns = [];
        int minTurns = int.MaxValue;
        foreach (var path in paths)
        {
            var pathTurns = CountTurnsInPath(path);
            turns.Add(pathTurns);
            if (pathTurns < minTurns)
            {
                minTurns = pathTurns;
            }
        }

        for (int i = 0; i < paths.Count; i++)
        {
            if (turns[i] > minTurns)
            {
                paths.RemoveAt(i);
            }
        }


    }

    private IEnumerable<GridVector> AdjacentNodes(GridVector pos)
    {
        foreach (var neigh in buttons.InBoundsFourNeighbours(pos))
        {
            if (buttons.ElementAt(neigh) == EMPTY) continue;
            yield return neigh;
        }
    }
}




/*

+---+---+---+
| 7 | 8 | 9 |
+---+---+---+
| 4 | 5 | 6 |
+---+---+---+
| 1 | 2 | 3 |
+---+---+---+
    | 0 | A |
    +---+---+


    +---+---+
    | ^ | A |
+---+---+---+
| < | v | > |
+---+---+---+


*/


class Day21
{

    /* (depth, path) => pathLength */
    private static readonly Dictionary<(int, string), long> cache = [];

    public static long GetRequiredInputs(string inputs, List<Robot> directionals, int depth)
    {
        long total = 0;

        bool isLastController = (depth == directionals.Count-1);        
        Robot currentRobot = directionals[depth];
        
        foreach (var button in inputs)
        {
            var allShortestPaths = currentRobot.AllShortestPathsToButton(button);
        
            if (isLastController)
            {
                /*
                    The robot arm is already pointing at the target button
                    Just press A
                        => Add 1 to the total inputs required
                */
                if (allShortestPaths.Count == 0) 
                {
                    total += 1;
                }
                else
                {
                    total += allShortestPaths.First().Length;
                }
            }
            else
            {
                List<long> possibleInputs = [];
                
                foreach (var path in currentRobot.AllShortestPathsToButton(button))
                {

                    if (!cache.TryGetValue((depth, path), out long inputLength))
                    {
                        inputLength = GetRequiredInputs(path, directionals, depth+1);
                        cache.Add((depth, path), inputLength);
                    }
                    possibleInputs.Add(inputLength);
                }

                var chosenPath = possibleInputs.Min();
                total += chosenPath;
            }

            currentRobot.MoveArm(button);
        }
        return total;
    }


    public static long SolveDay21(int numberOfDirectionals)
    {
        var numpadRobot = new NumpadRobot();
        List<Robot> robots = [numpadRobot];

        using StreamReader sr = File.OpenText(@"..\..\..\input_21.txt");

        var codes = sr.ReadToEnd().Split("\r\n");


        Dictionary<(char, char), List<string>>? pathsCache = null;
        for (var i = 0; i < numberOfDirectionals; i++)
        {
            var robot = new DirectionalRobot();
            if (pathsCache == null)
            {
                robot.PrecomputePaths();
                pathsCache = robot.ShortestPathsCache; 
            }
            else
            {
                robot.ShortestPathsCache = pathsCache;
            }
            robots.Add(robot);
        }

        long result = 0;

        foreach (var code in codes)
        {
            var codeValue = int.Parse(code.Substring(0, code.Length - 1));
            result += codeValue * GetRequiredInputs(code, robots, 0);
        }

        cache.Clear();
        return result;

    }


    public static long Part1()
    { 
        return SolveDay21(2);
    }

    
    public static long Part2()
    {
        return SolveDay21(25);
    }

}

