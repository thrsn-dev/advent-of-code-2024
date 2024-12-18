using System.Drawing;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public class SolutionDay18
{
    [Benchmark]
    public void SolveV1()
    {
        var lines = File.ReadAllLines("data/day18.txt").Select(l =>
        {
            var split = l.Split(",");
            var x = int.Parse(split[1]);
            var y = int.Parse(split[0]);
            return new Point(x, y);
        }).ToArray();
        var grid = new byte[71, 71];
        var start = new PositionWithDirection((0,0), (1,0));
        var end = (grid.GetLength(0) - 1, grid.GetLength(1) - 1);
        FillGrid(grid, lines[..1024]);
        
        var steps = Utils.DijkstraToEndNoContinue(start, point => GetNeighbors(point, grid), point => point.Position == end);
        Console.WriteLine($"Result 1: {steps.distance}");
    }

    [Benchmark]
    public void SolveV2()
    {
        var lines = File.ReadAllLines("data/day18.txt").Select(l =>
        {
            var split = l.Split(",");
            var x = int.Parse(split[1]);
            var y = int.Parse(split[0]);
            return new Point(x, y);
        }).ToArray();
        var grid = new byte[71, 71];
        var start = new PositionWithDirection((0,0), (1,0));
        var end = (grid.GetLength(0) - 1, grid.GetLength(1) - 1);
        FillGrid(grid, lines[..1024]);
        
        // copy grid to temp grid
        var tempGrid = CreateTempGrid(grid);
        
        // find middle between 1024 and length of lines
        var hasExit = 1024;
        var middle = hasExit + (lines.Length - hasExit) / 2;
        FillGrid(tempGrid, lines[hasExit..middle]);

        while (true)
        {
            // binary search for when there is no exit based on the lowest amount of added bytes
            var steps = Utils.DijkstraToEndNoContinue(start, point => GetNeighbors(point, tempGrid), point => point.Position == end);

            if (steps.distance > 0)
            {
                FillGrid(grid, lines[hasExit..middle]);
                hasExit = middle;
                middle = hasExit + (lines.Length - hasExit) / 2;
            }
            else
            {
                tempGrid = CreateTempGrid(grid);
                middle = hasExit + (middle - hasExit) / 2;
                
                if (middle == hasExit)
                {
                    break;
                }
            }

            FillGrid(tempGrid, lines[hasExit..middle]);
        }

        Console.WriteLine($"Result 2: {lines[middle]}");
    }

    private static void FillGrid(byte[,] grid, Point[] points)
    {
        foreach (var point in points)
        {
            grid[point.X, point.Y] = 1;
        }
    }

    private static byte[,] CreateTempGrid(byte[,] grid)
    {
        var tempGrid = new byte[71, 71];
        for (var i = 0; i < grid.GetLength(0); i++)
        {
            for (var j = 0; j < grid.GetLength(1); j++)
            {
                tempGrid[i, j] = grid[i, j];
            }
        }
        return tempGrid;
    }

    private static IEnumerable<(PositionWithDirection, int)> GetNeighbors(PositionWithDirection point, byte[,] grid)
    {
        var moves = new ((int x, int y) direction, int cost)[]
        {
            (point.Direction, 1), (GetLeftDirection(point.Direction), 1), (GetRightDirection(point.Direction), 1)
        };
        
        foreach (var move in moves)
        {
            (int x, int y) newPosition = (point.Position.x + move.direction.x, point.Position.y + move.direction.y);

            if (newPosition.x is >= 0 and < 71 && newPosition.y is >= 0 and < 71 && grid[newPosition.x, newPosition.y] == 0)
            {
                yield return (new PositionWithDirection(newPosition, move.direction), move.cost);
            }
        }
    }

    private static (int x, int y) GetLeftDirection((int x, int y) currentDirection) =>
        currentDirection switch
        {
            (0, 1) => (-1, 0),
            (1, 0) => (0, 1),
            (0, -1) => (1, 0),
            (-1, 0) => (0, -1),
            _ => throw new Exception("Invalid direction")
        };

    private static (int x, int y) GetRightDirection((int x, int y) currentDirection) =>
        currentDirection switch
        {
            (0, 1) => (1, 0),
            (1, 0) => (0, -1),
            (0, -1) => (-1, 0),
            (-1, 0) => (0, 1),
            _ => throw new Exception("Invalid direction")
        };
    
    private record struct PositionWithDirection((int x, int y) Position, (int dx, int dy) Direction);
}