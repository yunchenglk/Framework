using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace YunChengLK.Framework.Data.Core
{
    [Serializable]
    public abstract class BaseDatabase : IDatabase
    {
        public virtual string SqlText { get; set; }
        public virtual bool PrintSql { get; set; }
        public virtual int ExecutionTime { get; set; }
        public virtual bool OpenMonitor { get; set; }

        private string _businessName = string.Empty;
        public string BsinessName
        {
            get
            {
                return _businessName;
            }
            set
            {
                _businessName = value ?? string.Empty;
            }
        }

        private Stopwatch _moniter = null;
        public Stopwatch Moniter
        {
            get
            {
                if (_moniter == null)
                    _moniter = new Stopwatch();
                return _moniter;
            }
        }

        

        #region Parmeters

        protected DbParameter[] ConvertParameters<T>(string[] columns, object[] valus)
        {
            if ((columns == null || valus == null) || (columns.Length != valus.Length)) return null;
            DbParameter[] parms = new DbParameter[columns.Length];
            DbParameter parm = null;
            for (int i = 0; i < columns.Length; i++)
            {
                parm = this._command.CreateParameter();
                parm.ParameterName = "@" + columns[i];
                parm.Value = valus[i];
                parm.DbType = Entity<T>.Properties[columns[i]].ColumnType;
                parms[i] = parm;
            }
            return parms;
        }

        protected void ConvertBatchParameters<T>(T model, ref DbParameter[] parameters, ref StringBuilder scriptBlock, int index)
        {
            if (index >= 1) scriptBlock.AppendLine(" UNION ALL ");
            parameters = new DbParameter[Entity<T>.ColumnsPropertyWithOutReadony.Length];
            DbParameter parm = null;
            for (int i = 0; i < Entity<T>.ColumnsPropertyWithOutReadony.Length; i++)
            {
                parm = this._command.CreateParameter();
                parm.Value = Entity<T>.ColumnsPropertyWithOutReadony[i].GetValue(model, null) != null ? Entity<T>.ColumnsPropertyWithOutReadony[i].GetValue(model, null) : DBNull.Value;
                parm.ParameterName = Entity<T>.Properties[Entity<T>.ColumnsPropertyWithOutReadony[i].Name].ParameterName + index.ToString();
                parm.DbType = Entity<T>.Properties[Entity<T>.ColumnsPropertyWithOutReadony[i].Name].ColumnType;
                parameters[i] = parm;
            }
            scriptBlock.AppendLine(string.Format("SELECT {0}", string.Join(",", parameters.Select(s=>s.ParameterName))));
        }

        protected void ConvertParameters<T>(T model, ref DbParameter[] parameters) where T : class, IEntity, new()
        {
            parameters = new DbParameter[Entity<T>.ColumnsPropertyWithOutReadony.Length];
            DbParameter parm = null;
            for (int i = 0; i < Entity<T>.ColumnsPropertyWithOutReadony.Length; i++)
            {
                parm = this._command.CreateParameter();
                parm.Value = Entity<T>.ColumnsPropertyWithOutReadony[i].GetValue(model, null) != null ? Entity<T>.ColumnsPropertyWithOutReadony[i].GetValue(model, null) : DBNull.Value;
                parm.ParameterName = Entity<T>.Properties[Entity<T>.ColumnsPropertyWithOutReadony[i].Name].ParameterName;
                parm.DbType = Entity<T>.Properties[Entity<T>.ColumnsPropertyWithOutReadony[i].Name].ColumnType;
                parameters[i] = parm;
            }

            //foreach (PropertyInfo item in Entity<T>.ColumnsWithOutReadony)
            //{
            //    DbParameter parm = this.Command.CreateParameter();
            //    parm.Value = item.GetValue(model, null) != null ? item.GetValue(model, null) : DBNull.Value;
            //    //parm.Value = PropertyAccessor<T>.Get(model, item.Name);
            //    //parm.Value = FastReflectionCaches.PropertyAccessorCache.Get(item).GetValue(model);
            //    parm.ParameterName = Entity<T>.Properties[item.Name].ParameterName;
            //    parm.DbType = Entity<T>.Properties[item.Name].ColumnType;
            //    yield return parm;
            //}
        }

        protected void ConvertOrderBy<T>(Expression<Predicate<T>> order, ref string orderBlock) where T : class, IEntity, new()
        {
            if (order != null)
            {
                BuilderOrderby<T> orderbyBuilder = new BuilderOrderby<T>();
                orderbyBuilder.Build(order);
                orderBlock = orderbyBuilder.OrderBy;
            }
        }

        protected void ConvertWhere<T>(Expression<Predicate<T>> where, ref string whereBolck, ref DbParameter[] whereParameters) where T : class, IEntity, new()
        {
            if (where == null) return;
            
            BuilderWhere<T> whereBuilder = new BuilderWhere<T>();
            whereBuilder.Build(where.Body);
            whereBolck = whereBuilder.Where;

            DbParameter parm = null;
            whereParameters = new DbParameter[whereBuilder.Arguments.Length];
            for (int i = 0; i < whereBuilder.Arguments.Length; i++)
            {
                parm = this._command.CreateParameter();
                parm.ParameterName = string.Format(BuilderWhere<T>.ArgumetPrefix, i);
                parm.Value = whereBuilder.Arguments[i];
                whereParameters[i] = parm;
            }
        }
        
        #endregion

        #region Connecton
        private DbTransaction _transaction = null;
        public bool RollBack { get; set; }

        private DbCommand _command = null;
        private DbCommand commandProvider(string script, DbParameter[] parameters1, DbParameter[] parameters2 = null)
        {
            this._command.Parameters.Clear();
            this._command.Connection = this._command.Connection ?? this._connection;
            this._command.Transaction = this._transaction;
            this._command.CommandText = script;
            this._command.Parameters.Clear();
            if (parameters1 != null && parameters1.Length >= 1)
            {
                this._command.Parameters.AddRange(parameters1);
            }
            if (parameters2 != null && parameters2.Length >= 1)
            {
                this._command.Parameters.AddRange(parameters2);
            }
            return _command;
        }

        private DbConnection _connection = null;
        public DbConnection Connection
        {
            get
            {
                return this._connection;
            }
            set
            {
                this._connection = value;
                this._command = this._connection.CreateCommand();
            }
        }

        public virtual void Open()
        {
            try
            {
                this.Connection.Open();
            }
            catch
            {
                throw new Exception(this.BsinessName + " database open failure.");
            }
        }

        public virtual void Close()
        {
            this.Connection.Close();
        }

        public virtual bool TestConnection()
        { 
            bool pass = false;
            try
            {
                this.Connection.Open();
                this.Connection.Close();
                pass = true;
            }
            catch { }

            return pass;
        }

        #endregion

        #region Execute

        public virtual void Execute(Action action)
        {
            //Moniter.Restart();
            try
            {
                Connection.Open();
                action();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Connection.Close();
            }
            //Moniter.Stop();
            //Console.WriteLine(string.Format("{0}[{1}ss]", this.BsinessName, Moniter.ElapsedMilliseconds));
        }

        public virtual bool ExecuteTransaction(Action action)
        {
            bool scuress = false;
            //Moniter.Restart();
            try
            {
                Connection.Open();
                this._transaction = Connection.BeginTransaction(IsolationLevel.ReadCommitted);
                action();
                if (this.RollBack)
                {
                    this._transaction.Rollback();
                }
                else
                {
                    this._transaction.Commit();
                    scuress = true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Connection.Close();
            }
            //Moniter.Stop();
            //Console.WriteLine(string.Format("{0}[{1}ss]", this.BsinessName, Moniter.ElapsedMilliseconds));
            return scuress;
        }

        public virtual int ExecuteNonQuery(string script)
        {
            return this.ExecuteNonQuery(script, null);
        }

        public virtual int ExecuteNonQuery(string script, DbParameter[] parameters)
        {
            DbCommand cmd = commandProvider(script, parameters);
            int rows = cmd.ExecuteNonQuery();
            return rows;
        }

        public virtual int ExecuteNonQuery(string script, DbParameter[] parameters1, DbParameter[] parmmeters2 = null)
        {
            DbCommand cmd = commandProvider(script, parameters1, parmmeters2);
            int rows = cmd.ExecuteNonQuery();
            return rows;
        }

        public virtual R ExecuteScalar<R>(string script)
        {
            return ExecuteScalar<R>(script, null);
        }

        public virtual R ExecuteScalar<R>(string script, DbParameter[] parameters)
        {
            DbCommand cmd = commandProvider(script, parameters);
            R obj = (R)cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return obj;
        }

        public virtual IEnumerable<T> ExecuteList<T>(string script) where T : class, IEntity, new()
        {
            return ExecuteList<T>(script, null);
        }

        public virtual IEnumerable<T> ExecuteList<T>(string script, DbParameter[] parameters) where T : class, IEntity, new()
        {
            DbCommand cmd = commandProvider(script, parameters);
            DbDataReader reader = cmd.ExecuteReader();
            var list = reader.ToList<T>();
            reader.Close();
            cmd.Parameters.Clear();
            return list;
        }

        public virtual ArrayList ExecuteList<T1, T2>(string script, DbParameter[] parameters)
        {
            ArrayList list = new ArrayList();
            DbCommand cmd = commandProvider(script, parameters);
            DbDataReader reader = cmd.ExecuteReader();
            list.Add(reader.ToListWithoutCheck<T1>());
            if (reader.NextResult()) list.Add(reader.ToListWithoutCheck<T2>());
            reader.Close();
            cmd.Parameters.Clear();
            return list;
        }

        public virtual ArrayList ExecuteList<T1, T2, T3>(string script, DbParameter[] parameters) 
        {
            ArrayList list = new ArrayList();
            DbCommand cmd = commandProvider(script, parameters);
            DbDataReader reader = cmd.ExecuteReader();
            list.Add(reader.ToListWithoutCheck<T1>());
            if (reader.NextResult()) list.Add(reader.ToListWithoutCheck<T2>());
            if (reader.NextResult()) list.Add(reader.ToListWithoutCheck<T3>());
            reader.Close();
            cmd.Parameters.Clear();
            return list;
        }

        #endregion

        #region CRUD

        public virtual int Insert<T>(T model) where T : class, IEntity, new()
        {
            throw new Exception("BaseDatabase.Add<T>(T model)方法未实现");
        }

        public virtual int Insert<T>(IEnumerable<T> list) where T : class, IEntity, new()
        {
            throw new Exception("Insert<T>(IEnumerable<T> list)方法未实现");
        }

        public virtual int Delete<T>(Expression<Predicate<T>> where) where T : class, IEntity, new()
        {
            throw new Exception("BaseDatabase.Delete<T>(Expression<Predicate<T>> where) 方法未实现");
        }

        public virtual int Update<T>(T model, Expression<Predicate<T>> where) where T : class, IEntity, new()
        {
            throw new Exception("BaseDatabase.Update<T>(T model, Expression<Predicate<T>> where)方法未实现");
        }

        public virtual int Update<T>(Expression<Predicate<T>> columns, Expression<Predicate<T>> where) where T : class, IEntity, new()
        {
            throw new Exception("BaseDatabase.Update<T>(Expression<Predicate<T>> columns, Expression<Predicate<T>> where)方法未实现");
        }

        public virtual IEnumerable<T> Top<T>(int top, Expression<Predicate<T>> where, Expression<Predicate<T>> order) where T : class, IEntity, new()
        {
            throw new Exception("BaseDatabase.Top<T>(int top, Expression<Predicate<T>> where, Expression<Predicate<T>> order)方法未实现");
        }

        public virtual T Single<T>(Expression<Predicate<T>> where) where T : class, IEntity, new()
        {
            throw new Exception("BaseDatabase.Single<T>(Expression<Predicate<T>> where)方法未实现");
        }

        public virtual IEnumerable<T> GetList<T>() where T : class, IEntity, new()
        {
            return this.GetList<T>(null, null);
        }

        public virtual IEnumerable<T> GetList<T>(Expression<Predicate<T>> where) where T : class, IEntity, new()
        {
            return this.GetList(where, null);
        }

        public virtual IEnumerable<T> GetList<T>(Expression<Predicate<T>> where, Expression<Predicate<T>> order) where T : class, IEntity, new()
        {
            throw new Exception("BaseDatabase.GetList<T>(Expression<Predicate<T>> where, Expression<Predicate<T>>方法未实现");
        }

        public virtual IEnumerable<T> GetList<T>(Expression<Predicate<T>> where, Expression<Predicate<T>> order, int pageIndex, int pageSize) where T : class, IEntity, new()
        {
            throw new Exception("BaseDatabase.GetList<T>(Expression<Predicate<T>> where, Expression<Predicate<T>> order, int pageIndex, int pageSize)方法未实现");
        }

        public virtual int RowCount<T>(Expression<Predicate<T>> where) where T : class, IEntity, new()
        {
            throw new Exception("BaseDatabase.GetList<T>(Expression<Predicate<T>> where)方法未实现");
        }

        #endregion
    }
}
