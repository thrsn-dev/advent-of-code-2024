using BenchmarkDotNet.Attributes;

namespace AdventOfCode2024;

public class SolutionDay17
{
    [Benchmark]
    public void SolveV1()
    {
        var lines = File.ReadAllLines("data/day17.txt");
        var registerA = long.Parse(lines[0].Split(": ")[1]);
        var programs = lines[4].Split(": ")[1].Split(',').Select(byte.Parse).ToArray();
        var output = Solve(registerA, programs);
        
        Console.WriteLine($"Result 1: {string.Join(',', output)}");
    }

    [Benchmark]
    public void SolveV2()
    {
        var lines = File.ReadAllLines("data/day17.txt");
        var programs = lines[4].Split(": ")[1].Split(',').Select(byte.Parse).ToArray();
        
        for (var i = 1; i < 8; i++)
        {
            var found = SolveRecursion(i * 8, 1);
            if (found) break;
        }
        
        bool SolveRecursion(long currentNumber, int step)
        {
            for (var i = 0; i < 8; i++)
            {
                var output = Solve(currentNumber + i, programs);
                if (programs.Skip(programs.Length - output.Length).SequenceEqual(output))
                {
                    var found = SolveRecursion((currentNumber + i) * 8, step + 1);
                    if (found) return true;
                }

                if (step != programs.Length - 1) continue;
                if (!output.SequenceEqual(programs)) continue;
                Console.WriteLine($"Result 2 found: {currentNumber + i}");
                return true;
            }

            return false;
        }
    }
    
    private static byte[] Solve(long registerA, byte[] programs, bool compareOutput = false)
    {
        var RegisterA = registerA;
        long RegisterB = 0;
        long RegisterC = 0;

        var outValues = new List<byte>();
        
        // increase by 2 unless jump instruction
        var pointer = 0;

        var operands = new Dictionary<byte, Action<byte>>();
        operands[0] = input => { RegisterA = (long)(RegisterA / Math.Pow(2, ConvertOperandToValue(input, false))); };
        operands[1] = input => { RegisterB ^= ConvertOperandToValue(input, true); };
        operands[2] = input => { RegisterB = ConvertOperandToValue(input, false) % 8; };
        operands[3] = input => { pointer = (int)(RegisterA != 0 ? ConvertOperandToValue(input, true) : pointer);};
        operands[4] = input => { RegisterB ^= RegisterC; };
        operands[5] = input => { outValues.Add((byte)(ConvertOperandToValue(input, false) % 8)); };
        operands[6] = input => { RegisterB = (long)(RegisterA / Math.Pow(2, ConvertOperandToValue(input, false))); };
        operands[7] = input => { RegisterC = (long)(RegisterA / Math.Pow(2, ConvertOperandToValue(input, false))); };

        while (pointer < programs.Length - 1)
        {
            var instruction = programs[pointer];
            pointer++;
            var operand = programs[pointer];
            pointer++;
            operands[instruction](operand);

            if (compareOutput && outValues.Count > 0)
            {
                if (programs[..outValues.Count].SequenceEqual(outValues))
                {
                    continue;
                }

                return [];
            }
        }

        return outValues.ToArray();
        
        long ConvertOperandToValue(byte operand, bool isLiteral) => operand switch
        {
            0 => 0,
            1 => 1,
            2 => 2,
            3 => 3,
            4 when isLiteral => 4,
            4 => RegisterA,
            5 when isLiteral => 5,
            5 => RegisterB,
            6 when isLiteral => 6,
            6 => RegisterC,
            7 when isLiteral => 7,
            7 => throw new InvalidOperationException("Should not appear")
        };
    }
}