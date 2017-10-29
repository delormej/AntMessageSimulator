using System;

namespace AntMessageSimulator
{
    public class MessageException : ApplicationException
    {
        public MessageException(string message) : base(message)
        {
        }

        public MessageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
