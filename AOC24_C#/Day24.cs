using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Day24;



enum LogicOp
{
    AND,
    OR,
    XOR,
    INVALID
}

partial class Operation
{

    private readonly static Regex pattern = MyRegex();

    [GeneratedRegex(@"(\w+) (AND|XOR|OR) (\w+) -> (\w+)")]
    private static partial Regex MyRegex();


    public bool Executed {get; private set;}

    public string Op1 {get; private set;}

    public string Op2 {get; private set;}
    public string Op3 {get; private set;}

    public LogicOp LogicOperation {get; private set;}

    public Operation()
    {
        Op1 = "";
        Op2 = "";
        Op3 = "";
    }


    public Operation(string opStr)
    {
        Op1 = "";
        Op2 = "";
        Op3 = "";
        LogicOperation = LogicOp.INVALID;

        var match = pattern.Match(opStr);
        if (match.Success)
        {
            Op1 = match.Groups[1].Value;
            Op2 = match.Groups[3].Value;
            Op3 = match.Groups[4].Value;
            LogicOperation = Enum.Parse<LogicOp>(match.Groups[2].Value);
        }
    }


    public bool CanBeSolved(Dictionary<string, bool> variables)
    {
        return variables.ContainsKey(Op1) && variables.ContainsKey(Op2);
    }


    public void Execute(Dictionary<string, bool> variables)
    {
        switch(this.LogicOperation)
        {
            case LogicOp.AND:
                variables[Op3] = variables[Op1] && variables[Op2];
            break;
            
            case LogicOp.OR:
                variables[Op3] = variables[Op1] || variables[Op2];
            break;

            case LogicOp.XOR:
                variables[Op3] = variables[Op1] ^ variables[Op2];
            break;

            case LogicOp.INVALID:
                Console.Error.WriteLine("Invalid logical operation found");
            break;
        }

        Executed = true;
    }

    public static bool IsOperation(string opStr)
    {
        return pattern.Match(opStr).Success;    
    }
}

class Day24
{

    private static void ParseInput(string inputFile, out Dictionary<string, bool> variables, out List<Operation> ops)
    {
        variables = [];
        ops = [];

        foreach (var line in File.ReadAllLines(inputFile))
        {
            if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line)) continue;

            if (Operation.IsOperation(line))    
            {
                ops.Add(new(line));
            }
            else 
            {
                var split = line.Split(':');
                var varName = split[0].Trim();
                var varValue = int.Parse(split[1].Trim()) == 1;
                
                variables[varName] = varValue;
            }
 
        }
    }


    public static ulong Part1()
    {
        ParseInput(
            @"..\..\..\input_24.txt",
            out Dictionary<string, bool> variables,
            out List<Operation> operations
        );



        int operationsSolved = 0;
        while (operationsSolved != operations.Count)
        {
            var ops = operations.Where(x => x.CanBeSolved(variables) && !x.Executed).ToList();
            ops.ForEach(x => x.Execute(variables));
            operationsSolved += ops.Count;
        }


        var outputVars = variables
            .Where(x => x.Key.StartsWith('z'))
            .OrderByDescending(x => x.Key)
            .Select(x => x.Value? (ulong)1L: (ulong)0L);

        ulong result = 0L;
        foreach (var outVar in outputVars)
        {
            result <<= 1;
            result |= outVar;
        }
       
        return result;
    }

    
}