namespace AdventOfCode2024;

public static class SolutionDay1
{
    public static void Solve()
    {
        var lines = File.ReadAllLines("data/day1.txt");
        var sorted1 = new double[lines.Length];
        var sorted2 = new double[lines.Length];
        var duplicates2 = new Dictionary<double, int>();
        
        var idx = 0;
        foreach (var (n1, n2) in lines.Select(l =>
                 {
                     var n = l.Split("   ");
                     return (double.Parse(n[0]), double.Parse(n[1]));
                 }))
        {
            sorted1[idx] = n1;
            sorted2[idx] = n2;
            _ = duplicates2.ContainsKey(n2) ? duplicates2[n2] += 1 : duplicates2[n2] = 1;
            idx++;
        }

        sorted1 = sorted1.Order().ToArray();
        sorted2 = sorted2.Order().ToArray();

        var result = sorted1.Select((n, i) => Math.Abs(n - sorted2[i])).Sum();
        Console.WriteLine($"Result 1: {result}");

        var result2 = sorted1.Select(v1 => duplicates2.TryGetValue(v1, out var v2) ? v2 * v1 : 0).Sum();
        Console.WriteLine($"Result 2: {result2}");
    }
}