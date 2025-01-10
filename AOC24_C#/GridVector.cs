
struct GridVector
{

    public static readonly GridVector UP = new(-1, 0);
    public static readonly GridVector RIGHT = new(0, 1);
    public static readonly GridVector DOWN = new(+1, 0);
    public static readonly GridVector LEFT = new(0, -1);

    public static readonly GridVector ZERO = new (0, 0);

    public GridVector(int row, int col)
    {
        this.Row = row;
        this.Column = col;
    }

    public GridVector()
    {
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

    public static GridVector operator -(GridVector v1, GridVector v2) {
        return new GridVector(v1.Row - v2.Row, v1.Column - v2.Column);
    }

    public static GridVector operator *(GridVector v1, int s) {
        return new GridVector(v1.Row * s, v1.Column * s);
    }
    
    public static int ManhattanDistance(GridVector v1, GridVector v2)
    {
        return Math.Abs(v1.Row - v2.Row) + Math.Abs(v1.Column - v2.Column);
    }

    // public GridVector direction(GridVector from, GridVector to)
    // {
    //     return new(Math.Sign(to.Row - from.Row), Math.Sign(to.Column - from.Column));
    // }


    public int Row {get; set;}
    public int Column {get; set;} 
}
