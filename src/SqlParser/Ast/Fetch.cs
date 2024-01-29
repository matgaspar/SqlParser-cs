namespace SqlParser.Ast;

/// <summary>
/// Fetch operation
/// </summary>
/// <param name="Quantity">Fetch quantity</param>
/// <param name="WithTies">With ties flag</param>
/// <param name="Percent">Fetch is percentage</param>
public class Fetch(Expression? Quantity = null, bool WithTies = false, bool Percent = false) : IWriteSql, IElement
{
    public void ToSql(SqlTextWriter writer)
    {
        var extension = WithTies ? "WITH TIES" : "ONLY";
        if (Quantity != null)
        {
            var percent = Percent ? " PERCENT" : null;
            writer.Write($"FETCH FIRST {Quantity.ToSql()}{percent} ROWS {extension}");
        }
        else
        {
            writer.Write($"FETCH FIRST ROWS {extension}");
        }
    }
}

/// <summary>
/// Fetch direction
/// </summary>
public abstract class FetchDirection : IWriteSql
{
    /// <summary>
    /// Fetch with limit
    /// </summary>
    /// <param name="Limit"></param>
    public abstract class LimitedFetchDirection(Value Limit) : FetchDirection;

    /// <summary>
    /// Fetch count 
    /// </summary>
    public class Count(Value Limit) : LimitedFetchDirection(Limit);
    /// <summary>
    /// Fetch next
    /// </summary>
    public class Next : FetchDirection;
    /// <summary>
    /// Fetch prior
    /// </summary>
    public class Prior : FetchDirection;
    /// <summary>
    /// Fetch first
    /// </summary>
    public class First : FetchDirection;
    /// <summary>
    /// Fetch last
    /// </summary>
    public class Last : FetchDirection;
    /// <summary>
    /// Fetch absolute
    /// </summary>
    public class Absolute(Value Limit) : LimitedFetchDirection(Limit);
    /// <summary>
    /// 
    /// </summary>
    public class Relative(Value Limit) : LimitedFetchDirection(Limit);
    /// <summary>
    /// Fetch all
    /// </summary>
    public class All : FetchDirection;
    /// <summary>
    /// Fetch forward
    /// </summary>
    public class Forward(Value Limit) : LimitedFetchDirection(Limit);
    /// <summary>
    /// Fetch forward all
    /// </summary>
    public class ForwardAll : FetchDirection;
    /// <summary>
    /// Fetch backward
    /// </summary>
    public class Backward(Value Limit) : LimitedFetchDirection(Limit);
    /// <summary>
    /// Fetch backward all
    /// </summary>
    public class BackwardAll : FetchDirection;

    public void ToSql(SqlTextWriter writer)
    {
        switch (this)
        {
            case Count c:
                c.Limit.ToSql(writer);
                break;

            case Next:
                writer.Write("NEXT");
                break;

            case Prior:
                writer.Write("PRIOR");
                break;

            case First:
                writer.Write("FIRST");
                break;

            case Last:
                writer.Write("LAST");
                break;
            case Absolute a:
                writer.WriteSql($"ABSOLUTE {a.Limit}");
                break;
            case Relative r:
                writer.WriteSql($"RELATIVE {r.Limit}");
                break;
            case All:
                writer.Write("ALL");
                break;
            case Forward f:
                writer.Write("FORWARD");
                
                //TODO once optional is supported
                //if (f.Limit != null)
                //{
                    writer.WriteSql($" {f.Limit}");
                //}
                break;
            case ForwardAll:
                writer.Write("FORWARD ALL");
                break;
            case Backward b:
                writer.Write("BACKWARD");

                //TODO once optional is supported
                //if (f.Limit != null)
                //{
                    writer.WriteSql($" {b.Limit}");
                //}
                break;
            case BackwardAll:
                writer.Write("BACKWARD ALL");
                break;
        }
    }
}