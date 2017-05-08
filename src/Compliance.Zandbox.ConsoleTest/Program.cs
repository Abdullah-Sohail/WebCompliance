using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Compliance.Common.Extensions;

namespace Compliance.Zandbox.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var api = new ScoreCards.Api.ScoreCardReviewApi("", "");
            //var ret = api.GetByWorkItemId(Guid.Parse("1E17940A-5EDF-4B7F-BFC7-AABB8EF016A3"));
            var start = DateTime.Now;
            double total = 0;
            var finished = 0;

            for (var i = 0; i < 100; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    for (var y = 0; y < 2000000; y++)
                    {
                        var x = Guid.NewGuid();
                        if ((x.FromBase64Url(x.ToBase64Url()) == x).ToString() == "True")
                        {
                            if (y % 999999 == 0)
                                Console.Write("-|");
                        }
                        else
                        {
                            Console.WriteLine("******* FAILED *********: " + x.ToBase64Url() + " - " + x.ToString());
                            Console.ReadLine();
                        }
                    }

                    finished += 1;
                    if (finished == 100)
                    { 
                        total = DateTime.Now.Subtract(start).TotalMilliseconds;
                        Console.WriteLine("Finished 200,000,000 Guid's in " + total.ToString("#,##0") + " ms...");
                    }
                });
            }


            Console.ReadLine();


        }
    }
}
