using System;
using System.Collections.Generic;
using System.Linq;

namespace Music_Console.mSystem
{
    public class Messenger
    {
        /*
         * Using MC Color Codes
         */

        public enum Color
        {
            Black = 0,
            NavyBlue = 1,
            Green = 2,
            Teal = 3,
            Maroon = 4,
            Purple = 5,
            Olive = 6,
            Silver = 7,
            Gray = 8,
            Blue = 9,
            Lime = 10, // a
            Aqua = 11, // b
            Red = 12, // c
            Pink = 13, // d
            Yellow = 14, // e
            White = 15, // f
        }

        public static ConsoleColor ToConsoleColor(Color c)
        {
            switch (c)
            {
                case Color.Black:
                    return ConsoleColor.Black;
                case Color.NavyBlue:
                    return ConsoleColor.DarkBlue;
                case Color.Green:
                    return ConsoleColor.DarkGreen;
                case Color.Teal:
                    return ConsoleColor.DarkCyan;
                case Color.Maroon:
                    return ConsoleColor.DarkRed;
                case Color.Purple:
                    return ConsoleColor.DarkMagenta;
                case Color.Olive:
                    return ConsoleColor.DarkYellow;
                case Color.Silver:
                    return ConsoleColor.Gray;
                case Color.Gray:
                    return ConsoleColor.DarkGray;
                case Color.Blue:
                    return ConsoleColor.Blue;
                case Color.Lime:
                    return ConsoleColor.Green;
                case Color.Aqua:
                    return ConsoleColor.Cyan;
                case Color.Red:
                    return ConsoleColor.Red;
                case Color.Pink:
                    return ConsoleColor.Magenta;
                case Color.Yellow:
                    return ConsoleColor.Yellow;
                default:
                    return ConsoleColor.White;
            }
        }
        public static Color ToColor(string character)
        {
            try
            {
                int x = int.Parse(character);
                if (x < -1 || x > 16)
                {
                    throw new ArgumentOutOfRangeException();
                }
                else
                {
                    return (Color)x;
                }
            }
            catch (FormatException)
            {
                string chr = character.ToLower();
                switch (chr)
                {
                    case "a":
                        return Color.Lime;
                    case "b":
                        return Color.Aqua;
                    case "c":
                        return Color.Red;
                    case "d":
                        return Color.Pink;
                    case "e":
                        return Color.Yellow;
                    case "f":
                        return Color.White;
                }
            }
            return Color.White;
        }
        private static readonly string[] codes =
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
            "a", "b", "c", "d", "e", "f"
        };

        private static string Format(string rawText)
        {
            return "&e" + TimeStamp() + " " + rawText;
        }

        public static string TimeStamp()
        {
            return "<" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss") + ">";
        }

        public static void Send(string textRaw)
        {
            Send(textRaw, true);
        }

        public static void Send(string textRaw, int collumn, int row)
        {
            Send(textRaw, collumn, row, false);
            Logger.CurrentLogs[row] = Format(textRaw).Substring(2);
        }

        public static void Send(string textRaw, int collumn, int row, bool updateLogs)
        {
            int oldPosition = Console.CursorTop; // Get previous position
            Console.SetCursorPosition(collumn, row); // Set desired position
            Send(textRaw, updateLogs); // Send the message
            Console.SetCursorPosition(0, oldPosition); // Reset position
        }

        public static void Send(string textRaw, bool updateLogs)
        {
            if (updateLogs) Logger.CurrentLogs.Add(Format(textRaw).Substring(2));
            string text = Format(textRaw);
            List<int> skipOver = new List<int>();
            for (int txt = 0; txt <= text.Length - 1; txt++)
            {
                char[] chars = text.ToCharArray();
                if (chars[txt].ToString() == "&")
                {
                    List<string> hi = codes.ToList();
                    if (hi.Contains(chars[txt + 1].ToString().ToLower()))
                    {
                        skipOver.Add(txt);
                        skipOver.Add(txt + 1);
                    }
                }
            }

            for (int x = 0; x <= text.Length - 1; x++)
            {

                char[] chars = text.ToCharArray();
                if (chars[x] == "&".ToCharArray()[0]) continue;
                if (x <= 1 || skipOver.Contains(x)) continue;
                char behind2 = chars[x - 2];
                char behind1 = chars[x - 1];
                if (behind2.ToString() == "&")
                {
                    bool isGoodCode = false;
                    foreach (string s in codes)
                    {
                        if (s.ToLower() == behind1.ToString().ToLower())
                        {
                            isGoodCode = true;
                        }
                    }

                    if (isGoodCode)
                    {
                        skipOver.Add(x - 2);
                        skipOver.Add(x - 1);
                        if (x < text.Length - 1)
                        {
                            Color baseColor = ToColor(behind1.ToString());
                            ConsoleColor cColor = ToConsoleColor(baseColor);
                            Console.ForegroundColor = cColor;
                            Console.Write(chars[x]);
                        }
                        else if (x == text.Length - 1)
                        {
                            Color baseColor = ToColor(behind1.ToString());
                            ConsoleColor cColor = ToConsoleColor(baseColor);
                            Console.ForegroundColor = cColor;
                            Console.WriteLine(chars[x]);
                        }
                    }
                    else
                    {
                        if (x < text.Length - 1)
                        {
                            Console.Write(chars[x - 2]);
                            Console.Write(chars[x - 1]);
                            Console.Write(chars[x]);
                        }
                        else if (x == text.Length - 1)
                        {
                            Console.Write(chars[x - 2]);
                            Console.Write(chars[x - 1]);
                            Console.WriteLine(chars[x]);
                        }
                    }
                }
                else
                {
                    if (x < text.Length - 1)
                    {
                        Console.Write(chars[x]);
                    }
                    else if (x == text.Length - 1)
                    {
                        Console.WriteLine(chars[x]);
                    }
                }
            }
        }
    }
}
