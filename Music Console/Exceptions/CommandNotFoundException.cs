using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music_Console.Exceptions
{
    public class CommandNotFoundException : Exception
    {
        public CommandNotFoundException()
        {
            
        }

        public CommandNotFoundException(string message) : base(message)
        {
            
        }

        public CommandNotFoundException(string message, Exception inner) : base(message, inner)
        {
            
        }

        public CommandNotFoundException(Exception inner) : base("CommandNotFoundException was thrown", inner)
        {
            
        }
    }
}
