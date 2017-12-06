using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YunChengLK.Framework.Data;

namespace YunChengLK.Framework.Dome
{
    class Program
    {
        static void Main(string[] args)
        {
            IDatabase db = Database.Test;
            Test t = new Test() { ID = Guid.NewGuid(), Name = "name" };
            IList<Test> x = new List<Test>();
            db.Execute(() =>
            {
                db.Insert<Test>(t);
                x = db.GetList<Test>().ToList();
            });
        }
    }
}
