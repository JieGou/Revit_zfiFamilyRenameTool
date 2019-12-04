namespace zfiRenameTool.Exceptions
{
    using System;

    public class PluginException : Exception
    {
        public PluginException(string message)
            : base(message)
        {
        }

        public PluginException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}