namespace Stwalkerster.Bot.CommandLib.ExtensionMethods;

using System.Collections.Generic;

public static class ParameterExtensions
{
    public static T GetParameter<T>(this IDictionary<string, object> parameters, string key, T defaultValue)
    {
        if (!parameters.TryGetValue(key, out var parameter))
        {
            return defaultValue;
        }

        return (T)parameter;
    }
}