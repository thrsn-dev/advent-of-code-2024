using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public class SolutionDay5
{
    [Benchmark]
    public void SolveV1()
    {
        var text = File.ReadAllText("data/day5.txt");
        var values = text.Split("\n\n");
        var lookup = new HashSet<string>(values[0].Split('\n'));
        var comparer = new RuleComparer(lookup);
        
        var pages = values[1].Split('\n');
        var sum = 0;

        foreach (var pageNumbers in pages)
        {
            var numbers = pageNumbers.Split(',');
            if (numbers.OrderDescending(comparer).SequenceEqual(numbers))
            {
                sum += int.Parse(numbers[numbers.Length/2]);
            }
        }

        Console.WriteLine($"Result 1: {sum}");
    }
    
    [Benchmark]
    public void SolveV2()
    {
        var text = File.ReadAllText("data/day5.txt");
        var values = text.Split("\n\n");
        var lookup = new HashSet<string>(values[0].Split('\n'));
        var comparer = new RuleComparer(lookup);
        
        var pages = values[1].Split('\n');
        var sum = (from pageNumbers in pages
            select pageNumbers.Split(',')
            into numbers
            let result = numbers.OrderDescending(comparer).ToArray()
            where !result.SequenceEqual(numbers)
            select int.Parse(result.ElementAt(numbers.Length / 2))).Sum();

        Console.WriteLine($"Result 2: {sum}");
    }
    
    private class RuleComparer(HashSet<string> lookup) : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            return lookup.Contains($"{x}|{y}") ? 1 : -1;
        }
    }
}