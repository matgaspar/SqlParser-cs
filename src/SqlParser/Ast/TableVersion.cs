namespace SqlParser.Ast;

public abstract class TableVersion : IWriteSql
{
    public class ForSystemTimeAsOf(Expression Expression) : TableVersion
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($" FOR SYSTEM_TIME AS OF {Expression}");
        }
    }

    public abstract void ToSql(SqlTextWriter writer);
}