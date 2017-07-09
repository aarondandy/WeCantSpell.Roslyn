namespace WeCantSpell.Roslyn
{
    public enum SpellingMistakeKind : byte
    {
        Identifier,
        Literal,
        Comment,
        Documentation
    }
}
