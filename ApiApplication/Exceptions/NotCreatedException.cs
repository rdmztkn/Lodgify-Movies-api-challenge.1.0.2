using System;

namespace ApiApplication.Exceptions
{
    public class NotCreatedException : Exception
    {
        public NotCreatedException(string message) : base(message)
        {
        }
    }
}