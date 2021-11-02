namespace Stwalkerster.Bot.CommandLib.Testbot.Startup
{
    using Castle.Facilities.Logging;
    using Castle.Facilities.Startable;
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Services.Logging.Log4netIntegration;
    using Castle.Windsor;
    using Microsoft.Extensions.Logging;
    using Stwalkerster.Bot.CommandLib.Commands.Interfaces;
    using Stwalkerster.IrcClient;
    using Stwalkerster.IrcClient.Interfaces;

    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var loggerFactory = new LoggerFactory().AddLog4Net("log4net.xml");
            
            // Facilities
            container.AddFacility<LoggingFacility>(f => f.LogUsing<Log4netFactory>().WithConfig("log4net.xml"));
            container.AddFacility<StartableFacility>(f => f.DeferredStart());
            container.AddFacility<TypedFactoryFacility>();
            
            container.Install(
                new Stwalkerster.Bot.CommandLib.Startup.Installer()
            );
            
            string ns = "Stwalkerster.Bot.CommandLib.Testbot";

            container.Register(
                Component.For<ILoggerFactory>().Instance(loggerFactory),
                Component.For<ILogger<SupportHelper>>().UsingFactoryMethod(loggerFactory.CreateLogger<SupportHelper>),
                Classes.FromAssemblyContaining<Installer>().InNamespace(ns + ".Service").WithServiceAllInterfaces(),
                Classes.FromAssemblyContaining<Installer>().BasedOn<ICommand>().LifestyleTransient(),
                Component.For<ISupportHelper>().ImplementedBy<SupportHelper>(),
                Component.For<IIrcClient>().ImplementedBy<IrcClient>()
            );
        }
    }
}