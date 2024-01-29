namespace SqlParser.Ast;

public abstract class Password : IWriteSql
{
    public class ValidPassword(Expression Expression) : Password, IElement;

    public class NullPassword : Password;

    public void ToSql(SqlTextWriter writer)
    {
        if (this is ValidPassword v)
        {
            writer.WriteSql($" PASSWORD {v.Expression}");
        }
        else if(this is NullPassword)
        {
            writer.WriteSql($" PASSWORD NULL");
        }
    }
}