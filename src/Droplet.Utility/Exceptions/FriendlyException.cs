using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.Utility.Exceptions
{

    public class FriendlyException : Exception
    {
        public FriendlyException(string message) : base(message)
        {
        }
    }
}
