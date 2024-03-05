
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroERP.Bridge.Exceptions
{
    public class SearchException : Exception
    {
        public MicroERPException ErrorDetails { get; set; }

        internal SearchException(string message, MicroERPException errorDetails, Exception innerException)
            : base(message, innerException)
        {
            ErrorDetails = errorDetails;
        }
    }
}
