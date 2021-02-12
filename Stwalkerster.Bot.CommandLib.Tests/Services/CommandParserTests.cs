namespace Stwalkerster.Bot.CommandLib.Tests.Services
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Stwalkerster.Bot.CommandLib.Services;

    [TestFixture]
    public class CommandParserTests
    {
        [Test, TestCaseSource(typeof(CommandParserTests), "ParserTestCases")]
        public void ShouldParseCommandCorrectly(string input, string expectedCommandName, string expectedArgs, bool expectedOverrideSilence)
        {
            var parser = new CoreParserService();

            var result = parser.ParseCommandMessage(input, "Helpmebot", "!");

            Assert.AreEqual(expectedCommandName, result.CommandName);
            Assert.AreEqual(expectedArgs, result.ArgumentList);
            Assert.AreEqual(expectedOverrideSilence, result.OverrideSilence);
        }

        public static IEnumerable<TestCaseData> ParserTestCases
        {
            get
            {
                yield return new TestCaseData("!helpmebot foo", "foo", "", true);
                yield return new TestCaseData("!foo", "foo", "", false);
                yield return new TestCaseData("!bar", "bar", "", false);
                yield return new TestCaseData("Helpmebot foo", "foo", "", true);
                yield return new TestCaseData("Helpmebot foo", "foo", "", true);
                yield return new TestCaseData("Helpmebot: bar", "bar", "", true);
                yield return new TestCaseData("Helpmebot, baz", "baz", "", true);
                yield return new TestCaseData("helpmebot foo", "foo", "", true);
                yield return new TestCaseData("helpmebot: bar", "bar", "", true);
                yield return new TestCaseData("helpmebot, baz", "baz", "", true);
                yield return new TestCaseData("helpmebot:foo", "foo", "", true);
                yield return new TestCaseData("!helpmebot foo bar baz qux quux", "foo", "bar baz qux quux", true);
                yield return new TestCaseData("!foo bar baz qux quux", "foo", "bar baz qux quux", false);
                yield return new TestCaseData("!bar bar baz qux quux", "bar", "bar baz qux quux", false);
                yield return new TestCaseData("!foo 💩 💩 💩 💩", "foo", "💩 💩 💩 💩", false);
                yield return new TestCaseData("!💩 💩 💩 💩", "💩", "💩 💩 💩", false);
                yield return new TestCaseData("!💩 bar baz qux quux", "💩", "bar baz qux quux", false);
                yield return new TestCaseData("!💩", "💩", "", false);
                yield return new TestCaseData("!helpmebot 💩", "💩", "", true);
                yield return new TestCaseData("Helpmebot foo bar baz qux quux", "foo", "bar baz qux quux", true);
                yield return new TestCaseData("Helpmebot foo bar baz qux quux", "foo", "bar baz qux quux", true);
                yield return new TestCaseData("Helpmebot: bar bar baz qux quux", "bar", "bar baz qux quux", true);
                yield return new TestCaseData("Helpmebot, baz bar baz qux quux", "baz", "bar baz qux quux", true);
                yield return new TestCaseData("helpmebot foo bar baz qux quux", "foo", "bar baz qux quux", true);
                yield return new TestCaseData("helpmebot: bar bar baz qux quux", "bar", "bar baz qux quux", true);
                yield return new TestCaseData("helpmebot, baz bar baz qux quux", "baz", "bar baz qux quux", true);
                yield return new TestCaseData("helpmebot:foo bar baz qux quux", "foo", "bar baz qux quux", true);
            }
        }
    }
}