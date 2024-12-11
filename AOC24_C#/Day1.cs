

using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;


class Day1 {

    private static readonly String inputPath = @"..\..\..\input_1.txt";


    private static (List<int>, List<int>) parseLists() {

        var leftList = new List<int>();
        var rightList = new List<int>();


        using (StreamReader sr = File.OpenText(inputPath))
        {
            String line;
            while ((line = sr.ReadLine()) != null) 
            {
                var tokens = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                leftList.Add(int.Parse(tokens[0]));
                rightList.Add(int.Parse(tokens[1]));
            }


            Console.WriteLine(line);
        }
        return (leftList, rightList); 
    }

    public static int part1() {

        var result = parseLists();
        var left = result.Item1;
        var right = result.Item2;

        left.Sort();
        right.Sort();
        var total = 0;

        for (int i = 0; i < left.Count; i++) 
        {

            int diff = int.Abs(left[i] - right[i]);
            total += diff;

        }
        return total;
    }   

    public static int part2() {
        var result = parseLists();
        var left = result.Item1;
        var right = result.Item2;

        var rightFrequency = new Dictionary<int, int>();

        foreach (var item in right)
        {
            if (!rightFrequency.ContainsKey(item)) 
            {
                rightFrequency.Add(item, 1);
            }
            else 
            {
                rightFrequency[item] ++;
            }
        }

        var total = 0;
        foreach (var item in left) {
            if (rightFrequency.ContainsKey(item)) {
                total += item * rightFrequency[item];
            }
        }

        return total;
    }

    public static int linq_part2() {

        var result = parseLists();
        var left = result.Item1;
        var right = result.Item2;

        var groups = 
            (from item in right
            group item by item into grouped
            select new 
            {
                Item = grouped.Key,
                Count = grouped.Count()
            }).ToDictionary(i => i.Item, i => i.Count);


        var total = 0;
        left.ForEach( i => {
            if (groups.ContainsKey(i)) total += i * groups[i];
        });

        return total;
    }


}