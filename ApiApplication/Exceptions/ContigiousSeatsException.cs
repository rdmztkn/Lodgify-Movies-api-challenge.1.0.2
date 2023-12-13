using System;

namespace ApiApplication.Exceptions
{
    public class ContigiousSeatsException : Exception
    {
        public ContigiousSeatsException(string message) : base(message)
        {
        }
    }
}