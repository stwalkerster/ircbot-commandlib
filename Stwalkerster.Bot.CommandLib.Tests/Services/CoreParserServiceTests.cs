namespace Stwalkerster.Bot.CommandLib.Tests.Services
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Stwalkerster.Bot.CommandLib.Services;

    [TestFixture]
    public class CoreParserServiceTests
    {
        [Test, TestCaseSource(typeof(CoreParserServiceTests), "ParserTestCases")]
        public void ShouldParseCommandCorrectly(string input, string expectedCommandName, string expectedArgs, bool expectedOverrideSilence, bool isDirect)
        {
            var parser = new CoreParserService();

            var result = parser.ParseCommandMessage(input, "Helpmebot", "!", isDirect);

            Assert.AreEqual(expectedCommandName, result.CommandName);
            Assert.AreEqual(expectedArgs, result.ArgumentList);
            Assert.AreEqual(expectedOverrideSilence, result.OverrideSilence);
        }

        public static IEnumerable<TestCaseData> ParserTestCases
        {
            get
            {
                yield return new TestCaseData("!helpmebot foo", "foo", "", true, false);
                yield return new TestCaseData("!foo", "foo", "", false, false);
                yield return new TestCaseData("!bar", "bar", "", false, false);
                yield return new TestCaseData("Helpmebot foo", "foo", "", true, false);
                yield return new TestCaseData("Helpmebot foo", "foo", "", true, false);
                yield return new TestCaseData("Helpmebot: bar", "bar", "", true, false);
                yield return new TestCaseData("Helpmebot, baz", "baz", "", true, false);
                yield return new TestCaseData("helpmebot foo", "foo", "", true, false);
                yield return new TestCaseData("helpmebot: bar", "bar", "", true, false);
                yield return new TestCaseData("helpmebot, baz", "baz", "", true, false);
                yield return new TestCaseData("helpmebot:foo", "foo", "", true, false);
                yield return new TestCaseData("!helpmebot foo bar baz qux quux", "foo", "bar baz qux quux", true, false);
                yield return new TestCaseData("!foo bar baz qux quux", "foo", "bar baz qux quux", false, false);
                yield return new TestCaseData("!bar bar baz qux quux", "bar", "bar baz qux quux", false, false);
                yield return new TestCaseData("!foo 💩 💩 💩 💩", "foo", "💩 💩 💩 💩", false, false);
                yield return new TestCaseData("!💩 💩 💩 💩", "💩", "💩 💩 💩", false, false);
                yield return new TestCaseData("!💩 bar baz qux quux", "💩", "bar baz qux quux", false, false);
                yield return new TestCaseData("!💩", "💩", "", false, false);
                yield return new TestCaseData("!helpmebot 💩", "💩", "", true, false);
                yield return new TestCaseData("Helpmebot foo bar baz qux quux", "foo", "bar baz qux quux", true, false);
                yield return new TestCaseData("Helpmebot foo bar baz qux quux", "foo", "bar baz qux quux", true, false);
                yield return new TestCaseData("Helpmebot: bar bar baz qux quux", "bar", "bar baz qux quux", true, false);
                yield return new TestCaseData("Helpmebot, baz bar baz qux quux", "baz", "bar baz qux quux", true, false);
                yield return new TestCaseData("helpmebot foo bar baz qux quux", "foo", "bar baz qux quux", true, false);
                yield return new TestCaseData("helpmebot: bar bar baz qux quux", "bar", "bar baz qux quux", true, false);
                yield return new TestCaseData("helpmebot, baz bar baz qux quux", "baz", "bar baz qux quux", true, false);
                yield return new TestCaseData("helpmebot:foo bar baz qux quux", "foo", "bar baz qux quux", true, false);
                
                yield return new TestCaseData("!helpmebot foo", "foo", "", true, true);
                yield return new TestCaseData("!foo", "foo", "", true, true);
                yield return new TestCaseData("foo", "foo", "", true, true);
                yield return new TestCaseData("!bar", "bar", "", true, true);
                yield return new TestCaseData("bar", "bar", "", true, true);
                yield return new TestCaseData("Helpmebot foo", "foo", "", true, true);
                yield return new TestCaseData("Helpmebot foo", "foo", "", true, true);
                yield return new TestCaseData("Helpmebot: bar", "bar", "", true, true);
                yield return new TestCaseData("Helpmebot, baz", "baz", "", true, true);
                yield return new TestCaseData("helpmebot foo", "foo", "", true, true);
                yield return new TestCaseData("helpmebot: bar", "bar", "", true, true);
                yield return new TestCaseData("helpmebot, baz", "baz", "", true, true);
                yield return new TestCaseData("helpmebot:foo", "foo", "", true, true);
                yield return new TestCaseData("!helpmebot foo bar baz qux quux", "foo", "bar baz qux quux", true, true);
                yield return new TestCaseData("!foo bar baz qux quux", "foo", "bar baz qux quux", true, true);
                yield return new TestCaseData("!bar bar baz qux quux", "bar", "bar baz qux quux", true, true);
                yield return new TestCaseData("!foo 💩 💩 💩 💩", "foo", "💩 💩 💩 💩", true, true);
                yield return new TestCaseData("!💩 💩 💩 💩", "💩", "💩 💩 💩", true, true);
                yield return new TestCaseData("!💩 bar baz qux quux", "💩", "bar baz qux quux", true, true);
                yield return new TestCaseData("!💩", "💩", "", true, true);  
                yield return new TestCaseData("💩", "💩", "", true, true);
                yield return new TestCaseData("!helpmebot 💩", "💩", "", true, true);
                yield return new TestCaseData("Helpmebot foo bar baz qux quux", "foo", "bar baz qux quux", true, true);
                yield return new TestCaseData("Helpmebot foo bar baz qux quux", "foo", "bar baz qux quux", true, true);
                yield return new TestCaseData("Helpmebot: bar bar baz qux quux", "bar", "bar baz qux quux", true, true);
                yield return new TestCaseData("Helpmebot, baz bar baz qux quux", "baz", "bar baz qux quux", true, true);
                yield return new TestCaseData("helpmebot foo bar baz qux quux", "foo", "bar baz qux quux", true, true);
                yield return new TestCaseData("helpmebot: bar bar baz qux quux", "bar", "bar baz qux quux", true, true);
                yield return new TestCaseData("helpmebot, baz bar baz qux quux", "baz", "bar baz qux quux", true, true);
                yield return new TestCaseData("helpmebot:foo bar baz qux quux", "foo", "bar baz qux quux", true, true);
            }
        }
    }
}