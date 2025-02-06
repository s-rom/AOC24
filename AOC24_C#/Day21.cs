using System.Security.Permissions;

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

    public IEnumerable<string> AllShortestPathsToButton(char number)
    {
        GridVector target = buttonPositions[number];

        if (position == target)
        {
            yield return "";
            yield break;
        }


        char currentButton = buttons.ElementAt(position);
        char targetButton = number;

        if (shortestPathsCache.ContainsKey((currentButton, targetButton)))
        {
            foreach (var path in shortestPathsCache[(currentButton, targetButton)])
                yield return path;
                yield break;
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

        shortestPathsCache[(currentButton, targetButton)] = [];

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
                shortestPathsCache[(currentButton, targetButton)].Add(stringPath);
                
                yield return stringPath;
                continue;
            }
            
            foreach (var parentNode in parent[currentNode])
            {
                var newPath = new List<GridVector>(currentPath);
                newPath.Add(parentNode);
                pathQueue.Push(newPath);
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

    public static string Test(List<DirectionalRobot> robots, string inputs, int depth = 0)
    {

        int maxDepth = robots.Count;
        if (depth == maxDepth) return "";

        string shortestRequiredInput = "";
        foreach (var button in inputs)
        {
            // To achieve this button several shortest paths are available
            // For each possible input in path, check the inputs required at next input
            // and pick the shortest one

            Console.WriteLine($"--- {button} ---");
            var nextPath = robots[depth].AllShortestPathsToButton(button).ToList().First() + "A";
            Console.WriteLine($"\t " + nextPath);

            shortestRequiredInput +=  nextPath;
                
            
            robots[depth].MoveArm(button);
        }

        return shortestRequiredInput;
    }

    public static long Part1()
    { 
      
        // v<<A>>^A<A>AvA<^AA>A<vAAA>^A

        List<DirectionalRobot> robots = [];
        robots.AddRange(Enumerable.Repeat(new DirectionalRobot(), 2));
        
        NumpadRobot numpadRobot = new();



        // Populate shortest paths cache

        foreach (var n in "0123456789A")
        {
            foreach (var m in "0123456789A")
            {
                numpadRobot.MoveArm(m);
                numpadRobot.AllShortestPathsToButton(n).ToList();
            }
        }

        numpadRobot.MoveArm('A');


        string result = "";

        foreach (var num in "029A")
        {
            var pathTobutton = numpadRobot.AllShortestPathsToButton(num).ToList().First();
            result += Test(robots, pathTobutton + "A", 0);
            
            numpadRobot.MoveArm(num);
        }

        Console.WriteLine(result);

        // var code = "029A";

        // NumpadRobot numpadRobot = new();
        // DirectionalRobot directionalRobot = new();


        // foreach (var num in code)
        // {
        //     Console.WriteLine($"---- {num} ----");
        //     foreach (var path in numpadRobot.AllShortestPathsToButton(num))
        //     {
        //         Console.WriteLine(path + "A");
        //         directionalRobot.TestDirectional(path + "A");
        //     }

        //     numpadRobot.MoveArm(num);

        //     Console.WriteLine();
        // }



        return 0;
    }

    
    public static long Part2()
    {
        return 0;
    }

}

