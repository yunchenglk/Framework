using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using YunChengLK.Framework.Data.Core;

namespace YunChengLK.Framework.Data
{
    [Serializable]
    public class SqlServer : BaseDatabase
    {
        private string _connectionString = null;
        private string _mongoConnectionstr = null;
        public SqlServer(string connectionString)
        {
            this._connectionString = connectionString;
            this.Connection = new SqlConnection(connectionString);
        }
        public SqlServer(string connectionString, string mongoDB)
        {
            if (!string.IsNullOrEmpty(mongoDB))
            {
                _mongoConnectionstr = mongoDB;
            }
            this._connectionString = connectionString;
            this.Connection = new SqlConnection(connectionString);
        }


        public override int Insert<T>(T model)
        {
            DbParameter[] parms = null;
            this.ConvertParameters<T>(model, ref parms);
            int result = this.ExecuteNonQuery(SqlServerLanguage<T>.InsertScript, parms);
            if (!string.IsNullOrEmpty(_mongoConnectionstr))
                new MGServer<T>(_mongoConnectionstr, this.Connection.Database).Save(model);
            return result;
        }

        public override int Insert<T>(IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0) throw new ArgumentNullException("Insert<T>(IEnumerable<T> list) list is not null.");

            DbParameter[] parmsAll = new SqlParameter[0];
            DbParameter[] parms = null;
            StringBuilder scriptBlock = new StringBuilder();
            scriptBlock.AppendLine(SqlServerLanguage<T>.InsertBatchScript);
            int index = 0;

            foreach (T item in list)
            {
                this.ConvertBatchParameters<T>(item, ref parms, ref scriptBlock, index++);
                parmsAll = parmsAll.Concat(parms).ToArray();
            }

            return this.ExecuteNonQuery(scriptBlock.ToString(), parmsAll);
        }

        public override int Delete<T>(Expression<Predicate<T>> where)
        {
            //防止无条件全部删除
            if (where == null) throw new ArgumentNullException("Delete<T>(Expression<Predicate<T>> where) Where is not null.");

            DbParameter[] whereparms = null;
            string whereblock = string.Empty;

            this.ConvertWhere<T>(where, ref whereblock, ref whereparms);
            if (string.IsNullOrEmpty(whereblock) || whereparms == null) return 0;//防止无条件删除
            return this.ExecuteNonQuery(SqlServerLanguage<T>.DeleteScript + whereblock, whereparms);
        }

        public override int Update<T>(T model, Expression<Predicate<T>> where)
        {
            //防止无条件全部更新
            if (model == null || where == null) throw new ArgumentNullException("Update<T>(T model, Expression<Predicate<T>> where) Model and Where is not null.");

            DbParameter[] parms = null;
            DbParameter[] whereparms = null;
            string whereblock = string.Empty;

            this.ConvertParameters<T>(model, ref parms);
            this.ConvertWhere<T>(where, ref whereblock, ref whereparms);
            if (string.IsNullOrEmpty(whereblock) || whereparms == null) return 0;//防止无条件删除
            return this.ExecuteNonQuery(SqlServerLanguage<T>.UpdateScript + whereblock, parms, whereparms);
        }

        public override int Update<T>(Expression<Predicate<T>> columns, Expression<Predicate<T>> where)
        {
            //防止无条件全部更新
            if (columns == null || where == null) throw new ArgumentNullException("Update<T>(Expression<Predicate<T>> columns, Expression<Predicate<T>> where) Columns and Where is not null.");

            BuilderColumn<T> columnsBuilder = new BuilderColumn<T>();
            columnsBuilder.Build(columns);
            string columnsName = string.Join(", ", columnsBuilder.ColumnArgument);

            DbParameter[] parms = null;
            DbParameter[] whereparms = null;
            string whereblock = string.Empty;

            parms = this.ConvertParameters<T>(columnsBuilder.Columns, columnsBuilder.Values);
            this.ConvertWhere(where, ref whereblock, ref whereparms);
            string script = string.Format(SqlServerLanguage<T>.UpdateColmunsScript, columnsName, whereblock);
            return this.ExecuteNonQuery(script, parms, whereparms);
        }

        public override IEnumerable<T> Top<T>(int top, Expression<Predicate<T>> where = null, Expression<Predicate<T>> order = null)
        {
            DbParameter[] whereparms = null;
            string whereblock = string.Empty;
            string orderblock = string.Empty;

            this.ConvertWhere(where, ref whereblock, ref whereparms);
            this.ConvertOrderBy<T>(order, ref orderblock);

            string script = string.Format(SqlServerLanguage<T>.TopScript, top.ToString(), whereblock, orderblock);
            return this.ExecuteList<T>(script, whereparms);
        }

        public override T Single<T>(Expression<Predicate<T>> where)
        {
            //没有条件抛出，防止全表结果返回。
            if (where == null) throw new ArgumentNullException("Single<T>(Expression<Predicate<T>> where) Where is not null.");

            DbParameter[] whereparms = null;
            string whereblock = string.Empty;
            this.ConvertWhere(where, ref whereblock, ref whereparms);
            string script = string.Format(SqlServerLanguage<T>.SingleScript, whereblock);
            IEnumerable<T> list = this.ExecuteList<T>(script, whereparms);
            return list.Count() >= 1 ? list.First() : null;
        }

        public override IEnumerable<T> GetList<T>(Expression<Predicate<T>> where = null, Expression<Predicate<T>> order = null)
        {
            DbParameter[] whereparms = null;
            string whereblock = string.Empty;
            string orderblock = string.Empty;

            this.ConvertWhere(where, ref whereblock, ref whereparms);
            this.ConvertOrderBy<T>(order, ref orderblock);

            string script = string.Format(SqlServerLanguage<T>.SelectScript, whereblock, orderblock);
            return this.ExecuteList<T>(script, whereparms);
        }

        public override IEnumerable<T> GetList<T>(Expression<Predicate<T>> where, Expression<Predicate<T>> order, int pageIndex, int pageSize)
        {
            //防止无条件全部更新
            if (order == null) throw new ArgumentNullException("GetList<T>(Expression<Predicate<T>> where, Expression<Predicate<T>> order, int pageIndex, int pageSize) Order is not null.");

            DbParameter[] whereparms = null;
            string whereblock = string.Empty;
            string orderblock = string.Empty;

            this.ConvertWhere(where, ref whereblock, ref whereparms);
            this.ConvertOrderBy<T>(order, ref orderblock);

            int page = pageIndex;
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 1;
            pageIndex = pageSize * (pageIndex - 1);
            pageSize = pageSize * page;

            string script = string.Format(SqlServerLanguage<T>.SelectPaddingScript, whereblock, orderblock, pageIndex, pageSize);
            return this.ExecuteList<T>(script, whereparms);
        }

        public override int RowCount<T>(Expression<Predicate<T>> where)
        {
            DbParameter[] whereparms = null;
            string whereblock = string.Empty;
            this.ConvertWhere(where, ref whereblock, ref whereparms);
            return this.ExecuteScalar<int>(SqlServerLanguage<T>.RowCountScript + whereblock, whereparms);
        }
    }
}
