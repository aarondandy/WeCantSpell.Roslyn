namespace WeCantSpell.Tests.Integration.CSharp.Files
{
    /// <summary>
    /// This is a sentence with <c>inline code</c> within a sentence.
    /// </summary>
    /// <remarks>
    /// This is also a sentence made of words.
    /// This other line contains words as well.
    /// </remarks>
    /// Not in a tag
    /// <example>
    /// This is an example:
    /// <code>
    /// public void Main()
    /// {
    ///     // ignored
    ///     Console.WriteLine("ignored");
    /// }
    /// </code>
    /// More words.
    /// </example>
    /// <completionlist cref="System.Guid"/>
    /*
     * Words words words.
     * This is aardvark.
     * 
     * <code>
     * Console.WriteLine("code");
     * </code>
     */
    public class Xmldocexamples0001
    {
        /**
         * <summary>
         * A few more words.
         * </summary>
         * Here are some words.
         * <code>
         * Console.WriteLine("code");
         * </code> 
         * <returns>Always returns <c>0</c> or zero.</returns>
         */
        public int Main()
        {
            /// Just a comment
            return 0;
            // A simple comment
            // with another line under it.
        }
    }
}
