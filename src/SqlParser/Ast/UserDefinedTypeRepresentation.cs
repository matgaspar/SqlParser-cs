
namespace SqlParser.Ast;
public abstract class UserDefinedTypeRepresentation : IWriteSql
{
    public class Composite(Sequence<UserDefinedTypeCompositeAttributeDef> Attributes) : UserDefinedTypeRepresentation;

    public void ToSql(SqlTextWriter writer)
    {
        if (this is Composite c)
        {
            writer.Write("(");
            writer.WriteDelimited(c.Attributes, ", ");
            writer.Write(")");
        }
    }
}
