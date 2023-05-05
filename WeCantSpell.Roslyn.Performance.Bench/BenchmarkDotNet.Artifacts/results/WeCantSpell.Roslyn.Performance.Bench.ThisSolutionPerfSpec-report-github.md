``` ini

BenchmarkDotNet=v0.13.5, OS=macOS Ventura 13.3.1 (a) (22E772610a) [Darwin 22.4.0]
Apple M1 Max, 1 CPU, 10 logical and 10 physical cores
.NET SDK=7.0.202
  [Host]     : .NET 6.0.15 (6.0.1523.11507), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 6.0.15 (6.0.1523.11507), Arm64 RyuJIT AdvSIMD


```
|                                             Method |     Mean |    Error |   StdDev |      Gen0 |     Gen1 |     Gen2 | Allocated |
|--------------------------------------------------- |---------:|---------:|---------:|----------:|---------:|---------:|----------:|
| &#39;Measure how quickly a solution can be processed.&#39; | 31.68 ms | 0.602 ms | 0.693 ms | 2687.5000 | 500.0000 | 125.0000 |   7.46 MB |
