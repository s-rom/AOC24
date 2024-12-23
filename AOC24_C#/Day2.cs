
using System.Runtime.InteropServices;
using System.Xml.XPath;

class Day2  
{


    private static readonly string inputPath = @"..\..\..\input_2.txt";

    private static List<List<int>> ParseInput()
    {

        var result = new List<List<int>>();

        using StreamReader sr = File.OpenText(inputPath);
        string? line;
        while ( (line = sr.ReadLine()) != null) 
        {
            var row = line.Split(' ').Select(x => int.Parse(x)).ToList();
            result.Add(row);
        }
    
        return result;
    }

    private static bool IsDescending(List<int> list) 
    {
        for (int i = 0;  i < list.Count - 1; i ++ )
        {
            if (list[i] < list[i + 1]) return false;
        }

        return true;
    }

    private static bool IsAscending(List<int> list) 
    {
        for (int i = 0;  i < list.Count - 1; i ++ )
        {
            if (list[i] > list[i + 1]) return false;
        }

        return true;
    }

    private static bool DiffersByOneToThree(List<int> list) 
    {
        for (int i = 0;  i < list.Count - 1; i ++ )
        {
            var diff = int.Abs(list[i] - list[i + 1]);
            if (diff < 1 || diff > 3) return false;
        }

        return true;
    }

    public static int Part1()
    {
        var reports = ParseInput();
        
        var total = reports
                        .Where(l => IsReportSafe(l))
                        .Count();
        return total;
    }


    private static bool IsReportSafe(List<int> report) 
    {
        return (IsDescending(report) || IsAscending(report)) && DiffersByOneToThree(report);
    }

    private static bool TryFixReport(List<int> report) 
    {
        int size = report.Count;
        for (int i = 0; i < size; i++) 
        {  
            List<int> testReport = new List<int>(report);
            testReport.RemoveAt(i);

            if (IsReportSafe(testReport)) {
                return true;
            }  
        }

        return false;
    }

    public static int Part2() 
    {
        var reports = ParseInput();
        var safeReports = 0;

        foreach (var report in reports) 
        {
            if (IsReportSafe(report) || TryFixReport(report))
            {
                safeReports ++;
            } 
        }
        return safeReports;
    }


}