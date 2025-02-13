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

    public bool OperandIs(string str)
    {
        return Op1 == str || Op2 == str;
    }

    public bool OperandIsAnyOf(string[] operands)
    {
        foreach (var op in operands)
        {
            if (OperandIs(op)) return true;
        }

        return false;
    }


    public bool ResultIs(string str)
    {
        return Op3 == str;
    }


    public bool ResultIsAnyOf(string[] results)
    {
        foreach (var op in results)
        {
            if (ResultIs(op)) return true;
        }

        return false;
    }
    


    public override string ToString()
    {
        return $"{Op1} {LogicOperation} {Op2} = {Op3}";
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

    public static ulong GetVarAsUlong(Dictionary<string, bool> variables, char varName = 'z')
    {
        var outputVars = variables
            .Where(x => x.Key.StartsWith(varName))
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

    public static void ExecuteInstructions(Dictionary<string, bool> variables, List<Operation> operations)
    {
        int operationsSolved = 0;
        while (operationsSolved != operations.Count)
        {
            var ops = operations.Where(x => x.CanBeSolved(variables) && !x.Executed).ToList();
            ops.ForEach(x => x.Execute(variables));
            operationsSolved += ops.Count;
        }
    }


    public static ulong Part1()
    {
        ParseInput(
            @"..\..\..\input_24.txt",
            out Dictionary<string, bool> variables,
            out List<Operation> operations
        );

        ExecuteInstructions(variables, operations);   
        return GetVarAsUlong(variables);
    }

    

    /*

                                                                                           
             |         |            |         |             |         |                   
             |A2       |B2          |A1       |B1           |A0       |B0                 
             |         |            |         |             |         |                   
             V         V            V         V             V         V                   
            --------------         --------------          --------------                 
      Cout  |            |   Cout  |            |    Cout  |            |    Cin          
   <--------|   1 Bit    |<--------|   1 Bit    | <--------|   1 Bit    | <--------       
            |    adder   |         |    adder   |          |    adder   |                 
            |            |         |            |          |            |                 
            --------------         --------------          --------------                 
                                                                                          
    Full adder

   

    */

    public static ulong Part2()
    {

        ParseInput(
            @"..\..\..\input_24.txt",
            out Dictionary<string, bool> variables,
            out List<Operation> operations
        );

        /*
            X(44b) + Y(44b) = Z(45b)
            ----------------------------

            SUM  = A XOR B XOR Cin
            Cout = AB + (Cin(A XOR B))                                                                    
            
            ----------------------------
            
            x1 + y1 + c0 = z1, c1

                x1 XOR y1 = tcd
                tcd XOR bwv = z1

                x1 and y1 = wqt
                bwv AND tcd = sgv
                wqt OR sgv = hqq (c1) 

        */
        
        operations
            .Where(x => 
                    x.ResultIsAnyOf(["z01"]) ||
                    x.OperandIsAnyOf(["x01", "y01", "tcd", "sgv"])
                )
            .ToList()
            .ForEach(x => Console.WriteLine(x));


    
        string xVar, yVar;

        foreach (var i in Enumerable.Range(0, 44 + 1))
        {
            xVar = "x";
            if (i <= 9) xVar += "0";
            xVar += i.ToString();
            yVar = xVar.Replace('x', 'y');

            variables[xVar] = false;
            variables[yVar] = false;
        }

        // Index of the only bit at one. (0 is rightmost <-> LSB)
        var idxOfOne = 0;
        xVar = "x";
        if (idxOfOne <= 9) xVar += "0";
        xVar += idxOfOne.ToString();
        yVar = xVar.Replace('x', 'y');

        variables[xVar] = true;
        variables[yVar] = true;


        ExecuteInstructions(variables, operations);
        
        return GetVarAsUlong(variables);

    }
}