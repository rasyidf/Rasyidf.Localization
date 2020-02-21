using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Rasyidf.Localization {
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class LocalizedString : INotifyPropertyChanged {
        private string _uid;
        private string _vid;
        private string _default;
        private string _value;

        public string Uid {
            get => _uid;
            set {
                _uid = value;
                ReloadLocalization();
            }
        }

        public string Vid {
            get => _vid;
            set {
                _vid = value;
                ReloadLocalization();
            }
        }

        public string Default {
            get => _default;
            set {
                _default = value;
                ReloadLocalization();
            }
        }

        public string Value {
            get => _value;
            private set {
                _value = value;
                RaisePropertyChanged();
            }
        }

        public LocalizedString(string uid, string vid, string @default = "") {
            Uid = uid;
            Vid = vid;
            Default = @default;
            Initialize();
        }

        private void Initialize() {
            ReloadLocalization();
            LocalizationService.Current.PropertyChanged += (sender, args) => ReloadLocalization();
        }

        public void Set(string uid, string vid, string @default) {
            _uid = uid;
            _vid = vid;
            _default = @default;
            ReloadLocalization();
        }

        public void Set(string uid, string vid) {
            Set(uid, vid, string.Empty);
        }

        private void ReloadLocalization() {
            Value = LocalizationService.GetString(Uid, Vid, Default);
        }

        public override string ToString() {
            return Value;
        }

        public static implicit operator string(LocalizedString s) {
            return s.Value;
        }

        private string DebuggerDisplay => $"Loc: {Uid}.{Vid} - {Default}\n{Value}";

        public event PropertyChangedEventHandler PropertyChanged;

        [Localizable(false)]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propName = null) {
            var e = PropertyChanged;
            e?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}