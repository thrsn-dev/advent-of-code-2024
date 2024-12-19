using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public class SolutionDay19
{
    [Benchmark]
    public void SolveV1()
    {
        var lines = File.ReadAllLines("data/day19.txt");
        var combinations = lines[0].Split(", ").Order().ToArray();
        var longest = combinations.MaxBy(e => e.Length)!.Length;

        var designs = lines[1..];
        combinations = combinations.Where(e => designs.Any(d => d.Contains(e))).ToArray();
        var designCombinations = new Dictionary<string, long>();
        var func = new Func<string, string[]>(chunk => combinations.Where(chunk.StartsWith).ToArray());
        var memoize = func.Memoize();
        var combineMemo = new Dictionary<(string, int, int, string), long>();

        foreach (var design in designs)
        {
            foreach (var comb in combinations.Where(e => design.StartsWith(e)))
            {
                var currentLongest = Math.Min(longest, design.Length - comb.Length);
                var possibilities = Combine(design, comb.Length, currentLongest, memoize, combineMemo);
                if (possibilities > 0)
                {
                    designCombinations[design] = possibilities;
                }
            }
        }

        Console.WriteLine($"Result 1: {designCombinations.Count}");
    }

    [Benchmark]
    public void SolveV2()
    {
        var lines = File.ReadAllLines("data/day19.txt");
        var combinations = lines[0].Split(", ").Order().ToArray();
        var longest = combinations.MaxBy(e => e.Length)!.Length;
        var designs = lines[1..];
        combinations = combinations.Where(e => designs.Any(d => d.Contains(e))).ToArray();
        var designCombinations = new Dictionary<string, long>();

        var func = new Func<string, string[]>(chunk => combinations.Where(chunk.StartsWith).ToArray());
        var memoize = func.Memoize();
        var combineMemo = new Dictionary<(string, int, int, string), long>();

        foreach (var design in designs)
        {
            var sum = 0L;
            foreach (var comb in combinations.Where(e => design.StartsWith(e)))
            {
                var currentLongest = Math.Min(longest, design.Length - comb.Length);
                sum  += Combine(design, comb.Length, currentLongest, memoize, combineMemo);
            }
            if (sum > 0)
            {
                designCombinations[design] = sum;
            }
        }

        Console.WriteLine($"Result 2: {designCombinations.Values.Sum()}");
    }

    private static long Combine(string design,
        int currentIndex, int longest, Func<string, string[]> memoize, Dictionary<(string, int, int, string), long> combineMemo)
    {
        if (currentIndex == design.Length)
        {
            return 1;
        }
        
        var chunk = design[currentIndex..(currentIndex + longest)];
        if (combineMemo.TryGetValue((design, currentIndex, longest, chunk), out var value)) return value;
        
        var possible = memoize(chunk);
        var sum = 0L;
        
        foreach (var comb in possible)
        {
            var currentLongest = Math.Min(longest, design.Length - (currentIndex + comb.Length));
            sum += combineMemo[(design, currentIndex, longest, comb)] = Combine(design, currentIndex + comb.Length, currentLongest, memoize, combineMemo);
        }
        
        combineMemo[(design, currentIndex, longest, chunk)] = sum;
        return sum;
    }
}