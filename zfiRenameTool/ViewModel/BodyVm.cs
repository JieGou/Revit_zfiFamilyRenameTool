namespace zfiRenameTool.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstractions;
    using Autodesk.Revit.DB;
    using MicroMvvm;
    using Services;

    public class BodyVm : ViewModelBase, IDisposable
    {
        private readonly RevitService _service;

        public BodyVm()
        {
        }

        public BodyVm(
            RevitService service,
            IReadOnlyCollection<Document> docs,
            OptionsVm optionsVm)
        {
            Docs = docs;
            _service = service;
            Tabs = _service.Providers.Select(x => new TabVm(x, docs, optionsVm)).ToList();
        }

        public IReadOnlyCollection<Document> Docs { get; }

        public IReadOnlyCollection<TabVm> Tabs { get; }

        public void Dispose()
        {
            _service.CloseDocs(Docs);
        }

        public IReadOnlyCollection<IRenameable> GetRenameables()
        {
            return Tabs.SelectMany(x => x.Renameables.Where(r => r.IsChecked)).ToList();
        }
    }
}