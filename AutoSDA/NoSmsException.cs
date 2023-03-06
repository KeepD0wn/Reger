using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSDA
{
    internal class NoSmsException : Exception
    {
        public NoSmsException(string message)
            : base(message) { }
    }
}