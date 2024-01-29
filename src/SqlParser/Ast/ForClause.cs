namespace SqlParser.Ast;

public abstract class ForClause : IWriteSql
{
    public class Browse : ForClause
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("FOR BROWSE");
        }
    }
    public class Json(ForJson ForJson, string? Root, bool IncludeNullValues, bool WithoutArrayWrapper ) : ForClause
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"FOR JSON {ForJson}");

            if (Root != null)
            {
                writer.WriteSql($", ROOT('{Root}')");
            }

            if (IncludeNullValues)
            {
                writer.Write(", INCLUDE_NULL_VALUES");
            }

            if (WithoutArrayWrapper)
            {
                writer.Write(", WITHOUT_ARRAY_WRAPPER");
            }
        }
    }
    public class Xml(ForXml ForXml, bool Elements, bool BinaryBase64, string? Root, bool Type) : ForClause
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"FOR XML {ForXml}");
            
            if (BinaryBase64)
            {
                writer.Write(", BINARY BASE64");
            }
            if (Type)
            {
                writer.Write(", TYPE");
            }

            if (Root != null)
            {
                writer.WriteSql($", ROOT('{Root}')");
            }

            if (Elements)
            {
                writer.Write(", ELEMENTS");
            }
        }
    }

    public abstract void ToSql(SqlTextWriter writer);
}

public abstract class ForJson : IWriteSql
{
    public class Auto : ForJson
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("AUTO");
        }
    }
    public class Path : ForJson
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("PATH");
        }
    }

    public abstract void ToSql(SqlTextWriter writer);
}

public abstract class ForXml :IWriteSql
{
    public class Raw(string? Value) : ForXml
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("RAW");

            if (Value != null)
            {
                writer.Write($"('{Value}')");
            }
        }
    }

    public class Auto : ForXml
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("AUTO");
        }
    }

    public class Explicit : ForXml
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("EXPLICIT");
        }
    }

    public class Path(string? Value) : ForXml
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("PATH");

            if (Value != null)
            {
                writer.Write($"('{Value}')");
            }
        }
    }

    public abstract void ToSql(SqlTextWriter writer);
}