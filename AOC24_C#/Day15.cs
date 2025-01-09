namespace Day15;



class BoxesGrid 
{
    public int Rows {get; private set;}
    public int Columns {get; private set;}
    
    private bool expanded = false;

    public GridVector RobotPosition {get; private set;}

    private char [,] grid;
    private List<Input> inputs;

    private static char WALL = '#';
    private static char ROBOT = '@';
    private static char BOX_LEFT = '[';
    private static char BOX_RIGHT = ']';
    private static char BOX = 'O';
    private static char EMPTY = '.';

    enum Input 
    {
        UP, DOWN, RIGHT, LEFT, INVALID
    }

    private Input charToInput(char inputChar)
    {
        switch(inputChar)
        {
            case '^':
                return Input.UP;
            case '>':
                return Input.RIGHT;
            case 'V':
            case 'v':
                return Input.DOWN;
            case '<':
                return Input.LEFT;
        }

        return Input.INVALID;
    }

    public BoxesGrid (string inputFile, bool expanded = false)
    {
        List<List<char>> data = [];
        inputs = [];

        using StreamReader sr = File.OpenText(inputFile);
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            if (line.StartsWith('#'))
            {
                data.Add(line.ToCharArray().ToList());
                continue;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            inputs.AddRange(
                line.TrimEnd().ToCharArray().Select(charToInput).ToList()
            );
        }

        Rows = data.Count;
        Columns = data[0].Count;
        this.expanded = expanded;
        
        if (expanded)
        {
            Columns *= 2;
            CreateExpandedGrid(data);
        }
        else
        {
            CreateNormalGrid(data);
        }

        
    }

