namespace SqlParser.Ast;

/// <summary>
/// Primitive SQL values such as number and string
/// </summary>
public abstract class Value : IWriteSql
{
    public abstract class StringBasedValue(string Value) : Value;

    /// <summary>
    /// Boolean value true or false
    /// </summary>
    /// <param name="Value">True or false</param>
    public class Boolean(bool Value) : Value
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write(Value);
        }
    }
    /// <summary>
    /// $tag_name$string value$tag_name$ - Postgres syntax
    /// </summary>
    /// <param name="Value">Quoted value</param>
    public class DollarQuotedString(DollarQuotedStringValue Value) : Value
    {
        public override void ToSql(SqlTextWriter writer)
        {
            Value.ToSql(writer);
        }
    }
    /// <summary>
    /// B"string value"
    /// </summary>
    /// <param name="Value">String value</param>
    public class DoubleQuotedString(string Value) : StringBasedValue(Value)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"\"{Value.EscapeDoubleQuoteString()}\"");
        }
    }
    /// <summary>
    /// e'string value' - Postgres extension
    /// <see href="https://www.postgresql.org/docs/8.3/sql-syntax-lexical.html#SQL-SYNTAX-STRINGS"/>
    /// for more details.
    /// </summary>
    /// <param name="Value">String value</param>
    public class EscapedStringLiteral(string Value) : StringBasedValue(Value)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write($"E'{Value.EscapeEscapedString()}'");
        }
    }
    /// <summary>
    /// X'hex value'
    /// </summary>
    /// <param name="Value">String value</param>
    public class HexStringLiteral(string Value) : StringBasedValue(Value)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"X'{Value}'");
        }
    }
    /// <summary>
    /// N'string value'
    /// </summary>
    /// <param name="Value">String value</param>
    public class NationalStringLiteral(string Value) : StringBasedValue(Value)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"N'{Value}'");
        }
    }
    /// <summary>
    /// NULL value
    /// </summary>
    public class Null : Value
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"NULL");
        }
    }
    /// <summary>
    /// Numeric literal
    /// </summary>
    /// <param name="Value">String value</param>
    /// <param name="Long">True if long value</param>
    public class Number(string Value, bool Long = false) : StringBasedValue(Value)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"{Value}");
            writer.Write(Long ? "L" : null);
        }

        public int? AsInt()
        {
            if (int.TryParse(Value, out var val))
            {
                return val;
            }

            return null;
        }
    }
    /// <summary>
    /// `?` or `$` Prepared statement arg placeholder
    /// </summary>
    /// <param name="Value">String value</param>
    public class Placeholder(string Value) : StringBasedValue(Value)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write(Value);
        }
    }
    /// <summary>
    /// R'string value' or r'string value' or r"string value"
    /// <see href="https://cloud.google.com/bigquery/docs/reference/standard-sql/lexical#quoted_literals"/>
    /// </summary>
    /// <param name="Value">String value</param>
    public class RawStringLiteral(string Value) : StringBasedValue(Value)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write($"R'{Value}'");
        }
    }
    /// <summary>
    /// Single quoted string value
    /// </summary>
    /// <param name="Value">String value</param>
    public class SingleQuotedString(string Value) : StringBasedValue(Value)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write($"'{Value.EscapeSingleQuoteString()}'");
        }
    }
    /// <summary>
    /// B'string value'
    /// </summary>
    /// <param name="Value">String value</param>
    public class SingleQuotedByteStringLiteral(string Value) : StringBasedValue(Value)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"B'{Value}'");
        }
    }
    /// <summary>
    /// B"string value"
    /// </summary>
    /// <param name="Value">String value</param>
    public class DoubleQuotedByteStringLiteral(string Value) : StringBasedValue(Value)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"B\"{Value}\"");
        }
    }
    /// <summary>
    /// Add support of snowflake field:key - key should be a value
    /// </summary>
    /// <param name="Value">String value</param>
    public class UnQuotedString(string Value) : StringBasedValue(Value)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write(Value);
        }
    }

    public abstract void ToSql(SqlTextWriter writer);

    public T As<T>() where T : Value
    {
        return (T) this;
    }

    public Number AsNumber()
    {
        return As<Number>();
    }

    //public SingleQuotedString AsSingleQuoted()
    //{
    //    return As<SingleQuotedString>();
    //}


    //public DoubleQuotedString AsDoubleQuoted()
    //{
    //    return As<DoubleQuotedString>();
    //}
}
/// <summary>
/// Dollar quoted string value
/// </summary>
/// <param name="Value">String value</param>
/// <param name="Tag">Tag value</param>
public class DollarQuotedStringValue(string Value, string? Tag = null) : IWriteSql
{
    public void ToSql(SqlTextWriter writer)
    {
        writer.Write(Tag != null 
            ? $"${Tag}${Value}${Tag}$" 
            : $"$${Value}$$");
    }
}