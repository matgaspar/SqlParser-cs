namespace SqlParser.Ast;

public abstract class DistinctFilter : IWriteSql
{
    public class Distinct : DistinctFilter
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("DISTINCT");
        }
    }

    public class On(Sequence<Expression> ColumnNames) : DistinctFilter
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.Write("DISTINCT ON (");
            writer.WriteDelimited(ColumnNames, ", ");
            writer.Write(")");
        }
    }

    public virtual void ToSql(SqlTextWriter writer) { }
}
