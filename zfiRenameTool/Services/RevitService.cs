namespace zfiRenameTool.Services
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Abstractions;
    using Autodesk.Revit.DB;
    using Revit;
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

        public event EventHandler Renamed;

        public List<IRenameableProvider> Providers => _providers;

        public IReadOnlyCollection<Document> LoadDocs(IList<string> fileNames)
        {
            var documents = new List<Document>();
            foreach (var fileName in fileNames)
            {
                try
                {
                    var doc = _app.OpenDocumentFile(fileName);
                    documents.Add(doc);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Не удалось загрузить документы", "Внимание!", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }

            return documents;
        }

        public void Rename(IReadOnlyCollection<IRenameable> renameables)
        {
            _event.Run(app =>
            {
                foreach (var renameable in renameables)
                {
                    renameable.Rename();
                }

                OnRenamed();
            });
        }

        protected virtual void OnRenamed()
        {
            Renamed?.Invoke(this, EventArgs.Empty);
        }

        private void SetupProvides()
        {
            _providers = new List<IRenameableProvider>();
            Providers.Add(new FamilyParametersProvider());
        }

        public void SaveAllDocs(IReadOnlyCollection<Document> docs)
        {
            foreach (Document doc in docs)
            {
                if (doc.IsFamilyDocument)
                {
                    doc.Save();
                }
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