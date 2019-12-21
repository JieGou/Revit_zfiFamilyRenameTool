namespace zfiFamilyRenameTool.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstractions;
    using Autodesk.Revit.DB;
    using ModPlusAPI.Mvvm;
    using Services;

    public class BodyViewModel : VmBase, IDisposable
    {
        private readonly RevitService _service;

        public BodyViewModel()
        {
        }

        public BodyViewModel(
            RevitService service,
            IReadOnlyCollection<Document> docs,
            OptionsViewModel optionsViewModel)
        {
            Docs = docs;
            _service = service;
            Tabs = _service.Providers.Select(x => new TabViewModel(x, docs, optionsViewModel)).ToList();
        }

        public IReadOnlyCollection<Document> Docs { get; }

        public IReadOnlyCollection<TabViewModel> Tabs { get; }

        public void Dispose()
        {
            _service.CloseDocs(Docs);
        }

        public IReadOnlyCollection<IRenameable> GetRenameables()
        {
            return Tabs.SelectMany(x => x.Renameables.Where(r => r.IsChecked)).ToList();
        }

        public void ReloadRenameables()
        {
            foreach (var tab in Tabs)
            {
                tab.Reload();
            }
        }
    }
}