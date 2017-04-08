using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PSO2ProxyLauncherNew.Classes.PSO2.PSO2UserConfiguration
{
    public class RawPSO2UserConfiguration : PSO2ConfigToken
    {
        internal static readonly string levelJump = "    ";
        protected void ParseFromString(string content)
        {
            using (StringReader sr = new StringReader(content))
            {
                this.Clear();
                List<string> r_currentPath = new List<string>();
                string bufferedProperty = null, bufferedValue = null;
                char[] charBuffer = new char[2];
                PSO2ConfigToken token = null;
                StringBuilder bufferingText = new StringBuilder();
                int readCount = sr.ReadBlock(charBuffer, 0, 1);
                while (readCount > 0)
                {
                    switch (charBuffer[0])
                    {
                        case '\0':
                            break;
                        case '\n':
                            break;
                        case '\r':
                            break;
                        case '=':
                            bufferedProperty = bufferingText.ToString();
                            if (!string.IsNullOrEmpty(bufferedProperty))
                                bufferedProperty = bufferedProperty.Trim();
                            bufferingText.Clear();
                            break;
                        case '{':
                            if (bufferedProperty != null)
                            {
                                r_currentPath.Add(bufferedProperty);
                                token = selectRecursive(r_currentPath);
                                bufferedProperty = null;
                                bufferingText.Clear();
                            }
                            break;
                        case '}':
                            r_currentPath.RemoveAt(r_currentPath.Count - 1);
                            token = selectRecursive(r_currentPath);
                            break;
                        case ',':
                            bufferedValue = bufferingText.ToString();
                            if (!string.IsNullOrEmpty(bufferedValue))
                                bufferedValue = bufferedValue.Trim();
                            token.Values[bufferedProperty] = bufferedValue;
                            bufferingText.Clear();
                            break;
                        default:
                            bufferingText.Append(charBuffer, 0, readCount);
                            break;
                    }
                    readCount = sr.ReadBlock(charBuffer, 0, 1);
                }
            }
        }

        protected void ReadFromFile(string filepath)
        {
            using (StreamReader sr = new StreamReader(filepath))
            {
                this.Clear();
                List<string> r_currentPath = new List<string>();
                string bufferedProperty = null, bufferedValue = null;
                char[] charBuffer = new char[2];
                PSO2ConfigToken token = null;
                StringBuilder bufferingText = new StringBuilder();
                int readCount = sr.ReadBlock(charBuffer, 0, 1);
                while (readCount > 0)
                {
                    switch (charBuffer[0])
                    {
                        case '\0':
                            break;
                        case '\n':
                            break;
                        case '\r':
                            break;
                        case '=':
                            bufferedProperty = bufferingText.ToString();
                            if (!string.IsNullOrEmpty(bufferedProperty))
                                bufferedProperty = bufferedProperty.Trim();
                            bufferingText.Clear();
                            break;
                        case '{':
                            if (bufferedProperty != null)
                            {
                                r_currentPath.Add(bufferedProperty);
                                token = selectRecursive(r_currentPath);
                                bufferedProperty = null;
                                bufferingText.Clear();
                            }
                            break;
                        case '}':
                            r_currentPath.RemoveAt(r_currentPath.Count - 1);
                            token = selectRecursive(r_currentPath);
                            break;
                        case ',':
                            if (bufferedProperty != null)
                            {
                                bufferedValue = bufferingText.ToString();
                                if (!string.IsNullOrEmpty(bufferedValue))
                                    bufferedValue = bufferedValue.Trim();
                                token.Values[bufferedProperty] = bufferedValue;
                                bufferingText.Clear();
                                bufferedProperty = null;
                                bufferedValue = null;
                            }
                            break;
                        default:
                            bufferingText.Append(charBuffer, 0, readCount);
                            break;
                    }
                    readCount = sr.ReadBlock(charBuffer, 0, 1);
                }
            }
        }

        private PSO2ConfigToken selectRecursive(IEnumerable<string> path)
        {
            PSO2ConfigToken result = null;
            foreach (string smallpath in path)
            {
                if (result != null)
                    result = result[smallpath] as PSO2ConfigToken;
                else
                    result = this[smallpath];
            }
            return result;
        }

        public void SaveAs(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath, false, Encoding.UTF8))
                this.SaveAs(sw);
        }

        public void SaveAs(TextWriter _stream)
        {
            if (_disposed) throw new ObjectDisposedException("PSO2UserConfiguration");
            int howdeepiwent = 1, itemcount = 0;
            foreach (var item in this.InnerArray)
            {
                itemcount++;
                _stream.WriteLine(item.Key + " = {");
                foreach (var valuePair in item.Value.Values)
                    _stream.WriteLine(this.GetJumpString(howdeepiwent) + valuePair.Key + " = " + valuePair.Value + ",");
                this.RecursiveWriteKey(_stream, howdeepiwent, item.Value.GetChildren());
                if (itemcount == this.InnerArray.Count)
                    _stream.WriteLine("}");
                else
                    _stream.WriteLine("},");
            }
        }

        private void RecursiveWriteKey(TextWriter _stream, int level, IEnumerable<KeyValuePair<string, PSO2ConfigToken>> token)
        {
            level++;
            string jumpstring = this.GetJumpString(level - 1);
            foreach (var inneritem in token)
            {
                _stream.WriteLine(jumpstring + inneritem.Key + " = {");
                foreach (var valuePair in inneritem.Value.Values)
                    _stream.WriteLine(this.GetJumpString(level) + valuePair.Key + " = " + valuePair.Value + ",");
                this.RecursiveWriteKey(_stream, level, inneritem.Value.GetChildren());
                _stream.WriteLine(jumpstring + "},");
            }
        }

        private string GetJumpString(int jumpcount)
        {
            if (jumpcount == 0) return string.Empty;
            StringBuilder sb = new StringBuilder(levelJump.Length * jumpcount);
            for (int i = 1; i <= jumpcount; i++)
                sb.Append(levelJump);
            return sb.ToString();
        }

        //4

        public static RawPSO2UserConfiguration Parse(string content)
        {
            var result = new RawPSO2UserConfiguration();
            result.ParseFromString(content);
            return result;
        }

        public static RawPSO2UserConfiguration FromFile(string filepath)
        {
            var result = new RawPSO2UserConfiguration();
            result.ReadFromFile(filepath);
            return result;
        }

        private bool _disposed;
        public override void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            base.Dispose();
        }

        public RawPSO2UserConfiguration() : base() { }
    }
}
