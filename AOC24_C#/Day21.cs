using System.Runtime.InteropServices;
using AOC24;

namespace Day21;



class NumpadRobot : Robot
{
    public NumpadRobot()
    {
        var numpadButtons = new Grid<char>(4, 3);
        numpadButtons.Fill(
            [
                ['7', '8', '9'],
                ['4', '5', '6'],
                ['1', '2', '3'],
                [EMPTY, '0', 'A']
            ]
        );

        this.InitButtons(numpadButtons);
    }
}


class DirectionalRobot: Robot
{

    private static readonly Dictionary<char, char> invertedInput = new()
    {
        {'>', '<'},
        {'<', '>'},
        {'^', 'v'},
        {'v', '^'}
    };

    public DirectionalRobot()
    {
        var directionalButtons = new Grid<char>(2, 3);
        directionalButtons.Fill(
            [
                [EMPTY, '^',  'A'],
                ['<',   'v',  '>'],
            ]
        );

        this.InitButtons(directionalButtons);
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

    public void TestDirectional(string inputs)
    {
        foreach (var input in inputs)
        {
            Console.WriteLine($"\t -- {input} --");
            foreach (var possiblePath in this.AllShortestPathsToButton(input))
            {
                if (possiblePath != "A")
                    Console.WriteLine("\t" + possiblePath + "A");
            }

            this.MoveArm(input);
        }

    }



    /*
        OLD VERSION

    */
    private string ExecuteRequiredInputs(string inputs)
    {
        string result = "";

        foreach (var input in inputs)
        {
            // Go to button
            var directInputs = AllShortestPathsToButton(input);
            result += directInputs;             
            MoveArm(input);
            result += 'A';
        }

        // Return to A
        result += AllShortestPathsToButton('A');
        MoveArm('A');
        return result + 'A';
    }

    public string InvertPath(string path)
    {
        string inverted = "";
        foreach (var input in path)
        {
            inverted += invertedInput[input];
        }
        return inverted;
    }


}




abstract class Robot
{
    protected static readonly char EMPTY = ' ';

    protected GridVector position;

    protected Dictionary<char, GridVector> buttonPositions;

    protected Dictionary<(char, char), List<string>> shortestPathsCache = [];

    protected Grid<char> buttons;  

    protected void InitButtons(Grid<char> buttons)
    {
        this.buttons = buttons;
        var buttonAPosition = buttons.FirstPositionOf('A');
        if (buttonAPosition.HasValue)
        {
            position = buttonAPosition.Value;
        }
        else
        {
            throw new Exception("A button not found");
        }

        buttonPositions = [];
        for (int r = 0; r < buttons.Rows; r++)
        {
            for (int c = 0; c < buttons.Columns; c++)
            {
                var pos = new GridVector(r, c);
                buttonPositions.Add(buttons.ElementAt(pos), pos);
            }
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
            this.position = position;
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
        GridVector currentPos = this.position;
        foreach (var p in path)
        {
            GridVector dir = p - currentPos;
            inputs += InputChar(dir);
            currentPos += dir;
        }

        return inputs;

        // return new string([.. InputsString(path).Reverse()]);
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


        if (position == target)
        {
            return ["A"];
        }


        char currentButton = buttons.ElementAt(position);
        char targetButton = number;

        var combination = (currentButton, targetButton);

        if (shortestPathsCache.TryGetValue(combination, out List<string>? cachedPaths))
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
        queue.Enqueue(position);
        distance[position] = 0;

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
        GridVector start = this.position;
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
        shortestPathsCache.Add(combination, paths);
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


    public static long GetRequiredInputsStr(string inputs, NumpadRobot numpad, List<DirectionalRobot> directionals, int depth)
    {
        long total = 0;

        int directionalIdx = depth - 1;
        bool isLastController = (depth == directionals.Count);
        
        Robot currentRobot;
        if (depth == 0)
            currentRobot = numpad;
        else
            currentRobot = directionals[directionalIdx];


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
                    var inputLength = GetRequiredInputsStr(path, numpad, directionals, depth+1);
                    possibleInputs.Add(inputLength);
                }


                var chosenPath = possibleInputs.Min();

                // var chosenPath = possibleInputs.
                //     Aggregate((min, current) => current.Length < min.Length ? 
                //                                 current :
                //                                 min);
                
                total += chosenPath;
            }

            currentRobot.MoveArm(button);
        }
        return total;
    }


    public static long Part1()
    { 
      
        var numberOfDirectionals = 25;
        var numpadRobot = new NumpadRobot();
        List<DirectionalRobot> directionalRobots = [];

        using StreamReader sr = File.OpenText(@"..\..\..\input_21.txt");

        var codes = sr.ReadToEnd().Split("\r\n");
        for (var i = 0; i < numberOfDirectionals; i++)
            directionalRobots.Add(new DirectionalRobot());

        long result = 0;

        foreach (var code in codes)
        {
            var codeValue = int.Parse(code.Substring(0, code.Length - 1));
            result += codeValue * GetRequiredInputsStr(code, numpadRobot, directionalRobots, 0);
        }
        return result;
    }

    
    public static long Part2()
    {
        return 0;
    }

}

