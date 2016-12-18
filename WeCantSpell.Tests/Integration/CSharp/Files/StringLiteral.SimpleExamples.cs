using System;

namespace WeCantSpell.Tests.Integration.CSharp.Files
{
    public class Stringliteralexamples0001
    {
        public string Field = "apple banana cranberry";

        public void Main()
        {
            var local = "dragon-fruit edamame fig";
            var interpolated = $"gooseberry {"huckleberry"} イチゴ: {new InvalidOperationException()}";
            var verbatim = @"   «jackfruit»,
""kiwi""; 'lemon'       
  mango:   nectarine & orange. papaya
 (quince * `raspberry` ^ )   !";
            var skipped = "<ignore>ignore</ignore>|ignore|ignore";
        }
    }
}
