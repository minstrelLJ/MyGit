using System;
using System.Collections.Generic;
using System.Text;

namespace mysqlTest
{
    class Tool
    {
        public static bool Check2StringIs(string s1, string s2)
        {
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i] != s2[i])
                {
                    Console.WriteLine("i:{0} {1}", i, s1[i]);
                    return false;
                }
            }
            return true;
        }
    }
}
