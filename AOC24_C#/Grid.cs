class Grid<T>
{

    protected T[,] data;

    private Dictionary<GridVector, T>? debugValues = null;

    public int Rows {get; protected set;}
    public int Columns {get; protected set;}

    public Grid(int rows, int cols)
    {
        Rows = rows;
        Columns = cols;
        data = new T[rows, cols];
    }

    public Grid(int rows, int cols, T value)
    {
        Rows = rows;
        Columns = cols;
        data = new T[rows, cols];
        Fill(value);
    }

    protected Grid() {}

    public Grid(string inputFile, Func<char, T> parseChar)
    {
        using StreamReader sr = File.OpenText(inputFile);
        string? line;
        
        List<List<T>> rawData = [];
        int r = 0;
        while ((line = sr.ReadLine()) != null)
        {
            var column = line.ToCharArray().Select(x => parseChar(x)).ToList();
            rawData.Add(column);
            r ++;
        }

        Rows = r;
        Columns = rawData[0].Count;

        this.data = new T[Rows, Columns];

        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                this.data[i,j] = rawData[i][j];
            }
        }


    }


    public IEnumerable<GridVector> Positions()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                yield return new GridVector(r,c);
            }
        }
    }

    public IEnumerable<T> Values()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                yield return data[r,c];
            }
        }
    }

    public bool IsInBounds(GridVector v)
    {
        return 
            v.Row >= 0 && v.Row < Rows &&
            v.Column >= 0 && v.Column < Columns;
    }

    public void Fill(List<List<T>> raw)
    {
        Rows = raw.Count;
        Columns = raw[0].Count;

        this.data = new T[Rows, Columns];

        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Columns; c++)
            {
                data[r,c] = raw[r][c];
            }
        }
    }

    protected void Fill(T value)
    {
        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Columns; c++)
            {
                data[r,c] = value;
            }
        }
    }


    public GridVector? FirstPositionOf(T value)
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (data[r,c].Equals(value)) 
                    return new(r, c);
            }
        }


        return null;
    }

    public void InsertDebugValues(List<GridVector> positions, T value)
    {
        debugValues ??= [];
        foreach (var pos in positions)
        {
            debugValues.Add(pos, value);
        }
    }


    public void ClearDebugValues()
    {
        if (debugValues == null) return;

        debugValues.Clear();
    }

    public T ElementAt(GridVector v)
    {
        return data[v.Row, v.Column];
    }


    public void SetValue(GridVector v, T value)
    {
        data[v.Row, v.Column] = value;
    }

    public static IEnumerable<GridVector> FourNeighbours(GridVector pos)
    {
        yield return pos + GridVector.UP;
        yield return pos + GridVector.DOWN;
        yield return pos + GridVector.LEFT;
        yield return pos + GridVector.RIGHT;
    }

    public IEnumerable<GridVector> InBoundsFourNeighbours(GridVector pos)
    {
        foreach (var n in FourNeighbours(pos))
        {
            if (IsInBounds(n)) yield return n;
        }
    }



    public override string ToString()
    {
        var result = "";

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {

                GridVector pos = new(r, c);
                    
                if (debugValues != null && debugValues.TryGetValue(pos, out T? value))
                {
                    result += value;
                }
                else 
                {
                    result += data[r,c];
                }
            }
            result += "\n";
        }
    
        return result;
    }

}