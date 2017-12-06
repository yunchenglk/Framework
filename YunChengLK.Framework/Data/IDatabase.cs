using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq.Expressions;

namespace YunChengLK.Framework.Data
{
    public interface IDatabase
    {
        string SqlText { get; set; }
        bool PrintSql { get; set; }
        bool OpenMonitor { get; set; }
        int ExecutionTime { get; set; }
        string BsinessName { get; set; }
        Stopwatch Moniter { get; }
        /// ////////////////////
        DbConnection Connection { get; set; }
        void Open();
        void Close();
        bool RollBack { get; set; }
        bool TestConnection();
        /// /////////////////////////////////////////////////////////
        int Insert<T>(T model) where T : class, IEntity, new();
        int Insert<T>(IEnumerable<T> list) where T : class, IEntity, new();
        int Delete<T>(Expression<Predicate<T>> where) where T : class, IEntity, new();
        int Update<T>(T model, Expression<Predicate<T>> where) where T : class, IEntity, new();
        int Update<T>(Expression<Predicate<T>> columns, Expression<Predicate<T>> where) where T : class, IEntity, new();
        T Single<T>(Expression<Predicate<T>> where) where T : class, IEntity, new();
        IEnumerable<T> Top<T>(int top, Expression<Predicate<T>> where, Expression<Predicate<T>> order) where T : class, IEntity, new();
        IEnumerable<T> GetList<T>() where T : class, IEntity, new();
        IEnumerable<T> GetList<T>(Expression<Predicate<T>> where) where T : class, IEntity, new();
        IEnumerable<T> GetList<T>(Expression<Predicate<T>> where, Expression<Predicate<T>> order) where T : class, IEntity, new();
        IEnumerable<T> GetList<T>(Expression<Predicate<T>> where, Expression<Predicate<T>> order, int pageIndex, int pageSize) where T : class, IEntity, new();
        int RowCount<T>(Expression<Predicate<T>> where) where T : class, IEntity, new();
        /// /////////////////////////////////////////////////////////
        void Execute(Action action);
        bool ExecuteTransaction(Action action);
        int ExecuteNonQuery(string script);
        int ExecuteNonQuery(string script, DbParameter[] parameters);
        R ExecuteScalar<R>(string script);
        R ExecuteScalar<R>(string script, DbParameter[] parameters);
        IEnumerable<T> ExecuteList<T>(string script) where T : class, IEntity, new();
        IEnumerable<T> ExecuteList<T>(string script, DbParameter[] parameters) where T : class, IEntity, new();
        ArrayList ExecuteList<T1, T2>(string script, DbParameter[] parameters);
        ArrayList ExecuteList<T1, T2, T3>(string script, DbParameter[] parameters);
    }
}
