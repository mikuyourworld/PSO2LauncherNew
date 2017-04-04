using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Leayal.Forms
{
    public static class TextRendererWrapper
    {
        internal static readonly char[] SpaceOnly = { ' ' };

        public static WrapStringResult WrapString(string originaltext, int preferedWidth, System.Drawing.Font _font, TextFormatFlags _flag)
        {
            if (originaltext.IndexOf("\n") > -1)
            {
                int width = 0, height = 0;
                StringBuilder sb = new StringBuilder();
                WrapStringResult re;
                bool first = true;
                using (StringReader sr = new StringReader(originaltext))
                    while (sr.Peek() > -1)
                    {
                        re = WrapString(sr.ReadLine(), preferedWidth, _font, _flag);
                        if (first)
                        {
                            first = false;
                            sb.Append(re.Result);
                        }
                        else
                            sb.AppendFormat("\r\n{0}", re.Result);
                        width = Math.Max(width, re.Size.Width);
                        height = height + re.Size.Height;
                    }
                return new WrapStringResult(sb.ToString(), new System.Drawing.Size(width, height));
            }
            else
            {
                List<string> _list = new List<string>();
                System.Drawing.Size s = new System.Drawing.Size(preferedWidth, _font.Height), ss;
                StringBuilder sb = new StringBuilder(originaltext.Length);
                bool first = true;
                string[] splitted = originaltext.Split(SpaceOnly);
                string str;
                int _height = 0, _width = 0;
                for (int i = 0; i < splitted.Length; i++)
                {
                    str = splitted[i];
                    if (first)
                    {
                        first = false;
                        sb.Append(str);
                    }
                    else
                        sb.AppendFormat(" {0}", str);
                    ss = System.Windows.Forms.TextRenderer.MeasureText(sb.ToString(), _font, s, _flag);
                    if (ss.Width >= preferedWidth)
                    {
                        _list.Add(sb.ToString());
                        _width = Math.Max(ss.Width, _width);
                        _height = _height + ss.Height;
                        sb.Clear();
                        first = true;
                    }
                }
                if (_list.Count == 0)
                {
                    s = System.Windows.Forms.TextRenderer.MeasureText(sb.ToString(), _font, s, _flag);
                    _width = s.Width;
                    _height = s.Height;
                    _list.Add(sb.ToString());
                }
                else
                {
                    str = sb.ToString();
                    if (!string.IsNullOrEmpty(str))
                    {
                        s = System.Windows.Forms.TextRenderer.MeasureText(sb.ToString(), _font, s, _flag);
                        _width = Math.Max(s.Width, _width);
                        _height = _height + s.Height;
                        _list.Add(str);
                    }
                }
                first = true;
                sb.Clear();
                foreach (string sstr in _list)
                {
                    if (first)
                    {
                        first = false;
                        sb.Append(sstr);
                    }
                    else
                        sb.AppendFormat("\r\n{0}", sstr);
                }
                _list.Clear();
                return new WrapStringResult(sb.ToString(), new System.Drawing.Size(_width, _height));
            }
        }
    }
}
