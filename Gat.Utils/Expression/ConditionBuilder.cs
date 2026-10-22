
#region 文件头描述信息

/* Copyright (C) 2011 WE小神工作室版权所有
 * 文件名：ConditionBuilder.cs
 * 文件功能描述：1. 拼接条件
 * 
 * 创建标识：赵劼，网名老赵，洋名Jeffrey Zhao，
 * 未解决的：
 * 
 * 
 * 修改标识：
 * 修改描述：
 * 使用描述：
 */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Gat.Utils.Expression
{
    internal class ConditionBuilder : ExpressionVisitor
    {
        private List<object> m_arguments;
        private Stack<string> m_conditionParts;

        public string Condition { get; private set; }

        public object[] Arguments { get; private set; }

        public void Build(System.Linq.Expressions.Expression expression)
        {
            PartialEvaluator evaluator = new PartialEvaluator();
            System.Linq.Expressions.Expression evaluatedExpression = evaluator.Eval(expression);

            this.m_arguments = new List<object>();
            this.m_conditionParts = new Stack<string>();

            this.Visit(evaluatedExpression);

            this.Arguments = this.m_arguments.ToArray();
            this.Condition = this.m_conditionParts.Count > 0 ? this.m_conditionParts.Pop() : null;
        }

        protected override System.Linq.Expressions.Expression VisitBinary(BinaryExpression b)
        {
            if (b == null) return b;

            string opr;
            switch (b.NodeType)
            {
                case ExpressionType.Equal:
                    opr = "=";
                    break;
                case ExpressionType.NotEqual:
                    opr = "<>";
                    break;
                case ExpressionType.GreaterThan:
                    opr = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    opr = ">=";
                    break;
                case ExpressionType.LessThan:
                    opr = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    opr = "<=";
                    break;
                case ExpressionType.AndAlso:
                    opr = "AND";
                    break;
                case ExpressionType.OrElse:
                    opr = "OR";
                    break;
                case ExpressionType.Add:
                    opr = "+";
                    break;
                case ExpressionType.Subtract:
                    opr = "-";
                    break;
                case ExpressionType.Multiply:
                    opr = "*";
                    break;
                case ExpressionType.Divide:
                    opr = "/";
                    break;
                default:
                    throw new NotSupportedException(b.NodeType + "is not supported.");
            }

            this.Visit(b.Left);
            this.Visit(b.Right);

            string right = this.m_conditionParts.Pop();
            string left = this.m_conditionParts.Pop();

            string condition = String.Format("({0} {1} {2})", left, opr, right);
            this.m_conditionParts.Push(condition);

            return b;
        }

        protected override System.Linq.Expressions.Expression VisitConstant(ConstantExpression c)
        {
            if (c == null) return c;

            this.m_arguments.Add(c.Value);
            this.m_conditionParts.Push(String.Format("{{{0}}}", this.m_arguments.Count - 1));

            return c;
        }

        protected System.Linq.Expressions.Expression VisitMemberAccess(MemberExpression m)
        {
            if (m == null) return m;

            PropertyInfo propertyInfo = m.Member as PropertyInfo;
            if (propertyInfo == null) return m;

            this.m_conditionParts.Push(String.Format("[{0}]", propertyInfo.Name));

            return m;
        }
    }
}
