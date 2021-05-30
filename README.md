An IRC bot command-handling library.

## Usage
In-depth knowledge of [Castle Windsor](https://github.com/castleproject/Windsor/blob/master/docs/README.md) is assumed here. It's also assumed you know how to use `Stwalkerster.IrcClient`.

Grab yourself the following [NuGet](https://docs.microsoft.com/en-us/nuget/what-is-nuget) packages:
```xml
  <package id="Castle.Core-log4net" version="4.2.1" targetFramework="net45" />
  <package id="Castle.EventWiringFacility" version="4.1.0" targetFramework="net45" />
  <package id="Castle.LoggingFacility" version="4.1.0" targetFramework="net45" />
  <package id="Stwalkerster.IrcClient" version="7.0.x-beta" targetFramework="net45" />
  <package id="Stwalkerster.Bot.CommandLib" version="8.0.x-beta" targetFramework="net45" />

  <!-- these should be installed as dependencies of the above -->
  <package id="Castle.Core" version="4.2.1" targetFramework="net45" />
  <package id="Castle.Windsor" version="4.1.0" targetFramework="net45" />
  <package id="log4net" version="2.0.8" targetFramework="net45" />
```

I'll assume you're using a root namespace of `MyBot`.

Create yourself a basic [installer](https://github.com/castleproject/Windsor/blob/master/docs/installers.md) (this can be the same installer as the IRC client installer):
```csharp
namespace MyBot.Startup
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<LoggingFacility>(f => f.LogUsing<Log4netFactory>().WithConfig("log4net.xml"));
            container.AddFacility<EventWiringFacility>();
            container.AddFacility<StartableFacility>(f => f.DeferredStart());
            container.AddFacility<TypedFactoryFacility>();

            container.Install(
                new Stwalkerster.IrcClient.Startup.Installer()
            );

            container.Register(
                Classes.FromThisAssembly().InNamespace("MyBot.Services").WithServiceAllInterfaces(),
                Classes.FromThisAssembly().InNamespace("MyBot.Commands").LifestyleTransient(),
                Component.For<ISupportHelper>().ImplementedBy<SupportHelper>(),
                Component.For<IIrcConfiguration>().Instance(new IrcConfiguration(/* ... */)),
                Component.For<IIrcClient>().ImplementedBy<IrcClient>().Start()
            );
        }
    }
}
```

Configure log4net.

Create your main class:
```csharp
namespace MyBot.Services
{
    public class Program : IApplication
    {
        public static void Main()
        {
            var container = new WindsorContainer();
            container.Install(FromAssembly.This());
            var app = container.Resolve<IApplication>();
        }

        public Program(IIrcClient client)
        {
            client.JoinChannel("##stwalkerster-development");
        }
        
        public void Stop() {}
        public void Run() {}
    }
}
```

Implement `IConfigurationProvider` and `IFlagService`:
```csharp
namespace MyBot.Services
{
    public class ConfigProvider : IConfigurationProvider
    {
        public string CommandPrefix { get { return "!"; } }
        public string DebugChannel { get { return "##stwalkerster-development"; } }
    }

    public class BasicFlagService : IFlagService
    {
        public bool UserHasFlag(IUser user, string flag) { return true; }
        public IEnumerable<string> GetFlagsForUser(IUser user) { return new[] {"A", "D", "P", "O", "B", "C"}; }
    }
}
```
The string array is a list of flags for which the user should be reported as having access to - the implementation above is very basic, but should be extended into your own access control system.

Finally, implement your commands in the `MyBot.Command` namespace, using `Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.CommandBase` as a base class, and using the two attributes:
```csharp
    [CommandInvocation("whoami")]
    [CommandFlag("B")]
```

`CommandInvocation` determines what you'll use to execute the command - in the above example, you'd run `!whoami`. `CommandFlag` determines what is passed to `IFlagService` for access control. This is a simple string, though there are some predefined single-letter flags in `Stwalkerster.Bot.CommandLib.Model.Flag`.
