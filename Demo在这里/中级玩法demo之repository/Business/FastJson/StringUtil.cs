using System;
using System.Text;

namespace Top.Api.Util
{
    public abstract class StringUtil
    {
        public static string ToCamelStyle(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            char[] chars = name.ToCharArray();
            StringBuilder buf = new StringBuilder(chars.Length);
            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 0)
                {
                    buf.Append(char.ToLower(chars[i]));
                }
                else
                {
                    buf.Append(chars[i]);
                }
            }
            return buf.ToString();
        }

        public static string ToUnderlineStyle(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (char.IsUpper(c))
                {
                    if (i > 0)
                    {
                        buf.Append("_");
                    }
                    buf.Append(char.ToLower(c));
                }
                else
                {
                    buf.Append(c);
                }
            }
            return buf.ToString();
        }

        public static string EscapeXml(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            StringBuilder buf = new StringBuilder();
            char[] chars = value.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                switch (c)
                {
                    case '<':
                        buf.Append("&lt;");
                        break;
                    case '>':
                        buf.Append("&gt;");
                        break;
                    case '\'':
                        buf.Append("&apos;");
                        break;
                    case '&':
                        buf.Append("&amp;");
                        break;
                    case '"':
                        buf.Append("&quot;");
                        break;
                    default:
                        if ((c == 0x9) || (c == 0xA) || (c == 0xD) || ((c >= 0x20) && (c <= 0xD7FF))
                        || ((c >= 0xE000) && (c <= 0xFFFD)) || ((c >= 0x10000) && (c <= 0x10FFFF)))
                        {
                            buf.Append(c);
                        }
                        break;
                }
            }

            return buf.ToString();
        }

        public static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString(Constants.DATE_TIME_FORMAT);
        }
    }
}
