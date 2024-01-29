namespace SqlParser.Ast;

public abstract class MacroDefinition : IWriteSql
{
    public class MacroExpression(Expression Expression) : MacroDefinition;

    public class MacroTable(Query Query) : MacroDefinition;

    public void ToSql(SqlTextWriter writer)
    {
        if (this is MacroExpression e)
        {
            writer.WriteSql($"{e.Expression}");
        }
        else if (this is MacroTable t)
        {
            writer.WriteSql($"{t.Query}");
        }
    }
}