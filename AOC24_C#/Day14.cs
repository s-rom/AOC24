using System.Data;
using System.Text.RegularExpressions;
using Raylib_cs;

namespace Day14;




class Robot(Vector2<float> pos, Vector2<float> vel)
{
    public Vector2<float> Position {get; private set;} = pos;
    public Vector2<float> Velocity {get; } = vel;


    public void MoveForward(float seconds, int maxX, int maxY)
    {
        var newPos = Position + Velocity * seconds;
        Position = new (Mod(newPos.X, maxX), Mod(newPos.Y, maxY));
    }


    private static float Mod(float a, float b)
    {
        var r = a % b;
        if (r < 0)
        {
            return r + b; 
        }
        else
        {
            return r;
        }
    }

    public override string ToString()
    {
        return $"Pos: {Position.X}, {Position.Y} || Vel: {Velocity.X}, {Velocity.Y}";
    }

}


class RobotGrid 
{
    public List<Robot> Robots;
    public int MAX_X {get; private set;}
    public int MAX_Y {get; private set;}
    

    public RobotGrid(RobotGrid other)
    {
        this.MAX_X = other.MAX_X;
        this.MAX_Y = other.MAX_Y;
        this.Robots = new List<Robot>(other.Robots);
    }

    public RobotGrid(string inputFile, int maxX, int maxY)
    {
        Robots = [];
        this.MAX_X = maxX;
        this.MAX_Y = maxY;
        Regex linePattern = new(@"p=(\d+),(\d+) *v=(-?\d+),(-?\d+)");
        using StreamReader sr = File.OpenText(inputFile);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            var match = linePattern.Match(line);
            if (match is not null)
            {
                Robots.Add(
                    new (
                        new (int.Parse(match.Groups[1].ToString()), int.Parse(match.Groups[2].ToString())),
                        new (int.Parse(match.Groups[3].ToString()), int.Parse(match.Groups[4].ToString()))
                    )
                );
            }
            else
            {
                Console.WriteLine($"Cannot parse {line}");
            }
        }
    }


    public int[,] CreateRenderGrid()
    {
        int [,] renderGrid = new int[MAX_Y, MAX_X];

        for (int i = 0; i < MAX_Y; i++)
            for (int j = 0; j < MAX_X; j++)
                renderGrid[i,j] = 0;


        foreach(var r in Robots)
        {
            var row = r.Position.Y;
            var col = r.Position.X;

            renderGrid[(int)row,(int)col] ++;
        }

        return renderGrid;
    }

    public override string ToString()
    {
        var result = "";

        var renderGrid = CreateRenderGrid();
        for (int i = 0; i < MAX_Y; i++)
        {
            for (int j = 0; j < MAX_X; j++)
            {
                
                result += renderGrid[i,j] > 0? renderGrid[i,j] : ".";
            }
            result += "\n";
        }
        return result;
    }


    public void MoveRobot(ref Robot r, int seconds)
    {
        r.MoveForward(MAX_X, MAX_Y, seconds);
    }

    public void MoveAllRobots(float seconds)
    {
        foreach (var r in Robots)
        {
            r.MoveForward(seconds, MAX_X, MAX_Y);
        }
    }

    private bool IsInQuadrant(Robot r, int minX, int maxX, int minY, int maxY)
    {
        return 
            r.Position.X >= minX && r.Position.X <= maxX &&
            r.Position.Y >= minY && r.Position.Y <= maxY;
    }


    public bool TestChristmasTree()
    {
        var quadrants = GetQuadrantsDistribution();
        var biggest = quadrants.Max();
        var totalRobots = Robots.Count;
        var maxPercentage = (float)biggest / (float)totalRobots;

        return maxPercentage > 0.45f;
    }


    public List<int> GetQuadrantsDistribution()
    {
        var upLeft = 0;
        var upRight = 0;
        var downLeft = 0;
        var downRight = 0;

        foreach(var robot in Robots)
        {
            if (IsInQuadrant(robot, minX: 0, maxX: -1 + MAX_X/2, minY: 0, maxY: -1 + MAX_Y / 2))
            {
                upLeft ++;
            }
            
            if (IsInQuadrant(robot, minX: 1 + MAX_X/2, maxX: MAX_X, minY: 0, maxY: -1 + MAX_Y / 2))
            {
                upRight ++;
            }
            
            if (IsInQuadrant(robot, minX: 0, maxX: -1 + MAX_X/2, minY: 1+ MAX_Y /2, maxY: MAX_Y))
            {
                downLeft ++;
            }

            if (IsInQuadrant(robot, minX: 1 + MAX_X/2, maxX: MAX_X, minY: 1+ MAX_Y /2, maxY: MAX_Y))
            {
                downRight ++;
            }
        }

        return [upLeft, upRight, downLeft, downRight];
    }

    public int SafetyFactor()
    {
        var quadrants = GetQuadrantsDistribution();
        return quadrants.Aggregate(1, (accum, current) => accum * current);
    }



}

