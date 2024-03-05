using System;
using System.Collections.Generic;
using System.Text;

namespace MicroERP.Bridge.Exceptions
{
    public class MicroERPException : Exception
    {
        public MicroERPException(string message) : base (message) { }
    }
}
