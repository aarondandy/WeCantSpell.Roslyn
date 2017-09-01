public class Foo
{
    public void Method1()
    {
        using (var foo = (System.IDisposable)null)
        {
            foo.Dispose();
        }
    }
}