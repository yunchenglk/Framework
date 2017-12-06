using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace YunChengLK.Framework.Data.Core
{
    internal class BuilderWhere<T> : ExpressionVisitor where T : class, new()
    {
        internal static readonly string ArgumetPrefix = "@Arg{0}";
        private List<object> m_arguments;
        private Stack<string> m_conditionParts;

        private string where = null;
        internal string Where
        {
            get
            {
                if (string.IsNullOrEmpty(this.where)) return string.Empty;
                return string.Format("WHERE {0}", this.where);
            }
        }

        internal object[] Arguments { get; private set; }

        internal void Build(Expression expression)
        {
            PartialEvaluator evaluator = new PartialEvaluator();
            Expression evaluatedExpression = evaluator.Eval(expression);
            this.m_arguments = new List<object>();
            this.m_conditionParts = new Stack<string>();
            this.Visit(evaluatedExpression);
            this.Arguments = this.m_arguments.ToArray();
            this.where = this.m_conditionParts.Count > 0 ? this.m_conditionParts.Pop() : null;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            if (b != null)
            {
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

                string condition = String.Format("({0}{1}{2})", left, opr, right);
                this.m_conditionParts.Push(condition);
            }

            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c != null)
            {
                string arg;
                ICollection items = c.Value as ICollection;
                if (items == null)
                {
                    this.m_arguments.Add(c.Value);
                    arg = string.Format(ArgumetPrefix, this.m_arguments.Count - 1);
                }
                else
                {
                    if (items.Count <= 0)
                    {
                        throw new Exception(string.Format("{0}类的Lambda通用操作中的存在'集合包含', 不允许该集合没有任何元素!", typeof(T).Name));
                    }

                    IList<string> args = new List<string>();

                    foreach (var item in items)
                    {
                        this.m_arguments.Add(item);
                        args.Add(string.Format(ArgumetPrefix, this.m_arguments.Count - 1));
                    }
                    arg = string.Join(", ", args.ToArray());
                }

                this.m_conditionParts.Push(arg);
            }
            return c;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m != null)
            {
                PropertyInfo propInfo = m.Member as PropertyInfo;
                if (propInfo == null) return m;
                this.m_conditionParts.Push(Entity<T>.Properties[propInfo.Name].ColumnName);
            }
            return m;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m != null)
            {
                base.VisitMethodCall(m);
                string opr = string.Empty;
                string left = string.Empty;
                string right = string.Empty;

                switch (m.Method.Name)
                {
                    case "Contains":
                        if (m.Method.DeclaringType == typeof(string) && m.Method.ReturnType == typeof(bool))
                        {
                            opr = "LIKE";
                            right = this.m_conditionParts.Pop();
                            left = this.m_conditionParts.Pop();
                            this.m_arguments[this.m_arguments.Count - 1] = string.Format("%{0}%", this.m_arguments[this.m_arguments.Count - 1] as string);
                        }
                        else if (m.Method.DeclaringType.GetInterface("IEnumerable") != null || m.Method.DeclaringType == typeof(Enumerable) || m.Method.DeclaringType == typeof(Guid))
                        {
                            opr = "IN";
                            left = this.m_conditionParts.Pop();
                            right = this.m_conditionParts.Pop();
                        }

                        if (this.m_conditionParts.Count > 0)
                        {
                            string tmp = this.m_conditionParts.Pop();
                            if (tmp == "NOT") opr = string.Format("{0} {1}", tmp, opr);
                            else this.m_conditionParts.Push(tmp);
                        }
                        this.m_conditionParts.Push(string.Format("({0} {1} ({2}))", left, opr, right));
                        break;
                    case "StartsWith":
                        opr = "LIKE";
                        right = this.m_conditionParts.Pop();
                        left = this.m_conditionParts.Pop();
                        this.m_arguments[this.m_arguments.Count - 1] = string.Format("{0}%", this.m_arguments[this.m_arguments.Count - 1] as string);
                        break;
                    case "EndsWith":
                        opr = "LIKE";
                        right = this.m_conditionParts.Pop();
                        left = this.m_conditionParts.Pop();
                        this.m_arguments[this.m_arguments.Count - 1] = string.Format("%{0}", this.m_arguments[this.m_arguments.Count - 1] as string);
                        break;
                }

                //if (m.Method.Name == "Contains")
                //{
                //    if (m.Method.DeclaringType == typeof(string) && m.Method.ReturnType == typeof(bool))
                //    {
                //        opr = "LIKE";
                //        right = this.m_conditionParts.Pop();
                //        left = this.m_conditionParts.Pop();
                //        string content = this.m_arguments[this.m_arguments.Count - 1] as string;
                //        this.m_arguments[this.m_arguments.Count - 1] = string.Format("%{0}%", content);
                //    }
                //    else if (m.Method.DeclaringType.GetInterface("IEnumerable") != null || m.Method.DeclaringType == typeof(Enumerable) || m.Method.DeclaringType == typeof(Guid))
                //    {
                //        opr = "IN";
                //        left = this.m_conditionParts.Pop();
                //        right = this.m_conditionParts.Pop();
                //    }

                //    if (this.m_conditionParts.Count > 0)
                //    {
                //        string tmp = this.m_conditionParts.Pop();
                //        if (tmp == "NOT") opr = string.Format("{0} {1}", tmp, opr); 
                //        else this.m_conditionParts.Push(tmp);
                //    }
                //    string condition = string.Format("({0} {1} ({2}))", left, opr, right);
                //    this.m_conditionParts.Push(condition);
                //}
                //else if (m.Method.Name == "StartsWith")
                //{
                //    opr = "LIKE";
                //    right = this.m_conditionParts.Pop();
                //    left = this.m_conditionParts.Pop();
                //    string content = this.m_arguments[this.m_arguments.Count - 1] as string;
                //    this.m_arguments[this.m_arguments.Count - 1] = string.Format("{0}%", content);
                //}
                //else if (m.Method.Name == "EndsWith")
                //{
                //    opr = "LIKE";
                //    right = this.m_conditionParts.Pop();
                //    left = this.m_conditionParts.Pop();
                //    string content = this.m_arguments[this.m_arguments.Count - 1] as string;
                //    this.m_arguments[this.m_arguments.Count - 1] = string.Format("%{0}", content);
                //}
            }
            return m;
        }

        /// <summary>
        /// 支持Contains函数的非操作,即Sql的 NOT LIKE 和 NOT IN
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        protected override Expression VisitUnary(UnaryExpression u)
        {
            if (u.NodeType == ExpressionType.Not)
            {
                this.m_conditionParts.Push("NOT");
            }
            return base.VisitUnary(u);
        }

        protected override Expression VisitConditional(ConditionalExpression c)
        {
            if (c != null)
            {
                if (c.Test.Type == typeof(bool))
                {
                    if (bool.Parse(c.Test.ToString()))
                    {
                        this.Visit(c.IfTrue);
                    }
                    else
                    {
                        this.Visit(c.IfFalse);
                    }
                }
                else
                {
                    base.VisitConditional(c);
                }
            }
            return c;
        }
    }
}
