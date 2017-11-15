using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Music_Console.Commands.Categories;
using Music_Console.Exceptions;


namespace Music_Console.Commands
{
    public class CommandCategory
    {
        public List<Command> Commands = new List<Command>();

        public virtual void Init()
        {
            // First add commands
            // Commands.Add(someCmd);

        }
    }
    public class CommandManager
    {
        public static List<Command> RegisteredCommands = new List<Command>();

        public static List<CommandCategory> Categories = new List<CommandCategory>
        {
            new StupidCommands(), new SystemCommands(), new PlaybackCommands()
        };
        public static void InitCommands()
        {
            foreach (CommandCategory c in Categories)
            {
                c.Init();
                foreach (Command cmd in c.Commands)
                {
                    RegisteredCommands.Add(cmd);
                }
            }
        }
        

        #region System Commands
       
        #endregion

        #region Playback Commands

        #endregion
    }
}
