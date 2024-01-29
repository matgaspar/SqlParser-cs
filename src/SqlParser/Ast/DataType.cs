namespace SqlParser.Ast;

/// <summary>
/// SQL data types
/// </summary>
public abstract class DataType : IWriteSql
{
    /// <summary>
    /// Data type with character length specificity
    /// </summary>
    public abstract class CharacterLengthDataType(CharacterLength? CharacterLength) : DataType
    {
        protected CharacterLength? CharLength = CharacterLength;

        protected ulong? IntegerLength => CharLength is CharacterLength.IntegerLength length 
            ? length.Length 
            : null;

        protected void FormatCharacterStringType(SqlTextWriter writer, string sqlType, ulong? length)
        {
            writer.Write(sqlType);

            if (length != null)
            {
                writer.Write($"({length})");
            }
        }
    }

    /// <summary>
    /// Data type with length specificity
    /// </summary>
    /// <param name="Length">Data type length</param>
    public abstract class LengthDataType(ulong? Length = null) : DataType
    {
        protected void FormatTypeWithOptionalLength(SqlTextWriter writer, string sqlType, ulong? length, bool unsigned = false)
        {
            writer.Write($"{sqlType}");

            if (length != null)
            {
                writer.Write($"({length})");
            }
            if (unsigned)
            {
                writer.Write(" UNSIGNED");
            }
        }
    }
    /// <summary>
    /// Data type with exact number specificity
    /// </summary>
    /// <param name="ExactNumberInfo"></param>
    public abstract class ExactNumberDataType(ExactNumberInfo? ExactNumberInfo) : DataType;
    /// <summary>
    /// Data type with time zone information
    /// </summary>
    /// <param name="TimezoneInfo">Time zone info</param>
    /// <param name="Length"></param>
    public abstract class TimeZoneDataType(TimezoneInfo TimezoneInfo, ulong? Length = null) : DataType
    {
        protected void FormattedDatetimePrecisionAndTz(SqlTextWriter writer, string sqlType)
        {
            writer.Write($"{sqlType}");
            string? length = null;

            if (Length != null)
            {
                length = $"({Length})";
            }

            if (TimezoneInfo == TimezoneInfo.Tz)
            {
                writer.WriteSql($"{TimezoneInfo}{length}");
            }
            else if (TimezoneInfo != TimezoneInfo.None)
            {
                writer.WriteSql($"{length} {TimezoneInfo}");
            }
        }
    }

    /// <summary>
    /// Array data type
    /// </summary>
    /// <param name="DataType"></param>
    public class Array(ArrayElementTypeDef DataType) : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            switch (DataType)
            {
                case ArrayElementTypeDef.None:
                    writer.Write("ARRAY");
                    break;

                case ArrayElementTypeDef.SquareBracket:
                    writer.WriteSql($"{DataType}[]");
                    break;

