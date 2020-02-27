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
        private readonly IReadOnlyCollection<Document> _docs;
        private readonly OptionsViewModel _optionsViewModel;
        private bool _allSelected;

        public TabViewModel(IRenameableProvider provider, IReadOnlyCollection<Document> docs, OptionsViewModel optionsViewModel)
        {
            _provider = provider;
            _docs = docs;
            _optionsViewModel = optionsViewModel;

            Renameables = new ObservableCollection<RenameableViewModel>();

            FillRenameables();

            _optionsViewModel.PropertyChanged += (s, e) => OptionsVmOnPropertyChanged();
        }

        public ObservableCollection<RenameableViewModel> Renameables { get; }

        public string Title => _provider.Name;

        public TabItemType TabItemType => _provider.TabItemType;
        
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

        public void Reload()
        {
            Renameables.Clear();

            FillRenameables();

            AllSelected = false;
        }

        private void OptionsVmOnPropertyChanged()
        {
            Renameables.Where(x => x.IsChecked)
                .ToList()
                .ForEach(_optionsViewModel.Rename);
        }

        private void FillRenameables()
        {
            var renameables = new List<IRenameable>();
            var ordinalStringComparer = new ModPlusAPI.IO.OrdinalStringComparer();
            foreach (var doc in _docs)
            {
                renameables.AddRange(_provider.GetRenameables(doc).OrderBy(r => r.GroupCondition, ordinalStringComparer));
            }

            foreach (var renameableVm in renameables
                .GroupBy(x => x.GroupCondition)
                .Select(x => new RenameableViewModel(x.ToList())))
            {
                renameableVm.Checked += (sender, b) => OptionsVmOnPropertyChanged();
                Renameables.Add(renameableVm);
            }
        }
    }
}