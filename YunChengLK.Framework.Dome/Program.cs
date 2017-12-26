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
            Test t = new Test() { ID = Guid.NewGuid(), Name = "测试内容", pwd = "hao123" };
            db.Execute(() =>
            {
                int result = db.Insert<Test>(t);
                Console.WriteLine(result);
            });
            List<Test> list = new List<Test>();
            //for (int i = 0; i < 300; i++)
            //{
            //    Test t = new Test() { ID = Guid.NewGuid(), Name = "测试内容" + i };
            //    list.Add(t);
            //}
            //db.Execute(() =>
            //{
            //    int result = db.Insert<Test>(list);
            //    Console.WriteLine(result);
            //});
            //db.Execute(() =>
            //{
            //    list = db.GetList<Test>(m => m.ID == new Guid("A3DB7DCA-E313-4DE5-A0AE-6EFA4D897362")).ToList();
                
            //});

            //Test t = new Test() { Name = "test", pwd = "hao123" };
            //db.Execute(() =>
            //{
            //    Test t = db.Single<Test>(m => m.ID == new Guid("D88F67AD-3A7E-4792-B2CB-2A914CE7B08B"));
            //    db.Delete<Test>(m => m.ID == new Guid("D88F67AD-3A7E-4792-B2CB-2A914CE7B08B"));
            //});
        }
    }
}
