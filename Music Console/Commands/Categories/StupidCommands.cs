using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Music_Console.mSystem;

namespace Music_Console.Commands.Categories
{
    public class StupidCommands : CommandCategory
    {
        public override void Init()
        {
            Commands.Add(PingPong);
        }

        private static readonly Command PingPong = new Command
        {
            Name = "Ping",
            Usage = "/ping",
            Help = "Just wait and see",
            Aliases = new string[] { },
            Handler = PingPongHandler
        };

        private static void PingPongHandler(Command command)
        {
            Messenger.Send("&1P&2o&3n&4g!");
        }
    }
}
