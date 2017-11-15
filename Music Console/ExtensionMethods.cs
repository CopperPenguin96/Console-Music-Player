using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Music_Console
{
    public static class DateTimeUtil
    {
        static readonly NumberFormatInfo NumberFormatter = CultureInfo.InvariantCulture.NumberFormat;
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static readonly long TicksToUnixEpoch;
        const long TicksPerMillisecond = 10000;

        static DateTimeUtil()
        {
            TicksToUnixEpoch = UnixEpoch.Ticks;
        }

        /// <summary> Creates a DateTime from a Utc Unix Timestamp. </summary>
        public static DateTime TryParseDateTime(long timestamp)
        {
            return UnixEpoch.AddSeconds(timestamp);
        }


        #region To Unix Time

        /// <summary> Converts a DateTime to Utc Unix Timestamp. </summary>
        public static long ToUnixTime(this DateTime date)
        {
            return (long)date.Subtract(UnixEpoch).TotalSeconds;
        }


        public static long ToUnixTimeLegacy(this DateTime date)
        {
            return (date.Ticks - TicksToUnixEpoch) / TicksPerMillisecond;
        }


        /// <summary> Converts a DateTime to a string containing the Utc Unix Timestamp.
        /// If the date equals DateTime.MinValue, returns an empty string. </summary>
        public static string ToUnixTimeString(this DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                return "";
            }
            else
            {
                return date.ToUnixTime().ToString(NumberFormatter);
            }
        }


        /// <summary> Appends a Utc Unix Timestamp to the given StringBuilder.
        /// If the date equals DateTime.MinValue, nothing is appended. </summary>
        public static StringBuilder ToUnixTimeString(this DateTime date, StringBuilder sb)
        {
            if (date != DateTime.MinValue)
            {
                sb.Append(date.ToUnixTime());
            }
            return sb;
        }

        #endregion


        #region To Date Time

        /// <summary> Creates a DateTime from a Utc Unix Timestamp. </summary>
        public static DateTime ToDateTime(this long timestamp)
        {
            return UnixEpoch.AddSeconds(timestamp);
        }


        /// <summary> Tries to create a DateTime from a string containing a Utc Unix Timestamp.
        /// If the string was empty, returns false and does not affect result. </summary>
        public static bool ToDateTime(this string str, ref DateTime result)
        {
            if (str.Length <= 1 || !long.TryParse(str, out long t)) return false;
            result = UnixEpoch.AddSeconds(long.Parse(str));
            return true;
        }


        public static DateTime ToDateTimeLegacy(long timestamp)
        {
            return new DateTime(timestamp * TicksPerMillisecond + TicksToUnixEpoch, DateTimeKind.Utc);
        }


        public static bool ToDateTimeLegacy(this string str, ref DateTime result)
        {
            if (str.Length <= 1)
            {
                return false;
            }
            result = ToDateTimeLegacy(Int64.Parse(str));
            return true;
        }

        #endregion


        /// <summary> Converts a TimeSpan to a string containing the number of seconds.
        /// If the timestamp is zero seconds, returns an empty string. </summary>
        public static string ToTickString(this TimeSpan time)
        {
            if (time == TimeSpan.Zero)
            {
                return "";
            }
            else
            {
                return (time.Ticks / TimeSpan.TicksPerSecond).ToString(NumberFormatter);
            }
        }


        public static long ToSeconds(this TimeSpan time)
        {
            return (time.Ticks / TimeSpan.TicksPerSecond);
        }


        /// <summary> Tries to create a TimeSpan from a string containing the number of seconds.
        /// If the string was empty, returns false and sets result to TimeSpan.Zero </summary>
        public static bool ToTimeSpan(this string str, out TimeSpan result)
        {
            if (str == null) throw new ArgumentNullException("str");
            if (str.Length == 0)
            {
                result = TimeSpan.Zero;
                return true;
            }
            long ticks;
            if (Int64.TryParse(str, out ticks))
            {
                result = new TimeSpan(ticks * TimeSpan.TicksPerSecond);
                return true;
            }
            else
            {
                result = TimeSpan.Zero;
                return false;
            }
        }


        public static bool ToTimeSpanLegacy(this string str, ref TimeSpan result)
        {
            if (str.Length > 1)
            {
                result = new TimeSpan(Int64.Parse(str) * TicksPerMillisecond);
                return true;
            }
            else
            {
                return false;
            }
        }


        #region MiniString

        public static StringBuilder ToTickString(this TimeSpan time, StringBuilder sb)
        {
            if (time != TimeSpan.Zero)
            {
                sb.Append(time.Ticks / TimeSpan.TicksPerSecond);
            }
            return sb;
        }


        public static string ToMiniString(this TimeSpan span)
        {
            if (span.TotalSeconds < 60)
            {
                return $"{span.Seconds}s";
            }
            else if (span.TotalMinutes < 60)
            {
                return $"{span.Minutes}m{span.Seconds}s";
            }
            else if (span.TotalHours < 48)
            {
                return $"{(int)Math.Floor(span.TotalHours)}h{span.Minutes}m";
            }
            else if (span.TotalDays < 15)
            {
                return $"{span.Days}d{span.Hours}h";
            }
            else
            {
                return $"{Math.Floor(span.TotalDays / 7):0}w{Math.Floor(span.TotalDays) % 7:0}d";
            }
        }


        public static bool TryParseMiniTimespan(this string text, out TimeSpan result)
        {
            try
            {
                result = ParseMiniTimespan(text);
                return true;
            }
            catch (ArgumentException)
            {
            }
            catch (OverflowException)
            {
            }
            catch (FormatException) { }
            result = TimeSpan.Zero;
            return false;
        }


        public static readonly TimeSpan MaxTimeSpan = TimeSpan.FromDays(9999);


        public static TimeSpan ParseMiniTimespan(this string text)
        {
            if (text == null) throw new ArgumentNullException("text");

            text = text.Trim();
            bool expectingDigit = true;
            TimeSpan result = TimeSpan.Zero;
            int digitOffset = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (expectingDigit)
                {
                    if (text[i] < '0' || text[i] > '9')
                    {
                        throw new FormatException();
                    }
                    expectingDigit = false;
                }
                else
                {
                    if (text[i] >= '0' && text[i] <= '9')
                    {
                        // ReSharper disable once RedundantJumpStatement
                        continue;
                    }
                    else
                    {
                        string numberString = text.Substring(digitOffset, i - digitOffset);
                        digitOffset = i + 1;
                        int number = int.Parse(numberString);
                        switch (char.ToLower(text[i]))
                        {
                            case 's':
                                result += TimeSpan.FromSeconds(number);
                                break;
                            case 'm':
                                result += TimeSpan.FromMinutes(number);
                                break;
                            case 'h':
                                result += TimeSpan.FromHours(number);
                                break;
                            case 'd':
                                result += TimeSpan.FromDays(number);
                                break;
                            case 'w':
                                result += TimeSpan.FromDays(number * 7);
                                break;
                            default:
                                throw new FormatException();
                        }
                    }
                }
            }
            return result;
        }

        #endregion


        #region CompactString

        public static string ToCompactString(this DateTime date)
        {
            return date.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK");
        }


        public static string ToCompactString(this TimeSpan span)
        {
            return $"{span.Days}.{span.Hours:00}:{span.Minutes:00}:{span.Seconds:00}";
        }

        #endregion

        private static CultureInfo _cultureInfo = CultureInfo.CurrentCulture;

        /// <summary> Tries to parse a data in a culture-specific ways.
        /// This method is, unfortunately, necessary because in versions 0.520-0.522,
        /// fCraft saved dates in a culture-specific format. This means that if the
        /// server's culture settings were changed, or if the PlayerDB and IPBanList
        /// files were moved between machines, all dates became unparseable. </summary>
        /// <param name="dateString"> String to parse. </param>
        /// <param name="date"> Date to output. </param>
        /// <returns> True if date string could be parsed and was not empty/MinValue. </returns>
        public static bool TryParseLocalDate(string dateString, out DateTime date)
        {
            if (dateString == null) throw new ArgumentNullException("dateString");
            if (dateString.Length <= 1)
            {
                date = DateTime.MinValue;
                return false;
            }
            else
            {
                if (!DateTime.TryParse(dateString, _cultureInfo, DateTimeStyles.None, out date))
                {
#pragma warning disable 618
                    CultureInfo[] cultureList = CultureInfo.GetCultures(CultureTypes.FrameworkCultures);
#pragma warning restore 618
                    foreach (CultureInfo otherCultureInfo in cultureList)
                    {
                        _cultureInfo = otherCultureInfo;
                        try
                        {
                            if (DateTime.TryParse(dateString, _cultureInfo, DateTimeStyles.None, out date))
                            {
                                return true;
                            }
                        }
                        catch (NotSupportedException) { }
                    }
                    throw new Exception("Could not find a culture that would be able to parse date/time formats.");
                }
                else
                {
                    return true;
                }
            }
        }
    }

    public static class EnumerableUtil
    {
        

        /// <summary> Joins all items in a collection into one comma-separated string.
        /// If the items are not strings, .ToString() is called on them. </summary>
        public static string JoinToString<T>(this IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (T item in items)
            {
                if (!first) sb.Append(',').Append(' ');
                sb.Append(item);
                first = false;
            }
            return sb.ToString();
        }


        /// <summary> Joins all items in a collection into one string separated with the given separator.
        /// If the items are not strings, .ToString() is called on them. </summary>
        public static string JoinToString<T>(this IEnumerable<T> items, string separator)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (separator == null) throw new ArgumentNullException("separator");
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (T item in items)
            {
                if (!first) sb.Append(separator);
                sb.Append(item);
                first = false;
            }
            return sb.ToString();
        }


        /// <summary> Joins all items in a collection into one string separated with the given separator.
        /// A specified string conversion function is called on each item before contactenation. </summary>
        public static string JoinToString<T>(this IEnumerable<T> items, Func<T, string> stringConversionFunction)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (stringConversionFunction == null) throw new ArgumentNullException("stringConversionFunction");
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (T item in items)
            {
                if (!first) sb.Append(',').Append(' ');
                sb.Append(stringConversionFunction(item));
                first = false;
            }
            return sb.ToString();
        }


        /// <summary> Joins all items in a collection into one string separated with the given separator.
        /// A specified string conversion function is called on each item before contactenation. </summary>
        public static string JoinToString<T>(this IEnumerable<T> items, string separator, Func<T, string> stringConversionFunction)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (separator == null) throw new ArgumentNullException("separator");
            if (stringConversionFunction == null) throw new ArgumentNullException("stringConversionFunction");
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (T item in items)
            {
                if (!first) sb.Append(separator);
                sb.Append(stringConversionFunction(item));
                first = false;
            }
            return sb.ToString();
        }

        
    }


    internal static unsafe class FormatUtil
    {
        // Quicker StringBuilder.Append(int) by Sam Allen of http://www.dotnetperls.com
        public static StringBuilder Digits(this StringBuilder builder, int number)
        {
            if (builder == null) throw new ArgumentNullException("builder");
            if (number >= 100000000 || number < 0)
            {
                // Use system ToString.
                builder.Append(number);
                return builder;
            }
            int copy;
            int digit;
            if (number >= 10000000)
            {
                // 8.
                copy = number % 100000000;
                digit = copy / 10000000;
                builder.Append((char)(digit + 48));
            }
            if (number >= 1000000)
            {
                // 7.
                copy = number % 10000000;
                digit = copy / 1000000;
                builder.Append((char)(digit + 48));
            }
            if (number >= 100000)
            {
                // 6.
                copy = number % 1000000;
                digit = copy / 100000;
                builder.Append((char)(digit + 48));
            }
            if (number >= 10000)
            {
                // 5.
                copy = number % 100000;
                digit = copy / 10000;
                builder.Append((char)(digit + 48));
            }
            if (number >= 1000)
            {
                // 4.
                copy = number % 10000;
                digit = copy / 1000;
                builder.Append((char)(digit + 48));
            }
            if (number >= 100)
            {
                // 3.
                copy = number % 1000;
                digit = copy / 100;
                builder.Append((char)(digit + 48));
            }
            if (number >= 10)
            {
                // 2.
                copy = number % 100;
                digit = copy / 10;
                builder.Append((char)(digit + 48));
            }
            if (number >= 0)
            {
                // 1.
                copy = number % 10;
                builder.Append((char)(copy + 48));
            }
            return builder;
        }

        // Quicker Int32.Parse(string) by Karl Seguin
        public static int Parse(string stringToConvert)
        {
            if (stringToConvert == null) throw new ArgumentNullException("stringToConvert");
            int value = 0;
            int length = stringToConvert.Length;
            fixed (char* characters = stringToConvert)
            {
                for (int i = 0; i < length; ++i)
                {
                    value = 10 * value + (characters[i] - 48);
                }
            }
            return value;
        }

        // UppercaseFirst by Sam Allen of http://www.dotnetperls.com
        public static string UppercaseFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }

    public static class EnumUtil
    {
        public static bool TryParse<TEnum>(string value, out TEnum output, bool ignoreCase)
        {
            if (value == null) throw new ArgumentNullException("value");
            try
            {
                output = (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
                return Enum.IsDefined(typeof(TEnum), output);
            }
            catch (ArgumentException)
            {
                output = default(TEnum);
                return false;
            }
        }
    }

    public static class StringUtil
    {
        public static string[] GetWords(this string phrase)
        {
            var pattern = new Regex(
                @"( [^\W_\d]              # starting with a letter
                                          # followed by a run of either...
                    ( [^\W_\d] |          #   more letters or
                      [-'\d](?=[^\W_\d])  #   ', -, or digit followed by a letter
                    )*
                    [^\W_\d]              # and finishing with a letter
                )",
            RegexOptions.IgnorePatternWhitespace);

            var input = phrase;
            List<string> x = (from Match m in pattern.Matches(input) select $"{m.Groups[1].Value}").ToList();
            return x.ToArray();
        }

       

        public static string CalculateMd5Hash(this string input)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var t in hash)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        public static string FirstCharacter(this string str)
        {
            return str.ToCharArray()[0].ToString();
        }
    }

}
