using System;
using System.Collections.Generic;
using System.Text;

namespace MicroERP.Bridge.Exceptions
{
    public class DeleteException : Exception
    {
        public MicroERPException ErrorDetails { get; set; }

        internal DeleteException(string message, MicroERPException errorDetails, Exception innerException)
            : base(message, innerException)
        {
            ErrorDetails = errorDetails;
        }
    }
}
