using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public class SolutionDay9
{
    [Benchmark]
    public void SolveV1()
    {
        var text = File.ReadAllText("data/day9.txt").AsSpan();
        var backwardsIdx = text.Length - 2;
        var stepsLeft = 0;
        var currentIdx = 0;
        BigInteger sum = 0;
        for (int i = 0; i < backwardsIdx; i++)
        {
            // when reading backwards skip free spaces
            var value = ToInt(text[i]);
            sum = Enumerable.Range(currentIdx, value).Select(e => e * i/2).Aggregate(sum, (i1, i2) => i1 + i2);
            currentIdx += value;
            var freeSpace = ToInt(text[i + 1]);

            while (freeSpace > 0)
            {
                var last = ToInt(text[backwardsIdx]);
                if (stepsLeft == 0)
                {
                    stepsLeft = last;
                }
            
                var stepsToSum = Math.Min(freeSpace, stepsLeft);
                if (stepsToSum > 0)
                {
                    sum = Enumerable.Range(currentIdx, stepsToSum).Select(e => e * backwardsIdx/2).Aggregate(sum, (i1, i2) => i1 + i2);
                    currentIdx += stepsToSum;
                }

                stepsLeft -= stepsToSum;
                if (stepsLeft == 0)
                {
                    backwardsIdx-=2;
                }
                
                freeSpace -= stepsToSum;

                if (freeSpace == 0 && stepsLeft > 0 && i + 2 == backwardsIdx)
                {
                    sum = Enumerable.Range(currentIdx, stepsLeft).Select(e => e * backwardsIdx/2).Aggregate(sum, (i1, i2) => i1 + i2);
                }
            }

            i++;
        }
        
        Console.WriteLine($"Result 1: {sum}");
    }

    [Benchmark]
    public void SolveV2()
    {
        var usedFragments = new Dictionary<int, int>();
        var movedIndices = new Dictionary<int, HashSet<int>>();
        
        var text = File.ReadAllText("data/day9.txt").AsSpan();
        BigInteger sum = 0;

        for (var backwardsIdx = text.Length - 2; backwardsIdx > 0; backwardsIdx-=2)
        {
            var last = ToInt(text[backwardsIdx]);
            for (int i = 1; i < backwardsIdx; i+=2)
            {
                if (usedFragments.TryGetValue(i, out var freeSpaceLeft) && freeSpaceLeft >= last)
                {
                    usedFragments[i] = freeSpaceLeft - last;
                    if (movedIndices.TryGetValue(i, out var set))
                    {
                        set.Add(backwardsIdx);
                    }
                    else
                    {
                        movedIndices.Add(i, [backwardsIdx]);
                    }
                    break;
                }
                
                if (usedFragments.ContainsKey(i)) continue;
                
                var freeSpace = ToInt(text[i]);
                if (freeSpace >= last)
                {
                    usedFragments.Add(i, freeSpace - last);
                    if (movedIndices.TryGetValue(i, out var set))
                    {
                        set.Add(backwardsIdx);
                    }
                    else
                    {
                        movedIndices.Add(i, [backwardsIdx]);
                    }
                    break;
                }
            }
        }

        var moved = new HashSet<int>(movedIndices.Values.SelectMany(e => e));
        var currentIdx = 0;
        for (var i = 0; i < text.Length - 1; i+=2)
        {
            var value = ToInt(text[i]);
            var freeSpace = ToInt(text[i + 1]);
            if (!moved.Contains(i))
            {
                sum = Enumerable.Range(currentIdx, value).Select(e => e * i / 2).Aggregate(sum, (i1, i2) => i1 + i2);
            }

            currentIdx += value;
            
            if (movedIndices.TryGetValue(i + 1, out var indices))
            {
                foreach (var index in indices)
                {
                    var size = ToInt(text[index]);
                    sum = Enumerable.Range(currentIdx, size).Select(e => e * index/2).Aggregate(sum, (i1, i2) => i1 + i2);
                    currentIdx += size;
                    freeSpace -= size;
                }
            }
            
            currentIdx += freeSpace;
        }

        Console.WriteLine($"Result 2: {sum}");
    }
    
    static int ToInt(char c)
    {
        return c - '0';
    }
}