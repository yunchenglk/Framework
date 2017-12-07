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
            List<Test> list = new List<Test>();
            for (int i = 0; i < 300; i++)
            {
                Test t = new Test() { ID = Guid.NewGuid(), Name = "测试内容" + i };
                list.Add(t);
            }
            db.Execute(() =>
            {
                int result = db.Insert<Test>(list);
                Console.WriteLine(result);
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
