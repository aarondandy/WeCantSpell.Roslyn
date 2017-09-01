public class BarBuz
{
    public delegate void BarFoo(string foo);

    public event BarFoo Foo;

    public event BarFoo Buz
    {
        add
        {
            Foo += value;
        }
        remove
        {
            Foo -= value;
        }
    }
}
