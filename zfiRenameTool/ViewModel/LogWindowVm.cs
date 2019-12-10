namespace zfiFamilyRenameTool.ViewModel
{
    using System.Collections.Generic;
    using MicroMvvm;

    public class LogWindowVm : ViewModelBase
    {
        public LogWindowVm(IEnumerable<LogMessage> messages)
        {
            Messages = messages;
        }

        public IEnumerable<LogMessage> Messages { get; set; }
    }
}