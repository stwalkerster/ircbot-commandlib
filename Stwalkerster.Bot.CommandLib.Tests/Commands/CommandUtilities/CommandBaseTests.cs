namespace Stwalkerster.Bot.CommandLib.Tests.Commands.CommandUtilities
{
    using System;
    using System.Linq;
    using Castle.Core.Logging;
    using Moq;
    using Moq.Protected;
    using NUnit.Framework;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    [TestFixture]
    public class CommandBaseTests
    {
        private Mock<IUser> user;
        private Mock<ILogger> logger;
        private Mock<IFlagService> flags;
        private Mock<IConfigurationProvider> config;
        private Mock<IIrcClient> client;

        [SetUp]
        public void LocalSetup()
        {
            this.user = new Mock<IUser>();
            this.logger = new Mock<ILogger>();
            this.flags = new Mock<IFlagService>();
            this.config = new Mock<IConfigurationProvider>();
            this.client = new Mock<IIrcClient>();
        }
        
        [Test]
        public void ShouldHandleExceptionGracefully()
        {
            // arrange
            var commandBaseMock = new Mock<CommandBase>(
                string.Empty,
                this.user.Object,
                new string[0],
                this.logger.Object,
                this.flags.Object,
                this.config.Object,
                this.client.Object);
            
            commandBaseMock.CallBase = true;
            commandBaseMock.Protected().Setup("Execute").Throws<Exception>();
            commandBaseMock.Setup(x => x.CanExecute()).Returns(true);

            // act
            var commandResponses = commandBaseMock.Object.Run();
            
            // assert
            Assert.AreEqual(1, commandResponses.Count());
        }
        
        [Test]
        public void ShouldHandleArgumentsExceptionGracefully()
        {
            // arrange
            var commandBaseMock = new Mock<CommandBase>(
                string.Empty,
                this.user.Object,
                new string[0],
                this.logger.Object,
                this.flags.Object,
                this.config.Object,
                this.client.Object);
            
            commandBaseMock.CallBase = true;
            commandBaseMock.Protected().Setup("Execute").Throws<ArgumentCountException>();
            commandBaseMock.Setup(x => x.CanExecute()).Returns(true);

            // act
            var commandResponses = commandBaseMock.Object.Run();
            
            // assert
            Assert.GreaterOrEqual(commandResponses.Count(), 1);
        }
        
        [Test]
        public void ShouldHandleInvocationExceptionGracefully()
        {
            // arrange
            var commandBaseMock = new Mock<CommandBase>(
                string.Empty,
                this.user.Object,
                new string[0],
                this.logger.Object,
                this.flags.Object,
                this.config.Object,
                this.client.Object);
            
            commandBaseMock.CallBase = true;
            commandBaseMock.Protected().Setup("Execute").Throws<CommandInvocationException>();
            commandBaseMock.Setup(x => x.CanExecute()).Returns(true);

            // act
            var commandResponses = commandBaseMock.Object.Run();
            
            // assert
            Assert.GreaterOrEqual(commandResponses.Count(), 1);
        }
        
        [Test]
        public void ShouldHandleAccessDenied()
        {
            // arrange
            var commandBaseMock = new Mock<CommandBase>(
                string.Empty,
                this.user.Object,
                new string[0],
                this.logger.Object,
                this.flags.Object,
                this.config.Object,
                this.client.Object);
            
            commandBaseMock.CallBase = true;
            commandBaseMock.Protected().Setup("Execute").Throws<Exception>();
            commandBaseMock.Setup(x => x.CanExecute()).Returns(false);
            commandBaseMock.Protected().Setup("OnAccessDenied").Verifiable();

            // act
            commandBaseMock.Object.Run();
            
            // assert
            commandBaseMock.Protected().Verify("OnAccessDenied", Times.Once());
        }
        
        [Test]
        public void ShouldCallOnCompleted()
        {
            // arrange
            var commandBaseMock = new Mock<CommandBase>(
                string.Empty,
                this.user.Object,
                new string[0],
                this.logger.Object,
                this.flags.Object,
                this.config.Object,
                this.client.Object);
            
            commandBaseMock.CallBase = true;
            commandBaseMock.Setup(x => x.CanExecute()).Returns(true);
            commandBaseMock.Protected().Setup("OnCompleted").Verifiable();

            // act
            commandBaseMock.Object.Run();
            
            // assert
            commandBaseMock.Protected().Verify("OnCompleted", Times.Once());
        }

        
    }
}