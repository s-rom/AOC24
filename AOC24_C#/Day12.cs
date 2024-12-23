namespace Day12;

using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.SymbolStore;
using System.Security.Cryptography.X509Certificates;
using Day10;
using Raylib_cs;


class GardenRegion
{
    public List<GridVector> Positions
    {
        get;
        private set; 
    }

    public int Area { get => Positions.Count; }

    public char Plant { get; }

    public int GetPerimeter(GardenGrid garden)
    {
        
        int p = 0;

        foreach (var pos in Positions)
        {
            foreach (var adjPos in garden.AdjacentPositions(pos))
            {
                if (!garden.InBounds(adjPos)) 
                {
                    p ++;
                }
                else if (garden.PlantAt(adjPos) != this.Plant)
                {
                    p ++;
                }
            }
        }

        return p;
    }



    public bool PositionOutOfRegion(GridVector pos, GardenGrid garden)
    {
        return !garden.InBounds(pos) || garden.PlantAt(pos) != this.Plant;
    }


    public int ConcaveCorners(GridVector pos, GardenGrid garden)
    {

        var up = pos + GridVector.UP;
        var down = pos + GridVector.DOWN;
        var left = pos + GridVector.LEFT;
        var right = pos + GridVector.RIGHT;
        
        int corners = 0;

        if (PositionOutOfRegion(up,garden) && PositionOutOfRegion(right, garden))
            corners++;
    
        if (PositionOutOfRegion(right,garden) && PositionOutOfRegion(down, garden))
            corners++;

        if (PositionOutOfRegion(down,garden) && PositionOutOfRegion(left, garden))
            corners++;

        if (PositionOutOfRegion(left,garden) && PositionOutOfRegion(up, garden))
            corners++;


        return corners;
    }


    public int ConvexCorners(GridVector pos, GardenGrid garden)
    {
        var up = pos + GridVector.UP;
        var down = pos + GridVector.DOWN;
        var left = pos + GridVector.LEFT;
        var right = pos + GridVector.RIGHT;

        var up_right = pos + GridVector.UP_RIGHT;
        var up_left = pos + GridVector.UP_LEFT;
        var down_right = pos + GridVector.DOWN_RIGHT;
        var down_left = pos + GridVector.DOWN_LEFT;

        int corners = 0;

        if (PositionOutOfRegion(up_right, garden) && 
            !PositionOutOfRegion(up, garden) && !PositionOutOfRegion(right, garden))
        {
            corners ++;
        }

        if (PositionOutOfRegion(up_left, garden) && 
            !PositionOutOfRegion(up, garden) && !PositionOutOfRegion(left, garden))
        {
            corners ++;
        }

        if (PositionOutOfRegion(down_right, garden) && 
            !PositionOutOfRegion(down, garden) && !PositionOutOfRegion(right, garden))
        {
            corners ++;
        }

        if (PositionOutOfRegion(down_left, garden) && 
            !PositionOutOfRegion(down, garden) && !PositionOutOfRegion(left, garden))
        {
            corners ++;
        }

        return corners;
    }


    public int GetCorners(GardenGrid garden)
    {
        int corners = 0;
        foreach (var pos in Positions)
        {
            corners += ConvexCorners(pos, garden) + ConcaveCorners(pos, garden);
        }
        return corners;
    }


    public GardenRegion(List<GridVector> positions, char plant)
    {
        this.Positions = positions;
        this.Plant = plant;
    }


}


class GardenGrid
{
    public int Rows {get; private set;}

    public int Columns {get; private set;}

    public int Plants { get => Rows * Columns; }
    
    private char[,] gardenGrid;

    private bool[,] visited;

    public List<GardenRegion> Regions {get; private set;}

    public GardenGrid(string inputFile)
    {
        List<List<char>> data = [];
        Regions = [];

        using StreamReader sr = File.OpenText(inputFile);
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            data.Add(line.ToCharArray().ToList());
        }

        this.Rows = data.Count;
        this.Columns = data[0].Count;

