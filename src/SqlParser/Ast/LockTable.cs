﻿namespace SqlParser.Ast;

public class LockTable(Ident Table, Ident? Alias, LockTableType LockTableType) : IWriteSql
{
    public void ToSql(SqlTextWriter writer)
    {
        writer.WriteSql($"{Table} ");

        if (Alias != null)
        {
            writer.WriteSql($"AS {Alias} ");
        }

        writer.WriteSql($"{LockTableType}");
    }
}

public abstract class LockTableType : IWriteSql
{
    public class Read(bool Local) : LockTableType;
    public class Write(bool LowPriority) : LockTableType;

    public void ToSql(SqlTextWriter writer)
    {
        switch (this)
        {
            case Read r:
                writer.Write("READ");

                if (r.Local)
                {
                    writer.Write(" LOCAL");
                }

                break;
            case Write w:

                if (w.LowPriority)
                {
                    writer.Write("LOW_PRIORITY ");
                }

                writer.Write("WRITE");

                break;
        }
    }
}
