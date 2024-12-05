using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public class SolutionDay5
{
    [Benchmark]
    public void SolveV1()
    {
        var text = File.ReadAllText("data/day5.txt");
        var values = text.Split("\n\n");
        var lookup = values[0].Split('\n').ToLookup(e => e.Split('|')[0], e => e.Split('|')[1]);
        
        var pages = values[1].Split('\n');
        var sum = 0;

        foreach (var pageNumbers in pages)
        {
            var numbers = pageNumbers.Split(',');
            for (var i = 0; i < numbers.Length - 1; i++)
            {
                var number = numbers[i];
                if (lookup[number].Intersect(numbers[(i+1)..]).Count() == numbers.Length - (i+1))
                {
                    if (i == numbers.Length / 2)
                    {
                        sum += int.Parse(number);
                    }
                    continue;
                }
                break;
            }
        }

        Console.WriteLine($"Result 1: {sum}");
    }
    
    [Benchmark]
    public void SolveV2()
    {
        var text = File.ReadAllText("data/day5.txt");
        var values = text.Split("\n\n");
        var lookup = values[0].Split('\n').ToLookup(e => e.Split('|')[0], e => e.Split('|')[1]);
        
        var pages = values[1].Split('\n');
        var sum = (from pageNumbers in pages
            select pageNumbers.Split(',')
            into numbers
            let result = numbers.OrderByDescending(e => lookup[e].Intersect(numbers).Count()).ToArray()
            where !result.SequenceEqual(numbers)
            select int.Parse(result.ElementAt(numbers.Length / 2))).Sum();

        Console.WriteLine($"Result 2: {sum}");
    }
}