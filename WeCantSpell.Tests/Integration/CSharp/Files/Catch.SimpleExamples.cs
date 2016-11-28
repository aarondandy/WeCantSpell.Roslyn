using System;

namespace WeCantSpell.Tests.Integration.CSharp.Files
{
    public class Catchsamples0001
    {
        public static void Catch()
        {
            try
            {
                throw new NotFiniteNumberException();
            }
            catch (NotFiniteNumberException badValueEx)
            {
                throw new Exception("?", badValueEx);
            }
            catch (NotImplementedException)
            {
                throw new NotSupportedException();
            }
            catch
            {
                throw;
            }
        }
    }
}
