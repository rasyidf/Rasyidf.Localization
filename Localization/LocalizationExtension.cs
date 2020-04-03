using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Rasyidf.Localization
{
    /// <summary>
    /// Localize Extension
    ///  Tr [default], uid=[uid]
    /// </summary>
    [ContentProperty("Parameters")]
    public class Tr : MarkupExtension
    {
        #region Fields

        private DependencyProperty _property;
        private DependencyObject _target;

        #endregion Fields

        #region Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public Tr()
        {
        }

        /// <summary>
        /// Constructor with default value
        /// </summary>
        /// <param name="defaultValue"></param>
        public Tr(object defaultValue)
        {
            Default = defaultValue;
        }

        #endregion Initialization

        #region Properties

        /// <summary>
        /// Get or set the default value
        /// </summary>
        public object Default { get; set; }

        /// <summary>
        /// Get or set the Unique Identifier
        /// </summary>
        public string Uid { get; set; }

        public string Vid { get; set; }

        /// <summary>
        /// Parameters Collection
        /// </summary>
        public Collection<BindingBase> Parameters { get; } = new Collection<BindingBase>();

        #region UidProperty DProperty

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string GetUid(DependencyObject instance)
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return (string)instance.GetValue(UidProperty);
        }

        public static void SetUid(DependencyObject instance, string value)
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            instance.SetValue(UidProperty, value);
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty UidProperty =
            DependencyProperty.RegisterAttached("Uid", typeof(string), typeof(Tr), new UIPropertyMetadata(string.Empty));

        #endregion UidProperty DProperty

        #region VidProperty DProperty

        public static readonly DependencyProperty VidProperty = DependencyProperty.RegisterAttached(
            "Vid", typeof(string), typeof(Tr), new PropertyMetadata(default(string)));

        public static void SetVid(DependencyObject element, string value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            element.SetValue(VidProperty, value);
        }

        public static string GetVid(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            return (string)element.GetValue(VidProperty);
        }
        #endregion

        #endregion Properties

        #region Overrides

        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

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

        private object BindDictionary(IServiceProvider serviceProvider)
        {
            var uid = Uid ?? GetUid(_target);
            var vid = string.IsNullOrEmpty(Vid ?? GetVid(_target)) ? _property.Name : Vid ?? GetVid(_target);

            var binding = new Binding("LanguagePack")
            {
                Source = LocalizationService.Current,
                Mode = BindingMode.TwoWay
            };

            var converter = new LocalizationConverter(uid, vid, Default);
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
                var bindingBase = (Binding)i;
                multiBinding.Bindings.Add(bindingBase);
            }

            return multiBinding.ProvideValue(serviceProvider);
        }

        #endregion Privates
    }
}