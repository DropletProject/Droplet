using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.Utility.Exceptions
{
    public class CustomException : Exception
    {
        public CustomException(string message, int code = 1) : base(message)
        {
            Code = code;
        }

        public int Code { get; set; }
    }
}
