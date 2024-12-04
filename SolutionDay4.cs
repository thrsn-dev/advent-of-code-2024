using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public class SolutionDay4
{
    [Benchmark]
    public void SolveV1()
    {
        var count = 0;
        var lines = File.ReadAllLines("data/day4.txt");
        for (int i = 0; i < lines[0].Length - 3; i++)
        for (int j = 0; j < lines.Length - 3; j++)
        {
            var line1 = lines[j].AsSpan(i, 4);
            var line2 = lines[j + 1].AsSpan(i, 4);
            var line3 = lines[j + 2].AsSpan(i, 4);
            var line4 = lines[j + 3].AsSpan(i, 4);
            if (j == 0 || j % 4 == 0)
            {
                CheckXmas(line1, ref count);
                CheckXmas(line2, ref count);
                CheckXmas(line3, ref count);
                CheckXmas(line4, ref count);
            }

            if (i == 0 || i % 4 == 0)
            {
                CheckXmas(line1[0], line2[0], line3[0], line4[0], ref count);
                CheckXmas(line1[1], line2[1], line3[1], line4[1], ref count);
                CheckXmas(line1[2], line2[2], line3[2], line4[2], ref count);
                CheckXmas(line1[3], line2[3], line3[3], line4[3], ref count);
            }

            CheckXmas(line1[0], line2[1], line3[2], line4[3], ref count);
            CheckXmas(line1[3], line2[2], line3[1], line4[0], ref count);
        }

        Console.WriteLine($"Result 1: {count}");
    }

    [Benchmark]
    public void SolveV2()
    {
        var count = 0;
        var lines = File.ReadAllLines("data/day4.txt");
        for (int i = 0; i < lines[0].Length - 2; i++)
        for (int j = 0; j < lines.Length - 2; j++)
        {
            var line1 = lines[j].AsSpan(i, 3);
            var line2 = lines[j + 1].AsSpan(i, 3);
            var line3 = lines[j + 2].AsSpan(i, 3);

            if (CheckMas(line1[0], line2[1], line3[2]) && CheckMas(line1[2], line2[1], line3[0]))
            {
                count++;
            }
        }

        Console.WriteLine($"Result 2: {count}");
    }

    private static void CheckXmas(ReadOnlySpan<char> letters, ref int count)
    {
        if (letters is "XMAS" || letters is "SAMX")
        {
            count++;
        }
    }

    private static void CheckXmas(char l1, char l2, char l3, char l4, ref int count)
    {
        switch (l1)
        {
            case 'X' when l2 == 'M' && l3 == 'A' && l4 == 'S':
            case 'S' when l2 == 'A' && l3 == 'M' && l4 == 'X':
                count++;
                break;
        }
    }

    private static bool CheckMas(char l1, char l2, char l3)
    {
        switch (l1)
        {
            case 'M' when l2 == 'A' && l3 == 'S':
            case 'S' when l2 == 'A' && l3 == 'M':
                return true;
            default:
                return false;
        }
    }
}