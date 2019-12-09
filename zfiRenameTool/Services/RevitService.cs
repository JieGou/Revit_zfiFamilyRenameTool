namespace zfiRenameTool.Services
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Abstractions;
    using Autodesk.Revit.DB;
    using Revit;
    using View;
    using ViewModel;
    using Application = Autodesk.Revit.ApplicationServices.Application;

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

        public IReadOnlyCollection<Document> LoadDocs(IList<string> fileNames)
        {
            var logs = new List<LogMessage>();
            var documents = new List<Document>();
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
    }
}