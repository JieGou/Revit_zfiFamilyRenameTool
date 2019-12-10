namespace zfiFamilyRenameTool.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;
    using Abstractions;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Revit;
    using View;
    using ViewModel;
    using Application = Autodesk.Revit.ApplicationServices.Application;
    using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

    public class RevitService
    {
        private readonly Application _app;
        private readonly RevitEvent _event;
        private List<IRenameableProvider> _providers;

        public RevitService(Application app, RevitEvent @event)
        {
            _app = app;
            _event = @event;

            SetupProvides();
        }

        public event EventHandler<IEnumerable<LogMessage>> Renamed;

        public List<IRenameableProvider> Providers => _providers;

        public IReadOnlyCollection<Document> LoadDocs()
        {
            var logs = new List<LogMessage>();
            var documents = new List<Document>();
            var fileNames = GetFilesNames();
            foreach (var fileName in fileNames)
            {
                try
                {
                    var doc = _app.OpenDocumentFile(fileName);
                    documents.Add(doc);
                }
                catch
                {
                    logs.Add(new LogMessage("Не удалось загрузить документ", fileName, true));
                }
            }

            if (logs.Count > 0)
            {
                LogWindow.ShowLogs(logs);
            }

            return documents;
        }

        public void Rename(IReadOnlyCollection<IRenameable> renameables)
        {
            _event.Run(app =>
            {
                var logs = new List<LogMessage>();
                foreach (var renameable in renameables)
                {
                    try
                    {
                        renameable.Rename();
                        logs.Add(new LogMessage(
                            renameable.Title,
                            $"Переименнован из {renameable.Source} в {renameable.Destination}"));
                    }
                    catch
                    {
                        logs.Add(new LogMessage(
                            renameable.ToString(),
                            $"Не удалось переимменовать"));
                    }
                }

                OnRenamed(logs);
            });
        }

        protected virtual void OnRenamed(IEnumerable<LogMessage> logs)
        {
            Renamed?.Invoke(this, logs);
        }

        private void SetupProvides()
        {
            _providers = new List<IRenameableProvider>();
            Providers.Add(new FamilyParametersProvider());
        }

        public void SaveAllDocs(IReadOnlyCollection<Document> docs)
        {
            var logs = new List<LogMessage>();
            foreach (Document doc in docs)
            {
                try
                {
                    doc.Save();
                }
                catch
                {
                    logs.Add(new LogMessage("Не удалось сохранить документ", doc.PathName, true));
                }
            }

            if (logs.Count > 0)
            {
                LogWindow.ShowLogs(logs);
            }
        }

        public void CloseDocs(IReadOnlyCollection<Document> docs)
        {
            foreach (var doc in docs)
            {
                doc.Close(false);
            }
        }

        private IReadOnlyCollection<string> GetFilesNames()
        {
            var result = new List<string>();

            var fbd = new FolderBrowserDialog();

            var taskDialog = new TaskDialog("Выбор файлов");
            taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Выбрать файлы");
            taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Выбрать папку");
            taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink3, "Выбрать папку включая вложенные");
            var taskDialogResult = taskDialog.Show();

            switch (taskDialogResult)
            {
                case TaskDialogResult.CommandLink1:
                    var ofd = new OpenFileDialog
                    {
                        Multiselect = true,
                        Filter = "Revit families (*.rfa) | *.rfa"
                    };

                    if (ofd.ShowDialog() == true)
                    {
                        result.AddRange(ofd.FileNames);
                    }

                    break;
                case TaskDialogResult.CommandLink2:
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        result.AddRange(Directory.EnumerateFiles(fbd.SelectedPath, "*.rfa",
                            SearchOption.TopDirectoryOnly));
                    }

                    break;
                case TaskDialogResult.CommandLink3:
                    fbd = new FolderBrowserDialog();
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        result.AddRange(Directory.EnumerateFiles(fbd.SelectedPath, "*.rfa",
                            SearchOption.AllDirectories));
                    }

                    break;
            }

            return result;
        }
    }
}