namespace zfiFamilyRenameTool.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Abstractions;
    using Autodesk.Revit.DB;
    using MicroMvvm;

    public class TabVm : ViewModelBase
    {
        private readonly IRenameableProvider _provider;
        private readonly OptionsVm _optionsVm;
        private bool _allSelected;

        public TabVm(IRenameableProvider provider, IReadOnlyCollection<Document> docs, OptionsVm optionsVm)
        {
            _provider = provider;
            _optionsVm = optionsVm;

            Renameables = new ObservableCollection<RenameableVm>();

            foreach (var doc in docs)
            {
                var renameables = provider.GetRenameables(doc);
                foreach (var renameableVm in renameables.GroupBy(x => x.Source)
                    .Select(x => new RenameableVm(x.ToList())))
                {
                    renameableVm.Checked += (sender, b) => OptionsVmOnPropertyChanged();
                    Renameables.Add(renameableVm);
                }
            }

            _optionsVm.PropertyChanged += (s, e) => OptionsVmOnPropertyChanged();
        }

        public ObservableCollection<RenameableVm> Renameables { get; }

        public string Title => _provider.Name;

        public bool AllSelected
        {
            get => _allSelected;
            set
            {
                _allSelected = value;
                foreach (var renameableVm in Renameables)
                {
                    renameableVm.IsChecked = value;
                }

                RaisePropertyChanged();
            }
        }

        private void OptionsVmOnPropertyChanged()
        {
            Renameables.Where(x => x.IsChecked)
                .ToList()
                .ForEach(_optionsVm.Rename);
        }
    }
}