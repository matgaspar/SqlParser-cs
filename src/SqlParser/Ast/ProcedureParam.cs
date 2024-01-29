namespace SqlParser.Ast;

public class ProcedureParam(Ident Name, DataType DataType) : IWriteSql
{
    public void ToSql(SqlTextWriter writer)
    {
        writer.WriteSql($"{Name} {DataType}");
    }
}
