namespace zfiFamilyRenameTool.ViewModel
{
    using System.Collections.Generic;
    using ModPlusAPI.Mvvm;

    public class LogWindowViewModel : VmBase
    {
        public LogWindowViewModel(IEnumerable<LogMessage> messages)
        {
            Messages = messages;
        }

        public IEnumerable<LogMessage> Messages { get; set; }
    }
}