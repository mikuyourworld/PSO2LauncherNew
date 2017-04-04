using System.Collections.Generic;
using System.Linq;

namespace Leayal
{
    public static class ProcessHelper
    {
        public static string TableStringToArgs(IEnumerable<string> arg)
        {
            if (arg != null)
            {
                string dun = arg.First();
                if (dun != null)
                {
                    System.Text.StringBuilder builder = new System.Text.StringBuilder();
                    if (!string.IsNullOrWhiteSpace(dun))
                    {
                        if (dun.IndexOf(" ") > -1)
                            builder.AppendFormat("\"{0}\"", dun);
                        else
                            builder.Append(dun);
                    }
                    foreach (string tmp in arg.Skip(1))
                        if (!string.IsNullOrWhiteSpace(tmp))
                        {
                            if (tmp.IndexOf(" ") > -1)
                                builder.AppendFormat(" \"{0}\"", tmp);
                            else
                                builder.Append(" " + tmp);
                        }
                    return builder.ToString();
                }
                else { return string.Empty; }
            }
            else { return string.Empty; }
        }

        public static string TableStringToArgs(string[] arg)
        {
            if ((arg != null) && (arg.Length > 0))
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                string tmp;
                for (int i = 0; i < arg.Length; i++)
                {
                    tmp = arg[i];
                    if (!string.IsNullOrWhiteSpace(tmp))
                    {
                        if (i == 0)
                        {
                            if (tmp.IndexOf(" ") > -1)
                                builder.AppendFormat("\"{0}\"", tmp);
                            else
                                builder.Append(tmp);
                        }
                        else
                        {
                            if (tmp.IndexOf(" ") > -1)
                                builder.AppendFormat(" \"{0}\"", tmp);
                            else
                                builder.Append(" " + tmp);
                        }
                    }
                }
                return builder.ToString();
            }
            else { return string.Empty; }
        }
    }
}