        this.gardenGrid = new char[Rows,Columns];
        this.visited = new bool[Rows,Columns];

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                this.gardenGrid[r,c] = data[r][c];
                this.visited[r,c] = false;
            }
        }
    }


    public char PlantAt(GridVector pos)
    {
        return gardenGrid[pos.Row,pos.Column];
    }

    public bool InBounds(GridVector pos)
    {
        return 
            pos.Column >= 0 && pos.Column < Columns &&
            pos.Row >= 0 && pos.Row < Rows;
    }

    private void SetVisited(GridVector pos)
    {
        this.visited[pos.Row, pos.Column] = true;
    }

    private bool IsVisited(GridVector pos)
    {
        return this.visited[pos.Row, pos.Column];
    }

    public IEnumerable<GridVector> InBoundsNeighbours(GridVector pos)
    {

        foreach (var next in AdjacentPositions(pos))
        {
            if (InBounds(next))
                yield return next;
        }
    }

    public IEnumerable<GridVector> AdjacentPositions(GridVector pos)
    {
        yield return pos + GridVector.LEFT;
        yield return pos + GridVector.RIGHT;
        yield return pos + GridVector.UP;
        yield return pos + GridVector.DOWN;
    }


    public void DiscoverAllRegions()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                GridVector pos = new(r, c);
                if (IsVisited(pos)) continue;

                var region = DiscoverRegion(pos);
                Regions.Add(region);
            }
        }
    }


    public int GetFencePriceSimple()
    {
        int price = 0;

        foreach (var region in Regions)
        {
            price += region.GetPerimeter(this) * region.Area;
        }

        return price;
    }


    public int GetFencePriceOptimized()
    {
        int price = 0;

        foreach (var region in Regions)
        {
            price += region.GetCorners(this) * region.Area;
        }

        return price;
    }



    public GardenRegion DiscoverRegion(GridVector startingPos)
    {
        List<GridVector> region = [];
        char plant = PlantAt(startingPos);
        region.Add(startingPos);

        Queue<GridVector> queue = [];
        queue.Enqueue(startingPos);
        SetVisited(startingPos);

        while (queue.Count != 0)
        {
            GridVector pos = queue.Dequeue();
            
            foreach (var next in InBoundsNeighbours(pos))
            {
                if (IsVisited(next)) continue;
                if (PlantAt(next) == plant) 
                {
                    queue.Enqueue(next);
                    region.Add(next);
                    SetVisited(next);
                }
            }
        }

        return new GardenRegion(region, plant);
    }


    public override string ToString()
    {
        string map = "";
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                map += gardenGrid[r,c];
            }
            map += "\n";
        }
        return map;
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

    public static readonly GridVector UP_RIGHT = new(-1, +1);
    public static readonly GridVector UP_LEFT = new(-1, -1);
    public static readonly GridVector DOWN_RIGHT = new(+1, +1);
    public static readonly GridVector DOWN_LEFT = new(+1, -1);

    


    public override string ToString()
    {
        return $"r{Row} c{Column}";
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


class Day12
{

    private static readonly Color [] colors=
    {
        new(255, 0, 0, 255),     // Rojo puro
        new(0, 255, 0, 255),     // Verde puro
        new(0, 0, 255, 255),     // Azul puro
        new(255, 255, 0, 255),   // Amarillo puro
        new(0, 255, 255, 255),   // Cian puro
        new(255, 0, 255, 255),   // Magenta puro

        new(128, 0, 0, 255),     // Rojo oscuro
        new(0, 128, 0, 255),     // Verde oscuro
        new(0, 0, 128, 255),     // Azul oscuro
        new(255, 128, 0, 255),   // Naranja brillante
        new(128, 128, 0, 255),   // Oliva
        new(0, 128, 128, 255),   // Verde azulado

        new(128, 0, 128, 255),   // Morado oscuro
        new(255, 255, 128, 255), // Amarillo pastel brillante
        new(255, 128, 128, 255), // Rojo pastel brillante
        new(128, 255, 128, 255), // Verde pastel brillante
        new(128, 128, 255, 255), // Azul pastel brillante
        new(255, 128, 255, 255), // Magenta pastel brillante

        new(64, 0, 0, 255),      // Rojo muy oscuro
        new(0, 64, 0, 255),      // Verde muy oscuro
        new(0, 0, 64, 255),      // Azul muy oscuro
        new(64, 64, 64, 255),    // Gris oscuro
        new(192, 192, 192, 255), // Gris claro
        new(245, 245, 245, 255),
        new(0, 0, 0, 255),       // Negro puro
        new(255, 69, 0, 255),    // Rojo anaranjado
        new(75, 0, 130, 255),    // Índigo
        new(255, 20, 147, 255),  // Rosa fuerte
    };

    private static string inputFile = @"..\..\..\input_12.txt";



    public static void Visualize()
    {
        GardenGrid garden = new GardenGrid(inputFile);

        int WINDOW_WIDTH = 700;
        int WINDOW_HEIGHT = 700;

        int rectHeight = WINDOW_HEIGHT / garden.Rows;
        int rectWidth = WINDOW_WIDTH / garden.Columns;

        Raylib.InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "Day 12 Viz");
        Raylib.SetTargetFPS(60);
        Console.WriteLine($"{garden.Rows}, {garden.Columns}");


        Queue<Color> colorsQueue = new (colors);
        Dictionary<char, Color> plantColors = [];

        garden.DiscoverAllRegions();
        var regions = garden.Regions;

        foreach (var region in regions)
        {
            if (!plantColors.ContainsKey(region.Plant))
            {
                plantColors.Add(region.Plant, colorsQueue.Dequeue());
            }
        }

        Font customFont = Raylib.LoadFontEx(@"..\..\..\04B_30__.TTF", 48, null, 0);
        
        Console.WriteLine($"{rectWidth} x {rectHeight}");

        Queue<GridVector> regionsPoints = [];

        foreach (var region in regions)
        {
            foreach (var pos in region.Positions)
            {
                regionsPoints.Enqueue(pos);
            }
        } 


        List<GridVector> renderPoints = [];


        
        while (!Raylib.WindowShouldClose())
        {

           
            for (int i = 0; i < 30; i++)
            {
                if (regionsPoints.Count > 0)
                {
                    renderPoints.Add(regionsPoints.Dequeue());
                }
            }
            

        
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            // Raylib.DrawText(timeAcc + " s", 12, 12, 24, Color.Black);

            foreach (var pos in renderPoints)
            {
                var plant = garden.PlantAt(pos);
                var color = plantColors[plant];

                int sx = pos.Column * rectWidth;
                int sy = pos.Row * rectHeight;

                Raylib.DrawRectangle(sx, sy, rectWidth, rectHeight, color);
            }


            for (int r = 0; r < garden.Rows; r++)
            {
                for (int c = 0; c < garden.Columns; c++)
                {
                    int sx = c * rectWidth;
                    int sy = r * rectHeight; 
                    Raylib.DrawRectangleLinesEx(new(sx, sy, rectWidth, rectHeight), 0.6f, Color.Black);
                }
            }


                    // Raylib.DrawRectangle(sx, sy, rectWidth, rectHeight, plantColors[plant[0]]);
                    // Raylib.DrawRectangleLinesEx(new Rectangle(sx, sy, rectWidth, rectHeight), 0.5f, Color.Black);

            Raylib.EndDrawing();

        }

        Raylib.CloseWindow();
    }

    public static void Test()
    {

        GardenGrid garden = new GardenGrid(inputFile);

        int WINDOW_WIDTH = 700;
        int WINDOW_HEIGHT = 700;

        int rectHeight = WINDOW_HEIGHT / garden.Rows;
        int rectWidth = WINDOW_WIDTH / garden.Columns;

        Raylib.InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "Day 12 Viz");
        Raylib.SetTargetFPS(60);
        Console.WriteLine($"{garden.Rows}, {garden.Columns}");


        Queue<Color> colorsQueue = new (colors);
        Dictionary<char, Color> plantColors = [];

        garden.DiscoverAllRegions();
        var regions = garden.Regions;

        Font customFont = Raylib.LoadFontEx(@"..\..\..\04B_30__.TTF", 48, null, 0);
        
        Console.WriteLine($"{rectWidth} x {rectHeight}");

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            int sx;
            int sy = 0;

            for (int r = 0; r < garden.Rows; r++)
            {
                sy = r * rectHeight;
                for (int c = 0 ; c < garden.Columns; c++)
                {
                    sx = c * rectWidth;


                    string plant = garden.PlantAt(new(r,c)).ToString();                    
                    if (!plantColors.ContainsKey(plant[0]))
                    {
                        
                        var newColor = colorsQueue.Dequeue();
                        plantColors.Add(plant[0], newColor);
                    }

                    Raylib.DrawRectangle(sx, sy, rectWidth, rectHeight, plantColors[plant[0]]);
                    Raylib.DrawRectangleLinesEx(new Rectangle(sx, sy, rectWidth, rectHeight), 0.5f, Color.Black);
                }
            }



            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    public static int Part1()
    {
        GardenGrid garden = new(inputFile);
        garden.DiscoverAllRegions();
        return garden.GetFencePriceSimple();

    }


    public static int Part2()
    {
        GardenGrid garden = new(inputFile);
        garden.DiscoverAllRegions();
        return garden.GetFencePriceOptimized();    
    }


}