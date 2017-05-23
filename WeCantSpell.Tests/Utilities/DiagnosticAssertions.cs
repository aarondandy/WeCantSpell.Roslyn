using System;
using System.Globalization;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.CodeAnalysis;

namespace WeCantSpell.Tests.Utilities
{
    public class DiagnosticAssertions : ReferenceTypeAssertions<Diagnostic, DiagnosticAssertions>
    {
        public DiagnosticAssertions(Diagnostic value)
        {
            Subject = value;
        }

        protected override string Context => "diagnostic";

        public AndConstraint<DiagnosticAssertions> HaveId(string expected, string because = "", params object[] becauseArgs)
        {
            var actual = Subject?.Id;

            Execute.Assertion
                .ForCondition(string.Equals(actual, expected, StringComparison.Ordinal))
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected {context:id} to be {0}{reason}, but found {1}.", expected, actual);

            return new AndConstraint<DiagnosticAssertions>(this);
        }

        public AndConstraint<DiagnosticAssertions> BeFromFileName(string expected, string because = "", params object[] becauseArgs)
        {
            var actual = Subject?.Location?.SourceTree?.FilePath;
            Execute.Assertion
                .ForCondition(string.Equals(actual, expected, StringComparison.Ordinal))
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected {context:file} to be {0}{reason}, but found {1}.", expected, actual);

            return new AndConstraint<DiagnosticAssertions>(this);
        }

        public AndConstraint<DiagnosticAssertions> HaveSourceSpan(int expectedStart, int expectedEnd, string because = "", params object[] becauseArgs)
        {
            var location = Subject?.Location;

            Execute.Assertion
                .ForCondition(location != null)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected {context:diagnostic} to have a location.");

            var actual = location.SourceSpan;

            Execute.Assertion
                .ForCondition(actual.Start == expectedStart)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected {context:span} to start at {0}{reason}, but found {1}.", expectedStart, actual.Start);

            Execute.Assertion
                .ForCondition(actual.End == expectedEnd)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected {context:span} to end at {0}{reason}, but found {1}.", expectedEnd, actual.End);
            
            return new AndConstraint<DiagnosticAssertions>(this);
        }

        public AndConstraint<DiagnosticAssertions> HaveLocation(int expectedStart, int expectedEnd, string expectedFileName, string because = "", params object[] becauseArgs) =>
            HaveSourceSpan(expectedStart, expectedEnd, because, becauseArgs)
                .And.BeFromFileName(expectedFileName, because, becauseArgs);

        public AndConstraint<DiagnosticAssertions> HaveMessageContaining(string expectedSubstring, string because = "", params object[] becauseArgs)
        {
            var message = Subject?.GetMessage(CultureInfo.InvariantCulture);

            Execute.Assertion
                .ForCondition(message != null)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected {context:invariant message} to not be null.");

            Execute.Assertion
                .ForCondition(message.Contains(expectedSubstring))
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected {context:invariant message} to contain {0}{reason}, but did not.", expectedSubstring);

            return new AndConstraint<DiagnosticAssertions>(this);
        }
    }
}
