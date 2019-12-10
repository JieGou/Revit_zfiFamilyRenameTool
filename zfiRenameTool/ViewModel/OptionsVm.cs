namespace zfiFamilyRenameTool.ViewModel
{
    using System;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using Abstractions;
    using MicroMvvm;

    public partial class OptionsVm : ViewModelBase
    {
        private RenameOption _currentRenameOption = RenameOption.StartWith;
        private bool _isCaseSensitive;
        private string _find = string.Empty;
        private string _replaceValue = string.Empty;
        private string _prefix = string.Empty;
        private string _suffix = string.Empty;

        public ICommand OptionCheckedCmd => new RelayCommand<string>(OptionChecked);

        public bool IsCaseSensitive
        {
            get => _isCaseSensitive;
            set
            {
                _isCaseSensitive = value;
                RaisePropertyChanged();
            }
        }

        public string Find
        {
            get => _find;
            set
            {
                _find = value;
                RaisePropertyChanged();
            }
        }

        public string ReplaceValue
        {
            get => _replaceValue;
            set
            {
                _replaceValue = value;
                RaisePropertyChanged();
            }
        }

        public string Prefix
        {
            get => _prefix;
            set
            {
                _prefix = value;
                RaisePropertyChanged();
            }
        }

        public string Suffix
        {
            get => _suffix;
            set
            {
                _suffix = value;
                RaisePropertyChanged();
            }
        }

        public Action<IRenameable> Rename
        {
            get
            {
                Func<IRenameable, string> func = i => i.Source;

                if (!string.IsNullOrEmpty(Find))
                {
                    switch (_currentRenameOption)
                    {
                        case RenameOption.StartWith:
                            func = i =>
                            {
                                if (_isCaseSensitive
                                    ? i.Source.ToLower().StartsWith(Find.ToLower())
                                    : i.Source.StartsWith(Find))
                                {
                                    return ReplaceValue + i.Source.Substring(Find.Length);
                                }

                                return i.Source;
                            };
                            break;
                        case RenameOption.EndWith:
                            func = i =>
                            {
                                if (_isCaseSensitive
                                    ? i.Source.ToLower().EndsWith(Find.ToLower())
                                    : i.Source.EndsWith(Find))
                                {
                                    return i.Source.Substring(0, i.Source.Length - Find.Length) +
                                           ReplaceValue;
                                }

                                return i.Source;
                            };
                            break;
                        case RenameOption.Contains:
                            func = i => Regex.Replace(i.Source, Find, ReplaceValue,
                                _isCaseSensitive ? RegexOptions.IgnoreCase : RegexOptions.None);
                            break;
                        case RenameOption.MatchesWhole:
                            func = i => string.Equals(i.Source, Find,
                                _isCaseSensitive
                                    ? StringComparison.InvariantCultureIgnoreCase
                                    : StringComparison.InvariantCulture)
                                ? ReplaceValue
                                : i.Source;
                            break;
                        case RenameOption.Regex:
                            func = i => Regex.Replace(i.Source, Find, ReplaceValue);
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(Prefix))
                {
                    var oldFunc = func;
                    func = i => Prefix + oldFunc(i);
                }

                if (!string.IsNullOrEmpty(Suffix))
                {
                    var oldFunc = func;
                    func = i => oldFunc(i) + Suffix;
                }

                return i => i.Destination = i.CanRename() ? func(i) : i.Source;
            }
        }

        private void OptionChecked(string s)
        {
            _currentRenameOption = (RenameOption)Enum.Parse(typeof(RenameOption), s);
            RaisePropertyChanged();
        }
    }
}