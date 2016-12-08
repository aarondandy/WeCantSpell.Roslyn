using System.Linq;

namespace WeCantSpell.Tests.Integration.CSharp.Files
{
    public class Linqexamples0001
    {
        public void Main()
        {
            var numbers =
            (
                from number in Enumerable.Range(1, 10)
                from odd in Enumerable.Range(1, number)
                where odd % 2 == 1
                let intermediary = new
                {
                    Max = number,
                    Value = odd
                }
                group intermediary by intermediary.Value into @group
                orderby @group.Key descending
                select new
                {
                    Odd = @group.Key,
                    Values = @group.AsEnumerable()
                }
            );

            var letters = from letterint in Enumerable.Range(1, 100)
                          select (char)letterint;

            var results = from potato in numbers
                          from goat in potato.Values
                          join apple in letters on goat.Value equals (int)apple into basket
                          select new
                          {
                              Category = goat.Value,
                              Produce = basket
                          };
        }
    }
}
