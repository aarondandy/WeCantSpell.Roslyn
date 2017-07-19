namespace WeCantSpell.Tests.Integration.CSharp.Files
{
    public class Labelexamples0001
    {
        public void Mat()
        {
            goWild: int state = 0;

            jumpAgain:
            state++;

            if (state == 15)
            {
                goto goWild;
            }
            else if (state < 10)
            {
                goto jumpAgain;
            }
            else
            {
                goto strikeOut;
            }

            strikeOut:;
        }

        public int Switches(int value)
        {
            switch (value)
            {
                case 1:
                    goto case 2;
                case 2:
                    goto default;
                case 3:
                    return 3;
                default:
                    return -1;
            }
        }
    }
}
