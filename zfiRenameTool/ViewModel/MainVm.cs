namespace zfiRenameTool.ViewModel
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Abstractions;
    using MicroMvvm;
    using Microsoft.Win32;
    using Services;

    public class MainVm : ViewModelBase
    {
        private readonly RevitService _service;
        private BodyVm _body;

        public MainVm(RevitService service)
        {
            _service = service;
            Options = new OptionsVm();
        }

        public ICommand ApplyCmd => new RelayCommand(Apply);

        public ICommand CloseCmd => new RelayCommand<ICloseable>(Close);

        public ICommand CloseAndApplyCmd => new RelayCommand<ICloseable>(CloseAndApply);

        public ICommand OpenFamiliesCmd => new RelayCommand(OpenFamilies);

        public BodyVm Body
        {
            get => _body;
            set
            {
                _body = value;
                RaisePropertyChanged();
            }
        }

        public OptionsVm Options { get; }

        private void OpenFamilies()
        {
            var ofd = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Revit families (*.rfa) | *.rfa"
            };
            if (ofd.ShowDialog() == true && ofd.FileNames.Length > 0)
            {
                var docs = _service.LoadDocs(ofd.FileNames);
                Body = new BodyVm(_service.Providers, docs, _service, Options);
            }
            else
            {
                MessageBox.Show("Не выбранно ни одного файла семейства!", "Внимание!");
            }
        }

        private void CloseAndApply(ICloseable closeable)
        {
            Apply();
            _service.Renamed += (s, e) =>
            {
                _service.SaveAllDocs();
                Close(closeable);
            };
        }

        private void Close(ICloseable closeable)
        {
            closeable.Close();
            Body?.Dispose();
        }

        private void Apply()
        {
            var renameables = Body.GetRenameables();
            if (renameables.Count == 0)
            {
                MessageBox.Show("Ничего не выбранно!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _service.Rename(renameables);
        }
    }
}