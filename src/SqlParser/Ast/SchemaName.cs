namespace SqlParser.Ast;

/// <summary>
/// Schema name
/// </summary>
public abstract class SchemaName : Statement
{
    /// <summary>
    /// Only schema name specified: schema name
    /// </summary>
    /// <param name="Name"></param>
    public class Simple(ObjectName Name) : SchemaName
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write(Name);
        }
    }

    /// <summary>
    /// Only authorization identifier specified: `AUTHORIZATION schema authorization identifier`
    /// </summary>
    /// <param name="Value"></param>
    public class UnnamedAuthorization(Ident Value) : SchemaName
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write($"AUTHORIZATION {Value}");
        }
    }

    /// <summary>
    /// Both schema name and authorization identifier specified: `schema name  AUTHORIZATION schema authorization identifier`
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Value"></param>
    public class NamedAuthorization(ObjectName Name, Ident Value) : SchemaName
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write($"{Name} AUTHORIZATION {Value}");
        }
    }
}