                case ArrayElementTypeDef.AngleBracket:
                    writer.WriteSql($"ARRAY<{DataType}>");
                    break;
            }
        }
    }
    /// <summary>
    /// Big integer with optional display width e.g. BIGINT or BIGINT(20)
    /// </summary>
    /// <param name="Length">Length</param>
    public class BigInt(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "BIGINT", Length);
        }
    }
    /// <summary>
    /// This is alias for `BigNumeric` type used in BigQuery
    ///
    /// <see href="https://cloud.google.com/bigquery/docs/reference/standard-sql/data-types#decimal_types"/>
    /// </summary>
    /// <param ExactNumberInfo="Exact number"></param>
    public class BigNumeric(ExactNumberInfo ExactNumberInfo) : ExactNumberDataType(ExactNumberInfo)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            // ReSharper disable once StringLiteralTypo
            writer.WriteSql($"BIGNUMERIC{ExactNumberInfo}");
        }
    }
    /// <summary>
    /// Fixed-length binary type with optional length e.g.
    ///
    /// <see href="https://jakewheat.github.io/sql-overview/sql-2016-foundation-grammar.html#binary-string-type"/>
    /// <see href="https://learn.microsoft.com/pt-br/sql/t-sql/data-types/binary-and-varbinary-transact-sql?view=sql-server-ver16"/>
    /// </summary>
    /// <param name="Length">Length</param>
    public class Binary(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "BINARY", Length);
        }
    }
    /// <summary>
    /// Large binary object with optional length e.g. BLOB, BLOB(1000)
    /// 
    /// <see href="https://jakewheat.github.io/sql-overview/sql-2016-foundation-grammar.html#binary-large-object-string-type"/>
    /// <see href="https://docs.oracle.com/javadb/10.8.3.0/ref/rrefblob.html"/>
    /// </summary>
    public class Blob(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "BLOB", Length);
        }
    }
    /// <summary>
    /// Boolean data type
    /// </summary>
    public class Bool : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("BOOL");
        }
    }
    public class Boolean : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("BOOLEAN");
        }
    }
    /// <summary>
    /// Binary string data type
    /// </summary>
    // ReSharper disable IdentifierTypo
    public class Bytea : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            // ReSharper disable once StringLiteralTypo
            writer.Write("BYTEA");
        }
    }

    /// Variable-length binary data with optional length.
    ///
    /// [bigquery]: https://cloud.google.com/bigquery/docs/reference/standard-sql/data-types#bytes_type
    public class Bytes(ulong? Length) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            //writer.Write("BYTES");
            FormatTypeWithOptionalLength(writer, "BYTES", Length);
        }
    }
    /// <summary>
    /// Fixed-length char type e.g. CHAR(10)
    /// </summary>
    public class Char(CharacterLength? CharacterLength = null) : CharacterLengthDataType(CharacterLength)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            
            FormatCharacterStringType(writer, "CHAR", IntegerLength);
        }
    }
    /// <summary>
    /// Fixed-length character type e.g. CHARACTER(10)
    /// </summary>
    public class Character(CharacterLength? CharacterLength = null) : CharacterLengthDataType(CharacterLength)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatCharacterStringType(writer, "CHARACTER", IntegerLength);
        }
    }
    /// <summary>
    /// Large character object with optional length e.g. CHARACTER LARGE OBJECT, CHARACTER LARGE OBJECT(1000)
    ///
    /// <see href="https://jakewheat.github.io/sql-overview/sql-2016-foundation-grammar.html#character-large-object-type"/>
    /// </summary>
    /// <param name="Length">Length</param>
    public class CharacterLargeObject(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "CHARACTER", Length);
        }
    }
    /// <summary>
    /// Character varying type e.g. CHARACTER VARYING(10)
    /// </summary>
    public class CharacterVarying(CharacterLength? CharacterLength = null) : CharacterLengthDataType(CharacterLength)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            if (CharacterLength != null)
            {
                FormatCharacterStringType(writer, "CHARACTER VARYING", IntegerLength);
            }
        }
    }
    /// <summary>
    /// Large character object with optional length e.g. CHAR LARGE OBJECT, CHAR LARGE OBJECT(1000)
    ///
    /// <see href="https://jakewheat.github.io/sql-overview/sql-2016-foundation-grammar.html#character-large-object-type"/>
    /// </summary>
    /// <param name="Length">Length</param>
    public class CharLargeObject(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "CHARACTER LARGE OBJECT", Length);
        }
    }
    /// <summary>
    /// Char varying type e.g. CHAR VARYING(10)
    /// </summary>
    public class CharVarying(CharacterLength? CharacterLength = null) : CharacterLengthDataType(CharacterLength)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            if (CharacterLength != null)
            {
                FormatCharacterStringType(writer, "CHAR VARYING", IntegerLength);
            }
        }
    }
    /// <summary>
    /// Large character object with optional length e.g. CLOB, CLOB(1000)
    ///
    /// <see href="https://jakewheat.github.io/sql-overview/sql-2016-foundation-grammar.html#character-large-object-type"/>
    /// <see href="https://docs.oracle.com/javadb/10.10.1.2/ref/rrefclob.html"/>
    /// </summary>
    public class Clob(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "CLOB", Length);
        }
    }
    /// <summary>
    /// Custom type such as enums
    /// </summary>
    public class Custom(ObjectName Name, Sequence<string>? Values = null) : DataType, IElement
    {
        public override void ToSql(SqlTextWriter writer)
        {
            if (Values.SafeAny())
            {
                writer.WriteSql($"{Name}({Values})");
            }
            else
            {
                Name.ToSql(writer);
            }
        }
    }
    /// <summary>
    /// Date data type
    /// </summary>
    public class Date : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("DATE");
        }
    }
    /// <summary>
    /// Datetime with optional time precision e.g. MySQL
    /// 
    /// <see href="https://dev.mysql.com/doc/refman/8.0/en/datetime.html"/>
    /// </summary>
    public class Datetime(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "DATETIME", Length);
        }
    }
    /// <summary>
    /// Dec data type with optional precision and scale e.g. DEC(10,2): DataType
    ///
    /// <see href="https://jakewheat.github.io/sql-overview/sql-2016-foundation-grammar.html#exact-numeric-type"/>
    /// </summary>
    public class Dec(ExactNumberInfo ExactNumberInfo) : ExactNumberDataType(ExactNumberInfo)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"DEC{ExactNumberInfo}");
        }
    }
    /// <summary>
    /// Decimal type with optional precision and scale e.g. DECIMAL(10,2)
    ///
    /// <see href="https://jakewheat.github.io/sql-overview/sql-2016-foundation-grammar.html#exact-numeric-type"/>
    /// </summary>
    public class Decimal(ExactNumberInfo ExactNumberInfo) : ExactNumberDataType(ExactNumberInfo)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"DECIMAL{ExactNumberInfo}");
        }
    }
    /// <summary>
    /// Double data type
    /// </summary>
    public class Double : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("DOUBLE");
        }
    }
    /// <summary>
    /// Double PRECISION e.g. standard, PostgreSql
    ///
    /// <see href="https://jakewheat.github.io/sql-overview/sql-2016-foundation-grammar.html#approximate-numeric-type"/>
    /// <see href="https://www.postgresql.org/docs/current/datatype-numeric.html"/>
    /// </summary>
    public class DoublePrecision : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("DOUBLE PRECISION");
        }
    }
    /// <summary>
    /// Enum data types 
    /// </summary>
    /// <param name="Values"></param>
    public class Enum(Sequence<string> Values) : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("ENUM(");
            for (var i = 0; i < Values.Count; i++)
            {
                if (i > 0)
                {
                    writer.Write(", ");
                }
                writer.Write($"'{Values[i].EscapeSingleQuoteString()}'");
            }
            writer.Write(")");
        }
    }
    /// <summary>
    /// Floating point with optional precision e.g. FLOAT(8)
    /// </summary>
    public class Float(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "DATETIME", Length);
        }
    }
    /// <summary>
    /// FLOAT4 as alias for Real in postgresql
    /// </summary>
    public class Float4 : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("FLOAT4");
        }
    }
    /// <summary>
    /// FLOAT8 as alias for Double in postgresql
    /// </summary>
    public class Float8 : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("FLOAT8");
        }
    }
    /// <summary>
    /// FLOAT64
    /// </summary>
    public class Float64 : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("FLOAT64");
        }
    }
    /// <summary>
    /// Integer with optional display width e.g. INT or INT(11)
    /// <param name="Length">Length</param>
    /// </summary>
    public class Int(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "INT", Length);
        }
    }
    /// <summary>
    /// Integer with optional display width e.g. INTEGER or INTEGER(11)
    /// </summary>
    public class Integer(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "INTEGER", Length);
        }
    }
    /// <summary>
    /// Interval data type
    /// </summary>
    public class Interval : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("INTERVAL");
        }
    }
    /// <summary>
    /// Int2 as alias for SmallInt in [postgresql]
    /// Note: Int2 mean 2 bytes in postgres (not 2 bits)
    /// Int2 with optional display width e.g. INT2 or INT2(5)
    /// </summary>
    /// <param name="Length">Length</param>
    public class Int2(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "INT2", Length);
        }
    }
    /// <summary>
    /// Int4 as alias for Integer in [postgresql]
    /// Note: Int4 mean 4 bytes in postgres (not 4 bits)
    /// Int4 with optional display width e.g. Int4 or Int4(11)
    /// </summary>
    /// <param name="Length">Length</param>
    public class Int4(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "INT4", Length);
        }
    }
    /// <summary>
    /// Int8 as alias for Bigint in [postgresql]
    /// Note: Int8 mean 8 bytes in postgres (not 8 bits)
    /// Int8 with optional display width e.g. INT8 or INT8(11)
    /// </summary>
    /// <param name="Length">Length</param>
    public class Int8(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "INT8", Length);
        }
    }
    /// Integer type in [bigquery]
    ///
    /// [bigquery]: https://cloud.google.com/bigquery/docs/reference/standard-sql/data-types#integer_types
    public class Int64 : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("INT64");
        }
    }
    /// <summary>
    /// Join data type
    /// </summary>
    public class Json : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("JSON");
        }
    }

    /// <summary>
    /// MySQL medium integer ([1]) with optional display width e.g. MEDIUMINT or MEDIUMINT(5)
    ///
    /// <see href="https://dev.mysql.com/doc/refman/8.0/en/integer-types.html"/>
    /// </summary>
    public class MediumInt(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "MEDIUMINT", Length);
        }
    }
    /// <summary>
    /// Empty data type
    /// </summary>
    public class None : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
        }
    }
    /// <summary>
    /// Numeric type with optional precision and scale e.g. NUMERIC(10,2) 
    ///
    /// <see href="https://jakewheat.github.io/sql-overview/sql-2016-foundation-grammar.html#exact-numeric-type"/>
    /// </summary>
    public class Numeric(ExactNumberInfo ExactNumberInfo) : ExactNumberDataType(ExactNumberInfo)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"NUMERIC{ExactNumberInfo}");
        }
    }
    /// <summary>
    /// Variable-length character type e.g. NVARCHAR(10)
    /// </summary>
    public class Nvarchar(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "NVARCHAR", Length);
        }
    }
    /// <summary>
    /// Floating point e.g. REAL
    /// </summary>
    public class Real : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("REAL");
        }
    }
    /// <summary>
    /// Regclass used in postgresql serial
    /// </summary>
    public class Regclass : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("REGCLASS");
        }
    }
    /// <summary>
    /// Set data type
    /// </summary>
    public class Set(Sequence<string> Values) : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("SET(");
            for (var i = 0; i < Values.Count; i++)
            {
                if (i > 0)
                {
                    writer.Write(", ");
                }

                writer.Write($"'{Values[i].EscapeSingleQuoteString()}'");
            }
            writer.Write(")");
        }
    }
    /// <summary>
    /// Small integer with optional display width e.g. SMALLINT or SMALLINT(5)
    /// </summary>
    public class SmallInt(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "SMALLINT", Length);
        }
    }
    /// <summary>
    /// String data type
    /// </summary>
    public class StringType(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "STRING", Length);
        }
    }
    /// Struct
    ///
    /// Bive: https://docs.cloudera.com/cdw-runtime/cloud/impala-sql-reference/topics/impala-struct.html
    /// BigQuery: https://cloud.google.com/bigquery/docs/reference/standard-sql/data-types#struct_type
    public class Struct(Sequence<StructField> Fields) : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("STRUCT");

            if (Fields.SafeAny())
            {
                writer.Write("<");
                writer.WriteDelimited(Fields, ", ");
                writer.Write(">");
            }
        }
    }
    /// <summary>
    /// Text data type
    /// </summary>
    public class Text : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("TEXT");
        }
    }
    /// <summary>
    /// Time with optional time precision and time zone information
    ///
    /// <see href="https://jakewheat.github.io/sql-overview/sql-2016-foundation-grammar.html#datetime-type"/>
    /// </summary>
    public class Time(TimezoneInfo TimezoneInfo, ulong? When = null) : TimeZoneDataType(TimezoneInfo, When)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormattedDatetimePrecisionAndTz(writer, "TIME");
        }
    }
    /// <summary>
    /// Timestamp with optional time precision and time zone information
    ///
    /// <see href="https://jakewheat.github.io/sql-overview/sql-2016-foundation-grammar.html#datetime-type"/>
    /// </summary>
    public class Timestamp(TimezoneInfo TimezoneInfo, ulong? When = null) : TimeZoneDataType(TimezoneInfo, When)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormattedDatetimePrecisionAndTz(writer, "TIMESTAMP");
        }
    }
    /// <summary>
    /// Tiny integer with optional display width e.g. TINYINT or TINYINT(3)
    /// </summary>
    public class TinyInt(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "TINYINT", Length);
        }
    }
    /// <summary>
    /// Unsigned big integer with optional display width e.g. BIGINT UNSIGNED or BIGINT(20) UNSIGNED
    /// </summary>
    public class UnsignedBigInt(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "BIGINT", Length, true);
        }
    }
    /// <summary>
    /// Unsigned integer with optional display width e.g. INT UNSIGNED or INT(11) UNSIGNED
    /// </summary>
    public class UnsignedInt(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "INT", Length, true);
        }
    }
    /// <summary>
    /// Unsigned integer with optional display width e.g. INTEGER UNSIGNED or INTEGER(11) UNSIGNED
    /// </summary>
    public class UnsignedInteger(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "INT", Length, true);
        }
    }
    /// <summary>
    /// Unsigned medium integer ([1]) with optional display width e.g. MEDIUMINT UNSIGNED or MEDIUMINT(5) UNSIGNED
    ///
    /// <see href="https://dev.mysql.com/doc/refman/8.0/en/integer-types.html"/>
    /// </summary>
    public class UnsignedMediumInt(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "MEDIUMINT", Length, true);
        }
    }
    /// <summary>
    /// Unsigned small integer with optional display width e.g. SMALLINT UNSIGNED or SMALLINT(5) UNSIGNED
    /// </summary>
    public class UnsignedSmallInt(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "SMALLINT", Length, true);
        }
    }
    /// <summary>
    /// Unsigned tiny integer with optional display width e.g. TINYINT UNSIGNED or TINYINT(3) UNSIGNED
    /// </summary>
    public class UnsignedTinyInt(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "TINYINT", Length, true);
        }
    }
    /// <summary>
    /// Unsigned Int2 with optional display width e.g. INT2 Unsigned or INT2(5) Unsigned
    /// </summary>
    /// <param name="Length"></param>
    public class UnsignedInt2(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "INT2", Length, true);
        }
    }
    /// <summary>
    /// Unsigned Int4 with optional display width e.g. INT4 Unsigned or INT4(5) Unsigned
    /// </summary>
    /// <param name="Length"></param>
    public class UnsignedInt4(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "INT4", Length, true);
        }
    }
    /// <summary>
    /// Unsigned Int8 with optional display width e.g. INT8 Unsigned or INT8(5) Unsigned
    /// </summary>
    /// <param name="Length"></param>
    public class UnsignedInt8(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "INT8", Length, true);
        }
    }
    /// <summary>
    /// UUID data ype
    /// </summary>
    public class Uuid : DataType
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("UUID");
        }
    }
    /// <summary>
    /// Variable-length binary with optional length type
    ///
    /// <see href="https://jakewheat.github.io/sql-overview/sql-2016-foundation-grammar.html#binary-string-type"/>
    /// <see href="https://learn.microsoft.com/pt-br/sql/t-sql/data-types/binary-and-varbinary-transact-sql?view=sql-server-ver16"/>
    /// </summary>
    public class Varbinary(ulong? Length = null) : LengthDataType(Length)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatTypeWithOptionalLength(writer, "VARBINARY", Length);
        }
    }
    /// <summary>
    /// Variable-length character type e.g. VARCHAR(10)
    /// </summary>
    public class Varchar(CharacterLength? CharacterLength = null) : CharacterLengthDataType(CharacterLength)
    {
        public override void ToSql(SqlTextWriter writer)
        {
            FormatCharacterStringType(writer, "VARCHAR", IntegerLength);
        }
    }

    public abstract void ToSql(SqlTextWriter writer);
}