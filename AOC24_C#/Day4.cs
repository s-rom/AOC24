using System.Collections;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.IO.Compression;
using System.Runtime.InteropServices.JavaScript;
using System.Text.RegularExpressions;
using System.Xml.Schema;

class Day4
{

    private static String inputPath = @"..\..\..\input_4.txt";
    private static Regex xmasPattern = new Regex("XMAS");
    private static Regex samxPattern = new Regex("SAMX");

    private static Regex masPattern = new Regex("MAS");

    private static Regex samPattern = new Regex("SAM");
    

    private static char[][] parseInput() 
    {
        char [][] ?grid = null;
        int? dimension = null;

        using (StreamReader sr = File.OpenText(inputPath)) 
        {
            String line;
            int lineIdx = 0;
            while ((line  = sr.ReadLine()) != null)
            {
                dimension ??= line.Length;
                grid ??= new char[(int)dimension][];

                grid[lineIdx] = line.ToCharArray();
                lineIdx ++;
            }
        }

        return grid;
    }


    private static void printGrid(char[][] grid) 
    {
        for (int row = 0; row < grid.Length; row++) 
        {
            Console.WriteLine(new String(grid[row]));
        }
    }


    private static IEnumerable<String> iterateDiagonal(char [][] grid, bool alternative = false) {

        var result = "";

        for (int row = 0; row < grid.Length; row++)
        {

            var r = row;
            var c = 0;
            while (r >= 0) 
            {
                result += grid[r][alternative ? grid.Length - c - 1: c];
                r --;
                c ++;
            }

            yield return result;
            result = "";
        }

        result = "";
        var row2 = grid.Length - 1;
        var col = 1;

        while (col < grid[0].Length) 
        {
            var r = row2;
            var c = col;

            while (c < grid[0].Length) 
            {
                result += grid[r][alternative ? grid.Length - c - 1: c];
                r --;
                c ++;
            }

            col++;
            yield return result;
            result = "";
        }

    }


    private static IEnumerable<String> iterateHorizontal(char [][] grid) 
    {
        for (int row = 0; row < grid.Length; row ++) 
        {
            yield return new string(grid[row]);
        }
    }

    private static IEnumerable<String> iterateVertical(char [][] grid) 
    {
        var columnStr = "";

        for (int col = 0; col < grid[0].Length; col ++) 
        {
            for (int row = 0; row < grid.Length; row ++) 
            {
                columnStr += grid[row][col];
            }    

            yield return columnStr;
            columnStr = "";
        }
    }


    private static char[][] getWindow(char [][] grid, int rowOffset, int colOffset, int size) 
    {
        char [][] window = new char[size][];

        for (int i = 0; i < size; i++)
        {
            int row = rowOffset + i;            
            window[i] ??= new char[3];
            
            for (int j = 0; j < size; j++) 
            {
                int col = colOffset + j;
                window[i][j] = grid[row][col];
            }

        }

        return window; 
    }

    private static bool checkXMaxWindow(char[][] window)
    {
        int result = 0;

        foreach (var line in iterateDiagonal(window))
        {

            result += 
                masPattern.Matches(line).Count +
                samPattern.Matches(line).Count;
        }


        foreach (var line in iterateDiagonal(window, alternative: true))
        {
            
            result += 
                masPattern.Matches(line).Count +
                samPattern.Matches(line).Count;
        }


        return result == 2;
    }

    private static int countXMASOcurrences(String input) 
    {
        return 
            xmasPattern.Matches(input).Count + 
            samxPattern.Matches(input).Count;
    
    }



    public static int part1() 
    {
        
        var result = 0;
        var grid = parseInput();
        
        foreach(var line in iterateVertical(grid))
            result += countXMASOcurrences(line);

        foreach(var line in iterateHorizontal(grid))
            result += countXMASOcurrences(line);
        

        foreach(var line in iterateDiagonal(grid))
            result += countXMASOcurrences(line);
    
        foreach(var line in iterateDiagonal(grid, true))
            result += countXMASOcurrences(line);

        return result;
    }


    public static int part2() 
    {
        
        var result = 0;
        var grid = parseInput();
        
        int windowSize = 3;

        for (int row = 0; row < grid.Length - windowSize + 1; row ++) 
        {
            for (int col = 0; col < grid[0].Length - windowSize + 1; col ++) 
            {
                var window = getWindow(grid, row, col, windowSize);
                result += checkXMaxWindow(window)? 1: 0;
            }
        }

        return result;
    }



}