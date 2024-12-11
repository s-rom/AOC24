
using System.Runtime.InteropServices;
using System.Xml.XPath;

class Day2  
{


    private static readonly String inputPath = @"..\..\..\input_2.txt";

    private static List<List<int>> parseInput()
    {

        var result = new List<List<int>>();

        using (StreamReader sr = File.OpenText(inputPath))
        {
            String line;
            while ( (line = sr.ReadLine()) != null) 
            {
                var row = line.Split(' ').Select(x => int.Parse(x)).ToList();
                result.Add(row);
            }
        }

        return result;
    }

    private static bool isDescending(List<int> list) 
    {
        for (int i = 0;  i < list.Count - 1; i ++ )
        {
            if (list[i] < list[i + 1]) return false;
        }

        return true;
    }

    private static bool isAscending(List<int> list) 
    {
        for (int i = 0;  i < list.Count - 1; i ++ )
        {
            if (list[i] > list[i + 1]) return false;
        }

        return true;
    }

    private static bool differsByOneToThree(List<int> list) 
    {
        for (int i = 0;  i < list.Count - 1; i ++ )
        {
            var diff = int.Abs(list[i] - list[i + 1]);
            if (diff < 1 || diff > 3) return false;
        }

        return true;
    }

    public static int part1()
    {
        var reports = parseInput();
        
        var total = reports
                        .Where(l => isReportSafe(l))
                        .Count();
        return total;
    }


    private static bool isReportSafe(List<int> report) 
    {
        return (isDescending(report) || isAscending(report)) && differsByOneToThree(report);
    }

    private static bool tryFixReport(List<int> report) 
    {
        int size = report.Count;
        for (int i = 0; i < size; i++) 
        {  
            List<int> testReport = new List<int>(report);
            testReport.RemoveAt(i);

            if (isReportSafe(testReport)) {
                return true;
            }  
        }

        return false;
    }

    public static int part2() 
    {
        var reports = parseInput();
        var safeReports = 0;

        foreach (var report in reports) 
        {
            if (isReportSafe(report) || tryFixReport(report))
            {
                safeReports ++;
            } 
        }
        return safeReports;
    }






}