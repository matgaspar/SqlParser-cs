namespace SqlParser.Ast;

/// <summary>
/// Transaction mode
/// </summary>
public abstract class TransactionMode : IWriteSql
{
    public class AccessMode(TransactionAccessMode TransactionAccessMode) : TransactionMode;

    public class IsolationLevel(TransactionIsolationLevel TransactionIsolationLevel) : TransactionMode;

    public void ToSql(SqlTextWriter writer)
    {
        if (this is AccessMode a)
        {
            writer.WriteSql($"{a.TransactionAccessMode}");
        }
        else if (this is IsolationLevel i)
        {
            writer.WriteSql($"ISOLATION LEVEL {i.TransactionIsolationLevel}");
        }
    }
}