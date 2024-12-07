using System.Drawing;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public class SolutionDay6
{
    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    
    [Benchmark]
    public void SolveV1()
    {
        var text = File.ReadAllText("data/day6.txt");
        var lines = text.Split('\n');
        var guardIndex = text.IndexOf('^');
        var guardLine = guardIndex / (lines[0].Length + 1);
        var maxX = lines.Length;
        var maxY = lines[0].Length;
        var guardPosition = new Point(guardLine, guardIndex - guardLine * (maxY + 1));
        var direction = Direction.Up;
        var positions = new HashSet<Point>();
        positions.Add(guardPosition);
        string? currentLine = null;

        while (CanMove(ref direction, ref guardPosition, lines, maxX, maxY, ref currentLine))
        {
            positions.Add(guardPosition);
        }

        Console.WriteLine($"Result 1: {positions.Count}");
    }

    private static bool CanMove(ref Direction direction, ref Point guardPosition, string[] lines, int max, int maxY, ref string? currentLine)
    {
        var newPosition = NewPosition(direction, guardPosition);
        
        if (newPosition.X < 0 || newPosition.X >= max || newPosition.Y < 0 || newPosition.Y >= maxY)
        {
            return false;
        }

        if (currentLine == null || guardPosition.X != newPosition.X)
        {
            currentLine = lines[newPosition.X];
        }
        
        if (currentLine[newPosition.Y] == '#')
        {
            direction = direction switch
            {
                Direction.Down => Direction.Left,
                Direction.Left => Direction.Up,
                Direction.Up => Direction.Right,
                Direction.Right => Direction.Down,
            };
            currentLine = lines[guardPosition.X];
            // currentLine = null;

            return true;
        }
        
        guardPosition = newPosition;
        return true;
    }

    [Benchmark]
    public void SolveV2()
    {
        var text = File.ReadAllText("data/day6.txt");
        var lines = text.Split('\n');
        var guardIndex = text.IndexOf('^');
        var guardLine = guardIndex / (lines[0].Length + 1);
        var maxX = lines.Length;
        var maxY = lines[0].Length;
        var guardPosition = new Point(guardLine, guardIndex - guardLine * (maxY + 1));
        int counter = 0;
        var direction = Direction.Up;
        var visitedLines = new HashSet<(Point, Point)>();
        var positions = new HashSet<Point>();
        positions.Add(guardPosition);
        
        while (CanMove(ref direction, ref guardPosition, lines, maxX, null, visitedLines, ref counter, positions))
        {
            positions.Add(guardPosition);
        }

        Console.WriteLine($"Result 1: {positions.Count}");
        Console.WriteLine($"Result 2: {counter}");
    }
    
    // start at guard position
    // get next position in direction
    // check if position is out of bounds
    // check if position is obstacle or #
    // change direction 90 degrees and return true
    // if it is possible to move, try to add an obstacle in a recursive call
    // if the new obstacle results in a infinite loop (by checking the hashset) increase the count
    private static bool CanMove(ref Direction direction, ref Point guardPosition, string[] lines, int max,
        Point? obstacle, HashSet<(Point, Point)> visitedLines, ref int counter, HashSet<Point> positions)
    {
        var newPosition = NewPosition(direction, guardPosition);
        
        if (newPosition.X < 0 || newPosition.X >= max || newPosition.Y < 0 || newPosition.Y >= max)
        {
            return false;
        }
        
        if (lines[newPosition.X][newPosition.Y] == '#' || obstacle == newPosition)
        {
            direction = direction switch
            {
                Direction.Down => Direction.Left,
                Direction.Left => Direction.Up,
                Direction.Up => Direction.Right,
                Direction.Right => Direction.Down,
            };

            return true;
        }

        if (obstacle == null && !positions.Contains(newPosition))
        {
            var copyDirection = direction;
            var copyGuardPosition = guardPosition;
            HashSet<(Point, Point)> copyVisitedLines = [];
            while (CanMove(ref copyDirection, ref copyGuardPosition, lines, max, newPosition, copyVisitedLines, ref counter, positions))
            {
                
            }
        }

        if (obstacle != null)
        {
            var add = visitedLines.Add((guardPosition, newPosition));
            if (!add)
            {
                counter++;
                return false;
            }
        }
        
        guardPosition = newPosition;
        return true;
    }

    private static Point NewPosition(Direction direction, Point guardPosition)
    {
        var newPosition = direction switch
        {
            Direction.Down => guardPosition with { X = guardPosition.X + 1 },
            Direction.Up => guardPosition with { X = guardPosition.X - 1 },
            Direction.Left => guardPosition with { Y = guardPosition.Y - 1 },
            Direction.Right => guardPosition with { Y = guardPosition.Y + 1 },
        };
        return newPosition;
    }
}