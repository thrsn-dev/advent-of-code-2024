```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2461)
AMD Ryzen 9 7900X3D, 1 CPU, 24 logical and 12 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method  | Mean     | Error   | StdDev  |
|-------- |---------:|--------:|--------:|
| SolveV1 | 341.7 μs | 3.93 μs | 3.67 μs |
| SolveV2 | 349.7 μs | 4.46 μs | 4.17 μs |
