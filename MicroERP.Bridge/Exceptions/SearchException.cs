
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroERP.Bridge.Exceptions
{
    public class SearchException : Exception
    {
        public µERPException ErrorDetails { get; set; }

        internal SearchException(string message, µERPException errorDetails, Exception innerException)
            : base(message, innerException)
        {
            ErrorDetails = errorDetails;
        }
    }
}
