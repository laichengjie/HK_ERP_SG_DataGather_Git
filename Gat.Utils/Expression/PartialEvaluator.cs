using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Gat.Utils.Expression
{
    public class PartialEvaluator : ExpressionVisitor
    {
        private Func<System.Linq.Expressions.Expression, bool> m_fnCanBeEvaluated;
        private HashSet<System.Linq.Expressions.Expression> m_candidates;

        public PartialEvaluator()
            : this(CanBeEvaluatedLocally)
        { }

        public PartialEvaluator(Func<System.Linq.Expressions.Expression, bool> fnCanBeEvaluated)
        {
            this.m_fnCanBeEvaluated = fnCanBeEvaluated;
        }

        public System.Linq.Expressions.Expression Eval(System.Linq.Expressions.Expression exp)
        {
            this.m_candidates = new Nominator(this.m_fnCanBeEvaluated).Nominate(exp);

            return this.Visit(exp);
        }

        public override System.Linq.Expressions.Expression Visit(System.Linq.Expressions.Expression exp)
        {
            if (exp == null)
            {
                return null;
            }

            if (this.m_candidates.Contains(exp))
            {
                return this.Evaluate(exp);
            }

            return base.Visit(exp);
        }

        private System.Linq.Expressions.Expression Evaluate(System.Linq.Expressions.Expression e)
        {
            if (e.NodeType == ExpressionType.Constant)
            {
                return e;
            }

            LambdaExpression lambda = System.Linq.Expressions.Expression.Lambda(e);
            Delegate fn = lambda.Compile();

            return System.Linq.Expressions.Expression.Constant(fn.DynamicInvoke(null), e.Type);
        }

        private static bool CanBeEvaluatedLocally(System.Linq.Expressions.Expression exp)
        {
            return exp.NodeType != ExpressionType.Parameter;
        }

        #region Nominator

        /// <summary>
        /// Performs bottom-up analysis to determine which nodes can possibly
        /// be part of an evaluated sub-tree.
        /// </summary>
        private class Nominator : ExpressionVisitor
        {
            private Func<System.Linq.Expressions.Expression, bool> m_fnCanBeEvaluated;
            private HashSet<System.Linq.Expressions.Expression> m_candidates;
            private bool m_cannotBeEvaluated;

            internal Nominator(Func<System.Linq.Expressions.Expression, bool> fnCanBeEvaluated)
            {
                this.m_fnCanBeEvaluated = fnCanBeEvaluated;
            }

            internal HashSet<System.Linq.Expressions.Expression> Nominate(System.Linq.Expressions.Expression expression)
            {
                this.m_candidates = new HashSet<System.Linq.Expressions.Expression>();
                this.Visit(expression);
                return this.m_candidates;
            }

            public override System.Linq.Expressions.Expression Visit(System.Linq.Expressions.Expression expression)
            {
                if (expression != null)
                {
                    bool saveCannotBeEvaluated = this.m_cannotBeEvaluated;
                    this.m_cannotBeEvaluated = false;

                    base.Visit(expression);

                    if (!this.m_cannotBeEvaluated)
                    {
                        if (this.m_fnCanBeEvaluated(expression))
                        {
                            this.m_candidates.Add(expression);
                        }
                        else
                        {
                            this.m_cannotBeEvaluated = true;
                        }
                    }

                    this.m_cannotBeEvaluated |= saveCannotBeEvaluated;
                }

                return expression;
            }
        }

        #endregion
    }
}