    private void CreateExpandedGrid(List<List<char>> data)
    {
        grid = new char [Rows, Columns];

        for (int r = 0; r < Rows; r++)
        {
            int c = 0;
            foreach (var element in data[r])
            {
                if (element == ROBOT)
                {
                    RobotPosition = new GridVector(row: r, col: c);
                    grid[r, c] = ROBOT;
                    grid[r, c+1] = EMPTY;
                }
                else if (element == BOX)
                {
                    grid[r, c] = BOX_LEFT;
                    grid[r, c+1] = BOX_RIGHT;
                }
                else
                {
                    grid[r, c] = element;
                    grid[r, c+1] = element;
                }

                c += 2;
            }
        }

    }
    private void CreateNormalGrid(List<List<char>> data)
    {
        grid = new char [Rows, Columns];

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (data[r][c] == ROBOT)
                {
                    RobotPosition = new GridVector(row: r, col: c);
                }

                grid[r,c] = data[r][c];
            }
        }

    }


    public override string ToString()
    {
        var result = "";

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                result += grid[r,c];
            }
            result += "\n";
        }
    
        return result;
    }

    private static GridVector InputToVector(Input input)
    {
        return input switch
        {
            Input.DOWN => GridVector.DOWN,
            Input.LEFT => GridVector.LEFT,
            Input.RIGHT => GridVector.RIGHT,
            Input.UP => GridVector.UP,
            _ => new GridVector(0, 0),
        };
    }

    private bool IsInBounds(GridVector v)
    {
        return 
            v.Column >= 0 && v.Column < Columns &&
            v.Row >= 0 && v.Row < Rows;
    }

    private bool IsWall(GridVector v)
    {
        return grid[v.Row,v.Column] == WALL;
    }

    private bool IsBox(GridVector v)
    {
        return grid[v.Row,v.Column] == BOX;
    }

    private bool IsAnyBoxSide(GridVector v)
    {
        return IsBoxSide(v, BOX_LEFT) || IsBoxSide(v, BOX_RIGHT);
    }

    private bool IsBoxSide(GridVector v, char side)
    {
        return grid[v.Row, v.Column] == side;
    }

    private bool IsEmpty(GridVector v)
    {
        return grid[v.Row,v.Column] == EMPTY;
    }

    private bool IsRobot(GridVector v)
    {
        return grid[v.Row,v.Column] == ROBOT;
    }

    private void MoveObject(GridVector from, GridVector to)
    {
        if (IsRobot(from))
        {
            RobotPosition = to;
        }

        grid[to.Row, to.Column] = grid[from.Row, from.Column];
        grid[from.Row, from.Column] = EMPTY;
    }


    private void PushVerticallyExtended(GridVector pos, Input input)
    {
        GridVector nextPos = pos + InputToVector(input);
        
        if (IsEmpty(nextPos))
        {
            MoveObject(from: pos, to: nextPos);
            return;
        }

        if (IsAnyBoxSide(nextPos))
        {
            
            GridVector adjNextSide; 
            if (IsBoxSide(nextPos, BOX_LEFT))
                adjNextSide = nextPos + GridVector.RIGHT;
            else 
                adjNextSide = nextPos + GridVector.LEFT;

            PushVerticallyExtended(nextPos, input);
            PushVerticallyExtended(adjNextSide, input);

            MoveObject(from: pos, to: nextPos);
        }


    }

    private bool CanPushVerticallyExtended(GridVector pos, Input input)
    {
        GridVector nextPos = pos + InputToVector(input);

        if (!IsInBounds(nextPos) || IsWall(nextPos))
        { 
            return false;
        }


        if (IsEmpty(nextPos))
            return true;

        GridVector adjSide; 
        if (IsBoxSide(nextPos, BOX_LEFT))
            adjSide = nextPos + GridVector.RIGHT;
        else 
            adjSide = nextPos + GridVector.LEFT;


        bool mainSidePushed = CanPushVerticallyExtended(nextPos, input);
        bool adjSidePushed = CanPushVerticallyExtended(adjSide, input);
        
        return mainSidePushed && adjSidePushed;
    

        // This box can be pushed only if both sides can be pushed

        // GridVector nextPosition = objectPos + InputToVector(input);

        // if (!IsInBounds(nextPosition) || IsWall(nextPosition))
        // { 
        //     return false;
        // }

        // if (IsEmpty(nextPosition))
        // {
        //     if (IsRobot(objectPos) || input == Input.LEFT || input == Input.RIGHT) 
        //     {
        //         MoveObject(objectPos, nextPosition);
        //         return true;
        //     }
        //     else
        //     {
        //         // This side of a box can move into empty space
        //         // Check if the next side can move too


        //     }
        // }


        // if (input == Input.LEFT || input == Input.RIGHT)
        // {
        //     // Try to move box horizontally
        //     bool pushed = ExpandedPushObject(nextPosition, input);

        //     // Move if only box moved
        //     if (pushed)
        //     {
        //         MoveObject(from: objectPos, to: nextPosition);
        //     }

        //     return pushed;
        // }
        // else
        // {
        //     GridVector nextSide;
        //     if (IsBoxSide(nextPosition, BOX_LEFT))
        //     {
        //         nextSide = nextPosition + GridVector.RIGHT;
        //     }
        //     else // BOX_RIGHT ]
        //     {
        //         nextSide = nextPosition + GridVector.LEFT;
        //     }

        //     // Try to push both sides

        //     bool mainSidePushed = ExpandedPushObject(nextPosition, input);
        //     bool nextSidePushed = ExpandedPushObject(nextSide, input);
            
        //     if (mainSidePushed && nextSidePushed)
        //     {
        //         // ???
        //     }

        //     return mainSidePushed && nextSidePushed;
        // }
        
    }

    private bool PushObject(GridVector objectPos, Input input)
    {
        GridVector nextPosition = objectPos + InputToVector(input);

        if (!IsInBounds(nextPosition) || IsWall(nextPosition))
        { 
            return false;
        }

        if (IsEmpty(nextPosition))
        {
            MoveObject(objectPos, nextPosition);
            return true;
        }

        if (IsAnyBoxSide(nextPosition))
        {
            // Try to move box
            bool pushed = PushObject(nextPosition, input);

            // Move if only box moved
            if (pushed)
            {
                MoveObject(from: objectPos, to: nextPosition);
            }

            return pushed;
        }

        return false;
    }

    public int SumOfAllBoxGPSCoord()
    {
        int total = 0;

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                GridVector pos = new(row: r, col: c);
                if (IsBox(pos) || IsBoxSide(pos, BOX_LEFT))
                {
                    total += 100 * pos.Row + pos.Column;
                }
            }
        }
        return total;
    }

    public void ExecuteInputs()
    {
        foreach (var input in inputs)
        {
            // Console.WriteLine(input);
            if (expanded)
            {
                if (input == Input.LEFT || input == Input.RIGHT)
                    PushObject(RobotPosition, input);
                else 
                {
                    bool canPush = CanPushVerticallyExtended(RobotPosition, input);
                    // Console.WriteLine(canPush);
                    if (canPush)
                    {
                        PushVerticallyExtended(RobotPosition, input);
                    }

                }

            }
            else
                PushObject(RobotPosition, input);
            // Console.WriteLine(this);
        }
    }


}



class Day15
{
    public static int Part1()
    {        
        BoxesGrid grid = new (@"..\..\..\input_15.txt");
        grid.ExecuteInputs();
        return grid.SumOfAllBoxGPSCoord();
    }

    
    public static int Part2()
    {
        string inputFile = @"..\..\..\input_15.txt";
        BoxesGrid grid = new(inputFile, expanded: true);
        Console.WriteLine(grid);
        grid.ExecuteInputs();

        Console.WriteLine(grid);
        return grid.SumOfAllBoxGPSCoord();
    }
}