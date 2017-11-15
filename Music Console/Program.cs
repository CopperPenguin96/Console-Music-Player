using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Music_Console.Commands;
using Music_Console.Exceptions;
using Music_Console;
using Music_Console.mSystem;

namespace Music_Console
{
    public delegate void Run(Command command);

    class Program
    {
        public static string Version = "1.0";
        public static bool Shuffle = false;
        public static bool IsLocal = false;
        public static string Directory;

        private static bool _isClosing = false;
        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(ConsoleCtrlCheck, true);

            Logger.CurrentLogs.OnAdd += Logger.OnAdd;
            
            Messenger.Send("&1=&2-&3=&4-&9CopperPenquin96's &bMusic Player&4-&3=&2-&1=");
            Messenger.Send(
                "&8Use /load (Url Directory/Local Directory) to start playing. Use /help for a list of commands.");
            CommandManager.InitCommands();
            
            
            CheckForEntries();
            while (!_isClosing)
            {
                //UpdateColorfulStuff();
            }

        }

        private static readonly string[] StrArray = new string[]
        {
            "=", "-", "=", "-", "C", "o", "p", "p", "e", "r", "P", "e", "n", "q", "u", "i", "n", "9", "6", "'", "s",
            "M", "u", "s", "i", "c",
            "P", "l", "a", "y", "e", "r", "-", "=", "-", "="
        };
        private static void UpdateColorfulStuff(object sender)
        {
            string newString = "";
            Random rnd = new Random();
            for (int x = 1; x <= 36; x++)
            {
                int rndMaf = rnd.Next(15);
                string ch = "";
                if (rndMaf > 9)
                {
                    if (rndMaf == 10) ch = "a";
                    if (rndMaf == 11) ch = "b";
                    if (rndMaf == 12) ch = "c";
                    if (rndMaf == 13) ch = "d";
                    if (rndMaf == 14) ch = "e";
                    if (rndMaf == 15) ch = "f";
                }
                else ch = "" + rndMaf;
                if (x < 21)
                {
                    newString += "&" + ch + StrArray[x];
                }
                else if (x == 19 || x == 25)
                {
                    newString += "&" + ch + StrArray[x] + " ";
                }
                else
                {
                    try
                    {
                        newString += "&" + ch + StrArray[x];
                    }
                    catch
                    {
                        // Ignored
                    }
                }
            }
            Messenger.Send(newString, 22, 0);
        }
        private static void CheckForEntries()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                var text = Console.ReadLine();
                Logger.CurrentLogs.Add(Messenger.TimeStamp() + " " + text);
                try
                {
                    if (!SendEntry(text))
                    {
                        Console.WriteLine("Failed to send");
                    }
                }
                catch (CommandNotFoundException ex)
                {
                    Messenger.Send("Commmand does not exist (" + text.GetWords()[0] + ")");
                }
            }
        }

        private static bool SendEntry(string text)
        {
            try
            {
                if (text.FirstCharacter() != "/")
                {
                    return false;
                }
                else
                {
                    string text2 = text.Substring(1);

                    string commandname = text2.GetWords()[0];

                    foreach (Command c in CommandManager.RegisteredCommands)
                    {
                        if (c.Name.ToLower() == commandname.ToLower())
                        {
                            c.Run(text2.Substring(commandname.Length));
                            return true;
                        }
                        else
                        {
                            foreach (string alias in c.Aliases)
                            {
                                if (alias.ToLower() == commandname.ToLower())
                                {
                                    c.Run(text2.Substring(commandname.Length));
                                    return true;
                                }
                            }
                        }
                    }
                    throw new CommandNotFoundException();
                }
            }
            catch (CommandNotFoundException ex)
            {
                throw ex; // Placed here so that the catch (Exception) does not go beyond
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            // Put your own handler here

            switch (ctrlType)
            {
                case CtrlTypes.CTRL_CLOSE_EVENT:
                    _isClosing = true;
                    Logger.SaveLog();
                    break;
            }

            return true;
        }

        #region unmanaged
        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.

        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        #endregion

    }


}
