namespace Day17;

public enum Opcode
{
    ADV = 0,
    BXL = 1,
    BST = 2,
    JNZ = 3,
    BXC = 4,
    OUT = 5,
    BDV = 6,
    CDV = 7
}

class ChronoSpatialProgram
{
    public ulong A {get; set;} = 0;
    public ulong B {get; set;} = 0;
    public ulong C {get; set;} = 0;

    private int programCounter = 0;

    private List<ulong> Program;

    private List<ulong> output;
    
    private Dictionary<Opcode, ExecuteInstruction> actions;

    public string GetOutput()
    {
        return string.Join(",", output);
    }

    public ChronoSpatialProgram(List<ulong> program)
    {
        Program = new List<ulong>(program);
        output = [];
        actions = new()
        {
            {Opcode.ADV, ExecuteADV},
            {Opcode.BDV, ExecuteBDV},
            {Opcode.CDV, ExecuteCDV},
            {Opcode.BST, ExecuteBST},
            {Opcode.OUT, ExecuteOUT},
            {Opcode.BXL, ExecuteBXL},
            {Opcode.BXC, ExecuteBXC}
        };
    }

    public ChronoSpatialProgram(List<ulong> program, ulong A)
    : this(program)
    {
        this.A = A;
    }

    

    private delegate void ExecuteInstruction(ulong operand);

    private delegate string ExplainInstruction(ulong operand);


    
    public string Dissasemble()
    {
        var result = "";
        programCounter = 0;

        while (programCounter < Program.Count)
        {
            var instruction = (Opcode) Program[programCounter];
            ulong operand = Program[programCounter + 1];

            result += DissasembleInstruction(instruction, operand);
            result += "\n";

            programCounter += 2;

        }
        return result;
    }

    public string DissasembleInstruction(Opcode instruction, ulong operand)
    {
        var result = "";

        Dictionary<Opcode, ExplainInstruction> explain = new()
        {
            {Opcode.ADV, (x) => ExplainDIV("A", x)},
            {Opcode.BDV, (x) => ExplainDIV("B", x)},
            {Opcode.CDV, (x) => ExplainDIV("C", x)},
            {Opcode.BXC, ExplainBXC},
            {Opcode.JNZ, ExplainJNZ},
            {Opcode.OUT, ExplainOUT},
            {Opcode.BXL, ExplainBXL},
            {Opcode.BST, ExplainBST}
        };

        result += instruction + " ";

        if (instruction == Opcode.JNZ || // literal
            instruction == Opcode.BXL)    // literal
        {
            result += operand;
        }
        else if (instruction != Opcode.BXC)
        {
            result += GetComboString(operand);
        }


        // Explain
        result += "\t// " + explain[instruction].Invoke(operand); 

        return result;
    }


    public void Execute()
    {   
        programCounter = 0;

        while (programCounter < Program.Count)
        {
            var instruction = (Opcode) Program[programCounter];
            ulong operand = Program[programCounter + 1];

            if (instruction == Opcode.JNZ)
            {
                if (A != 0)
                {
                    programCounter = (int)operand;
                    continue;
                }
            }
            else
            {
                actions[instruction].Invoke(operand);
            }

            programCounter += 2;

        }

    }

    public void ExecuteWithHeuristic()
    {   
        programCounter = 0;
        var expected = new Queue<ulong>(Program);

        while (programCounter < Program.Count)
        {
            var instruction = (Opcode) Program[programCounter];
            ulong operand = Program[programCounter + 1];

            if (instruction == Opcode.JNZ)
            {
                if (A != 0)
                {
                    programCounter = (int)operand;
                    continue;
                }
            }
            else
            {
                actions[instruction].Invoke(operand);

                if (instruction == Opcode.OUT && output.Last() != expected.Dequeue())
                {
                    return;
                }
            }

            programCounter += 2;

        }

    }


    public ulong GetComboOperand(ulong operand)
    {
        return operand switch
        {
            <= 3 => operand,
               4 => A,
               5 => B,
               6 => C,
               _ => throw new Exception($"Invalid operand found: {operand}")
        };
    }


    public string GetComboString(ulong operand)
    {
        return operand switch
        {
            <= 3 => operand.ToString(),
               4 => "A",
               5 => "B",
               6 => "C",
               _ => throw new Exception($"Invalid operand found: {operand}")
        };
    }


    private string ExplainJNZ(ulong operand)
    {
        return "if A!=0 JUMP to " + operand;
    }

    public void ExecuteADV(ulong operand)
    {
        var denominator = GetComboOperand(operand);
        A = (ulong) Math.Floor(A / Math.Pow(2, denominator));
    }

    private string ExplainDIV(string reg, ulong operand)
    {
        string op;

        if (operand <= 3)
        {
            op = "" + (ulong) Math.Floor(Math.Pow(2, operand));
        }
        else 
        {
            op = "(2^"+GetComboString(operand)+")";
        }

        return reg + " = A / " + op;


    }
   

    public void ExecuteBXL(ulong operand)
    {
        B ^= operand; // B = B XOR operand
    }

    private string ExplainBXL(ulong operand)
    {
        return "B = B XOR " + operand;
    }

    public void ExecuteBST(ulong operand)
    {
        B = MathUtil.Mod(GetComboOperand(operand), (ulong)8); 
    }

    private string ExplainBST(ulong operand)
    {
        return "B = " + GetComboString(operand) + " % 8 <==> Last 3 bits";
    } 

    public void ExecuteBXC(ulong operand)
    {
        B ^= C;
    }

