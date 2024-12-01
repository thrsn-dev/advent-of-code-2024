using System.Diagnostics;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public class SolutionDay1
{
    [Benchmark]
    public void SolveV1()
    {
        var lines = File.ReadAllLines("data/day1.txt");
        var sorted1 = new List<double>(lines.Length);
        var sorted2 = new List<double>(lines.Length);
        var duplicates2 = new Dictionary<double, int>();
        
        foreach (var (n1, n2) in lines.Select(l =>
                 {
                     var n = l.Split("   ");
                     return (double.Parse(n[0]), double.Parse(n[1]));
                 }))
        {
            sorted1.Add(n1);
            sorted2.Add(n2);
            _ = duplicates2.ContainsKey(n2) ? duplicates2[n2] += 1 : duplicates2[n2] = 1;
        }

        sorted1 = sorted1.Order().ToList();
        sorted2 = sorted2.Order().ToList();
        
        var result = sorted1.Select((n, i) => Math.Abs(n - sorted2[i])).Sum();
        var result2 = sorted1.Select(v1 => duplicates2.TryGetValue(v1, out var v2) ? v2 * v1 : 0).Sum();
        
        Console.WriteLine($"Result 1: {result}");
        Console.WriteLine($"Result 2: {result2}");
    }
    
    [Benchmark]
    public void SolveV1_1()
    {
        var lines = File.ReadAllLines("data/day1.txt");
        var sorted1 = new List<double>(lines.Length);
        var sorted2 = new List<double>(lines.Length);
        var duplicates2 = new Dictionary<double, int>();
        
        foreach (var (n1, n2) in lines.Select(l =>
                 {
                     var n = l.Split("   ");
                     return (double.Parse(n[0]), double.Parse(n[1]));
                 }))
        {
            sorted1.Add(n1);
            sorted2.Add(n2);
            _ = duplicates2.ContainsKey(n2) ? duplicates2[n2] += 1 : duplicates2[n2] = 1;
        }

        sorted2 = sorted2.Order().ToList();

        var idx = 0;
        var duplicateSum = 0D;
        var valuesSum = 0D;
        foreach (var v1 in sorted1.Order())
        {
            duplicateSum += duplicates2.TryGetValue(v1, out var v2) ? v2 * v1 : 0;
            valuesSum += Math.Abs(v1 - sorted2[idx]);
            idx++;
        }

        Console.WriteLine($"Result 1: {valuesSum}");
        Console.WriteLine($"Result 2: {duplicateSum}");
        
    }

    [Benchmark]
    public void SolveV2()
    {
        var duplicates2 = new Dictionary<double, int>();
        var values1 = new List<double>();
        var values2 = new List<double>();
        using var fileStream = File.OpenRead("data/day1.txt");
        using var stream = new StreamReader(fileStream);
        var idx = 0;
        while (stream.ReadLine() is { } line)
        {
            var split = line.Split("   ");
            var n1 = double.Parse(split[0]);
            var n2 = double.Parse(split[1]);

            if (idx == 0)
            {
                values1.Add(n1);
                values2.Add(n2);
            }
            else if (idx == 1)
            {
                if (values1[0] > n1)
                {
                    values1.Insert(0, n1);
                }
                else
                {
                    values1.Add(n1);
                }
                
                if (values2[0] > n2)
                {
                    values2.Insert(0, n2);
                }
                else
                {
                    values2.Add(n2);
                }
            }
            else
            {
                var index = values1.BinarySearch(n1);
                if (index < 0)
                {
                    values1.Insert(~index, n1);
                }
                else
                {
                    values1.Insert(index, n1);
                }
                
                index = values2.BinarySearch(n2);
                if (index < 0)
                {
                    values2.Insert(~index, n2);
                }
                else
                {
                    values2.Insert(index, n2);
                }
            }

            _ = duplicates2.ContainsKey(n2) ? duplicates2[n2] += 1 : duplicates2[n2] = 1;
            idx++;
        }
        
        var result = values1.Select((n, i) => Math.Abs(n - values2[i])).Sum();
        var result2 = values1.Select(v1 => duplicates2.TryGetValue(v1, out var v2) ? v2 * v1 : 0).Sum();
        
        Console.WriteLine($"Result 1: {result}");
        Console.WriteLine($"Result 2: {result2}");
    }
}