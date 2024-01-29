namespace SqlParser.Ast;

/// <summary>
/// Do Update action
/// </summary>
/// <param name="Assignments">Update assignments</param>
/// <param name="Selection">Selection expression</param>
public class DoUpdateAction(Sequence<Statement.Assignment> Assignments, Expression? Selection = null) : IElement;