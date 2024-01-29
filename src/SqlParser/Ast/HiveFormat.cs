namespace SqlParser.Ast;

/// <summary>
/// Hive-specific format
/// </summary>
public class HiveFormat: IElement
{
    public HiveRowFormat? RowFormat { get; internal set; }
    public HiveIOFormat? Storage { get; internal set; }
    public string? Location { get; internal set; }
}
/// <summary>
/// Hive row format
/// </summary>
public abstract class HiveRowFormat
{
    /// <summary>
    /// Hive Serde row format
    /// </summary>
    /// <param name="Class">String class name</param>
    public class Serde(string Class) : HiveRowFormat;
    /// <summary>
    /// Hive delimited row format
    /// </summary>
    public class Delimited : HiveRowFormat;
}
/// <summary>
/// Hive distribution style
/// </summary>
public abstract class HiveDistributionStyle : IElement
{
    /// <summary>
    /// Hive partitioned distribution
    /// </summary>
    /// <param name="Columns"></param>
    public class Partitioned(Sequence<ColumnDef> Columns) : HiveDistributionStyle;
    /// <summary>
    /// Hive clustered distribution
    /// </summary>
    public class Clustered : HiveDistributionStyle
    {
        public Sequence<Ident>? Columns { get; init; }
        public Sequence<ColumnDef>? SortedBy { get; init; }
        public int NumBuckets { get; init; }
    }
    /// <summary>
    /// Hive skewed distribution
    /// </summary>
    public class Skewed(Sequence<ColumnDef> Columns, Sequence<ColumnDef> On) : HiveDistributionStyle
    {
        public bool StoredAsDirectories { get; init; }
    }
    /// <summary>
    /// Hive no distribution style
    /// </summary>
    public class None : HiveDistributionStyle;
}

/// <summary>
/// Hive IO format
/// </summary>
// ReSharper disable once InconsistentNaming
public abstract class HiveIOFormat
{
    /// <summary>
    /// Hive IOF format
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class IOF(Expression InputFormat, Expression OutputFormat) : HiveIOFormat, IElement;
    /// <summary>
    /// Hive File IO format
    /// </summary>
    public class FileFormat : HiveIOFormat
    {
        public Ast.FileFormat Format { get; init; }
    }
}
