namespace zfiRenameTool.ViewModel
{
    using MicroMvvm;

    public class LogMessage : ModelBase
    {
        public LogMessage(string title, string message, bool isError = false)
        {
            Message = message;
            IsError = isError;
            Title = title;
        }

        public string Title { get; set; }

        public string Message { get; set; }

        public bool IsError { get; set; }
    }
}