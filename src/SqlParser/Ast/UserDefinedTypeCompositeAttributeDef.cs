﻿namespace SqlParser.Ast;

public class UserDefinedTypeCompositeAttributeDef(Ident Name, DataType DataType, ObjectName? Collation = null) : IWriteSql
{
    public void ToSql(SqlTextWriter writer)
    {
        writer.WriteSql($"{Name} {DataType}");

        if (Collation != null)
        {
            writer.WriteSql($" COLLATE {Collation}");
        }
    }
}
