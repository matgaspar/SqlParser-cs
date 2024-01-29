namespace SqlParser.Ast;

public class JsonTableColumn(Ident Name, DataType Type, Value Path, bool Exists,
    JsonTableColumnErrorHandling? OnEmpty, JsonTableColumnErrorHandling? OnError) : IWriteSql
{
    public void ToSql(SqlTextWriter writer)
    {
        var exists = Exists ? " EXISTS" : null;

        writer.WriteSql($"{Name} {Type}{exists} PATH {Path}");

        if (OnEmpty != null)
        {
            writer.WriteSql($" {OnEmpty} ON EMPTY");
        }

        if (OnError != null)
        {
            writer.WriteSql($" {OnError} ON ERROR");
        }
    }
}

public abstract class JsonTableColumnErrorHandling : IWriteSql
{
    public class Null : JsonTableColumnErrorHandling;
    public class Default(Value Value) : JsonTableColumnErrorHandling;
    public class Error : JsonTableColumnErrorHandling;

    public void ToSql(SqlTextWriter writer)
    {
        switch (this)
        {
            case Null:
                writer.Write("NULL");
                break;

            case Default d:
                writer.WriteSql($"DEFAULT {d.Value}");
                break;

            case Error:
                writer.Write("ERROR");
                break;
        }
    }
}