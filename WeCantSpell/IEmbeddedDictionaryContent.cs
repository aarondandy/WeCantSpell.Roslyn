using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeCantSpell
{
    public interface IEmbeddedDictionaryContent
    {
        string Name { get; }

        Stream GetDictionaryStream();

        Stream GetAffixStream();
    }
}
