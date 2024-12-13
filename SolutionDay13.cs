using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public partial class SolutionDay13
{
    [Benchmark]
    public void SolveV1()
    {
        const int MAX_MOVES_PER_BUTTON = 100;
        const int A_PRICE = 3;
        const int B_PRICE = 1;

        var sum = 0;
        var buttonRegex = ButtonRegex();
        var prizeRegex = PrizeRegex();

        var lines = File.ReadAllLines("data/day13.txt").Where(e => e != "").ToArray();

        for (int i = 0; i < lines.Length/3; i++)
        {
            var inputs = lines.Skip(i*3).Take(3).ToArray();
            var buttonAMatch = buttonRegex.Match(inputs[0]);
            var buttonBMatch = buttonRegex.Match(inputs[1]);
            var prizeMatch = prizeRegex.Match(inputs[2]);
            
            if (buttonAMatch.Success && buttonBMatch.Success && prizeMatch.Success)
            {
                (int x, int y) buttonA = (int.Parse(buttonAMatch.Groups[2].Value), int.Parse(buttonAMatch.Groups[4].Value));
                (int x, int y) buttonB = (int.Parse(buttonBMatch.Groups[2].Value), int.Parse(buttonBMatch.Groups[4].Value));
                (int x, int y) prize = (int.Parse(prizeMatch.Groups[2].Value), int.Parse(prizeMatch.Groups[4].Value));
                var cheapest = int.MaxValue;
                for (var a = 1; a <= MAX_MOVES_PER_BUTTON; a++)
                for (var b = 1; b <= MAX_MOVES_PER_BUTTON; b++)
                {
                    var tmpCheapest = a * A_PRICE + b * B_PRICE;
                    if (tmpCheapest >= cheapest) continue;
                    (int x, int y) currentPosition = (buttonA.x * a + buttonB.x * b, buttonA.y * a + buttonB.y * b);
                    if (currentPosition.x > prize.x) continue;
                    if (currentPosition.y > prize.y) continue;
                    
                    if (currentPosition == prize)
                    {
                        cheapest = Math.Min(cheapest, a * A_PRICE + b * B_PRICE);
                    }
                }
                
                // var cheapest = CalculateTokens(buttonA, buttonB, prize, (0,0));
                if (cheapest == int.MaxValue) continue;
                sum += cheapest;
            }
        }

        Console.WriteLine($"Result 1: {sum}");
    }
    
    [Benchmark]
    public void SolveV2()
    {
        var sum = 0L;
        var buttonRegex = ButtonRegex();
        var prizeRegex = PrizeRegex();

        var lines = File.ReadAllLines("data/day13.txt").Where(e => e != "").ToArray();

        for (int i = 0; i < lines.Length/3; i++)
        {
            var inputs = lines.Skip(i*3).Take(3).ToArray();
            var buttonAMatch = buttonRegex.Match(inputs[0]);
            var buttonBMatch = buttonRegex.Match(inputs[1]);
            var prizeMatch = prizeRegex.Match(inputs[2]);
            
            if (buttonAMatch.Success && buttonBMatch.Success && prizeMatch.Success)
            {
                (int x, int y) buttonA = (int.Parse(buttonAMatch.Groups[2].Value), int.Parse(buttonAMatch.Groups[4].Value));
                (int x, int y) buttonB = (int.Parse(buttonBMatch.Groups[2].Value), int.Parse(buttonBMatch.Groups[4].Value));
                (int x, int y) prize = (int.Parse(prizeMatch.Groups[2].Value), int.Parse(prizeMatch.Groups[4].Value));
                var target = 10_000_000_000_000;
                sum += GetTokens(buttonA, buttonB, prize, target);
            }
        }

        Console.WriteLine($"Result 2: {sum}");
    }

    private static long GetTokens((int x, int y) buttonA, (int x, int y) buttonB, (long x, long y) prize, long offset)
    {
        (long x, long y) newPrize = (prize.x + offset, prize.y + offset);
        var deterministic = buttonA.x * buttonB.y - buttonA.y * buttonB.x;
        var a = (newPrize.x * buttonB.y - newPrize.y * buttonB.x) / deterministic;
        var b = (buttonA.x * newPrize.y - buttonA.y * newPrize.x) / deterministic;
        if ((a * buttonA.x + b * buttonB.x, a * buttonA.y + b * buttonB.y) == newPrize)
        {
            return a * 3 + b;
        }

        return 0;
    }

    [GeneratedRegex(@"(X\+(\d+)).*(Y\+(\d+))")]
    private static partial Regex ButtonRegex();
    [GeneratedRegex(@"(X=(\d+)),.*(Y=(\d+))")]
    private static partial Regex PrizeRegex();
}