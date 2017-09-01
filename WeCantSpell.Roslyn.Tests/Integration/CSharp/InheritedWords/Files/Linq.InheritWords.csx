class FooBaz
{
    public void BarBuz()
    {
        var numbers =
        (
            from bar in Enumerable.Range(1, 10)
            from foo in Enumerable.Range(1, bar)
            where foo % 2 == 1
            let buz = new
            {
                Max = bar,
                Value = foo
            }
            group buz by buz.Value into baz
            orderby baz.Key descending
            select new
            {
                Odd = baz.Key,
                Values = baz.AsEnumerable()
            }
        );

        var letters = from fooBaz in Enumerable.Range(1, 100) select (char)fooBaz;

        var results = from fooBuz in numbers
                      from buzBuz in fooBuz.Values
                      join bazBuz in letters on buzBuz.Value equals (int)bazBuz into buzFoo
                      select new
                      {
                          Category = buzBuz.Value,
                          Produce = buzFoo
                      };
    }
}
