namespace Stwalkerster.Bot.CommandLib.ExtensionMethods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Mono.Options;

    public static class MethodInfoExtensions
    {
        public static OptionSet ParseOptionSet(
            this MethodInfo info,
            Action<string, string, Func<bool, bool>> parseBool,
            Action<string, string> parseString)
        {
            var attr = info.GetCustomAttributes<CommandParameterAttribute>();
            var optionSet = new OptionSet();

            foreach (var a in attr)
            {
                if (a.ResultType == typeof(bool))
                {
                    Func<bool, bool> mungeFunc = x => x;
                    if (a.BooleanInverse)
                    {
                        mungeFunc = x => !x;
                    }
                        
                    optionSet.Add(a.Prototype, a.Description, x => parseBool(x, a.ResultName, mungeFunc), a.Hidden);
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

        public static IEnumerable<T> GetAttributes<T>(this MemberInfo info) where T : Attribute
        {
            return info.GetCustomAttributes<T>();
        }

        public static T GetAttribute<T>(this MemberInfo info) where T : Attribute
        {
            return info.GetCustomAttributes<T>().FirstOrDefault();
        }
    }
}