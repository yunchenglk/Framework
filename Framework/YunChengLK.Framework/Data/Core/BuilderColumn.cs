using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace YunChengLK.Framework.Data.Core
{
    internal class BuilderColumn<T> : ExpressionVisitor where T : class, new()
    {
        private Stack<string> m_colArgs = new Stack<string>();
        private Stack<string> m_colName = new Stack<string>();
        private Stack<object> m_values = new Stack<object>();

        internal object[] Values { get; private set; }
        internal string[] ColumnArgument { get; private set; }
        internal string[] Columns { get; set; }

        internal void Build(Expression expression)
        {
            PartialEvaluator evaluator = new PartialEvaluator();
            Expression evaluatedExpression = evaluator.Eval(expression);

            this.Visit(evaluatedExpression);

            this.ColumnArgument = m_colArgs.ToArray();
            this.Columns = m_colName.ToArray();
            this.Values = this.m_values.ToArray();
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c != null)
            {
                this.m_values.Push(c.Value);
            }
            return c;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m != null)
            {
                PropertyInfo propertyInfo = m.Member as PropertyInfo;
                if (propertyInfo == null) return m;
                this.m_colName.Push(Entity<T>.Properties[propertyInfo.Name].ColumnName);
                this.m_colArgs.Push(string.Format("{0} = @{0}",Entity<T>.Properties[propertyInfo.Name].ColumnName));
            }
            return m;
        }
    }
}