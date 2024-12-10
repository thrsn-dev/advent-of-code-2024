using System.Collections.Concurrent;
using System.Drawing;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public class SolutionDay10
{
    [Benchmark]
    public void SolveV1()
    {
        var lines = File.ReadAllLines("data/day10.txt");
        // 1 - find all 0 positions
        // 2 - spawn threads from each position
        // 3 - Recursively travel from position until finding a 9, but next position has to be previous value + 1 and only direction of up,down,left,right. No need to check previous position
        // 4 - Create a map of 0 position and possible 9 positions
        // 5 - Sum all 9 positions in the map

        var map = new ConcurrentDictionary<Point, HashSet<Point>>();
        var tasks = new List<Task>();
        
        (int, int)[] directions = [
            (0, 1),
            (0, -1),
            (1, 0),
            (-1, 0)
        ];

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].AsSpan();
            var idx = line.IndexOf('0');

            while (idx != -1)
            {
                var position = new Point(idx, i);
                map[position] = [];
            
                tasks.Add(Task.Run(() => Search(position, position, '0')));

                var tmpIdx = line[(idx + 1)..].IndexOf('0');
                if (tmpIdx >= 0)
                    idx = tmpIdx + idx + 1;
                else
                    idx = tmpIdx;
            }
        }
        
        Task.WaitAll(tasks);
        var sum = map.Values.Select(e => e.Count).Sum();

        Console.WriteLine($"Result 1: {sum}");

        void Search(Point startPosition, Point position, char currentValue)
        {
            if (currentValue == '9')
            {
                if (map.TryGetValue(startPosition, out var positions))
                    positions.Add(position);
                
                return;
            }

            foreach (var (x, y) in directions)
            {
                var newPosition = new Point(position.X + x, position.Y + y);
                if (newPosition.X < 0 || newPosition.X >= lines[0].Length || newPosition.Y < 0 || newPosition.Y >= lines.Length)
                    continue;
                
                var newValue = lines[newPosition.Y][newPosition.X];
                
                if (currentValue + 1 == newValue)
                    Search(startPosition, newPosition, newValue);
            }
        }
    }
    
    [Benchmark]
    public void SolveV2()
    {
        var lines = File.ReadAllLines("data/day10.txt");
        // 1 - find all 0 positions
        // 2 - spawn threads from each position
        // 3 - Recursively travel from position until finding a 9, but next position has to be previous value + 1 and only direction of up,down,left,right. No need to check previous position
        // 4 - Create a map of 0 position and possible 9 positions
        // 5 - Sum all unique routes from 0 position to 9 positions

        var routes = new ConcurrentDictionary<string, byte>();
        
        var tasks = new List<Task>();
        
        (int, int)[] directions = [
            (0, 1),
            (0, -1),
            (1, 0),
            (-1, 0)
        ];

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].AsSpan();
            var idx = line.IndexOf('0');

            while (idx != -1)
            {
                var position = new Point(idx, i);
                List<Point> route = [position];
            
                tasks.Add(Task.Run(() => Search(position, '0', route)));

                var tmpIdx = line[(idx + 1)..].IndexOf('0');
                if (tmpIdx >= 0)
                    idx = tmpIdx + idx + 1;
                else
                    idx = tmpIdx;
            }
        }
        
        Task.WaitAll(tasks);
        var sum = routes.Count;

        Console.WriteLine($"Result 2: {sum}");

        void Search(Point position, char currentValue, List<Point> route)
        {
            if (currentValue == '9')
            {
                routes.TryAdd(string.Join(',', route.Select(e => $"{e.X}|{e.Y}")), 1);
                return;
            }

            foreach (var (x, y) in directions)
            {
                var newPosition = new Point(position.X + x, position.Y + y);
                if (newPosition.X < 0 || newPosition.X >= lines[0].Length || newPosition.Y < 0 || newPosition.Y >= lines.Length)
                    continue;
                
                var newValue = lines[newPosition.Y][newPosition.X];
                
                if (currentValue + 1 == newValue)
                {
                    route.Add(newPosition);
                    Search(newPosition, newValue, route);
                }
            }
        }
    }
}