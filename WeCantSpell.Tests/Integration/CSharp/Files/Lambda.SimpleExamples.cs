using System;
using System.Linq.Expressions;

namespace WeCantSpell.Tests.Integration.CSharp.Files
{
    public class Lambda0001
    {
        public static void Sample()
        {
            Action<string> sample1 = word => word.ToCharArray();
            Func<int, double, string> sample2 = (countOfThings, value) => count.ToString() + value.ToString();
            Expression<Func<int, int>> sample3 = number => (number + 1) * 3;
        }
    }
}
