using System;

namespace WeCantSpell.Roslyn
{
    public readonly struct ParsedTextSpan : IEquatable<ParsedTextSpan>
    {
        public ParsedTextSpan(string text, int start, bool isWord)
        {
            Text = text;
            Start = start;
            IsWord = isWord;
        }

        public string Text { get; }

        public int Start { get; }

        public bool IsWord { get; }

        public int Length => Text.Length;

        public int End => Start + Text.Length;

        public bool Equals(ParsedTextSpan other) =>
            Start == other.Start
            && IsWord == other.IsWord
            && Text == other.Text;

        public override bool Equals(object obj) => obj is ParsedTextSpan span && Equals(span);

        public override int GetHashCode() => unchecked(Text.GetHashCode() ^ Start);
    }
}
