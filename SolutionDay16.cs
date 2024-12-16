using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public class SolutionDay16
{
    [Benchmark]
    public void SolveV1()
    {
        var lines = File.ReadAllLines("data/day16.txt");
        (int x, int y) end = (1, lines[0].Length - 2);
        (int x, int y) start = (lines.Length - 2, 1);
        var startNode = new PositionWithDirection
        {
            Position = start,
            Direction = (0, 1)
        };

        var result = Utils.DijkstraToEnd(startNode, position => GetNeighbors(position, lines), node => node.Position == end);

        Console.WriteLine($"Result 1: {result.distance}");
    }

    [Benchmark]
    public void SolveV2()
    {
        var lines = File.ReadAllLines("data/day16.txt");
        (int x, int y) end = (1, lines[0].Length - 2);
        (int x, int y) start = (lines.Length - 2, 1);
        var startNode = new PositionWithDirection
        {
            Position = start,
            Direction = (0, 1)
        };

        var result = Utils.DijkstraToEnd(startNode, position => GetNeighbors(position, lines), node => node.Position == end);
        
        var endNode = result.path.First(e => e.Key.Position == end && e.Value.distance == result.distance);
        var shortestPathWithDistance = new Dictionary<(int, int), int>();
        
        while (endNode.Key.Position != start)
        {
            shortestPathWithDistance[endNode.Key.Position] = endNode.Value.distance;
            endNode = result.path.First(e => e.Key == endNode.Value.parent);
        }

        var lookup = result.path.ToLookup(e => e.Value.parent);

        var uniquePositions = TraverseParentNodes((startNode, 0));

        HashSet<(int, int)> TraverseParentNodes((PositionWithDirection nwd, int distance) node, bool isOnShortestPath = true)
        {
            var positions = new HashSet<(int, int)> { node.nwd.Position };
            if (node.nwd.Position == end)
            {
                return positions;
            }

            var parents = lookup[node.nwd];
            
            foreach (var parent in parents.Where(e => e.Value.distance > 0))
            {
                var isFollowingShortestPath = shortestPathWithDistance.TryGetValue(parent.Key.Position, out var value) &&
                                              (value == parent.Value.distance || value - 1000 == parent.Value.distance);
                if (isOnShortestPath == false && isFollowingShortestPath)
                {
                    positions.Add(parent.Key.Position);
                    continue;
                }

                if (value != 0 && !isOnShortestPath && !isFollowingShortestPath)
                {
                    return [];
                }

                positions.UnionWith(TraverseParentNodes((parent.Key, parent.Value.distance), isFollowingShortestPath));
            }

            return !positions.Intersect(shortestPathWithDistance.Keys).Any() ? [] : positions;
        }
        
        Console.WriteLine($"Result 2: {uniquePositions.Count}");
    }

    private static IEnumerable<(PositionWithDirection, int)> GetNeighbors(PositionWithDirection node, string[] lines)
    {
        var moves = new ((int x, int y) direction, int cost)[]
        {
            (node.Direction, 1), (GetLeftDirection(node.Direction), 1001), (GetRightDirection(node.Direction), 1001)
        };

        foreach (var move in moves)
        {
            (int x, int y) newPosition = (node.Position.x + move.direction.x, node.Position.y + move.direction.y);

            if (lines[newPosition.x][newPosition.y] != '#')
            {
                yield return (new PositionWithDirection
                {
                    Position = newPosition,
                    Direction = move.direction
                }, move.cost);
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
    
    private record struct PositionWithDirection
    {
        public (int x, int y) Position;
        public (int dx, int dy) Direction;
    }
}