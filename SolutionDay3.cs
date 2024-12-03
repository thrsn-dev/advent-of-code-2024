using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public partial class SolutionDay3
{
    [Benchmark]
    public void SolveV1()
    {
        var regex = MyRegex();
        
        var text = File.ReadAllText("data/day3.txt");
        var matches = regex.Matches(text);
        var result = matches.Sum(m => long.Parse(m.Groups[1].Value) * int.Parse(m.Groups[2].Value));
        Console.WriteLine($"Result 1: {result}");
    }

    [Benchmark]
    public void SolveV2()
    {
        var regex = MyRegex();
        var sum = 0L;
        var text = File.ReadAllText("data/day3.txt");
        int index;
        while ((index = text.IndexOf("don't()", StringComparison.Ordinal)) != -1)
        {
            var tmpText = text[..index];
            var matches = regex.Matches(tmpText);
            sum += matches.Sum(m => long.Parse(m.Groups[1].Value) * int.Parse(m.Groups[2].Value));
            text = text[(index + 6)..];
            index = text.IndexOf("do()", StringComparison.Ordinal);
            if (index == -1) break;
            text = text[(index + 4)..];
        }
        
        var lastMatches = regex.Matches(text);
        sum += lastMatches.Sum(m => long.Parse(m.Groups[1].Value) * int.Parse(m.Groups[2].Value));
        
        Console.WriteLine($"Result 2: {sum}");
    }

    [GeneratedRegex(@"mul\(([0-9]{1,3}),([0-9]{1,3})\)")]
    private static partial Regex MyRegex();
}