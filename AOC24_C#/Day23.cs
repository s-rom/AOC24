
namespace Day23;

using ComputerNode = string;   


public class Day23
{

    public static readonly Dictionary<ComputerNode, List<ComputerNode>> connections = [];

    public static readonly HashSet<ComputerNode> visited = [];

    public static readonly HashSet<string> triangles = [];

    private static void AddConnection(ComputerNode comp1, ComputerNode comp2)
    {
        if (!connections.ContainsKey(comp1))
            connections.Add(comp1, []);
       
        if (!connections.ContainsKey(comp2))
            connections.Add(comp2, []);
     

        connections[comp1].Add(comp2);
        connections[comp2].Add(comp1);
    }

    private static void ParseGraph(string inputFile)
    {
        var lines  = File.ReadAllLines(inputFile);
        foreach (var line in lines)
        {
            var computers =  line.Split('-');
            var computer1 = computers[0];
            var computer2 = computers[1];
            AddConnection(computer1, computer2);
            
        }
    }


    public static void FindNCycles(ComputerNode computer, int n, List<ComputerNode> cycle)
    {

        if (cycle.Count == n)
        {
            
            if (connections[cycle.First()].Contains(cycle.Last()))
            {
                var newCycle = new List<ComputerNode>(cycle);
                string triangleStr = "";
                newCycle.Sort();
                foreach (var node in newCycle) triangleStr += node;
                

                triangles.Add(triangleStr);
            }

            return;
        }

        var previousComputer = cycle.Last();

        foreach (var neighbour in connections[computer])
        {
            // Avoid going back
            if (neighbour == previousComputer) continue;

            cycle.Add(neighbour);
            FindNCycles(neighbour, n, cycle);
            cycle.RemoveAt(cycle.Count - 1);

        }
    }

    public static long Part1()
    {
        ParseGraph(@"..\..\..\input_23.txt");
        

        foreach (var computer in connections.Keys)
        {
            FindNCycles(computer, 3, [computer]);
        }


    
        // return 0L;
        return triangles.Count(x => x[0] == 't' || x[2] == 't' || x[4] == 't');
    }
}