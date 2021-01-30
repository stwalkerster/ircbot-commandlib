namespace Stwalkerster.Bot.CommandLib.Testbot.Service
{
    using System.IO;
    using System.Reflection;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Stwalkerster.Bot.CommandLib.Commands.Interfaces;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient;
    using Stwalkerster.IrcClient.Interfaces;
    using Installer = Stwalkerster.Bot.CommandLib.Testbot.Startup.Installer;

    public class Program : IApplication
    {
        public static void Main()
        {
            var container = new WindsorContainer();
            container.Register(
                Component.For<IIrcConfiguration>()
                    .Instance(
                        new IrcConfiguration(
                            hostname: "irc.freenode.net",
                            port: 7000,
                            authToServices: false,
                            nickname: "stwtestbot",
                            username: "stwtestbot",
                            realName: "stwtestbot",
                            ssl: true,
                            clientName: "TestClient"
                        )));

            var a = Assembly.LoadFile(Path.GetFullPath("Stwalkerster.Bot.CommandLib.Testbot.Commands.dll"));
            container.Register(Classes.FromAssembly(a).BasedOn<ICommand>());
            
            container.Install(new Installer());

            var app = container.Resolve<IApplication>();
        }

        public Program(IIrcClient client, ICommandHandler commandHandler)
        {
            client.ReceivedMessage += commandHandler.OnMessageReceived;
            
            client.JoinChannel("##stwalkerster-development");
        }
        
        public void Stop()
        {
        }

        public void Run()
        {
        }
    }
}