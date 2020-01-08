using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Rasyidf.Localization
{
    [ContentProperty("Parameters")]
    public class Tr : MarkupExtension
    {
        #region Fields

        DependencyProperty _property;
        DependencyObject _target;

        #endregion Fields

        #region Initialization

        public Tr()
        {
        }

        public Tr(object defaultValue)
        {
            Default = defaultValue;
        }

        #endregion Initialization

        #region Properties

        public object Default { get; set; }

        public string Uid { get; set; }

        public Collection<BindingBase> Parameters { get; } = new Collection<BindingBase>();

        #region UidProperty DProperty

        public static string GetUid(DependencyObject obj)
        {
            return (string)obj.GetValue(UidProperty);
        }

        public static void SetUid(DependencyObject obj, string value)
        {
            obj.SetValue(UidProperty, value);
        }

        public static readonly DependencyProperty UidProperty =
            DependencyProperty.RegisterAttached("Uid", typeof(string), typeof(Tr), new UIPropertyMetadata(string.Empty));

        #endregion UidProperty DProperty

        #endregion Properties

        #region Overrides

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget service))
            {
                return this;
            }
            
            if (!(service.TargetProperty is DependencyProperty property) || !(service.TargetObject is DependencyObject target))
            {
                return this;
            }

            _target = target;
            _property = property;

            return BindDictionary(serviceProvider);
        }

        #endregion Overrides

        #region Privates

        object BindDictionary(IServiceProvider serviceProvider)
        {
            var uid = Uid ?? GetUid(_target);
            var vid = _property.Name;

            var binding = new Binding("LanguagePack")
            {
                Source = LanguageService.Current,
                Mode = BindingMode.TwoWay
            };

            var converter = new LanguageConverter(uid, vid, Default);
            if (Parameters.Count == 0)
            {
                binding.Converter = converter;
                return binding.ProvideValue(serviceProvider);
            }

            var multiBinding = new MultiBinding()
            {
                Mode = BindingMode.OneWay,
                Converter = converter
            };

            multiBinding.Bindings.Add(binding);
            if (string.IsNullOrEmpty(uid) && !(Parameters[0] is Binding))
            {
                throw new ArgumentException("Uid Binding parameter must be the first, and of type Binding");
            }

            foreach (var i in Parameters)
            {
                var bindingBase = (Binding) i;
                multiBinding.Bindings.Add(bindingBase);
            }
            return multiBinding.ProvideValue(serviceProvider);
        }

        #endregion Privates
    }
}