namespace Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;

using System.Collections.Generic;
using System.Linq;
using Stwalkerster.IrcClient.Extensions;

public class CommandResponse
{
    public string ClientToClientProtocol { get; set; }
    public CommandResponseDestination Destination { get; set; }
    public string Message { get; set; }
    public IEnumerable<string> RedirectionTarget { get; set; }
    public CommandResponseType Type { get; set; }
    public bool IgnoreRedirection { get; set; }

    public string CompileMessage()
    {
        var message = this.Message;

        if (this.ClientToClientProtocol != null)
        {
            message = message.SetupForCtcp(this.ClientToClientProtocol);
        }
        else
        {
            if (!this.IgnoreRedirection && this.RedirectionTarget != null && this.RedirectionTarget.Any())
            {
                message = $"{string.Join(", ", this.RedirectionTarget)}: {message}";
            }
        }

        return message;
    }
}