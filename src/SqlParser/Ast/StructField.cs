namespace SqlParser.Ast;

public class StructField(DataType FieldType, Ident? FieldName = null) : IWriteSql
{
    public void ToSql(SqlTextWriter writer)
    {
        if (FieldName != null)
        {
            writer.WriteSql($"{FieldName} {FieldType}");
        }
        else
        {
            writer.WriteSql($"{FieldType}");
        }
    }
}