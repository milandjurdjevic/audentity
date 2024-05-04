# Audentity [![latest version](https://img.shields.io/nuget/v/Audentity)](https://www.nuget.org/packages/Audentity)

**Auditing library for Entity Framework**

The Audentity project is designed to enhance data integrity and accountability within applications utilizing the Entity
Framework
for data access. By integrating specialized auditing functionality, the project enables developers to effortlessly track
and log changes to the database, including inserts, updates, and deletions. This empowers organizations to maintain a
clear and transparent record of data modifications, ensuring compliance, detecting unauthorized activities, and
facilitating
forensic analysis when necessary. The project aims to streamline the implementation of robust auditing features, making
it seamless for developers to integrate comprehensive data tracking capabilities into their Entity Framework-based
applications.

## Usage

To read more about how to use Audentity, please refer to the [documentation](src/Audentity/README.md#usage) link.

## Benchmark

| Method    | Count |     Mean |    Error |   StdDev |   Gen0 | Allocated |
|-----------|-------|---------:|---------:|---------:|-------:|----------:|
| FromEntry | 1     | 46.55 ns | 0.080 ns | 0.067 ns | 0.0051 |      32 B |
| FromEntry | 10    | 46.82 ns | 0.082 ns | 0.073 ns | 0.0051 |      32 B |
| FromEntry | 100   | 46.89 ns | 0.041 ns | 0.036 ns | 0.0051 |      32 B |
| FromEntry | 1000  | 46.66 ns | 0.160 ns | 0.150 ns | 0.0051 |      32 B |

### Legend
- Count     : Value of the 'Count' parameter
- Mean      : Arithmetic mean of all measurements
- Error     : Half of 99.9% confidence interval
- StdDev    : Standard deviation of all measurements
- Gen0      : GC Generation 0 collects per 1000 operations
- Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
- 1 ns      : 1 Nanosecond (0.000000001 sec)


## License

This project is licensed under the [MIT License](LICENSE).