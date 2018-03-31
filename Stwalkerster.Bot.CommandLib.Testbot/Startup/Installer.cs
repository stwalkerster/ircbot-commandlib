namespace Stwalkerster.Bot.CommandLib.Testbot.Startup
{
    using Castle.Facilities.EventWiring;
    using Castle.Facilities.Logging;
    using Castle.Facilities.Startable;
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Services.Logging.Log4netIntegration;
    using Castle.Windsor;
    using Stwalkerster.Bot.CommandLib.Services;
    using Stwalkerster.IrcClient;
    using Stwalkerster.IrcClient.Interfaces;

    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Facilities
            container.AddFacility<LoggingFacility>(f => f.LogUsing<Log4netFactory>().WithConfig("log4net.xml"));
            container.AddFacility<EventWiringFacility>();
            container.AddFacility<StartableFacility>(f => f.DeferredStart());
            container.AddFacility<TypedFactoryFacility>();
            
            container.Install(
                new Startup.Installer()
            );
            
            string ns = "Stwalkerster.Bot.CommandLib.Testbot";

            container.Register(
                Classes.FromThisAssembly().InNamespace(ns + ".Service").WithServiceAllInterfaces(),
                Classes.FromThisAssembly().InNamespace(ns + ".Command").LifestyleTransient(),
                Component.For<ISupportHelper>().ImplementedBy<SupportHelper>(),
                Component.For<IIrcClient>()
                    .ImplementedBy<IrcClient>()
                    .PublishEvent(
                        p => p.ReceivedMessage += null,
                        x => x.To<CommandHandler>(l => l.OnMessageReceived(null, null)))
            );

        }
    }
}