using System;

namespace WeCantSpell.Tests.Integration.CSharp.Files
{
    public class Properties0001
    {
        public int ReadOnly { get; }

        public string GeneratedBacking { get; set; }

        public double HandMade
        {
            get
            {
                return 9.4;
            }
            set
            {
                ;
            }
        }

        private Guid Uuid => Guid.NewGuid();

        public int this[int index, string word]
        {
            get
            {
                return 0;
            }
        }
    }
}
