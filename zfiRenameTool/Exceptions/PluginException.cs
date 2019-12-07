namespace zfiRenameTool.Exceptions
{
    using System;
    using System.Collections.Generic;
    using Autodesk.Revit.DB;

    public class PluginException : Exception
    {
        public PluginException(string message)
            : base(message)
        {
        }

        public PluginException(string message, IReadOnlyCollection<Element> elements)
            : base(message)
        {
            Elements = elements;
        }

        public PluginException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public PluginException(string message, IReadOnlyCollection<Element> elements, Exception innerException)
            : base(message, innerException)
        {
        }

        public IReadOnlyCollection<Element> Elements { get; } = new List<Element>();
    }
}