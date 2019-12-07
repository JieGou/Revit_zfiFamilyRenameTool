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
        private readonly IReadOnlyCollection<Document> _docs;
        private readonly RevitService _service;

        public BodyVm()
        {
        }

        public BodyVm(
            IReadOnlyCollection<IRenameableProvider> serviceProviders,
            IReadOnlyCollection<Document> docs,
            RevitService service,
            OptionsVm optionsVm)
        {
            _docs = docs;
            _service = service;
            Tabs = serviceProviders.Select(x => new TabVm(x, docs, optionsVm)).ToList();
        }

        public IReadOnlyCollection<TabVm> Tabs { get; }

        public void Dispose()
        {
            _service.CloseDocs(_docs);
        }

        public IReadOnlyCollection<IRenameable> GetRenameables()
        {
            return Tabs.SelectMany(x => x.Renameables.Where(r => r.IsChecked)).ToList();
        }
    }
}