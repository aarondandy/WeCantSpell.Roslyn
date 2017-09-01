public class FooBar
{
    string Field = "foo bar";

    public void BazBuz()
    {
        var local = "baz foo bar";

        var interpolated = $"baz {local} bar buz";
    }
}
