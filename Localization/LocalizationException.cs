using System.Collections;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Rasyidf.Localization
{
    [Serializable]
    public class LocalizationException : Exception
    {
        public override string Message => base.Message;

        public override IDictionary Data => base.Data;

        public override string StackTrace => base.StackTrace;

        public override string HelpLink { get => base.HelpLink; set => base.HelpLink = value; }
        public override string Source { get => base.Source; set => base.Source = value; }
        public LocalizerError LocalizationError { get; private set; }

        public LocalizationException(string message, LocalizerError error) : base(message)
        {
            LocalizationError = error;
        }

        public LocalizationException(string message, LocalizerError error, Exception innerException) : base(message, innerException)
        {
            LocalizationError = error;
        }

        public LocalizationException(LocalizerError error)
        {
            LocalizationError = error;
        }

        protected LocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public LocalizationException()
        {
        }

        public LocalizationException(string message) : base(message)
        {
        }

        public LocalizationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}