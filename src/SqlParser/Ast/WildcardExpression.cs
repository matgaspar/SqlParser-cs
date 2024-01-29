namespace SqlParser.Ast;

/// <summary>
/// Wildcard expressions
/// </summary>
public abstract class WildcardExpression : Expression
{
    /// <summary>
    /// Expression
    /// </summary>
    /// <param name="Expression">Expression</param>
    public class Expr(Expression Expression) : WildcardExpression, IElement;
    /// <summary>
    /// Qualified expression
    /// </summary>
    /// <param name="Name">Object name</param>
    public class QualifiedWildcard(ObjectName Name) : WildcardExpression, IElement;
    /// <summary>
    /// Wildcard expression
    /// </summary>
    public class Wildcard : WildcardExpression;

    public static implicit operator FunctionArgExpression(WildcardExpression expr)
    {
        return expr switch
        {
            Expr e => new FunctionArgExpression.FunctionExpression(e.Expression),
            QualifiedWildcard q => new FunctionArgExpression.QualifiedWildcard(q.Name),
            _ => new FunctionArgExpression.Wildcard()
        };
    }
}