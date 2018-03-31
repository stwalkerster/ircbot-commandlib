namespace Stwalkerster.Bot.CommandLib.Testbot.Service
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient;
    using Stwalkerster.IrcClient.Interfaces;

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
            
            container.Install(FromAssembly.This());

            var app = container.Resolve<IApplication>();
        }

        public Program(IIrcClient client)
        {
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