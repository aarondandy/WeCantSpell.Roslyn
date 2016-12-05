namespace WeCantSpell.Tests.Integration.CSharp.Files
{
    public class Eventsexamples0001
    {
        public static event System.Action DoTheThing;

        public event System.Action ClickClack
        {
            add
            {
                DoTheThing += value;
            }
            remove
            {
                DoTheThing();
                DoTheThing -= value;
            }
        }
    }
}
