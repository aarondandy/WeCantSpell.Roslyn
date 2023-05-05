using System;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace WeCantSpell.Roslyn.Tests
{
    public class IdentifierWordParserTests
    {
        public static object[][] SplitsNameToTextPartsData =>
            new[]
            {
                new object[] { "", Array.Empty<string>() },
                new object[] { "x", new[] { "x" } },
                new object[] { "word", new[] { "word" } },
                new object[] { "WORD", new[] { "WORD" } },
                new object[] { "Maßstab", new[] { "Maßstab" } },
                new object[] { "maßstab", new[] { "maßstab" } },
                new object[] { "aß", new[] { "aß" } },
                new object[] { "Aß", new[] { "Aß" } },
                new object[] { "FirstMiddleLast", new[] { "First", "Middle", "Last" } },
                new object[] { "firstMiddleLast", new[] { "first", "Middle", "Last" } },
                new object[] { "idColumn", new[] { "id", "Column" } },
                new object[] { "IDColumn", new[] { "ID", "Column" } },
                new object[] { "recordId", new[] { "record", "Id" } },
                new object[] { "recordID", new[] { "record", "ID" } },
                new object[] { "AVariable", new[] { "A", "Variable" } },
                new object[] { "variableA", new[] { "variable", "A" } },
                new object[] { "XMLNamespace", new[] { "XML", "Namespace" } },
                new object[] { "xmlNamespace", new[] { "xml", "Namespace" } },
                new object[] { "dataJSON", new[] { "data", "JSON" } },
                new object[] { "dataJson", new[] { "data", "Json" } },
                new object[] { "OGRE_CAPS", new[] { "OGRE", "_", "CAPS" } },
                new object[] { "SCREAMING_SNAKE_CASE", new[] { "SCREAMING", "_", "SNAKE", "_", "CASE" } },
                new object[] { "snake_case", new[] { "snake", "_", "case" } },
                new object[] { "someMixed_caseStyles", new[] { "some", "Mixed", "_", "case", "Styles" } },
                new object[] { "SomeMixed_CaseStyles", new[] { "Some", "Mixed", "_", "Case", "Styles" } },
                new object[] { "END-OF-FILE", new[] { "END", "-", "OF", "-", "FILE" } },
                new object[] { "a - b", new[] { "a", " - ", "b" } },
                new object[] { "TCP IP socket ID", new[] { "TCP", " ", "IP", " ", "socket", " ", "ID" } },
                new object[] { "TCPIPSocketID", new[] { "TCPIP", "Socket", "ID" } },
                new object[] { "TcpIpSocketId", new[] { "Tcp", "Ip", "Socket", "Id" } },
                new object[] { "__Hello_world", new[] { "__", "Hello", "_", "world" } }
            };

        [Theory, MemberData(nameof(SplitsNameToTextPartsData))]
        public void splits_name_to_text_parts(string givenName, string[] expected)
        {
            var actual = IdentifierWordParser.SplitWordParts(givenName);

            actual.Select(p => p.Text).Should().BeEquivalentTo(expected);
        }

        public static object[][] TextPartsAreCorrectTypeData =>
            new[]
            {
                new object[] { "", Array.Empty<bool>() },
                new object[] { "a", new[] { true } },
                new object[] { "_", new[] { false } },
                new object[] { "a_", new[] { true, false } },
                new object[] { "_a", new[] { false, true } },
                new object[] { "a_a", new[] { true, false, true } },
                new object[] { "_a_", new[] { false, true, false } },
                new object[] { "SCREAMING_SNAKE_CASE", new[] { true, false, true, false, true } },
                new object[] { "snake_case", new[] { true, false, true } },
                new object[] { "someMixed_caseStyles", new[] { true, true, false, true, true } },
                new object[] { "SomeMixed_CaseStyles", new[] { true, true, false, true, true } },
                new object[] { "END-OF-FILE", new[] { true, false, true, false, true } },
                new object[] { "a - b", new[] { true, false, true } },
                new object[] { "TCP IP socket ID", new[] { true, false, true, false, true, false, true } },
                new object[] { "TCPIPSocketID", new[] { true, true, true } },
                new object[] { "TcpIpSocketId", new[] { true, true, true, true } },
                new object[] { "__Hello_world", new[] { false, true, false, true } }
            };

        [Theory, MemberData(nameof(TextPartsAreCorrectTypeData))]
        public void text_parts_are_correct_type(string givenName, bool[] expectedWordPart)
        {
            var actual = IdentifierWordParser.SplitWordParts(givenName);

            actual.Select(p => p.IsWord).Should().Equal(expectedWordPart);
        }
    }
}
