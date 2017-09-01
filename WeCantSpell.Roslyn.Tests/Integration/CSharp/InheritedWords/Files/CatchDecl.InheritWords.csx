public class Foo
{
    public void Bar()
    {
        try
        {
            throw new System.Exception();
        }
        catch(System.Exception fooBar)
        {
            throw;
        }
    }
}