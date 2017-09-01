public class Foo
{
    public void Bar()
    {
        poo: goto boo;
        foo: goto bar;

        Bar();

        bar: goto poo;
        boo: goto foo;
    }
}
