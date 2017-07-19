using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeCantSpell.Tests.Integration.CSharp.Files
{
    public class Loopsexamples0001
    {
        public void Main()
        {
            var count = 0;

            foreach (var thing in "abc".ToCharArray())
            {
                count++;
            }

            for (var index = 0, jndex = 1; jndex < count; index = jndex++)
            {
                count++;
            }

            for (double floating = 0.0; floating < 8; floating += 1.1)
            {
                count++;
            }
        }
    }
}
