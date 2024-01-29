namespace SqlParser.Ast;

/// <summary>
/// One item of the comma-separated list following SELECT
/// </summary>
public abstract class SelectItem : IWriteSql, IElement
{
    /// <summary>
    ///  Any expression, not followed by [ AS ] alias
    /// </summary>
    /// <param name="Expression">Select expression</param>
    public class UnnamedExpression(Expression Expression) : SelectItem
    {
        public override void ToSql(SqlTextWriter writer)
        {
            Expression.ToSql(writer);
        }
    }
    /// <summary>
    /// alias.* or even schema.table.*
    /// </summary>
    /// <param name="Name">Object name</param>
    /// <param name="WildcardAdditionalOptions">Select options</param>
    public class QualifiedWildcard(ObjectName Name, WildcardAdditionalOptions WildcardAdditionalOptions) : SelectItem
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"{Name}.*{WildcardAdditionalOptions}");
        }
    }
    /// <summary>
    /// An expression, followed by [ AS ] alias
    /// </summary>
    /// <param name="Expression">Select expression</param>
    /// <param name="Alias">Select alias</param>
    public class ExpressionWithAlias(Expression Expression, Ident Alias) : SelectItem
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"{Expression} AS {Alias}");
        }
    }
    /// <summary>
    /// An unqualified *
    /// </summary>
    /// <param name="WildcardAdditionalOptions"></param>
    public class Wildcard(WildcardAdditionalOptions WildcardAdditionalOptions) : SelectItem
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"*{WildcardAdditionalOptions}");
        }
    }

    public abstract void ToSql(SqlTextWriter writer);

    public T As<T>() where T : SelectItem
    {
        return (T) this;
    }

    public UnnamedExpression AsUnnamed()
    {
        return As<UnnamedExpression>();
    }

    public Wildcard AsWildcard()
    {
        return As<Wildcard>();
    }
}

/// <summary>
/// Excluded select item
/// </summary>
public abstract class ExcludeSelectItem : IWriteSql, IElement
{
    /// <summary>
    /// Single exclusion
    /// </summary>
    /// <param name="Name">Name identifier</param>
    public class Single(Ident Name) : ExcludeSelectItem
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"EXCLUDE {Name}");
        }
    }
    /// <summary>
    /// Multiple exclusions
    /// </summary>
    /// <param name="Columns">Name identifiers</param>
    public class Multiple(Sequence<Ident> Columns) : ExcludeSelectItem
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"EXCLUDE ({Columns})");
        }
    }

    public abstract void ToSql(SqlTextWriter writer);
}

/// <summary>
/// Rename select item
/// </summary>
public abstract class RenameSelectItem : IWriteSql, IElement
{
    /// <summary>
    /// Single rename
    /// </summary>
    /// <param name="Name">Name identifier</param>
    public class Single(IdentWithAlias Name) : RenameSelectItem
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"RENAME {Name}");
        }
    }
    /// <summary>
    /// Multiple exclusions
    /// </summary>
    /// <param name="Columns">Name identifiers</param>
    public class Multiple(Sequence<IdentWithAlias> Columns) : RenameSelectItem
    {
        public override void ToSql(SqlTextWriter writer)
        {
            writer.WriteSql($"RENAME ({Columns})");
        }
    }

    public abstract void ToSql(SqlTextWriter writer);
}

/// <summary>
/// Expected select item
/// </summary>
/// <param name="FirstElement">First item in the list</param>
/// <param name="AdditionalElements">Additional items</param>
public class ExceptSelectItem(Ident FirstElement, Sequence<Ident> AdditionalElements) : IWriteSql, IElement
{
    public void ToSql(SqlTextWriter writer)
    {
        writer.Write("EXCEPT ");

        if (AdditionalElements.SafeAny())
        {
            writer.WriteSql($"({FirstElement}, {AdditionalElements})");
        }
        else
        {
            writer.WriteSql($"({FirstElement})");
        }
    }
}
