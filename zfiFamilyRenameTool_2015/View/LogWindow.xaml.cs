namespace zfiFamilyRenameTool.View
{
    using System.Collections.Generic;
    using ViewModel;

    public partial class LogWindow
    {
        public LogWindow()
        {
            InitializeComponent();
        }

        public LogWindow(IEnumerable<LogMessage> logs)
            : this()
        {
            DataContext = new LogWindowViewModel(logs);
        }

        public static void ShowLogs(IEnumerable<LogMessage> logs)
        {
            new LogWindow(logs).ShowDialog();
        }
    }
}