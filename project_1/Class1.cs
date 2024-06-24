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

            string result = "";

            for (int i = 0; i < repeat; i++)

            {
                result += "Hello, pidr!\n";
                Console.WriteLine(result);

            }

            return result;

        }
    }
}










