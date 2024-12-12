using System.Drawing;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public class SolutionDay12
{
    [Benchmark]
    public void SolveV1()
    {
        // start at 0,0 corner
        // find first letter and flood fill to determine area and perimeter
        // when encountering edge of another letter add position plus letter to queue

        var lines = File.ReadAllLines("data/day12.txt");
        var queue = new Queue<(char letter, Point position)>();
        var visitedNodes = new HashSet<Point>();
        var sum = 0;
        
        (int, int)[] directions = [
            (0, 1),
            (0, -1),
            (1, 0),
            (-1, 0)
        ];

        var firstLetter = lines[0][0];
        
        queue.Enqueue((firstLetter, new Point(0, 0)));

        while (queue.TryDequeue(out var edge))
        {
            var area = 0;
            var perimeter = 0;
            VisitNode(edge.letter, edge.position, ref area, ref perimeter);
            sum += area * perimeter;
        }

        Console.WriteLine($"Result 1: {sum}");

        void VisitNode(char search, Point position, ref int area, ref int perimeter)
        {
            if (!visitedNodes.Add(position))
            {
                return;
            }
            
            area++;

            foreach (var direction in directions)
            {
                var newPosition = new Point(position.X + direction.Item1, position.Y + direction.Item2);
                if (newPosition.X < 0 || newPosition.Y < 0 || newPosition.X >= lines.Length || newPosition.Y >= lines.Length)
                {
                    perimeter++;
                    continue;
                }

                if (lines[newPosition.X][newPosition.Y] != search)
                {
                    queue.Enqueue((lines[newPosition.X][newPosition.Y], newPosition));
                    
                    // this is an edge
                    perimeter++;
                    
                    continue;
                }
                
                VisitNode(search, newPosition, ref area, ref perimeter);
            }
        }
    }
    
    [Benchmark]
    public void SolveV2()
    {
        // start at 0,0 corner
        // find first letter and flood fill to determine area and perimeter
        // when encountering edge of another letter add position plus letter to queue

        var lines = File.ReadAllLines("data/day12.txt");
        var queue = new Queue<(char letter, Point position)>();
        var visitedNodes = new HashSet<Point>();
        var sum = 0;
        
        (int, int)[] directions = [
            (0, 1),
            (0, -1),
            (1, 0),
            (-1, 0)
        ];
        
        var firstLetter = lines[0][0];
        
        queue.Enqueue((firstLetter, new Point(0, 0)));

        while (queue.TryDequeue(out var edge))
        {
            var area = 0;
            var corners = 0;
            VisitNode(edge.letter, edge.position, ref area, ref corners, 0, []);
            sum += area * corners;
        }

        Console.WriteLine($"Result 2: {sum}");

        void VisitNode(char search, Point position, ref int area, ref int corners, int level, HashSet<(Point position, (int x, int y) direction)> visitedEdges)
        {
            if (!visitedNodes.Add(position))
            {
                return;
            }
            
            area++;

            foreach (var direction in directions)
            {
                var newPosition = new Point(position.X + direction.Item1, position.Y + direction.Item2);
                if (newPosition.X < 0 || newPosition.Y < 0 || newPosition.X >= lines.Length || newPosition.Y >= lines.Length)
                {
                    visitedEdges.Add((position, direction));
                    continue;
                }

                if (lines[newPosition.X][newPosition.Y] != search)
                {
                    queue.Enqueue((lines[newPosition.X][newPosition.Y], newPosition));
                    visitedEdges.Add((position, direction));
                    continue;
                }
                
                VisitNode(search, newPosition, ref area, ref corners, level + 1, visitedEdges);
            }
            
            if (level == 0)
            {
                var edges = visitedEdges.ToList();
                foreach (var edge in edges)
                {
                    visitedEdges.Remove((edge.position with { X = edge.position.X - 1 }, edge.direction));
                    visitedEdges.Remove((edge.position with { Y = edge.position.Y - 1 }, edge.direction));
                }
                
                corners = visitedEdges.Count;
            }
        }
    }
}