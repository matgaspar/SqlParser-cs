namespace SqlParser.Ast;

public class MacroArg(Ident Name, Expression? DefaultExpression = null) : IWriteSql
{
    public void ToSql(SqlTextWriter writer)
    {
        writer.WriteSql($"{Name}");

        if (DefaultExpression != null)
        {
            writer.WriteSql($" := {DefaultExpression}");
        }
    }
}