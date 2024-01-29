namespace SqlParser.Ast
{
    public class Join : IWriteSql, IElement
    {
        public Join(TableFactor relation, JoinOperator joinOperator)
        {
            Relation = relation;
            JoinOperator = joinOperator;
        }

        public TableFactor Relation { get; }
        public JoinOperator JoinOperator { get; }

        public void ToSql(SqlTextWriter writer)
        {
            string Prefix(JoinConstraint constraint)
            {
                return constraint is JoinConstraint.Natural ? "NATURAL " : null;
            }

            string Suffix(JoinConstraint constraint)
            {
                if (constraint is JoinConstraint.On on)
                {
                    return $" ON {on.Expression.ToSql()}";
                }
            
                if (constraint is JoinConstraint.Using @using)
                {
                    return $" USING({ @using.Idents.ToSqlDelimited() })";
                }
                return null;
            }

            switch (JoinOperator)
            {
                case JoinOperator.CrossApply:
                    writer.WriteSql($" CROSS APPLY {Relation}");
                    return;
                case JoinOperator.OuterApply:
                    writer.WriteSql($" OUTER APPLY {Relation}");
                    return;
                case JoinOperator.CrossJoin:
                    writer.WriteSql($" CROSS JOIN {Relation}");
                    return;
            }

            string joinText = null!;
            JoinConstraint constraint = null!;
            switch (JoinOperator)
            {
                case JoinOperator.Inner inner:
                    joinText = "JOIN";
                    constraint = inner.JoinConstraint;
                    break;
                case JoinOperator.LeftOuter left:
                    joinText = "LEFT JOIN";
                    constraint = left.JoinConstraint;
                    break;
                case JoinOperator.RightOuter right:
                    joinText = "RIGHT JOIN";
                    constraint = right.JoinConstraint;
                    break;
                case JoinOperator.FullOuter full:
                    joinText = "FULL JOIN";
                    constraint = full.JoinConstraint;
                    break;
                case JoinOperator.LeftSemi leftSemi:
                    joinText = "LEFT SEMI JOIN";
                    constraint = leftSemi.JoinConstraint;
                    break;
                case JoinOperator.RightSemi rightSemi:
                    joinText = "RIGHT SEMI JOIN";
                    constraint = rightSemi.JoinConstraint;
                    break;
                case JoinOperator.LeftAnti leftAnti:
                    joinText = "LEFT ANTI JOIN";
                    constraint = leftAnti.JoinConstraint;
                    break;
                case JoinOperator.RightAnti rightAnti:
                    joinText = "RIGHT ANTI JOIN";
                    constraint = rightAnti.JoinConstraint;
                    break;
            }

            writer.WriteSql($" {Prefix(constraint)}{joinText} {Relation}{Suffix(constraint)}");
        }
    }

    /// <summary>
    /// Join operator
    /// </summary>
    public abstract class JoinOperator : IElement
    {
        public abstract class ConstrainedJoinOperator : JoinOperator
        {
            public JoinConstraint JoinConstraint { get; }

            protected ConstrainedJoinOperator(JoinConstraint joinConstraint)
            {
                JoinConstraint = joinConstraint;
            }
        }

        /// <summary>
        /// Inner join
        /// </summary>
        /// <param name="JoinConstraint">Join constraint</param>
        public class Inner(JoinConstraint JoinConstraint) : ConstrainedJoinOperator(JoinConstraint);
        /// <summary>
        /// Left join
        /// </summary>
        /// <param name="JoinConstraint">Join constraint</param>
        public class LeftOuter(JoinConstraint JoinConstraint) : ConstrainedJoinOperator(JoinConstraint);
        /// <summary>
        /// Right outer join
        /// </summary>
        /// <param name="JoinConstraint">Join constraint</param>
        public class RightOuter(JoinConstraint JoinConstraint) : ConstrainedJoinOperator(JoinConstraint);
        /// <summary>
        /// Full outer join
        /// </summary>
        /// <param name="JoinConstraint">Join constraint</param>
        public class FullOuter(JoinConstraint JoinConstraint) : ConstrainedJoinOperator(JoinConstraint);
        /// <summary>
        /// Cross join
        /// </summary>
        public class CrossJoin : JoinOperator;
        /// <summary>
        /// Left semi join
        /// </summary>
        /// <param name="JoinConstraint">Join constraint</param>
        public class LeftSemi(JoinConstraint JoinConstraint) : ConstrainedJoinOperator(JoinConstraint);
        /// <summary>
        /// Right semi join
        /// </summary>
        /// <param name="JoinConstraint">Join constraint</param>
        public class RightSemi(JoinConstraint JoinConstraint) : ConstrainedJoinOperator(JoinConstraint);
        /// <summary>
        /// Left anti join
        /// </summary>
        /// <param name="JoinConstraint">Join constraint</param>
        public class LeftAnti(JoinConstraint JoinConstraint) : ConstrainedJoinOperator(JoinConstraint);
        /// <summary>
        /// Right anti join
        /// </summary>
        /// <param name="JoinConstraint">Join constraint</param>
        public class RightAnti(JoinConstraint JoinConstraint) : ConstrainedJoinOperator(JoinConstraint);
        /// <summary>
        /// Cross apply join
        /// </summary>
        public class CrossApply : JoinOperator;
        /// <summary>
        /// 
        /// </summary>
        public class OuterApply : JoinOperator;
    }

    /// <summary>
    /// Join constraint
    /// </summary>
    public abstract class JoinConstraint: IElement
    {
        /// <summary>
        /// On join constraint
        /// </summary>
        /// <param name="Expression">Constraint expression</param>
        public class On(Expression Expression) : JoinConstraint;
        /// <summary>
        /// Using join constraint
        /// </summary>
        /// <param name="Idents">Name identifiers</param>
        public class Using(Sequence<Ident> Idents) : JoinConstraint;
        /// <summary>
        /// Natural join constraint
        /// </summary>
        public class Natural : JoinConstraint;
        /// <summary>
        /// No join constraint
        /// </summary>
        public class None : JoinConstraint;
    }
}