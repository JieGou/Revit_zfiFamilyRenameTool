namespace zfiFamilyRenameTool.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Abstractions;
    using Autodesk.Revit.DB;
    using ModPlusAPI.Mvvm;

    public class TabViewModel : VmBase
    {
        private readonly IRenameableProvider _provider;
        private readonly OptionsViewModel _optionsViewModel;
        private bool _allSelected;

        public TabViewModel(IRenameableProvider provider, IReadOnlyCollection<Document> docs, OptionsViewModel optionsViewModel)
        {
            _provider = provider;
            _optionsViewModel = optionsViewModel;

            Renameables = new ObservableCollection<RenameableViewModel>();

            foreach (var doc in docs)
            {
                var renameables = provider.GetRenameables(doc);
                foreach (var renameableVm in renameables.GroupBy(x => x.Source)
                    .Select(x => new RenameableViewModel(x.ToList())))
                {
                    renameableVm.Checked += (sender, b) => OptionsVmOnPropertyChanged();
                    Renameables.Add(renameableVm);
                }
            }

            _optionsViewModel.PropertyChanged += (s, e) => OptionsVmOnPropertyChanged();
        }

        public ObservableCollection<RenameableViewModel> Renameables { get; }

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

                OnPropertyChanged();
            }
        }

        private void OptionsVmOnPropertyChanged()
        {
            Renameables.Where(x => x.IsChecked)
                .ToList()
                .ForEach(_optionsViewModel.Rename);
        }
    }
}