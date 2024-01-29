namespace SqlParser.Ast;

public abstract class WindowType : IWriteSql
{
    public class WindowSpecType(WindowSpec Spec) : WindowType;
    public class NamedWindow(Ident Name) : WindowType;

    public void ToSql(SqlTextWriter writer)
    {
        if (this is WindowSpecType w)
        {
            writer.WriteSql($"({w.Spec})");
        }
        else if (this is NamedWindow n)
        {
            writer.WriteSql($"{n.Name}");
        }
    }
}