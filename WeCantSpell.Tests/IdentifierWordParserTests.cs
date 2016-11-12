﻿using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace WeCantSpell.Tests
{
    public class IdentifierWordParserTests
    {
        public static IEnumerable<object[]> splits_name_to_text_parts_data
        {
            get
            {
                yield return new object[] { "", new string[0] };
                yield return new object[] { "x", new[] { "x" } };
                yield return new object[] { "word", new[] { "word" } };
                yield return new object[] { "WORD", new[] { "WORD" } };
                yield return new object[] { "Maßstab", new[] { "Maßstab" } };
                yield return new object[] { "maßstab", new[] { "maßstab" } };
                yield return new object[] { "aß", new[] { "aß" } };
                yield return new object[] { "Aß", new[] { "Aß" } };
                yield return new object[] { "FirstMiddleLast", new[] { "First", "Middle", "Last" } };
                yield return new object[] { "firstMiddleLast", new[] { "first", "Middle", "Last" } };
                yield return new object[] { "idColumn", new[] { "id", "Column" } };
                yield return new object[] { "IDColumn", new[] { "ID", "Column" } };
                yield return new object[] { "recordId", new[] { "record", "Id" } };
                yield return new object[] { "recordID", new[] { "record", "ID" } };
                yield return new object[] { "AVariable", new[] { "A", "Variable" } };
                yield return new object[] { "variableA", new[] { "variable", "A" } };
                yield return new object[] { "XMLNamespace", new[] { "XML", "Namespace" } };
                yield return new object[] { "xmlNamespace", new[] { "xml", "Namespace" } };
                yield return new object[] { "dataJSON", new[] { "data", "JSON" } };
                yield return new object[] { "dataJson", new[] { "data", "Json" } };
                yield return new object[] { "OGRE_CAPS", new[] { "OGRE", "_", "CAPS" } };
                yield return new object[] { "SCREAMING_SNAKE_CASE", new[] { "SCREAMING", "_", "SNAKE", "_", "CASE" } };
                yield return new object[] { "snake_case", new[] { "snake", "_", "case" } };
                yield return new object[] { "someMixed_caseStyles", new[] { "some", "Mixed", "_", "case", "Styles" } };
                yield return new object[] { "SomeMixed_CaseStyles", new[] { "Some", "Mixed", "_", "Case", "Styles" } };
                yield return new object[] { "END-OF-FILE", new[] { "END", "-", "OF", "-", "FILE" } };
                yield return new object[] { "a - b", new[] { "a", " - ", "b" } };
                yield return new object[] { "TCP IP socket ID", new[] { "TCP", " ", "IP", " ", "socket", " ", "ID" } };
                yield return new object[] { "TCPIPSocketID", new[] { "TCPIP", "Socket", "ID" } };
                yield return new object[] { "TcpIpSocketId", new[] { "Tcp", "Ip", "Socket", "Id" } };
                yield return new object[] { "__Hello_world", new[] { "__", "Hello", "_", "world" } };
            }
        }

        [Theory, MemberData(nameof(splits_name_to_text_parts_data))]
        public void splits_name_to_text_parts(string givenName, string[] expected)
        {
            var wordSplitter = new IdentifierWordParser();

            var actual = wordSplitter.SplitWordParts(givenName);

            actual.Select(p => p.Text).Should().BeEquivalentTo(expected);
        }

        public static IEnumerable<object[]> text_parts_are_correct_type_data
        {
            get
            {
                yield return new object[] { "", new bool[0] };
                yield return new object[] { "a", new[] { true } };
                yield return new object[] { "_", new[] { false } };
                yield return new object[] { "a_", new[] { true, false } };
                yield return new object[] { "_a", new[] { false, true } };
                yield return new object[] { "a_a", new[] { true, false, true } };
                yield return new object[] { "_a_", new[] { false, true, false } };
                yield return new object[] { "SCREAMING_SNAKE_CASE", new[] { true, false, true, false, true } };
                yield return new object[] { "snake_case", new[] { true, false, true } };
                yield return new object[] { "someMixed_caseStyles", new[] { true, true, false, true, true } };
                yield return new object[] { "SomeMixed_CaseStyles", new[] { true, true, false, true, true } };
                yield return new object[] { "END-OF-FILE", new[] { true, false, true, false, true } };
                yield return new object[] { "a - b", new[] { true, false, true } };
                yield return new object[] { "TCP IP socket ID", new[] { true, false, true, false, true, false, true } };
                yield return new object[] { "TCPIPSocketID", new[] { true, true, true } };
                yield return new object[] { "TcpIpSocketId", new[] { true, true, true, true } };
                yield return new object[] { "__Hello_world", new[] { false, true, false, true } };
            }
        }

        [Theory, MemberData(nameof(text_parts_are_correct_type_data))]
        public void text_parts_are_correct_type(string givenName, bool[] expectedWordPart)
        {
            var wordSplitter = new IdentifierWordParser();

            var actual = wordSplitter.SplitWordParts(givenName);

            actual.Select(p => p.IsWord).Should().Equal(expectedWordPart);
        }
    }
}
