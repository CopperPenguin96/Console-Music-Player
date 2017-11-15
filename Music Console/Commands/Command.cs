using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Music_Console;

namespace Music_Console.Commands
{
    public class Command
    {
        public string Name;
        public string Usage;
        public string Help;
        public string[] Aliases;
        public Run Handler;

        public override string ToString()
        {
            return Help;
        }

        private string allone;
        private string[] _arguments;
        private int _argumentInt = 0;

        public string NextAllOne()
        {
            return allone;
        }

        public string[] NextAll()
        {
            return _arguments;
        }

        public string Next()
        {
            while (true)
            {
                if (_arguments.Length == 0) throw new ArgumentNullException(" Command.Next() ");
                if (_argumentInt == _arguments.Length)
                {
                    _argumentInt = 0;
                }
                else
                {
                    _argumentInt++;
                    return _arguments[_argumentInt - 1];
                }
            }
            
        }

        public void Run(string args)
        {
            allone = args;
            _arguments = args.GetWords();
            Handler.Invoke(this);
        }
    }
}
