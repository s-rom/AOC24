
using System.Numerics;
using System.Xml.XPath;

namespace Day10;



class Map 
{

    private readonly int TRAIL_START = 0;
    private readonly int TRAIL_END = 9;
    

    public static readonly GridVector[] DIRECTIONS = [
        GridVector.UP,
        GridVector.RIGHT,
        GridVector.DOWN,
        GridVector.LEFT
    ];

    private int[,] mapData;
    private bool[,] visited;
    public int Rows {get; private set;}
    public int Columns {get; private set;}

    public Map (string inputFile)
    {
        using StreamReader sr = File.OpenText(inputFile);

        List<List<int>> data = [];
        
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            data.Add(line.Select(x => x == '.' ? -1 : int.Parse(x.ToString())).ToList());
        }

        Rows = data.Count;
        Columns = data[0].Count;
        mapData = new int[Rows, Columns];
        visited = new bool[Rows, Columns];

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                mapData[r,c] = data[r][c];
                visited[r,c] = false;
            }
        }
    }

    public void ResetVisited()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                visited[r,c] = false;
            }
        }
    }

    public bool IsInBounds(GridVector pos)
    {
        return 
            pos.Column >= 0 && pos.Column < Columns &&
            pos.Row >= 0 && pos.Row < Rows;
    }

    public int FindAllTrailEnds()
    {
        int score = 0;

        foreach (var row in Enumerable.Range(0, Rows))
        {
            foreach(var col in Enumerable.Range(0, Columns))
            {
                if (mapData[row, col] == TRAIL_START)
                {
                    ResetVisited();
                    score += FindAllTrailEndsFrom(new (row, col), checkVisited: true);
                }
            }
        }


        return score;
    }

    public int FindAllTrails()
    {
        int score = 0;

        foreach (var row in Enumerable.Range(0, Rows))
        {
            foreach(var col in Enumerable.Range(0, Columns))
            {
                if (mapData[row, col] == TRAIL_START)
                {
                    ResetVisited();
                    score += FindAllTrailEndsFrom(new (row, col), checkVisited: false);
                }
            }
        }


        return score;
    }



    public int FindAllTrailEndsFrom(GridVector position, bool checkVisited)
    {

        if (!IsInBounds(position))
        {
            return 0;
        }


        visited[position.Row, position.Column] = true;
        int currentHeight = mapData[position.Row, position.Column];

        if (currentHeight == TRAIL_END)
        {
            return 1;   
        }

        int score = 0;

        foreach (var direction in DIRECTIONS)
        {
            GridVector nextPosition = direction + position;

            if (IsInBounds(nextPosition))
            {

                if (checkVisited && visited[nextPosition.Row, nextPosition.Column])
                {
                    continue;
                }

                int nextHeight = mapData[nextPosition.Row, nextPosition.Column]; 
                if (nextHeight - currentHeight == 1)
                {
                    score += FindAllTrailEndsFrom(nextPosition, checkVisited);
                }
            }
        }

        return score;
    }



    public override string ToString()
    {
        string result = "";

        for (int r  = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                result += mapData[r,c];
            }
            result += "\n";
        }


        return result;
    }

}


struct GridVector(int row, int col)
{

    public int Row { get; set; } = row;
    public int Column { get; set; } = col;

    public static readonly GridVector UP = new(-1, 0);
    public static readonly GridVector RIGHT = new(0, 1);
    public static readonly GridVector DOWN = new(+1, 0);
    public static readonly GridVector LEFT = new(0, -1);

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

}



class Day10
{

    private static readonly String inputFile = @"..\..\..\input_10.txt";

    public static int Part1()
    {
        Map m = new Map(inputFile);
        return m.FindAllTrailEnds();
    }


    public static int Part2()
    {
        Map m = new Map(inputFile);
        return m.FindAllTrails();    
    }
}