
class Equation 
{
    
    public long ExpectedValue {get; set;}
    public List<long> Numbers {get; private set;}

    public Equation(String rawInput)
    {
        parseInput(rawInput);
    }

    public Equation(Equation eq) 
    {
        this.ExpectedValue = eq.ExpectedValue;
        this.Numbers = new List<long>(eq.Numbers);
    }

    public override string ToString()
    {
        return $"{ExpectedValue}: " + String.Join(' ', Numbers.ToArray());
    }

    private void parseInput(String rawInput)
    {
        var pieces = rawInput.Split(':');
        ExpectedValue = long.Parse(pieces[0]);
        Numbers = pieces[1].Trim().Split(' ').Select(x => long.Parse(x.Trim())).ToList();

    }

}


enum Operation 
{
    SUM,
    MULT,
    CONCAT
}



class Day7
{

    private static readonly string inputFile = @"..\..\..\input_7.txt";

    private static List<Equation> ParseInput(String file)
    {
        List<Equation> equations = [];

        using (StreamReader sr = File.OpenText(file))
        {
            String? line = null;
            while ((line = sr.ReadLine()) != null)
            {
                equations.Add(new Equation(line));
            }
        }
        return equations;
    }



    private static bool TestEquation(Equation eq)
    {
        return TestEquationRecursive(new Queue<long>(eq.Numbers), 0, eq.ExpectedValue, Operation.SUM);
    }

    private static bool TestEquationRecursive(Queue<long> numbers, long acc, long expected, Operation op)
    {
     
        if (numbers.Count == 0)
        {
            return acc == expected;
        }

        var queue = new Queue<long>(numbers);
        var front = queue.Dequeue();

        switch (op)
        {
            case Operation.SUM:
                acc += front;
                break;

            case Operation.MULT:
                acc *= front;
                break;

            case Operation.CONCAT:
                acc = long.Parse(acc.ToString() + front.ToString());
                break;
        }

        if (acc > expected)
        {
            return false;
        }

        var sumSuccess = TestEquationRecursive(queue, acc, expected, Operation.SUM);
        var multSuccess = TestEquationRecursive(queue, acc, expected, Operation.MULT);
        var concatSuccess =TestEquationRecursive(queue, acc, expected, Operation.CONCAT);

        return sumSuccess || multSuccess || concatSuccess;
    }




    public static long singlePart() 
    {   
        var equations = ParseInput(inputFile);

        long result = 0;
        foreach (var eq in equations)
        {
            if (TestEquation(eq))
                result += eq.ExpectedValue;
        }
    
        return result;

    }


}