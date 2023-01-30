namespace Stwalkerster.Bot.CommandLib.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandParameterAttribute : Attribute
    {
        public string Prototype { get; }
        public string Description { get; }
        public string ResultName { get; }
        public Type ResultType { get; }
        public bool Hidden { get; }

        public CommandParameterAttribute(
            string prototype,
            string description,
            string resultName,
            Type resultType = null,
            bool hidden = false)
        {
            this.Prototype = prototype;
            this.Description = description;
            this.ResultName = resultName;
            this.ResultType = resultType;
            this.Hidden = hidden;
        }
    }
}