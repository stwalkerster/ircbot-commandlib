namespace Stwalkerster.Bot.CommandLib.Tests.Attributes
{
    using NUnit.Framework;
    using Stwalkerster.Bot.CommandLib.Attributes;

    [TestFixture]
    public class CommandInvocationAttributeTests
    {
        [Test]
        public void ShouldReturnValue()
        {
            var attr = new CommandInvocationAttribute("foo");
            
            Assert.That(attr.CommandName, Is.EqualTo("foo"));
        }
    }
}