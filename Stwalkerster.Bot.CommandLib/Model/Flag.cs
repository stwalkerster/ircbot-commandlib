namespace Stwalkerster.Bot.CommandLib.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class Flag
{
    /// <summary>
    /// The owner-level access, stuff only the bot owner should be able to use
    /// </summary>
    public const string Owner = "O";

    /// <summary>
    /// The standard uncontroversial commands.
    /// </summary>
    public const string Standard = "S";

    public static ISet<string> GetValidFlags()
    {
        var fieldInfos = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(
                assembly => assembly.GetTypes(),
                (assembly, type) => type)
            .Where(t => t.IsSubclassOf(typeof(Flag)))
            .SelectMany(
                t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy),
                (type, info) => info)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string));

        var enumerable = fieldInfos.Select(x => (string)x.GetRawConstantValue()).ToList();

        return new HashSet<string>(enumerable);
    }
}