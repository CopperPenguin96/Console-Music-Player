using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Music_Console.Exceptions;
using Music_Console.mSystem;

namespace Music_Console.Commands.Categories
{
    public class SystemCommands : CommandCategory
    {
        public override void Init()
        {
            Commands.Add(Exit);
            Commands.Add(GetUsage);
            Commands.Add(Help);
            Commands.Add(GetVersion);
        }
        private static readonly Command Exit = new Command
        {
            Name = "Exit",
            Usage = "/exit",
            Help = "Exits the application",
            Aliases = new string[] { "divorce", "leave", "shutdown" },
            Handler = ExitHandler
        };

        private static void ExitHandler(Command command)
        {
            Messenger.Send("&4Closing the console down");
            Environment.Exit(0);
        }

        private static readonly Command GetUsage = new Command
        {
            Name = "GetUsage",
            Usage = "/getusage command or command alias",
            Help = "Get the usage of a command",
            Aliases = new string[] { },
            Handler = GetUsageHandler
        };

        private static void GetUsageHandler(Command command)
        {
            try
            {
                string commandStr = command.Next();
                foreach (Command c in CommandManager.RegisteredCommands)
                {
                    if (c.Name.ToLower() == commandStr.ToLower())
                    {
                        Messenger.Send("&7Usage for " + c.Name + ":&f " + c.Usage);
                        return;
                    }
                    else
                    {
                        foreach (string alias in c.Aliases)
                        {
                            if (alias.ToLower() == commandStr.ToLower())
                            {
                                Messenger.Send("&7Usage for " + c.Name + "/" + alias + ":&f " + c.Usage);
                                return;
                            }
                        }
                    }
                }
                throw new CommandNotFoundException();
            }
            catch (ArgumentNullException)
            {
                Messenger.Send("&cYou must enter a command name");
            }
            catch (CommandNotFoundException)
            {
                Messenger.Send("&cSorry, that command and/or alias does not exist");
            }
        }

        private static readonly Command Help = new Command
        {
            Name = "Help",
            Usage = "/help (Optional: Command/Command Alias)",
            Help = "Calls help (list of commands) or help for a specific command",
            Aliases = new string[] { "aid", "firstaid" },
            Handler = HelpHandler
        };

        private static void HelpHandler(Command command)
        {
            try
            {
                string commandStr = command.Next();
                foreach (Command c in CommandManager.RegisteredCommands)
                {
                    if (c.Name.ToLower() == commandStr.ToLower())
                    {
                        Messenger.Send("&9Help for " + c.Name + ":&f " + c.Help);
                        return;
                    }
                    else
                    {
                        foreach (string alias in c.Aliases)
                        {
                            if (alias.ToLower() == commandStr.ToLower())
                            {
                                Messenger.Send("&9Help for " + c.Name + "/" + alias + ":&f " + c.Usage);
                                return;
                            }
                        }
                    }
                }
                throw new CommandNotFoundException();
            }
            catch (ArgumentNullException)
            {
                Messenger.Send("&b-=-=&aHelp (All)&b=-=-");
                foreach (Command c in CommandManager.RegisteredCommands)
                {
                    Messenger.Send("&9" + c.Name + "&f: " + c.Help);
                }
            }
            catch (CommandNotFoundException)
            {
                Messenger.Send("&cSorry, that command and/or alias does not exist");
            }
        }

        private static readonly Command GetVersion = new Command
        {
            Name = "GetVersion",
            Usage = "/GetVersion",
            Help = "Tells your the current version of the console",
            Aliases = new string[] {},
            Handler = GetVersionHandler
        };

        private static void GetVersionHandler(Command command)
        {
            Messenger.Send("&bCurrent Version is: &f" + Program.Version);
        }

    }
}
