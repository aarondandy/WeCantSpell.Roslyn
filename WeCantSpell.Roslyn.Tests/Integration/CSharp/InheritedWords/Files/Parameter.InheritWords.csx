public class Bar : IBaz
{
    public int Foo(int bar, int baz, string foo) =>
        bar + baz + (foo?.Length ?? 3);
}