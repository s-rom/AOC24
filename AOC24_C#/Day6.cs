
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


struct GridVector
{

    public static readonly GridVector UP = new(-1, 0);
    public static readonly GridVector RIGHT = new(0, 1);
    public static readonly GridVector DOWN = new(+1, 0);
    public static readonly GridVector LEFT = new(0, -1);




    public GridVector(int row, int col)
    {
        this.Row = row;
        this.Column = col;
    }

    public void Rotate90ToRight() 
    {
        if (this == UP) 
        {
            this = RIGHT;
        }
        else if (this == RIGHT)
        {
            this = DOWN;
        }
        else if (this == DOWN)
        {
            this = LEFT;
        }
        else
        {
            this = UP;
        }

    }

    public override string ToString()
    {
        return $"Row {Row}, Column: {Column}";
    }

    public static bool operator == (GridVector v1, GridVector v2)
    {
        return v1.Column == v2.Column && v1.Row == v2.Row;
    }

    public static bool operator != (GridVector v1, GridVector v2)
    {
        return v1.Column != v2.Column || v1.Row != v2.Row;
    }

    public static GridVector operator +(GridVector v1, GridVector v2) {
        return new GridVector(v1.Row + v2.Row, v1.Column + v2.Column);
    }

    public int Row {get; set;}
    public int Column {get; set;} 
}


class Grid
{
    
    public Grid(Grid g) 
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

    public Grid(String inputFile) 
    {
        List<GridState[]> list = [];

        using StreamReader sr = File.OpenText(inputFile);
        String line;
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
        Grid grid = new Grid(inputFile);
        while(grid.NextMovement());
        return grid.TotalVisited;
    
    }

    public static int part2() 
    {
        Grid grid = new Grid(inputFile);

        var result = 0;

        for (int r = 0; r < grid.Rows; r++) 
        {
            for (int c = 0; c < grid.Columns; c++) {

                Grid gridCopy = new Grid(grid);
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