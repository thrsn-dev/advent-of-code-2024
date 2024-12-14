using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public class SolutionDay14
{
    [Benchmark]
    public void SolveV1()
    {
        const int Width = 101;
        const int Height = 103;
        const int Number = 100;
        var q1Sum = 0;
        var q2Sum = 0;
        var q3Sum = 0;
        var q4Sum = 0;
        
        var lines = File.ReadAllLines("data/day14.txt");

        foreach (var line in lines)
        {
            // format of line is p=d,d v=d,d where d can be negative
            var parts = line.Split(" ");
            var p = parts[0].Split(",");
            var v = parts[1].Split(",");
            (int x, int y) pCoord = (int.Parse(p[0][2..]), int.Parse(p[1]));
            (int x, int y) vCoord = (int.Parse(v[0][2..]), int.Parse(v[1]));
            (int x, int y) end = (pCoord.x + Number * vCoord.x, pCoord.y + Number * vCoord.y);
            (int x, int y) wrapped = (end.x % Width, end.y % Height);
            if (wrapped.x < 0)
            {
                wrapped.x += Width;
            }
            if (wrapped.y < 0)
            {
                wrapped.y += Height;
            }

            switch (wrapped)
            {
                //check quadrants, ignore middle
                case { x: < Width / 2, y: < Height / 2 }:
                    q1Sum++;
                    break;
                case { x: > Width / 2, y: < Height / 2 }:
                    q2Sum++;
                    break;
                case { x: < Width / 2, y: > Height / 2 }:
                    q3Sum++;
                    break;
                case { x: > Width / 2, y: > Height / 2 }:
                    q4Sum++;
                    break;
            }
        }

        Console.WriteLine($"Result 1: {q1Sum * q2Sum * q3Sum * q4Sum}");
    }

    [Benchmark]
    public void SolveV2()
    {
        var lines = File.ReadAllLines("data/day14.txt");
        var origins = new List<(int x, int y)>();
        var vectors = new List<(int x, int y)>();
        const int Width = 101;
        const int Height = 103;
        (int x, int y)[] directions = [
            (0, 1),
            (0, -1),
            (1, 0),
            (-1, 0)
        ];

        foreach (var line in lines)
        {
            var parts = line.Split(" ");
            var p = parts[0].Split(",");
            var v = parts[1].Split(",");
            (int x, int y) pCoord = (int.Parse(p[0][2..]), int.Parse(p[1]));
            (int x, int y) vCoord = (int.Parse(v[0][2..]), int.Parse(v[1]));
            origins.Add(pCoord);
            vectors.Add(vCoord);
        }

        var number = 1;
        
        var tmpSet = new HashSet<(int x, int y)>();
        for (int i = 1; i < Width * Height; i++)
        {
            tmpSet.Clear();
            for (int j = 0; j < origins.Count; j++)
            {
                (int x, int y) pCoord = origins[j];
                (int x, int y) vCoord = vectors[j];
                (int x, int y) end = (pCoord.x + i * vCoord.x, pCoord.y + i * vCoord.y);
                (int x, int y) wrapped = (end.x % Width, end.y % Height);
                if (wrapped.x < 0)
                {
                    wrapped.x += Width;
                }
                if (wrapped.y < 0)
                {
                    wrapped.y += Height;
                }
                
                tmpSet.Add(wrapped);
            }

            var count = tmpSet.Where(e => e.y + 1 < Height && e.x + 1 < Width && e is { y: > 0, x: > 0 })
                .Count(s => directions.Count(d =>
            {
                var x = s.x + d.x;
                var y = s.y + d.y;
                return tmpSet.Contains((x,y));
            }) == 4);
            
            if (count > 6)
            {
                number = i;
                break;
            }
            
        }
        Console.WriteLine($"Result 2: {number}");
        // // print a grid of the values in set
        // var grid = new char[Height, Width];
        // for (int i = 0; i < Height; i++)
        // {
        //     for (int j = 0; j < Width; j++)
        //     {
        //         grid[i, j] = '.';
        //     }
        // }
        //
        // foreach (var coord in set)
        // {
        //     grid[coord.y, coord.x] = '#';
        // }
        //
        // Console.WriteLine($"Result 2: {number}");
        // for (int i = 0; i < Height; i++)
        // {
        //     for (int j = 0; j < Width; j++)
        //     {
        //         Console.Write(grid[i, j]);
        //     }
        //     Console.WriteLine();
        // }
    }
}