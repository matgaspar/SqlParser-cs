namespace SqlParser.Ast;

public abstract class ArrayElementTypeDef : IWriteSql
{
    public class None : ArrayElementTypeDef;

    public class AngleBracket(DataType DataType) : ArrayElementTypeDef;
    
    public class SquareBracket(DataType DataType) : ArrayElementTypeDef;
    
    public void ToSql(SqlTextWriter writer)
    {
        switch (this)
        {
            case AngleBracket a:
                writer.WriteSql($"{a.DataType}");
                break;

            case SquareBracket s:
                writer.WriteSql($"{s.DataType}");
                break;
        }
    }
}