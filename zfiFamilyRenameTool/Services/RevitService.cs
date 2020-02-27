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
    using ModPlusAPI;
    using Revit;
    using View;
    using ViewModel;

    public class RevitService
    {
        private readonly Autodesk.Revit.ApplicationServices.Application _app;
        private readonly RevitEvent _revitEvent;
        private readonly string _langItem = ModPlusConnector.Instance.Name;

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
                catch (Exception exception)
                {
                    // Не удалось загрузить документ
                    logs.Add(new LogMessage(Language.GetItem(_langItem, "err2"), $"{fileName} - {exception.Message}", true));
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

                        // Выполнено изменение значения с "{0}" на "{1}"
                        logs.Add(new LogMessage(
                            renameable.Title,
                            string.Format(Language.GetItem(_langItem, "msg1"), renameable.Source, renameable.Destination)));
                    }
                    catch (Exception exception)
                    {
                        // Failed to change value
                        logs.Add(new LogMessage(
                            renameable.Title,
                            $"{Language.GetItem(_langItem, "msg2")} - {exception.Message}"));
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
                new FamilyParameterValuesProvider(),
                new FamilyIsInstanceParametersProvider()
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
                    // Не удалось сохранить документ
                    logs.Add(new LogMessage(
                        Language.GetItem(_langItem, "err3"), $"{doc.PathName} - {exception.Message}", true));
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

            // Выбрать семейства
            var taskDialog = new TaskDialog(Language.GetItem(_langItem, "h1"));

            // Выбрать файлы
            taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, Language.GetItem(_langItem, "h20"));

            // Выбрать папку
            taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, Language.GetItem(_langItem, "h21"));

            // Выбрать папку включая вложенные
            taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink3, Language.GetItem(_langItem, "h22"));
            var taskDialogResult = taskDialog.Show();

            switch (taskDialogResult)
            {
                case TaskDialogResult.CommandLink1:
                    var ofd = new Microsoft.Win32.OpenFileDialog
                    {
                        Multiselect = true,

                        // Семейства Revit
                        Filter = $"{Language.GetItem(_langItem, "h23")} (*.rfa) | *.rfa"
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
                // Среди выбранных файлов семейств имеются файлы со свойством «Только для чтения».
                // Эти файлы невозможно будет сохранить. Отключить для этих файлов свойство «Только для чтения»?
                if (ModPlusAPI.Windows.MessageBox.ShowYesNo(Language.GetItem(_langItem, "msg3")))
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