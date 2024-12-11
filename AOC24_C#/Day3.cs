using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;
using Microsoft.VisualBasic;

class Day3 
{
    private static readonly String inputPath = @"..\..\..\input_3.txt";
    private static Regex pattern = new Regex(@"mul\((\d+),(\d+)\)");

    private static String parseInput() 
    {
        using (StreamReader sr = File.OpenText(inputPath)) 
        {
            String s = sr.ReadToEnd();
            return s;
        }
    }


    private static int processProgram(String program) 
    {
        var mulInstructions = pattern.Matches(program);

        var result = 0;
        foreach (Match mul in mulInstructions) 
        {
            result += int.Parse(mul.Groups[1].Value) * int.Parse(mul.Groups[2].Value);
        }

        return result;
    }

    public static int part1()
    {
        String inputProgram = parseInput();
        return processProgram(inputProgram);
    }

    public static int part2() 
    {
        String inputProgram = parseInput();
        String DONT = "don't()";
        String DO = "do()";
        
        int IDX_NOT_FOUND = -1;

        var result = 0;


        while (!String.IsNullOrEmpty(inputProgram))
        {

            Console.WriteLine("--------- ITER ---------");
            // Previously we DID found a do(), so...
            // Find next don't()
            var dontIdx = inputProgram.IndexOf(DONT);

            if (dontIdx == IDX_NOT_FOUND) 
            {
                Console.WriteLine("Don't() not found, processing last part");
                // PROCESS EVERYTHING ELSE
                result += processProgram(inputProgram);
                inputProgram = "";

                // We don't care about next do()
            }
            else 
            {
                // PROCESS UNTIL NEXT DON'T()
                var programSlice = inputProgram.Substring(0, dontIdx);
                result += processProgram(programSlice);

                Console.WriteLine($"Finding next don't... {dontIdx}");
               
                Console.WriteLine("Processing up to don't()...");
                // Remove the processed slice
                // (remove up to next don't())
                inputProgram = inputProgram.Substring(dontIdx + DONT.Length);        


                // Find next do()
                var doIdx = inputProgram.IndexOf(DO);
                Console.WriteLine($"Finding next do...{doIdx}");
                
                if (doIdx == IDX_NOT_FOUND) 
                {
                    Console.WriteLine("No more do(), end");
                    break;
                }

                // Remove up to first 
                inputProgram = inputProgram.Substring(doIdx + DO.Length);
                
            }   
        }

        return result;
    }

}