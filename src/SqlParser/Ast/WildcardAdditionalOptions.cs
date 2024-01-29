namespace SqlParser.Ast;

/// <summary>
/// Additional options for wildcards, e.g. Snowflake EXCLUDE/RENAME and BigQuery EXCEPT.
/// </summary>
public class WildcardAdditionalOptions : IWriteSql, IElement
{
    // [EXCLUDE...]
    public ExcludeSelectItem? ExcludeOption { get; init; }
    // [EXCEPT...]
    public ExceptSelectItem? ExceptOption { get; init; }
    // [RENAME ...]
    public RenameSelectItem? RenameOption { get; init; }
    // [REPLACE]
    // BigQuery syntax: <https://cloud.google.com/bigquery/docs/reference/standard-sql/query-syntax#select_replace>
    public ReplaceSelectItem? ReplaceOption { get; init; }
    
    public void ToSql(SqlTextWriter writer)
    {
        if (ExcludeOption != null)
        {
            writer.WriteSql($" {ExcludeOption}");
        }

        if (ExceptOption != null)
        {
            writer.WriteSql($" {ExceptOption}");
        }

        if (RenameOption != null)
        {
            writer.WriteSql($" {RenameOption}");
        }

        if (ReplaceOption != null)
        {
            writer.WriteSql($" {ReplaceOption}");
        }
    }
}

public class ReplaceSelectItem(Sequence<ReplaceSelectElement> Items) : IWriteSql, IElement
{
    public void ToSql(SqlTextWriter writer)
    {
        writer.WriteSql($"REPLACE ({Items})");
    }
}

public class ReplaceSelectElement(Expression Expr, Ident Name, bool AsKeyword) : IWriteSql, IElement
{
    public void ToSql(SqlTextWriter writer)
    {
        if (AsKeyword)
        {
            writer.WriteSql($"{Expr} AS {Name}");
        }
        else
        {
            writer.WriteSql($"{Expr} {Name}");
        }
    }
}