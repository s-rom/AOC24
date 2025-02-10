
namespace Day23;

using Node = string;   


public class Day23
{

    public static readonly Dictionary<Node, HashSet<Node>> connections = [];

    public static readonly HashSet<string> triangles = [];

    private static void AddConnection(Node comp1, Node comp2)
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


    public static void FindNCycles(Node computer, int n, List<Node> cycle)
    {

        if (cycle.Count == n)
        {
            
            if (connections[cycle.First()].Contains(cycle.Last()))
            {
                var newCycle = new List<Node>(cycle);
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



    public static readonly List<HashSet<Node>> maximalCliques = [];

    public static void BronKerboschNaive(HashSet<Node> candidates, HashSet<Node> visited, HashSet<Node> currentClique)
    {
        if (candidates.Count == 0 & visited.Count == 0)
        {
            maximalCliques.Add(currentClique);
        }

        foreach (var vertex in candidates)
        {

            HashSet<Node> nextClique = [.. currentClique, vertex];

            BronKerboschNaive(
                [.. candidates.Intersect(connections[vertex])],
                [.. visited.Intersect(connections[vertex])],
                nextClique                
            );

            candidates.Remove(vertex);
            visited.Add(vertex);

        }
    }


    public static string Part2()
    {

        /*

        R = set of nodes (current clique) = []
        P = candidates to be added = [.. nodes]
        X = visited = []

        algorithm BronKerbosch1(currentClique, candidates, visited) is
            if candidates and visited are both empty then
                report currentClique as a maximal clique
            for each vertex v in candidates do
                BronKerbosch1(currentClique ⋃ {v}, candidates ⋂ N(v), visited ⋂ N(v))
                candidates := candidates \ {v}
                visited := visited ⋃ {v}
        */

        ParseGraph(@"..\..\..\input_23.txt");
        BronKerboschNaive([.. connections.Keys], [], []);

        var maxClique = maximalCliques.MaxBy(x => x.Count);

        var cliqueNodes = maxClique!.ToList();
        cliqueNodes.Sort();

        return string.Join(',', cliqueNodes);
    }
}