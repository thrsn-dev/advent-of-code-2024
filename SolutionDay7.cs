using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public partial class SolutionDay7
{
    [Benchmark]
    public void SolveV1()
    {
        var regex = MyRegex();
        var combinedSum = 0L;
        
        var lines = File.ReadAllLines("data/day7.txt").Select(e => regex.Matches(e).Select(m => long.Parse(m.Value)).ToArray());
        foreach (var line in lines)
        {
            var sum = line[0];
            var values = line[1..];
            
            var tmpSum = CalculateNext(values[0], 0);
            if (tmpSum == sum)
            {
                combinedSum += sum;
            }
            
            long CalculateNext(long currentSum, int index)
            {
                if (index == values.Length - 1)
                    return currentSum;
                
                var addSum = CalculateNextByOperator(currentSum, index + 1, Operator.Add, values);
                var mulSum = CalculateNextByOperator(currentSum, index + 1, Operator.Multiply, values);
                if (addSum < sum)
                {
                    addSum = CalculateNext(addSum, index + 1);
                }
                if (mulSum < sum)
                {
                    mulSum = CalculateNext(mulSum, index + 1);
                }

                return addSum == sum ? addSum : mulSum;
            }
        }
        
        Console.WriteLine($"Result 1: {combinedSum}");
    }

    [Benchmark]
    public void SolveV2()
    {
        var regex = MyRegex();
        var combinedSum = 0L;
        
        var lines = File.ReadAllLines("data/day7.txt").Select(e => regex.Matches(e).Select(m => long.Parse(m.Value)).ToArray());
        
        foreach (var line in lines)
        {
            var sum = line[0];
            var values = line[1..];
            
            var tmpSum = CalculateNext(values[0], 0);
            if (tmpSum == sum)
            {
                combinedSum += sum;
            }
            
            long CalculateNext(long currentSum, int index)
            {
                if (index == values.Length - 1)
                    return currentSum;
                
                var addSum = CalculateNextByOperator(currentSum, index + 1, Operator.Add, values);
                var mulSum = CalculateNextByOperator(currentSum, index + 1, Operator.Multiply, values);
                var concatSum = CalculateNextByOperator(currentSum, index + 1, Operator.Concatenate, values);
                if (addSum < sum)
                {
                    addSum = CalculateNext(addSum, index + 1);
                }
                if (mulSum < sum)
                {
                    mulSum = CalculateNext(mulSum, index + 1);
                }
                if (concatSum < sum)
                {
                    concatSum = CalculateNext(concatSum, index + 1);
                }
                
                return addSum == sum ? addSum : mulSum == sum ? mulSum : concatSum;
            }
        }
        
        Console.WriteLine($"Result 2: {combinedSum}");
    }

    private static long CalculateNextByOperator(long currentSum, int index, Operator op, long[] values)
    {
        return op switch
        {
            Operator.Add => currentSum + values[index],
            Operator.Multiply => currentSum * values[index],
            Operator.Concatenate => long.Parse($"{currentSum}{values[index]}"),
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }

    private enum Operator
    {
        Add,
        Multiply,
        Concatenate
    }

    [GeneratedRegex("(\\d+)")]
    private static partial Regex MyRegex();
}