    private string ExplainBXC(ulong operand)
    {
        return "B = B XOR C";
    }

    public void ExecuteOUT(ulong operand)
    {
        output.Add(MathUtil.Mod(GetComboOperand(operand), (ulong)8));
    }


    private string ExplainOUT(ulong operand)
    {
        return "print " + GetComboString(operand);
    }

    public void ExecuteBDV(ulong operand)
    {
        ulong denominator = GetComboOperand(operand);
        B = (ulong) Math.Floor(A / Math.Pow(2, denominator));
    }



    public void ExecuteCDV(ulong operand)
    {
        ulong denominator = GetComboOperand(operand);
        C = (ulong) Math.Floor(A / Math.Pow(2, denominator));
    }



}


class Day17
{

    public static void Part1()
    {
        var program = new ChronoSpatialProgram([
           2,4,1,5,7,5,1,6,4,2,5,5,0,3,3,0
        ]);

        program.A = 33940147;
        program.B = 0;
        program.C = 0;


        Console.WriteLine(Convert.ToString(33940147, 8));


        program.Execute();

        Console.WriteLine(program.GetOutput());

    }

    public static bool EqualLists(List<ulong> a, List<ulong> b)
    {
        if (a.Count != b.Count) return false;

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i]) return false;
        }

        return true;
    }



    public static bool TestProgramEqualsOutput(List<ulong> prog, string octalA, int depth, int maxDepth = 16)
    {

        var expected = prog.Slice(prog.Count - depth, depth);

        for (int i = 0; i < 8; i++)
        {

            var octalStr = octalA + i;
            var program = new ChronoSpatialProgram(prog);
            program.A = Convert.ToUInt64(octalStr, 8);
            program.Execute();

            var output = program.GetOutput().Split(",").Select(x => ulong.Parse(x)).ToList();
            
            if (depth > output.Count) continue;
            var subOutput = output[0..depth];

            if (EqualLists(subOutput, expected))
            {

                if (prog.Count == subOutput.Count)
                {
                    Console.WriteLine(Convert.ToUInt64(octalStr, 8));
                    return true;
                }
                else
                {
                    // Check candidate
                    var found = TestProgramEqualsOutput(prog, octalStr, depth + 1);
                    if (found) return true;
                    else       continue;

                }
            }
        }

        return false;
    }




    public static ulong Part2()
    {
        List<ulong> baseProgram = [2,4,1,5,7,5,1,6,4,2,5,5,0,3,3,0];



        Console.WriteLine("Program: " + string.Join(",", baseProgram));

        var program = new ChronoSpatialProgram(baseProgram);

        // var octalStr = "3033076032004233";
        var octalStr = "2024";
        program.A = Convert.ToUInt64(octalStr, 8);
        program.B = 0;
        program.C = 0;

        Console.WriteLine("A: " + program.A + 
            " 0o" + Convert.ToString((long)program.A, 8) + 
            " 0b" + Convert.ToString((long)program.A, 2));
        Console.WriteLine(program.Dissasemble());

        program.Execute();
        var output = program.GetOutput();
        var digitsInOutput = output.Split(",").Count();
        Console.WriteLine(output + " => " + digitsInOutput);

        TestProgramEqualsOutput(baseProgram, "", depth:1, maxDepth: baseProgram.Count);




        /* 
            35 = 3,0
            37 = 3,0
            30 = 3,0
            31 = 3,0

        */


        // for (int d = 0; d < 16; d++)
        // {


        //     // Generate all digits that generate

        //     // digit generated ==> list of digits that generates
        //     Dictionary<int, List<int>> known = [];

        //     for (int i = 0; i < 8; i++)
        //     {
        //         octalStr = "" + i;
        //         program = new ChronoSpatialProgram(baseProgram);
        //         program.A = Convert.ToUInt64(octalStr, 8);
        //         program.Execute();

        //         Console.WriteLine($"0o{octalStr} ==> {program.GetOutput()}");

        //         if (!known.ContainsKey(i))
        //         {
        //             known[i] = [];
        //         }

        //         known[i].Add(int.Parse(program.GetOutput().Split(",").Last()));
        //     }

            
        //     foreach (var i in known.Keys)
        //     {
        //         Console.WriteLine($"digit {i} generates {string.Join(",",known[i])}");
        //     }

        //     break;

        // }
        
        /*
            El programa solo reduce A en 8 cada iteración
            nºiter == nºout == log8(A) + 1 == digitos octal A

            El programa tiene 16 numeros 
                ==> A en octal debe tener 16 digitos

            ulong A = 33940147;
            int d = 0;
            while (A != 0)
            {
                A /= 8;
                d ++;
            }

            
            ------------------

            B = 000

            B = 101

            C = 63

            B = 011

            B = 60 = 111 100

            OUT = 100

        */



        /*   Bruteforce   */

        // ulong minOctal = 1000000000000000;
        // var octalStr = minOctal.ToString();
        // ulong A = Convert.ToUInt64(octalStr, 8);

        // while (true)
        // {
            

        //     var newProgram = new ChronoSpatialProgram(baseProgram, A);
        //     newProgram.ExecuteWithHeuristic();
        //     var programOut = newProgram.GetOutput().Split(",").Select(x => ulong.Parse(x)).ToList();
            
        //     if (EqualLists(programOut, baseProgram))
        //     {
        //         Console.WriteLine(A);
        //         break;
        //     }


        //     A ++;
        // }



        return 0;
    }
}