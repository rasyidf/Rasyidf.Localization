namespace Rasyidf.Localization
{
    /// <summary>
    /// Specifies error conditions that may be encountered by Localizer Service
    /// </summary>
    public enum LocalizerError
    {
        /// <summary>
        /// More than one element has the same Uid
        /// value.
        /// </summary>
        DuplicateUid = 0,

        /// <summary>
        /// The localized Dictionary contains more than one reference to the same element.
        /// </summary>
        DuplicateElement = 1,

        /// <summary>
        /// The element's substitution contains incomplete child placeholders.
        /// </summary>
        IncompleteElementPlaceholder = 2,

        /// <summary>
        /// XML comments do not have the correct format.
        /// </summary>
        InvalidCommentingXml = 3,

        /// <summary>
        /// The localization commenting text contains invalid attributes.
        /// </summary>
        InvalidLocalizationAttributes = 4,

        /// <summary>
        /// The localization commenting text contains invalid comments.
        /// </summary>
        InvalidLocalizationComments = 5,

        /// <summary>
        /// The Uid does not correspond to any element in the source.
        /// </summary>
        InvalidUid = 6,

        /// <summary>
        /// Indicates a mismatch between substitution and source. The substitution must contain all the element placeholders in the source.
        /// </summary>
        MismatchedElements = 7,

        /// <summary>
        /// The substitution of an element's content cannot be parsed as XML, therefore any
        /// formatting tags in the substitution are not recognized. The substitution is instead
        /// applied as plain text.
        /// </summary>
        SubstitutionAsPlaintext = 8,

        /// <summary>
        /// A child element does not have a System.Windows.Markup.Localizer.BamlLocalizableResourceKey.Uid.
        /// As a result, it cannot be represented as a placeholder in the parent's content
        /// string.
        /// </summary>
        UidMissingOnChildElement = 9,

        /// <summary>
        /// A formatting tag in the substitution is not recognized.
        /// </summary>
        UnknownFormattingTag = 10,

        FormattingFailed = 11,
        Unknown = 12
    }
}