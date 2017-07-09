namespace WeCantSpell.Tests.Integration.CSharp.Files
{
    public interface Interfaceexample<in TIn, out TOut>
    {
    }

    public class NotGeneric
    {
        public static void UseValue<TStruct>(TStruct valueTypeValue) where TStruct : struct
        {
        }
    }

    public class GenericClass<TThing>
    {
        public TGadget Product<TGadget>(TThing tool) => default(TGadget);

        public delegate void Func<TIon>(TIon x);
    }
}
