using System.Drawing;
using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public class SolutionDay8
{
    [Benchmark]
    public void SolveV1()
    {
        var lines = File.ReadAllLines("data/day8.txt");
        var coordinates = new Dictionary<char, List<Point>>();
        var uniqueAntiNodes = new HashSet<Point>();
        for (var i = lines.Length - 1; i >= 0; i--)
        {
            var line = lines[i].AsSpan();
            for (var j = 0; j < line.Length; j++)
            {
                var chr = line[j];
                if (!char.IsLetterOrDigit(chr)) continue;
                var point = new Point(j, lines.Length - i - 1);
                if (coordinates.TryGetValue(chr, out var values))
                {
                    foreach (var coordinate in values)
                    {
                        var distance = new Vector2(coordinate.X - point.X, coordinate.Y - point.Y);
                        
                        var c1 = new Point((int)(coordinate.X + distance.X), (int)(coordinate.Y + distance.Y));
                        var c2 = new Point((int)(point.X + -distance.X), (int)(point.Y + -distance.Y));
                        
                        if (c1.X < line.Length && c1.X >= 0 && c1.Y < lines.Length && c1.Y >= 0)
                        {
                            uniqueAntiNodes.Add(c1);
                        }
                        if (c2.X < line.Length && c2.X >= 0 && c2.Y < lines.Length && c2.Y >= 0)
                        {
                            uniqueAntiNodes.Add(c2);
                        }
                    }

                    values.Add(point);
                }
                else
                {
                    coordinates[chr] = [point];
                }
            }
        }

        Console.WriteLine($"Result 1: {uniqueAntiNodes.Count}");

    }
    
    [Benchmark]
    public void SolveV2()
    {
        var lines = File.ReadAllLines("data/day8.txt");
        var coordinates = new Dictionary<char, List<Point>>();
        var uniqueAntiNodes = new HashSet<Point>();
        for (var i = lines.Length - 1; i >= 0; i--)
        {
            var line = lines[i].AsSpan();
            for (var j = 0; j < line.Length; j++)
            {
                var chr = line[j];
                if (!char.IsLetterOrDigit(chr)) continue;
                var point = new Point(j, lines.Length - i - 1);
                if (coordinates.TryGetValue(chr, out var values))
                {
                    foreach (var coordinate in values)
                    {
                        var distance = new Point(coordinate.X - point.X, coordinate.Y - point.Y);
                        var c1 = new Point(coordinate.X + distance.X, coordinate.Y + distance.Y);
                        var c2 = new Point(point.X + -distance.X, point.Y + -distance.Y);
                        while (true)
                        {
                            var c1InBounds = c1.X < line.Length && c1.X >= 0 && c1.Y < lines.Length && c1.Y >= 0;
                            if (c1InBounds)
                            {
                                uniqueAntiNodes.Add(c1);
                            }

                            var c2InBounds = c2.X < line.Length && c2.X >= 0 && c2.Y < lines.Length && c2.Y >= 0;
                            if (c2InBounds)
                            {
                                uniqueAntiNodes.Add(c2);
                            }
                            
                            if (!c1InBounds && !c2InBounds)
                            {
                                break;
                            }

                            c1 = new Point(c1.X + distance.X, c1.Y + distance.Y);
                            c2 = new Point(c2.X + -distance.X, c2.Y + -distance.Y);
                        }
                    }

                    values.Add(point);
                    uniqueAntiNodes.Add(point);
                    uniqueAntiNodes.Add(values[0]);
                }
                else
                {
                    coordinates[chr] = [point];
                }
            }
        }

        Console.WriteLine($"Result 2: {uniqueAntiNodes.Count}");
    }
}