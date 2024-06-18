namespace Stwalkerster.Bot.CommandLib.Tests.Commands.CommandUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using NUnit.Framework;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    [TestFixture]
    public class CommandBaseTests
    {
        private IUser user;
        private ILogger logger;
        private IFlagService flags;
        private IConfigurationProvider config;
        private IIrcClient client;

        [SetUp]
        public void LocalSetup()
        {
            this.user = Substitute.For<IUser>();
            this.logger = Substitute.For<ILogger>();
            this.flags = Substitute.For<IFlagService>();
            this.config = Substitute.For<IConfigurationProvider>();
            this.client = Substitute.For<IIrcClient>();
        }
        
        [Test]
        public void ShouldHandleExceptionGracefully()
        {
            // arrange
            var commandBaseMock = Substitute.ForPartsOf<CommandHarness>(
                string.Empty,
                this.user,
                Array.Empty<string>(),
                this.logger,
                this.flags,
                this.config,
                this.client);

            commandBaseMock.OnExecute.Returns(() => throw new Exception());
            
            // act
            var commandResponses = commandBaseMock.Run();
            
            // assert
            Assert.That(commandResponses, Has.Count.EqualTo(1));
        }
        
        [Test]
        public void ShouldHandleArgumentsExceptionGracefully()
        {
            // arrange
            var commandBaseMock = Substitute.ForPartsOf<CommandHarness>(
                string.Empty,
                this.user,
                Array.Empty<string>(),
                this.logger,
                this.flags,
                this.config,
                this.client);
            
            commandBaseMock.OnExecute.Returns(() => throw new ArgumentCountException());
            
            // act
            var commandResponses = commandBaseMock.Run();
            
            // assert
            Assert.That(commandResponses, Has.Count.GreaterThanOrEqualTo(1));

        }
        
        [Test]
        public void ShouldHandleInvocationExceptionGracefully()
        {
            // arrange
            var commandBaseMock = Substitute.ForPartsOf<CommandHarness>(
                string.Empty,
                this.user,
                Array.Empty<string>(),
                this.logger,
                this.flags,
                this.config,
                this.client);
            
            commandBaseMock.OnExecute.Returns(() => throw new CommandInvocationException());
            
            // act
            var commandResponses = commandBaseMock.Run();
            
            // assert
            Assert.That(commandResponses, Has.Count.GreaterThanOrEqualTo(1));
        }
        
        [Test]
        public void ShouldHandleAccessDenied()
        {
            // arrange
            var commandBaseMock = Substitute.ForPartsOf<CommandHarness>(
                string.Empty,
                this.user,
                Array.Empty<string>(),
                this.logger,
                this.flags,
                this.config,
                this.client);
            
            commandBaseMock.OnExecute.Returns(() => throw new Exception());

            // act
            commandBaseMock.Run();
            
            // assert
            Assert.That(commandBaseMock.DoneAccessDenied, Is.EqualTo(1));
        }
        
        [Test]
        public void ShouldCallOnCompleted()
        {
            // arrange
            var commandBaseMock = Substitute.ForPartsOf<CommandHarness>(
                string.Empty,
                this.user,
                new List<string>(),
                this.logger,
                this.flags,
                this.config,
                this.client);

            this.flags.UserHasFlag(Arg.Any<IUser>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            
            // act
            commandBaseMock.Run();
            
            // assert
            Assert.That(commandBaseMock.DoneCompleted, Is.EqualTo(1));
        }

        
    }
}