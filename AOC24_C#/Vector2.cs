

using System.Numerics;

struct Vector2<T> where T: INumber<T>
{

    public T X {get; set;}
    public T Y {get; set;} 

    public Vector2(T x, T y) : this()
    {
        this.X = x;
        this.Y = y;
    }

    public override string ToString()
    {
        return $"X {X}, Y: {Y}";
    }



    public static bool operator == (Vector2<T> v1, Vector2<T> v2)
    {
        return v1.X == v2.X && v1.Y == v2.Y;
    }

    public static bool operator != (Vector2<T> v1, Vector2<T> v2)
    {
        return v1.X != v2.X || v1.Y != v2.Y;
    }

    public static Vector2<T> Scale(Vector2<T> v, T t)
    {
        return new (v.X * t, v.Y * t);
    }

    public static Vector2<T> operator *(Vector2<T> v, T t)
    {
        return Scale(v, t);
    }

    public static Vector2<T> operator +(Vector2<T> v1, Vector2<T> v2) {
        return new Vector2<T>(v1.X + v2.X, v1.Y + v2.Y);
    }

    public static Vector2<T> operator -(Vector2<T> v1, Vector2<T> v2) {
        return new Vector2<T>(v1.X - v2.X, v1.Y - v2.Y);
    }

}
