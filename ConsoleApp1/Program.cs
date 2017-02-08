using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Contextually.Relevant.Info(new System.Collections.Specialized.NameValueCollection { ["Key1"] = "Value1" }))
            {
                Console.WriteLine(Contextually.Relevant.Info()["Key1"]);
            }

            Console.ReadLine();
        }
    }
}
