using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public class SolutionDay15
{
    [Benchmark]
    public void SolveV1()
    {
        const int MAP_HEIGHT = 50;
        
        var lines = File.ReadAllLines("data/day15.txt");

        var map = lines[..MAP_HEIGHT].Select(e => e.ToArray()).ToArray();
        var MAP_WIDTH = map[0].Length - 1;
        
        var moves = lines[(MAP_HEIGHT + 1)..];
        var getDirection = GetDirection();

        (int x, int y) robotPosition = (0, 0);
        for (var i = 0; i < map.Length; i++)
        {
            var index = Array.IndexOf(map[i], '@');
            if (index <= 0) continue;
            robotPosition = (i, index);
            break;
        }

        foreach (var moveCommands in moves)
        foreach (var moveCommand in moveCommands)
        {
            var direction = getDirection(moveCommand);
            var indicesToSwitch = new List<(int x, int y)>();
            var tmpPosition = robotPosition;
            while (true)
            {
                var newPosition = NewPosition(direction, tmpPosition);
                
                if (newPosition.x < 1 || newPosition.x >= MAP_HEIGHT || newPosition.y < 1 || newPosition.y >= MAP_WIDTH)
                {
                    break;
                }

                var mapPosition = map[newPosition.x][newPosition.y];
                if (mapPosition == '#')
                {
                    break;
                }

                if (indicesToSwitch.Count == 0 && mapPosition == '.')
                {
                    // swap
                    map[robotPosition.x][robotPosition.y] = '.';
                    map[newPosition.x][newPosition.y] = '@';
                    
                    robotPosition = newPosition;
                    break;
                }

                if (mapPosition is 'O' && indicesToSwitch.Count == 1)
                {
                    tmpPosition = newPosition;
                    continue;
                }
                
                if (mapPosition is '.')
                {
                    indicesToSwitch.Add(newPosition);
                    break;
                }
                
                if (mapPosition is 'O')
                {
                    indicesToSwitch.Add(newPosition);
                }
                
                tmpPosition = newPosition;
            }
            
            if (indicesToSwitch.Count == 2)
            {
                (map[indicesToSwitch[0].x][indicesToSwitch[0].y], map[indicesToSwitch[1].x][indicesToSwitch[1].y]) = 
                    (map[indicesToSwitch[1].x][indicesToSwitch[1].y], map[indicesToSwitch[0].x][indicesToSwitch[0].y]);
                map[robotPosition.x][robotPosition.y] = '.';
                map[indicesToSwitch[0].x][indicesToSwitch[0].y] = '@';
                
                robotPosition = indicesToSwitch[0];
            }
        }

        var sum = 0;
        
        //print map
        for (var i = 0; i < map.Length; i++)
        {
            for (var j = 0; j < map[i].Length; j++)
            {
                if (map[i][j] == 'O')
                {
                    sum += 100 * i + j;
                }
                
                // Console.Write(map[i][j]);
            }
            // Console.WriteLine();
        }
        // Console.WriteLine();

        Console.WriteLine($"Result 1: {sum}");
    }

    [Benchmark]
    public void SolveV2()
    {
        const int MAP_HEIGHT = 50;
        var getDirection = GetDirection();
        
        var lines = File.ReadAllLines("data/day15.txt");

        var map = lines[..MAP_HEIGHT].Select(e => e.ToArray()).ToArray();
        var moves = lines[(MAP_HEIGHT + 1)..];
        var MAP_WIDTH = map[0].Length;

        var newMap = new char[MAP_HEIGHT][];
        var replacementMap = new Dictionary<char, (char c1, char c2)>
        {
            ['@'] = ('@', '.'),
            ['O'] = ('[', ']'),
            ['.'] = ('.', '.'),
            ['#'] = ('#', '#')
        };

        (int x, int y) robotPosition = (0, 0);
        for (var i = 0; i < MAP_HEIGHT; i++)
        {
            newMap[i] = new char[MAP_WIDTH*2];
            for (var j = 0; j < MAP_WIDTH*2; j+=2)
            {
                var replacement = replacementMap[map[i][j/2]];
                newMap[i][j] = replacement.c1;
                newMap[i][j + 1] = replacement.c2;

                if (replacement.c1 == '@')
                {
                    robotPosition = (i, j);
                }
            }
        }
        
        foreach (var moveCommands in moves)
        foreach (var moveCommand in moveCommands)
        {
            var direction = getDirection(moveCommand);
            var indicesToSwitch = new HashSet<((int x, int y), (int x, int y))>();
            var tmpPosition = robotPosition;
            
            while (true)
            {
                var newPosition = NewPosition(direction, tmpPosition);
                
                if (newPosition.x < 1 || newPosition.x >= MAP_HEIGHT || newPosition.y < 1 || newPosition.y >= MAP_WIDTH*2)
                {
                    break;
                }

                var mapSymbol = newMap[newPosition.x][newPosition.y];
                if (mapSymbol == '#')
                {
                    indicesToSwitch.Clear();
                    break;
                }

                if (indicesToSwitch.Count == 0 && mapSymbol == '.')
                {
                    // swap
                    newMap[robotPosition.x][robotPosition.y] = '.';
                    newMap[newPosition.x][newPosition.y] = '@';
                    
                    robotPosition = newPosition;
                    break;
                }
                
                // logic based on direction here
                if (moveCommand is '^' or 'v')
                {
                    // we need to check recursively here to determine if the path is clear for each box we are potentially moving here.
                    var tmpIndicesToSwap = new HashSet<((int x, int y), (int x, int y))>();
                    if (HandleUpOrDown(newPosition, tmpIndicesToSwap))
                    {
                        indicesToSwitch.UnionWith(tmpIndicesToSwap);
                        indicesToSwitch.Add((newPosition, tmpPosition));
                    }
                    break;
                }

                if (HandleLeftOrRight(mapSymbol, newPosition, tmpPosition) == false)
                {
                    break;
                }

                tmpPosition = newPosition;
            }

            if (indicesToSwitch.Count > 0)
            {
                var newPosition = NewPosition(direction, robotPosition);
                robotPosition = newPosition;
            }
            
            var ordered = moveCommand is '>' ? indicesToSwitch.OrderByDescending(e => e.Item1.y) : 
                moveCommand is '<' ? indicesToSwitch.OrderBy(e => e.Item1.y) : 
                moveCommand is '^' ? indicesToSwitch.OrderBy(e => e.Item1.x) : 
                indicesToSwitch.OrderByDescending(e => e.Item1.x);

            foreach (var indices in ordered)
            {
                (newMap[indices.Item1.x][indices.Item1.y], newMap[indices.Item2.x][indices.Item2.y]) = 
                    (newMap[indices.Item2.x][indices.Item2.y], newMap[indices.Item1.x][indices.Item1.y]);
            }

            bool HandleLeftOrRight(char mapSymbol, (int x, int y) newPosition, (int x, int y) prevPosition)
            {
                indicesToSwitch.Add((newPosition, prevPosition));
                return mapSymbol is '[' or ']';
            }

            bool HandleUpOrDown((int x, int y) newPosition, HashSet<((int x, int y), (int x, int y))> indicesToSwap)
            {
                if (newPosition.x < 1 || newPosition.x >= MAP_HEIGHT || newPosition.y < 1 || newPosition.y >= MAP_WIDTH*2)
                {
                    return false;
                }
                var mapSymbol = newMap[newPosition.x][newPosition.y];
                switch (mapSymbol)
                {
                    case '.':
                        return true;
                    case '#':
                        return false;
                }

                (int x, int y) neighbor = mapSymbol switch
                {
                    '[' => (newPosition.x, newPosition.y + 1),
                    ']' => (newPosition.x, newPosition.y - 1),
                    _ => (0, 0)
                };

                var nextPosition = NewPosition(direction, newPosition);
                var nextNeighborPosition = NewPosition(direction, neighbor);
                
                if (HandleUpOrDown(nextPosition, indicesToSwap) == false)
                {
                    return false;
                }
                
                indicesToSwap.Add((newPosition, nextPosition));
                
                if (HandleUpOrDown(nextNeighborPosition, indicesToSwap) == false)
                {
                    return false;
                }
                
                indicesToSwap.Add((neighbor, nextNeighborPosition));
                return true;
            }
        }

        var sum = 0;
        
        //print map
        for (var i = 0; i < newMap.Length; i++)
        {
            for (var j = 0; j < newMap[i].Length; j++)
            {
                if (newMap[i][j] == '[')
                {
                    sum += 100 * i + j;
                }
                // Console.Write(newMap[i][j]);
            }
            // Console.WriteLine();
        }
        // Console.WriteLine();
        
        Console.WriteLine($"Result 2: {sum}");
    }

    private static Func<char, (int x, int y)> GetDirection()
    {
        return Direction;

        (int x, int y) Direction(char m) =>
            m switch
            {
                '^' => (-1, 0),
                'v' => (1, 0),
                '<' => (0, -1),
                '>' => (0, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(m))
            };
    }

    private static (int x, int y) NewPosition((int x, int y) direction, (int x, int y) tmpPosition)
    {
        (int x, int y) newPosition;
        if (direction.x is 1 or -1)
        {
            newPosition = (tmpPosition.x + direction.x, tmpPosition.y);
        }
        else
        {
            newPosition = (tmpPosition.x, tmpPosition.y + direction.y);
        }

        return newPosition;
    }
}