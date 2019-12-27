namespace zfiFamilyRenameTool.ViewModel
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using Abstractions;
    using ModPlusAPI;
    using ModPlusAPI.Mvvm;
    using ModPlusAPI.Windows;
    using Revit;
    using Services;
    using View;

    public class MainViewModel : VmBase
    {
        private readonly RevitService _service;
        private BodyViewModel _body;

        public MainViewModel(RevitService service)
        {
            _service = service;
            Options = new OptionsViewModel();
        }

        public ICommand ApplyCommand => new RelayCommandWithoutParameter(ApplyAndShowLogs);

        public ICommand CloseCommand => new RelayCommand<ICloseable>(Close);

        public ICommand CloseAndApplyCommand => new RelayCommand<ICloseable>(CloseAndApply);

        public ICommand OpenFamiliesCommand => new RelayCommandWithoutParameter(OpenFamilies);

        public BodyViewModel Body
        {
            get => _body;
            set
            {
                _body = value;
                OnPropertyChanged();
            }
        }

        public OptionsViewModel Options { get; }

        private void OpenFamilies()
        {
            var docs = _service.LoadDocs();
            if (docs.Count == 0)
            {
                // Не выбрано ни одного файла семейства!
                MessageBox.Show(Language.GetItem(ModPlusConnector.Instance.Name, "msg4"), MessageBoxIcon.Alert);
                return;
            }

            Body = new BodyViewModel(_service, docs, Options);

            RenamerCommand.RenamerWindow.Activate();
        }

        private void CloseAndApply(ICloseable closeable)
        {
            _service.Renamed += (s, e) =>
            {
                LogWindow.ShowLogs(e);
                _service.SaveAllDocs(_body.Docs);
                Close(closeable);
            };
            Apply();
        }

        private void Close(ICloseable closeable)
        {
            closeable.Close();
            Body?.Dispose();
        }

        private void ApplyAndShowLogs()
        {
            _service.Renamed += SaveAnShowLogs;
            Apply();
        }

        private void Apply()
        {
            var renameables = Body.GetRenameables();
            if (renameables.Count == 0)
            {
                // Не выбрано значений для изменения!
                MessageBox.Show(Language.GetItem(ModPlusConnector.Instance.Name, "msg5"), MessageBoxIcon.Alert);
                return;
            }

            _service.Rename(renameables);
        }

        private void SaveAnShowLogs(object sender, IEnumerable<LogMessage> e)
        {
            LogWindow.ShowLogs(e);
            _service.SaveAllDocs(_body.Docs);
            _service.Renamed -= SaveAnShowLogs;
            Body.ReloadRenameables();
        }
    }
}