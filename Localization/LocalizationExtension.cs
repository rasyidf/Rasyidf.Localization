namespace Rasyidf.Localization
{
    /// <summary>
    /// Localize Extension
    /// use Tr [default], uid=[uid]
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
            var vid = _property.Name;

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