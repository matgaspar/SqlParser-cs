namespace SqlParser.Ast;

public abstract class ExactNumberInfo : IWriteSql
{
    /// <summary>
    /// No additional information e.g. `DECIMAL`
    /// </summary>
    public class None : ExactNumberInfo;
    /// <summary>
    /// Only precision information e.g. `DECIMAL(10)`
    /// </summary>
    /// <param name="Length">Length</param>
    public class Precision(ulong Length) : ExactNumberInfo;
    /// <summary>
    /// Precision and scale information e.g. `DECIMAL(10,2)`
    /// </summary>
    /// <param name="Length">Length</param>
    /// <param name="Scale">Scale</param>
    public class PrecisionAndScale(ulong Length, ulong Scale) : ExactNumberInfo;

    public void ToSql(SqlTextWriter writer)
    {
        switch (this)
        {
            case Precision p:
                writer.Write($"({p.Length})");
                break;

            case PrecisionAndScale ps:
                writer.Write($"({ps.Length},{ps.Scale})");
                break;
        }
    }
}