using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public class SolutionDay2
{
    [Benchmark]
    public void SolveV1()
    {
        var counter = 0;
        var lines = File.ReadAllLines("data/day2.txt").Select(l => l.Split(' ').Select(int.Parse).ToArray()).ToArray();
        foreach (var reports in lines)
        {
            var previous = reports[0];
            var isIncreasing = true;
            var isDecreasing = true;
            for (var i = 1; i < reports.Length; i++)
            {
                isIncreasing = isIncreasing && previous - reports[i] is -1 or -2 or -3;
                isDecreasing = isDecreasing && previous - reports[i] is 1 or 2 or 3;
                if ((!isIncreasing && !isDecreasing) || (isIncreasing && isDecreasing))
                {
                    break;
                }
                previous = reports[i];
            }
            
            if (isIncreasing || isDecreasing)
                counter++;
        }

        Console.WriteLine($"Result 1: {counter}");
    }
    
    [Benchmark]
    public void SolveV2()
    {
        var idx = 0;
        var counter = 0;
        var errorIndices = new List<int>();
        var lines = File.ReadAllLines("data/day2.txt").Select(l => l.Split(' ').Select(int.Parse).ToArray()).ToArray();
        foreach (var reports in lines)
        {
            var previous = reports[0];
            var isIncreasing = true;
            var isDecreasing = true;
            for (var i = 1; i < reports.Length; i++)
            {
                isIncreasing = isIncreasing && previous - reports[i] is -1 or -2 or -3;
                isDecreasing = isDecreasing && previous - reports[i] is 1 or 2 or 3;
                if ((!isIncreasing && !isDecreasing) || (isIncreasing && isDecreasing))
                {
                    break;
                }
                previous = reports[i];
            }
            
            if (isIncreasing || isDecreasing)
            {
                counter++;
            }
            else
            {
                errorIndices.Add(idx);
            }

            idx++;
        }

        foreach (var errorIndex in errorIndices)
        {
            var reports = lines[errorIndex];
            for (int j = 0; j < reports.Length; j++)
            {
                var report = reports.Take(j).Concat(reports.Skip(j + 1)).ToArray();
                var previous = report[0];
                var isIncreasing = true;
                var isDecreasing = true;
                for (var i = 1; i < report.Length; i++)
                {
                    isIncreasing = isIncreasing && previous - report[i] is -1 or -2 or -3;
                    isDecreasing = isDecreasing && previous - report[i] is 1 or 2 or 3;
                    if ((!isIncreasing && !isDecreasing) || (isIncreasing && isDecreasing))
                    {
                        break;
                    }
                    previous = report[i];
                }
            
                if (isIncreasing || isDecreasing)
                {
                    counter++;
                    break;
                }
            }
        }

        Console.WriteLine($"Result 2: {counter}");
    }
}