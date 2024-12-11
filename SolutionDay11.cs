using System.Collections.Concurrent;
using System.Globalization;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public class SolutionDay11
{
    private const int LEVEL = 75;
    
    [Benchmark]
    public void SolveV1()
    {
        var text = File.ReadAllText("data/day11.txt");
        var chunks = text.Split(' ');
        for (var i = 0; i < Math.Min(LEVEL, 25); i++)
        {
            chunks = chunks.SelectMany<string, string>(e =>
            {
                var stones = AdjustStone(e);
                return stones.v2 is null ? [stones.v1] : [stones.v1, stones.v2];
            }).ToArray();
        }

        Console.WriteLine($"Result 1: {chunks.Length}");

        (string v1, string? v2) AdjustStone(string input)
        {
            return input switch
            {
                not null when input.Length % 2 == 0 => (input[..(input.Length / 2)], $"{long.Parse(input[(input.Length / 2)..])}"),
                "0" => ("1", null),
                _ => ($"{long.Parse(input!) * 2024}", null)
            };
        }
    }
    
    [Benchmark]
    public void SolveV2()
    {
        // store visited nodes and the level as key for sum
        var visitedNodes = new ConcurrentDictionary<(string, int), long>();
        var text = File.ReadAllText("data/day11.txt");
        var chunks = text.Split(' ');
        var sums = new long[chunks.Length];
        
        for (var i = 0; i < chunks.Length; i++)
        {
            var chunk = chunks[i];
            sums[i] = AdjustStone(chunk);
        }

        Console.WriteLine($"Result 2: {sums.Sum()}");

        long AdjustStone(ReadOnlySpan<char> input, int currentLevel = 0)
        {
            var tmpSum = 1L;
            while (true)
            {
                if (currentLevel == LEVEL) return tmpSum;

                if (currentLevel > 0 && visitedNodes.TryGetValue((new string(input), currentLevel), out var value))
                {
                    return tmpSum + value - 1;
                }

                if (input[0] == '0' && input.Length == 1)
                {
                    input = ['1'];
                }
                else if (input.Length % 2 == 0)
                {
                    var adjustStone = AdjustStone(input[..(input.Length / 2)], currentLevel + 1);
                    tmpSum += adjustStone;
                    
                    visitedNodes.TryAdd((new string(input[..(input.Length / 2)]), currentLevel + 1), adjustStone);
                    
                    // remove leading zeros
                    var half = input[(input.Length / 2)..];
                    while (half[0] == '0' && half.Length > 1)
                    {
                        half = half[1..];
                    }

                    input = half;
                }
                else
                {
                    input = (long.Parse(input) * 2024).ToString();
                }

                currentLevel += 1;
            }
        }
    }
}