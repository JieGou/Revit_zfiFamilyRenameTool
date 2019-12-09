namespace zfiRenameTool.View
{
    using System.Collections.Generic;
    using System.Windows;
    using ViewModel;

    public partial class LogWindow : Window
    {
        public LogWindow()
        {
            InitializeComponent();
        }

        public LogWindow(IEnumerable<LogMessage> logs)
            : this()
        {
            DataContext = new LogWindowVm(logs);
        }

        public static void ShowLogs(IEnumerable<LogMessage> logs)
        {
            new LogWindow(logs).ShowDialog();
        }
    }
}