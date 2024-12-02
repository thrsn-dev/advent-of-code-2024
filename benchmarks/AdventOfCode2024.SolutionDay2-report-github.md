```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2461)
AMD Ryzen 9 7900X3D, 1 CPU, 24 logical and 12 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method  | Mean     | Error   | StdDev  |
|-------- |---------:|--------:|--------:|
| SolveV1 | 178.5 μs | 3.04 μs | 2.85 μs |
| SolveV2 | 272.2 μs | 4.45 μs | 3.94 μs |
