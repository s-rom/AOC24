
class ManualData
{
    private Dictionary<int, HashSet<int>> reverseRules;
    private List<List<int>> updates;

    public ManualData()
    {
        this.reverseRules = new Dictionary<int, HashSet<int>>();
        this.updates = new List<List<int>>();
    }


    public ManualData(string rawInput) : this()
    {
        int IDX_NOT_FOUND = -1;
        var lines = rawInput.Split(['\n', '\r']);
         
        foreach (var line in lines) 
        {
            if (String.IsNullOrEmpty(line)) continue;

            if (line.IndexOf('|') == IDX_NOT_FOUND)
            {
                var update = line.Trim().Split(",").Select(x => int.Parse(x)).ToList();
                this.updates.Add(update);
            }
            else 
            {
                var rules = line.Trim().Split("|").Select(x => int.Parse(x)).ToList();
                int left = rules[0];
                int right = rules[1];


                if (!this.reverseRules.ContainsKey(right))
                {
                    this.reverseRules.Add(right, []);
                }

                this.reverseRules.TryGetValue(right, out HashSet<int>? preList);
                preList?.Add(left);

            }
        }
    }



    public bool IsValidUpdate(List<int> update) {

        for (int i = 0; i < update.Count; i++) 
        {
            var num = update[i];
            if (!this.reverseRules.ContainsKey(num)) continue;

            HashSet<int> leftSet = this.reverseRules[num];
            
            for (int j = i + 1; j < update.Count; j++) 
            {
                var rightNum = update[j];
                if (leftSet.Contains(rightNum)) return false;
            }

            // Si el numero no esta en las reglas, fuera
            // Si no
                // Regla es invalida si algun numero de left está 
                // a la derecha de este número (entre i + 1 y Count)
        }

        return true;
    }

    private static T GetMiddle<T>(List<T> list) 
    {
        return list[list.Count/2];
    }


    public int GetPart1Result() 
    {
        var total = 0;
        foreach( var update in updates) 
        {
            
            if (this.IsValidUpdate(update))
            {
                total += GetMiddle(update);
            }
        }
        return total;
    }

    private void FixUpdate(List<int> update) 
    {
         for (int i = 0; i < update.Count; i++) 
        {
            var num = update[i];
            if (!this.reverseRules.ContainsKey(num)) continue;

            HashSet<int> leftSet = this.reverseRules[num];
            
            for (int j = i + 1; j < update.Count; j++) 
            {
                var rightNum = update[j];
                if (leftSet.Contains(rightNum))
                {
                    update.RemoveAt(i);
                    update.Add(num);
                    break;
                }
            }
        }
    }

    public int GetPart2Result() 
    {
        var total = 0;
        foreach (var update in updates) 
        {
            if (!this.IsValidUpdate(update))
            {
                while (!this.IsValidUpdate(update))
                {
                    this.FixUpdate(update);
                }

                total += GetMiddle(update);
            }
        }

        return total;
    }
}


class Day5
{
    private static readonly String inputPath = @"..\..\..\input_5.txt";


    private static ManualData ParseInput() 
    {
        String rawInput;
        using (StreamReader sr = File.OpenText(inputPath))
        {
            rawInput = sr.ReadToEnd();
        }

        return new ManualData(rawInput);
    }

    public static int part1()
    {
        var manualData = ParseInput();
        return manualData.GetPart1Result();
    }


    public static int part2()
    {
        var manualData = ParseInput();
        return manualData.GetPart2Result();
    }


}