partial class Day14
{

    private static readonly string inputFile = @"..\..\..\input_14.txt";
    public static int Part1()
    {
        var MAX_X = 101;
        var MAX_Y = 103;
        RobotGrid r = new(inputFile, maxX: MAX_X, maxY: MAX_Y);


        r.MoveAllRobots(seconds: 100);

        Console.WriteLine(r);
        return r.SafetyFactor();
    }

    public static int Part2()
    {
        var MAX_X = 101;
        var MAX_Y = 103;
        RobotGrid robotGrid = new(inputFile, maxX: MAX_X, maxY: MAX_Y);

        robotGrid.MoveAllRobots(34);


        return 0;
    }


    public static int AltViz()
    {
        var MAX_X = 101;
        var MAX_Y = 103;
        RobotGrid robotGrid = new(inputFile, maxX: MAX_X, maxY: MAX_Y);


        var robotSize = 6;

        var gridWidth = robotSize * MAX_X;
        var gridHeight = robotSize * MAX_Y;

        var windowWidth = gridWidth;
        var gridYOffset = 40;

        var windowHeight = gridHeight + gridYOffset;


            

        Raylib.InitWindow(windowWidth, windowHeight, "Day 14 part 2");
        Raylib.SetTargetFPS(60);


        bool shouldStop = true;

        float simulationSeconds = (float) 7030;
        robotGrid.MoveAllRobots(simulationSeconds);

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            Raylib.DrawRectangle(0, gridYOffset, gridWidth, gridHeight, Color.Gray);

            if (!shouldStop)
            {
                float speed = 3f;
                float seconds = Raylib.GetFrameTime() * speed;

                robotGrid.MoveAllRobots(seconds: seconds);
                simulationSeconds += seconds;

                if (simulationSeconds >= 7037)
                {
                    robotGrid.MoveAllRobots(-simulationSeconds);
                    robotGrid.MoveAllRobots(7037f);
                    simulationSeconds = 7037f;
                    shouldStop = true;
                }
            }
            

            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                shouldStop = !shouldStop;
            }

            Raylib.DrawText(
                $"Seconds: {simulationSeconds}"
                , 0, 0, 26, Color.Black
            );

           
            foreach (var r in robotGrid.Robots)
            {
                Raylib.DrawCircleV(
                    new(
                        (float) r.Position.X * robotSize,
                        (float) r.Position.Y * robotSize + gridYOffset
                    ),
                    robotSize,
                    Color.Green
                );
            }


            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
        return 0;
    }


    public static int Part2Viz()
    {
        var MAX_X = 101;
        var MAX_Y = 103;
        RobotGrid robotGrid = new(inputFile, maxX: MAX_X, maxY: MAX_Y);




        int rectWidth = 6;
        int rectHeight = 6;

        var gridWidth = MAX_X * rectWidth;
        var gridHeight = MAX_Y * rectHeight;

        var windowWidth = gridWidth;
        var windowHeight = gridHeight + 40;

        var gridYOffset = windowHeight - gridHeight;
            

        Raylib.InitWindow(windowWidth, windowHeight, "Day 14 part 2");
        Raylib.SetTargetFPS(60);

        double secondsBetweenUpdate = 0.4;

        SyncTimer timer = new(secondsBetweenUpdate);

        bool shouldStop = true;

        robotGrid.MoveAllRobots(33);
        int simulationSeconds = 33;


        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            if (!shouldStop && timer.Ended)
            {
                timer.Reset();
                robotGrid.MoveAllRobots(seconds: 1);
                simulationSeconds ++;
            }
            
            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                shouldStop = !shouldStop;
            }

            Raylib.DrawText($"Seconds: {simulationSeconds}", 0, 0, 26, Color.Black);

            var renderGrid = robotGrid.CreateRenderGrid();
           
            for (int r = 0; r < MAX_Y; r++)
            {
                for (int c = 0; c < MAX_X; c++)
                {

                    var colorIntensity = (renderGrid[r,c] * 50);
                    var greenColor = new Color(0, colorIntensity, 0, 255);

                    var color = renderGrid[r,c]==0? Color.Black : greenColor;
                    Raylib.DrawRectangle(
                        posX: c * rectWidth,
                        posY: r * rectHeight + gridYOffset,
                        width: rectWidth,
                        height: rectHeight,
                        color 
                    );
                }        
            }

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
        return 0;
    }

}