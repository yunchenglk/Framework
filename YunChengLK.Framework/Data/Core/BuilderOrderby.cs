using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace YunChengLK.Framework.Data.Core
{
    internal class BuilderOrderby<T> : ExpressionVisitor where T : class, new()
    {
        private Stack<string> m_orderbyParts;
        private string orderBy = null;

        internal string OrderBy
        {
            get
            {
                if (string.IsNullOrEmpty(this.orderBy)) return string.Empty;
                return string.Format("ORDER BY {0}", this.orderBy);
            }
        }

        internal void Build(Expression expression)
        {
            PartialEvaluator evaluator = new PartialEvaluator();
            Expression evaluatedExpression = evaluator.Eval(expression);
            this.m_orderbyParts = new Stack<string>();
            this.Visit(evaluatedExpression);
            if (this.m_orderbyParts.Count > 0)
            {
                this.orderBy = string.Join(", ", this.m_orderbyParts.Reverse().ToArray());
            }
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m != null)
            {
                PropertyInfo propertyInfo = m.Member as PropertyInfo;
                if (propertyInfo == null) return m;

                this.m_orderbyParts.Push(Entity<T>.Properties[propertyInfo.Name].ColumnName);
            }
            return m;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m != null)
            {
                base.VisitMethodCall(m);

                string strFildName = this.m_orderbyParts.Pop();

                if (m.Method.Name == "Ascending")
                {
                    this.m_orderbyParts.Push(string.Format("{0} ASC", strFildName));
                }
                else if (m.Method.Name == "Descending")
                {
                    this.m_orderbyParts.Push(string.Format("{0} DESC", strFildName));
                }
            }
            return m;
        }
    }
}
