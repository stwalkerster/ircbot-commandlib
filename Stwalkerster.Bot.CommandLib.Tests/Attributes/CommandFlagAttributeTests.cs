namespace Stwalkerster.Bot.CommandLib.Tests.Attributes
{
    using NUnit.Framework;
    using Stwalkerster.Bot.CommandLib.Attributes;

    [TestFixture]
    public class CommandFlagAttributeTests
    {
        [Test]
        public void ShouldReturnValue()
        {
            var attr = new CommandFlagAttribute("foo");
            
            Assert.That(attr.Flag, Is.EqualTo("foo"));
        }
    }
}