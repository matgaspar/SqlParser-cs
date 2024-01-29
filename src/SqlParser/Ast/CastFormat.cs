namespace SqlParser.Ast;

/// <summary>
/// Options for `CAST` / `TRY_CAST`
/// </summary>
public abstract class CastFormat : IWriteSql
{
    public class Value(Ast.Value Val) : CastFormat;

    public class ValueAtTimeZone(Ast.Value Val, Ast.Value TimeZone) : CastFormat;

    public void ToSql(SqlTextWriter writer)
    {
        switch (this)
        {
            case Value v:
                writer.WriteSql($"{v.Val}");
                break;
            case ValueAtTimeZone tz:
                writer.WriteSql($"{tz.Val} AT TIME ZONE {tz.TimeZone}");
                break;
        }
    }
}
