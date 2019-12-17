namespace zfiFamilyRenameTool.ViewModel
{
    using System.Collections.Generic;
    using ModPlusAPI.Mvvm;

    public class LogWindowVm : VmBase
    {
        public LogWindowVm(IEnumerable<LogMessage> messages)
        {
            Messages = messages;
        }

        public IEnumerable<LogMessage> Messages { get; set; }
    }
}