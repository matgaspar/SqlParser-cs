namespace SqlParser.Ast;

public abstract class NullTreatment : IWriteSql
{
    public class IgnoreNulls : NullTreatment;

    public class RespectNulls : NullTreatment;

    public void ToSql(SqlTextWriter writer)
    {
        switch (this)
        {
            case IgnoreNulls:
                writer.Write("IGNORE NULLS");
                break;

            case RespectNulls:
                writer.Write("RESPECT NULLS");
                break;
        }
    }
}