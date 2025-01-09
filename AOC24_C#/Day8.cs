

class AntenaGrid 
{

    Dictionary<char, List<Vector2<int>>> antenas;
    HashSet<Vector2<int>> antinodes;

    public int MAX_X {get; private set;}
    public int MAX_Y {get; private set;}

    public int Antinodes {
        get => antinodes.Count;
    }

    public AntenaGrid(String fileInput)
    {

        int x = 0;
        int y = 0;

        antenas = [];
        antinodes = [];

        using StreamReader sr = File.OpenText(fileInput);
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            x = 0;

            foreach (var c in line)
            {
                if (c != '.')
                {
                    Vector2<int> pos = new (x, y);
                    
                    if (!antenas.ContainsKey(c)){
                        antenas.Add(c, []);    
                    }

                    antenas[c].Add(pos);
                }
                x ++;
            }
            y ++;
        }

        this.MAX_X = x;
        this.MAX_Y = y;


        foreach(var c in antenas.Keys)
        {
            antenas[c] = antenas[c].Select(
                point => FlipY(point)
            ).ToList();
        }

       
    }


    public bool IsInRange(Vector2<int> pos){
        return 
            pos.X >= 0 && pos.X < MAX_X && 
            pos.Y >= 0 && pos.Y < MAX_Y;
    }

    public void AddAntinode(Vector2<int> anti)
    {
        if (this.IsInRange(anti) && !antinodes.Contains(anti))
        {
            antinodes.Add(anti);
        }
    }

    
    public static (Vector2<int>, Vector2<int>) GetAntinodes(Vector2<int> a1, Vector2<int> a2)
    {
        Vector2<int> v = a2 - a1;
        return new(a2 + v, a1 - v);
    }

    public void GenerateAllAntinodes()
    {
        foreach (var c in antenas.Keys)
        {
            foreach (var a1 in antenas[c])
            {
                foreach (var a2 in antenas[c])
                {
                    if (a1 == a2) continue;

                    var (anti1, anti2) = GetAntinodes(a1, a2);
                    AddAntinode(anti1);
                    AddAntinode(anti2);
                }
            }
        }
    }

    public void GeneratePart2Antinodes()
    {
        foreach (var c in antenas.Keys)
        {
            foreach (var a in antenas[c])
            {
                foreach (var b in antenas[c])
                {
                    if (a == b) continue;

                    Vector2<int> v = b - a;
                   
                    var i = 1;
                    Vector2<int> anti = b + Vector2<int>.Scale(v, i);
                    while (IsInRange(anti))
                    {
                        AddAntinode(anti);
                        i++;
                        anti = b + Vector2<int>.Scale(v, i);
                    }

                    i = 1;
                    anti = b - Vector2<int>.Scale(v, i);
                    while (IsInRange(anti))
                    {
                        AddAntinode(anti);
                        i++;
                        anti = b - Vector2<int>.Scale(v, i);
                    }


                }
            }
        }
    }

    private Vector2<int> FlipY(Vector2<int> v)
    {
       
        return new (v.X, this.MAX_Y - v.Y - 1);
    }

    public String Render()
    {
        String result = "";
        
        char [,] grid = new char[MAX_Y, MAX_X];
        for (int i = 0; i < MAX_Y; i++)
        {
            for (int j = 0; j< MAX_X; j++)
            {
                grid[i,j] = '.';

                Vector2<int> pos = this.FlipY(new (j, i));

                foreach (var anti in antinodes)
                {
                    if (anti == pos)
                    {
                        grid[i, j] = '#';
                    }
                }

            }
        }


        for (int i = 0; i < MAX_Y; i++)
        {
            for (int j = 0; j< MAX_X; j++)
            {
                result += grid[i, j];
            }
            result += "\n";
        }
        


        return result;
    }

}


class Day8
{

    private static readonly String inputPath = @"..\..\..\input_8.txt";

    public static int Part1() 
    {
        var grid = new AntenaGrid(inputPath);
        grid.GenerateAllAntinodes();
        return grid.Antinodes;
    }


    public static int Part2() 
    {
        var grid = new AntenaGrid(inputPath);
        grid.GeneratePart2Antinodes();
        // Console.WriteLine(grid.Render());
        return grid.Antinodes;
    }

}
