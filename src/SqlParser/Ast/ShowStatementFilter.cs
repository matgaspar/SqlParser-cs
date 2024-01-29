namespace SqlParser.Ast;

/// <summary>
/// Show statement filter
/// </summary>
public abstract class ShowStatementFilter : IWriteSql
{
    /// <summary>
    /// Like filter
    /// </summary>
    /// <param name="Filter">Filter</param>
    public class Like(string Filter) : ShowStatementFilter
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"LIKE '{Filter.EscapeSingleQuoteString()}'");
        }
    }
    /// <summary>
    /// ILike filter
    /// </summary>
    /// <param name="Filter">Filter</param>
    // ReSharper disable once InconsistentNaming
    public class ILike(string Filter) : ShowStatementFilter
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write($"ILIKE '{Filter.EscapeSingleQuoteString()}'");
        }
    }
    /// <summary>
    /// Where filter
    /// </summary>
    /// <param name="Expression">Filter</param>
    public class Where(Expression Expression) : ShowStatementFilter, IElement
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"WHERE {Expression}");
        }
    }

    public abstract void ToSql(SqlTextWriter writer);
}