namespace zfiFamilyRenameTool.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using Abstractions;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Revit;
    using View;
    using ViewModel;

    public class RevitService
    {
        private readonly Autodesk.Revit.ApplicationServices.Application _app;
        private readonly RevitEvent _revitEvent;

        public RevitService(Autodesk.Revit.ApplicationServices.Application app, RevitEvent revitEvent)
        {
            _app = app;
            _revitEvent = revitEvent;

            SetupProvides();
        }

        public event EventHandler<IEnumerable<LogMessage>> Renamed;

        public List<IRenameableProvider> Providers { get; private set; }

        public IReadOnlyCollection<Document> LoadDocs()
        {
            var logs = new List<LogMessage>();
            var documents = new List<Document>();
            var fileNames = GetFilesNames();
            fileNames = CheckIsReadOnly(fileNames);
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
            _revitEvent.Run(app =>
            {
                var logs = new List<LogMessage>();
                foreach (var renameable in renameables)
                {
                    try
                    {
                        renameable.Rename();
                        logs.Add(new LogMessage(
                            renameable.Title,
                            $"Переименован из {renameable.Source} в {renameable.Destination}"));
                    }
                    catch
                    {
                        logs.Add(new LogMessage(
                            renameable.ToString(),
                            $"Не удалось переименовать"));
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
            Providers = new List<IRenameableProvider>
            {
                new FamilyParametersProvider(),
                new FamilyTypesProvider(),
                new FamilyParameterValuesProvider()
            };
        }

        public void SaveAllDocs(IReadOnlyCollection<Document> docs)
        {
            var logs = new List<LogMessage>();
            foreach (var doc in docs)
            {
                try
                {
                    doc.Save();
                }
                catch (Exception exception)
                {
                    logs.Add(new LogMessage("Не удалось сохранить документ", $"{doc.PathName} - {exception.Message}", true));
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

        private IEnumerable<string> GetFilesNames()
        {
            var fbd = new FolderBrowserDialog();

            var taskDialog = new TaskDialog("Выбор файлов");
            taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Выбрать файлы");
            taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Выбрать папку");
            taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink3, "Выбрать папку включая вложенные");
            var taskDialogResult = taskDialog.Show();

            switch (taskDialogResult)
            {
                case TaskDialogResult.CommandLink1:
                    var ofd = new Microsoft.Win32.OpenFileDialog
                    {
                        Multiselect = true,
                        Filter = "Revit families (*.rfa) | *.rfa"
                    };

                    if (ofd.ShowDialog() == true)
                    {
                        return ofd.FileNames;
                    }

                    break;
                case TaskDialogResult.CommandLink2:
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        return Directory.EnumerateFiles(fbd.SelectedPath, "*.rfa", SearchOption.TopDirectoryOnly);
                    }

                    break;
                case TaskDialogResult.CommandLink3:
                    fbd = new FolderBrowserDialog();
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        return Directory.EnumerateFiles(fbd.SelectedPath, "*.rfa", SearchOption.AllDirectories);
                    }

                    break;
            }

            return new List<string>();
        }

        private IEnumerable<string> CheckIsReadOnly(IEnumerable<string> files)
        {
            files = files.ToList();
            var fileInfos = files.Select(f => new FileInfo(f)).ToList();
            if (fileInfos.Any(fi => fi.IsReadOnly))
            {
                if (ModPlusAPI.Windows.MessageBox.ShowYesNo("Set ReadOnly to false?"))
                {
                    foreach (var fileInfo in fileInfos)
                    {
                        if (fileInfo.IsReadOnly)
                        {
                            File.SetAttributes(fileInfo.FullName, File.GetAttributes(fileInfo.FullName) & ~FileAttributes.ReadOnly);
                        }

                        yield return fileInfo.FullName;
                    }
                }
                else
                {
                    foreach (var fileInfo in fileInfos)
                    {
                        if (!fileInfo.IsReadOnly)
                            yield return fileInfo.FullName;
                    }
                }
            }
            else
            {
                foreach (var file in files)
                {
                    yield return file;
                }
            }
        }
    }
}