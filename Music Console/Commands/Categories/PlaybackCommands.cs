using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Music_Console.mSystem;

namespace Music_Console.Commands.Categories
{
    public class PlaybackCommands : CommandCategory
    {
        public override void Init()
        {
            Commands.Add(SetShuffle);
            Commands.Add(Load);
        }

        private static readonly Command SetShuffle = new Command
        {
            Name = "SetShuffle",
            Usage = "/setshuffle true/false",
            Help = "Turns shuffle on or off.",
            Aliases = new string[] { },
            Handler = SetShuffleHandler
        };

        private static void SetShuffleHandler(Command command)
        {
            try
            {
                string dir = command.Next().ToLower();
                Program.Shuffle = dir != "false";
                Messenger.Send("&fShuffle set to " + dir);
            }
            catch (ArgumentNullException)
            {
                Messenger.Send(command.Usage);
            }
        }

        private static readonly Command Load = new Command
        {
            Name = "Load",
            Usage = "/load (Url Directory/Local Directory (Optional [Shuffle]: true/false)",
            Help = "Loads directory to play music off of. Shuffle is reset to false if not specified in arguments.",
            Aliases = new string[] {},
            Handler = LoadHandler
        };

        private static void LoadHandler(Command command)
        {
            try
            {
                command.Next();
                string dir = command.NextAllOne().Substring(1);
                try
                {
                    string dirX = command.Next();
                    Program.Shuffle = dirX.ToLower() != "false";
                }
                catch (ArgumentNullException)
                {
                    Program.Shuffle = false;
                }

                if (dir.ToLower().StartsWith("http://")) // Test it as a web url
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            string s2 = client.DownloadString(dir);
                        }
                        Messenger.Send("&aUrl directory loaded. Use /play to begin");
                        Program.Directory = dir;
                        Program.IsLocal = false;
                    }
                    catch (WebException)
                    {
                        Messenger.Send("&8" + dir + " &cdoes not exist");
                    }
                }
                else
                {
                    if (!Directory.Exists(dir))
                    {
                        Messenger.Send("&cLocal Directory does not exist");
                    }
                    else
                    {
                        Program.IsLocal = true;
                        Program.Directory = dir;
                        Messenger.Send("&aLocal directory loaded. Use /play to begin");
                    }
                }

            }
            catch (ArgumentNullException)
            {
                Messenger.Send("&cPlease enter a url directory or local directory to play music off of");
            }
        }

    }
}
