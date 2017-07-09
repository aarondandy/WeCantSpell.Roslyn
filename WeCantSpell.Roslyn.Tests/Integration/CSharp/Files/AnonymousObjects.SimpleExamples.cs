namespace WeCantSpell.Tests.Integration.CSharp.Files
{
    public class Anonymousobjects0001
    {
        public void Method()
        {
            var anon1 = new
            {
                Count = 1,
                Distance = 2.7,
                Nested = new
                {
                    Value = "letters"
                }
            };
        }
    }
}
