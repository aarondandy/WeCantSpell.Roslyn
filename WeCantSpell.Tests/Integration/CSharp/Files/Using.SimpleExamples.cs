using System;

using static System.Guid;

using Nope = System.NotSupportedException;
using NotDone = System.NotImplementedException;

namespace WeCantSpell.Tests.Integration.CSharp.Files
{
    public class Usingexample0001
    {
        public Guid CreateId()
        {
            throw new Nope();
        }

        public void UseId(Guid id)
        {
            using (var bytes = new System.IO.MemoryStream())
            {
                throw new NotDone();
            }            
        }
    }
}
