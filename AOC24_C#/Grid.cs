class Grid<T>
{

    protected T[,] data;

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


    protected Grid()
    {

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

    protected void Fill(List<List<T>> raw)
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
                result += data[r,c];
            }
            result += "\n";
        }
    
        return result;
    }

}