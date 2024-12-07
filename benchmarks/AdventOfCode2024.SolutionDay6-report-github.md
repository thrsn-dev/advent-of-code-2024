```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2461)
AMD Ryzen 9 7900X3D, 1 CPU, 24 logical and 12 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method  | Mean         | Error       | StdDev      |
|-------- |-------------:|------------:|------------:|
| SolveV1 |     228.1 μs |     1.29 μs |     1.20 μs |
| SolveV2 | 183,341.4 μs | 1,263.11 μs | 1,181.51 μs |
