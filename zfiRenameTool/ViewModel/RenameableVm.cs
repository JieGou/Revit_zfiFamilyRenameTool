namespace zfiRenameTool.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstractions;
    using MicroMvvm;

    public class RenameableVm : ViewModelBase, IRenameable
    {
        private readonly List<IRenameable> _renameable;
        private bool _isChecked;

        public RenameableVm(List<IRenameable> renameable)
        {
            _renameable = renameable;
        }

        public event EventHandler<bool> Checked;

        public string Title => _renameable.First().Title;

        public string Source => _renameable.First().Source;

        public string Destination
        {
            get => _renameable.First().Destination;
            set
            {
                _renameable.ForEach(x => x.Destination = value);
                RaisePropertyChanged();
            }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                if (!_isChecked)
                {
                    Destination = string.Empty;
                }

                OnChecked(value);
                RaisePropertyChanged();
            }
        }

        public void Rename()
        {
            _renameable.ForEach(x => x.Rename());
        }

        protected virtual void OnChecked(bool e)
        {
            Checked?.Invoke(this, e);
        }
    }
}