using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YunChengLK.Framework.Data;
using YunChengLK.Framework.Serialization;

namespace YunChengLK.Framework.Dome
{
    class Program
    {
        static void Main(string[] args)
        {
            IDatabase db = Database.Test;
            //List<Test> list = new List<Test>();
            //for (int i = 0; i < 10; i++)
            //{
            //    Test t = new Test() { ID = Guid.NewGuid(), Name = "测试内容" };
            //    list.Add(t);

            //}
            Test t = new Test() { ID = Guid.NewGuid(), Name = "测试内容" };
            db.Execute(() =>
            {
                int result = db.Insert<Test>(t);
            });

            //IList<USER> x = new List<USER>();
            //db.Execute(() =>
            //{
            //    // db.Insert<Test>(t);
            //    x = db.GetList<USER>(m => m.LoginName == "aaa").ToList();
            //});
        }
    }
}
