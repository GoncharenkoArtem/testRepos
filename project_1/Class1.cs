using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project_1
{



    internal class Class1
    {
        /// <summary>
        ///  Опсиание чего-либо
        /// </summary>

        public string HelloHabr(int repeat)

        {

            string result1 = "";

            for (int i = 0; i < repeat; i++)

            {
                result1 += "Hello, pidr!\n";
                Console.WriteLine(result1);

            }

            return result1;

        }
    }
}










