
namespace SqlParser.Ast;

/// <summary>
/// On insert statement
/// </summary>
public abstract class OnInsert : IWriteSql, IElement
{
    /// <summary>
    /// MySQL when the key already exists, then execute an update instead
    /// <example>
    /// <c>
    /// ON DUPLICATE KEY UPDATE 
    /// </c>
    /// </example>
    /// </summary>
    /// <param name="Assignments"></param>
    public class DuplicateKeyUpdate(Sequence<Statement.Assignment> Assignments) : OnInsert
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($" ON DUPLICATE KEY UPDATE {Assignments}");
        }
    }
    /// <summary>
    /// This is a PostgreSQL and Sqlite extension
    /// <example>
    /// <c>
    /// ON CONFLICT 
    /// </c>
    /// </example>
    /// </summary>
    /// <param name="OnConflict"></param>
    public class Conflict(OnConflict OnConflict) : OnInsert
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"{OnConflict}");
        }
    }

    public abstract void ToSql(SqlTextWriter writer);
}