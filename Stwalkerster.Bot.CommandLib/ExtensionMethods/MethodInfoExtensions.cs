namespace Stwalkerster.Bot.CommandLib.ExtensionMethods
{
    using System;
    using System.Reflection;
    using Attributes;
    using Castle.Core.Internal;
    using Mono.Options;

    public static class MethodInfoExtensions
    {
        public static OptionSet ParseOptionSet(
            this MethodInfo info,
            Action<string, string> parseBool,
            Action<string, string> parseString)
        {
            var attr = info.GetAttributes<CommandParameterAttribute>();
            var optionSet = new OptionSet();

            foreach (var a in attr)
            {
                if (a.ResultType == typeof(bool))
                {
                    optionSet.Add(a.Prototype, a.Description, x => parseBool(x, a.ResultName), a.Hidden);
                    continue;
                }

                if (a.ResultType == typeof(string))
                {
                    optionSet.Add(a.Prototype, a.Description, x => parseString(x, a.ResultName), a.Hidden);
                    continue;
                }

                throw new NotImplementedException(
                    $"The requested parameter type ({a.ResultType}) for parameter {a.Prototype} has not been implemented.");
            }

            return optionSet;
        }
    }
}