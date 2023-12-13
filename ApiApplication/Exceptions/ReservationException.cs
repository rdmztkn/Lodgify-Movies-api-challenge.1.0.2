using System;

namespace ApiApplication.Exceptions
{
    public class ReservationException : Exception
    {
        public ReservationException(string message) : base(message)
        {
        }
    }
}