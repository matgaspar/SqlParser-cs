namespace SqlParser.Ast;

/// <summary>
/// Framed boundary
/// </summary>
/// <param name="Units">Window unit flag</param>
/// <param name="StartBound">Boundary start</param>
/// <param name="EndBound">Boundary end</param>
public class WindowFrame(WindowFrameUnit Units, WindowFrameBound? StartBound, WindowFrameBound? EndBound) : IElement;

public abstract class WindowFrameBound : IWriteSql, IElement
{
    public class CurrentRow : WindowFrameBound
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("CURRENT ROW");
        }
    }
    public class Preceding(Expression? Expression) : WindowFrameBound
    {
        public override void ToSql(SqlTextWriter writer)
        {
            if (Expression == null)
            {
                writer.Write("UNBOUNDED PRECEDING");
            }
            else
            {
                writer.WriteSql($"{Expression} PRECEDING");
            }
        }
    }
    public class Following(Expression? Expression) : WindowFrameBound
    {
        public override void ToSql(SqlTextWriter writer)
        {
            if (Expression == null)
            {
                writer.Write("UNBOUNDED FOLLOWING");
            }
            else
            {
                writer.WriteSql($"{Expression} FOLLOWING");
            }
        }
    }

    public abstract void ToSql(SqlTextWriter writer);
}

public class WindowSpec(Sequence<Expression>? PartitionBy = null, Sequence<OrderByExpression>? OrderBy = null, WindowFrame? WindowFrame = null) : IWriteSql, IElement
{
    public void ToSql(SqlTextWriter writer)
    {
        var delimiter = string.Empty;
        if (PartitionBy.SafeAny())
        {
            delimiter = " ";
            writer.WriteSql($"PARTITION BY {PartitionBy}");
        }

        if (OrderBy != null)
        {
            writer.Write(delimiter);
            delimiter = " ";
            writer.WriteSql($"ORDER BY {OrderBy}");
        }

        if (WindowFrame != null)
        {
            writer.Write(delimiter);
            if (WindowFrame.EndBound != null)
            {
                writer.WriteSql($"{WindowFrame.Units} BETWEEN {WindowFrame.StartBound} AND {WindowFrame.EndBound}");
            }
            else
            {
                writer.WriteSql($"{WindowFrame.Units} {WindowFrame.StartBound}");
            }
        }
    }
}

public class NamedWindowDefinition(Ident Name, WindowSpec WindowSpec) : IWriteSql
{
    public void ToSql(SqlTextWriter writer)
    {
        writer.WriteSql($"{Name} as ({WindowSpec})");
    }
}