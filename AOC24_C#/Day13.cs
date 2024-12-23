using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace Day13;



struct ClawMachine (Vector2<long> buttonA, Vector2<long> buttonB, Vector2<long> prize)
{
    public Vector2<long> ButtonA { get; private set;} = buttonA;
    public Vector2<long> ButtonB { get; private set;} = buttonB;
    public Vector2<long> Prize{ get; private set;} = prize;

    public void FixPrize()
    {
        this.Prize = new(Prize.X + 10000000000000, Prize.Y + 10000000000000);
    }

    public Vector2<long> Solve()
    {
        long aPresses;
        long bPresses;


        aPresses = (Prize.X * ButtonB.Y - Prize.Y * ButtonB.X) /
                    (ButtonA.X * ButtonB.Y - ButtonA.Y * ButtonB.X);

        bPresses = (ButtonA.X*Prize.Y - ButtonA.Y*Prize.X) / 
                    (ButtonA.X*ButtonB.Y - ButtonA.Y*ButtonB.X);


        return new(aPresses, bPresses);
    }


    public bool TestSolution(Vector2<long> solution) 
    {
        long aPresses = solution.X;
        long bPresses = solution.Y;

        return 
            ButtonA.X * aPresses + ButtonB.X * bPresses == Prize.X &&
            ButtonA.Y * aPresses + ButtonB.Y * bPresses == Prize.Y;
    }


}

class Day13
{

    private static readonly string inputFile = @"..\..\..\input_13.txt";

    private static List<ClawMachine> ParseInput()
    {
        Regex  buttonAPattern = new Regex(@"Button A: X\+(\d+), Y\+(\d+)");
        Regex  buttonBPattern = new Regex(@"Button B: X\+(\d+), Y\+(\d+)");
        Regex  prizePattern = new Regex(@"Prize: X=(\d+), Y=(\d+)");

        List<ClawMachine> list = [];
        using StreamReader sr = File.OpenText(inputFile);
        var lines = sr.ReadToEnd().Split("\n", StringSplitOptions.RemoveEmptyEntries).ToList();

        for (int i = 0; i < lines.Count; i += 3)
        {
            var A = buttonAPattern.Match(lines[i]);
            var AX = long.Parse(A.Groups[1].ToString());
            var AY = long.Parse(A.Groups[2].ToString());

            var B = buttonBPattern.Match(lines[i + 1]);
            var BX = long.Parse(B.Groups[1].ToString());
            var BY = long.Parse(B.Groups[2].ToString());

            var prize = prizePattern.Match(lines[i + 2]);
            var PX = long.Parse(prize.Groups[1].ToString());
            var PY = long.Parse(prize.Groups[2].ToString());

            list.Add(new ClawMachine(new(AX, AY), new (BX, BY), new(PX,PY)));

        }

        return list;
    }

    public static long Part1()
    {
        var machines = ParseInput();
        long total = 0;
        foreach (var machine in machines)
        {
            var solution = machine.Solve();
            if (machine.TestSolution(solution))
            {
                total += solution.X * 3 + solution.Y;
            }
        }
        return total;
    }

    public static long Part2()
    {
        var machines = ParseInput();

        long total = 0;
        foreach (var machine in machines)
        {
            machine.FixPrize();
            var solution = machine.Solve();
            if (machine.TestSolution(solution))
            {
                total += solution.X * 3 + solution.Y;
            }
        }
        return total;    
    }
}