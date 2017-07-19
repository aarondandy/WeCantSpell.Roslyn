namespace WeCantSpell.Tests.Integration.CSharp.Files
{
    public class CastableThing
    {
		public static implicit operator string(CastableThing t)
        {
            return string.Empty;
        }

		public static explicit operator System.Guid(CastableThing t)
        {
            return System.Guid.NewGuid();
        }

		public static string operator+(CastableThing t, string s)
        {
            return string.Empty;
        }
    }
}
