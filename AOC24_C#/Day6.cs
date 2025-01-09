
struct GridState 
{

    public GridState(bool hasObstacle = false, bool visited = false) 
    {
        this.Visited = visited;
        this.HasObstacle = hasObstacle;
    }

    public bool Visited { get; set; } = false;

    public bool HasObstacle {get; set; } = false;

}


class GuardGrid
{
    
    public GuardGrid(GuardGrid g) 
    {
        this.Rows = g.Rows;
        this.Columns = g.Columns;
        this.data = Copy(g.data);
        this.StartingPlayerPos = g.StartingPlayerPos;
        this.TotalVisited = 1;
        this.currentPlayerPos = this.StartingPlayerPos;
        this.currentPlayerDirection = GridVector.UP;
    }

    private static GridState[][] Copy(GridState[][] grid) {

        int rows = grid.Length;
        int columns = grid[0].Length;

        GridState[][] newGrid = new GridState[rows][];

        for (int r = 0; r < rows; r++) 
        {
            newGrid[r] ??= new GridState[columns];
            
            for (int c = 0 ; c < columns; c++)
            {
                newGrid[r][c] = grid[r][c];

            }
        }
        return newGrid;
    }

    public int Rows { get; private set; }

    public int Columns { get; private set; }

    public int TotalVisited {get; private set;}

    public GridVector StartingPlayerPos { get; private set; }
    
    private GridVector currentPlayerPos;
   
    private GridVector currentPlayerDirection;
    


    private GridState [][] data;

    public GuardGrid(String inputFile) 
    {
        List<GridState[]> list = [];

        using StreamReader sr = File.OpenText(inputFile);
        String? line;
        int columns = 0;
        int rows = 0;

        while ((line = sr.ReadLine()) != null)
        {
            columns = line.Length;
            GridState[] rowList = new GridState[columns];

            for (int i = 0; i < line.Length; i++)
            {
                var character = line[i];
                switch(character)
                {
                    case '.':
                        rowList[i] = new GridState();
                        break;
                    case '#':
                        rowList[i] = new GridState(hasObstacle: true);
                        break;
                    case '^':
                        rowList[i] = new GridState(visited: true);
                        this.StartingPlayerPos = new GridVector(rows, i);
                        this.currentPlayerPos = this.StartingPlayerPos;
                        this.TotalVisited = 1;
                        break;
                }
            }


            list.Add(rowList);
            rows++;
        }

        this.currentPlayerDirection = GridVector.UP;
        this.data = list.ToArray();
        this.Columns = columns;
        this.Rows = rows;
    }

    public override string ToString()
    {
        String map = "";
        foreach (var row in data) {
            foreach (var col in row) {

                char repr = col.HasObstacle? '#' : '.';

                if (col.Visited) 
                {
                    repr = 'X'; 
                }

                map += repr;
            }
            map += "\n";
        }
        return map;
    }  


    public void AddObstacle(GridVector position) 
    {
        data[position.Row][position.Column].HasObstacle = true;
    }


    public bool IsInBounds(GridVector position) 
    {
        return
            position.Row >= 0 && position.Row < Rows && 
            position.Column >= 0 && position.Column < Columns;
    }


    public bool NextMovement() 
    {
        GridVector nextPosition = currentPlayerPos + currentPlayerDirection;

        if (!IsInBounds(nextPosition)) 
        {
            return false;
        }

        ref GridState nextCell = ref data[nextPosition.Row][nextPosition.Column];
        
        if (nextCell.HasObstacle) {
            currentPlayerDirection.Rotate90ToRight();
            return NextMovement();
        }


        if (!nextCell.Visited)
        {
           nextCell.Visited = true;
           TotalVisited ++;

        }

        currentPlayerPos = nextPosition;

        return true;

    }


    public bool IsGuardStuckInLoop() {

        int iter = 0;
        
        int MAX_ITER = Rows * Columns;

        while (iter <= MAX_ITER)
        {
            if (!NextMovement())
            {
                return false;
            }

            iter ++;
        }

        return true;
    }

}


class Day6
{

    private static string inputFile = @"..\..\..\input_6.txt";

    public static int part1() 
    {
        GuardGrid grid = new GuardGrid(inputFile);
        while(grid.NextMovement());
        return grid.TotalVisited;
    
    }

    public static int part2() 
    {
        GuardGrid grid = new GuardGrid(inputFile);

        var result = 0;

        for (int r = 0; r < grid.Rows; r++) 
        {
            for (int c = 0; c < grid.Columns; c++) {

                GuardGrid gridCopy = new GuardGrid(grid);
                var pos = new GridVector(r, c);

                if (pos == gridCopy.StartingPlayerPos) continue;
                gridCopy.AddObstacle(pos);
                if (gridCopy.IsGuardStuckInLoop())
                {
                    result++;
                }
            }
        }

        return result;
    }


}