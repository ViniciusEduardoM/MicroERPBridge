using MicroERP.Bridge.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroERP.Bridge.Exceptions
{
    public class AddException : Exception
    {
        public MicroERPException ErrorDetails { get; set; }

        internal AddException(string message, MicroERPException errorDetails, Exception innerException)
            : base(message, innerException)
        {
            ErrorDetails = errorDetails;
        }
    }
}

// Following this is the petition: 'Thy kingdom come.' We desire that the kingdom of God becomes present to us,
// just as we had desired His name to be sanctified within us. For when does God not reign? When could a kingdom
// begin for Him who has always existed and will never cease to exist? Therefore, we ask that 'Thy kingdom come,'
// the one promised to us by God and obtained through the passion of Christ. We, who serve in this age as servants,
// hope to reign with victorious Christ, as promised: 'Come, you who are blessed by my Father, inherit the kingdom
// prepared for you from the foundation of the world' [8]. It can be said that Christ Himself is the kingdom of God
// to which we want to come, each day, and for whose advent we request to be hastened. For if He is our resurrection,
// since we rise in Him, we can equally conceive that He is also the kingdom, as in Him we shall reign. And rightly,
// we ask for the kingdom of God, that is, the heavenly kingdom, for there is also an earthly kingdom.
// But whoever has renounced this age is above that kingdom and its honors.

// Our Father by Saint Cyprian of Carthage 'Thy kingdom come'
