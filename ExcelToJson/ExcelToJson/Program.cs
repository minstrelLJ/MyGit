using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelToJson
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ExcelTools.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.TargetSite);
                Console.ReadLine();
            }
        }
    }
}
