using System;
using System.Runtime.Serialization;

namespace Diplom.other
{
    public class Exeptions : Exception
    {
        public string AdditionalErrorInfo { get; set; }

        public Exeptions(string message, string additionalInfo) : base(message)
        {
            AdditionalErrorInfo = additionalInfo;
        }

        public Exeptions(string message) : base(message)
        {
        }

        public Exeptions(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected Exeptions(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
