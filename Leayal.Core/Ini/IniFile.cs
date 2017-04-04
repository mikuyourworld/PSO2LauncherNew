using Microsoft.VisualBasic;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Leayal.Ini
{
    public class IniFile
    {
        private string sFilename;
        private System.Text.StringBuilder tmpStringBuild;

        private System.Collections.Generic.Dictionary<string, IniSection> o_Sections = new System.Collections.Generic.Dictionary<string, IniSection>();
        #region "Constructors"
        public IniFile(string filePath)
        {
            this.sFilename = filePath;
            this.tmpStringBuild = new System.Text.StringBuilder();
            if (System.IO.File.Exists(filePath))
            {
                using (System.IO.StreamReader theReader = new System.IO.StreamReader(filePath))
                {
                    ReadIniFromTextStream(theReader);
                }
            }
        }


        public IniFile(System.IO.TextReader Stream, bool CloseAfterRead = true)
        {
            this.sFilename = string.Empty;
            this.tmpStringBuild = new System.Text.StringBuilder();
            this.ReadIniFromTextStream(Stream);
            if (CloseAfterRead)
            {
                Stream.Close();
            }
        }
        #endregion

        #region "Methods"
        public string GetValue(string section, string key, string defaultValue)
        {
            foreach (var theNode in this.o_Sections)
            {
                if (theNode.Key.ToLower() == section.ToLower())
                {
                    foreach (var theInsideNode in this.o_Sections[theNode.Key].IniKeyValues)
                    {
                        if (theInsideNode.Key.ToLower() == key.ToLower())
                        {
                            return theInsideNode.Value.Value;
                        }
                    }
                }
            }
            return defaultValue;
        }

        public void SetValue(string section, string key, string value)
        {
            if (!checkSection(section))
                this.o_Sections.Add(section, new IniSection());
            foreach (var theNode in this.o_Sections[section].IniKeyValues)
            {
                if (theNode.Key.ToLower() == key.ToLower())
                {
                    theNode.Value.Value = value;
                    return;
                }
            }
            this.o_Sections[section].IniKeyValues.Add(key, new IniKeyValue(value));
        }

        public Dictionary<string, IniKeyValue> GetAllValues(string section)
        {
            if (checkSection(section))
                return this.o_Sections[section].IniKeyValues;
            else
                return null;
        }

        public void Save()
        {
            this.Save(System.Text.Encoding.UTF8);
        }

        public void Save(System.Text.Encoding encode)
        {
            if (string.IsNullOrWhiteSpace(this.sFilename))
            {
                return;
            }
            using (System.IO.StreamWriter theWriter = new System.IO.StreamWriter(this.sFilename, false, encode))
            {
                WriteToStream(theWriter);
            }
        }

        public void SaveAs(string newPath)
        {
            this.SaveAs(newPath, System.Text.Encoding.UTF8);
        }

        public void SaveAs(string newPath, System.Text.Encoding encode)
        {
            if (string.IsNullOrWhiteSpace(newPath))
            {
                return;
            }
            this.sFilename = newPath;
            using (System.IO.StreamWriter theWriter = new System.IO.StreamWriter(newPath, false, encode))
            {
                WriteToStream(theWriter);
            }
        }

        public void Close()
        {
            this.tmpStringBuild.Clear();
            this.sFilename = null;
            this.o_Sections.Clear();
            this.tmpStringBuild = null;
            this.o_Sections = null;
        }

        public override string ToString()
        {
            this.tmpStringBuild.Clear();
            foreach (var Section_loopVariable in this.o_Sections)
            {
                if (Section_loopVariable.Value.IsComment == false)
                {
                    this.tmpStringBuild.AppendLine("[" + Section_loopVariable.Key + "]");
                }
                else
                {
                    this.tmpStringBuild.AppendLine(";[" + Section_loopVariable.Key + "]");
                }
                foreach (var KeyValue_loopVariable in Section_loopVariable.Value.IniKeyValues)
                {
                    if (Section_loopVariable.Value.IsComment == false)
                    {
                        this.tmpStringBuild.AppendLine(KeyValue_loopVariable.Key + "=" + KeyValue_loopVariable.Value.Value);
                    }
                    else
                    {
                        this.tmpStringBuild.AppendLine(";" + KeyValue_loopVariable.Key + "=" + KeyValue_loopVariable.Value.Value);
                    }
                }
            }
            return this.tmpStringBuild.ToString();
        }
        #endregion

        #region "Properties"
        public System.Collections.ObjectModel.ReadOnlyCollection<string> Sections
        {
            get { return new System.Collections.ObjectModel.ReadOnlyCollection<string>(this.o_Sections.Keys.ToList()); }
        }

        public int SectionCount
        {
            get { return this.o_Sections.Keys.Count; }
        }
        #endregion

        #region "Private Methods"
        private bool checkSection(string theKey)
        {
            foreach (var theNode in this.o_Sections)
                if (theNode.Key.ToLower() == theKey.ToLower())
                    return true;
            return false;
        }

        private void ReadIniFromTextStream(System.IO.TextReader Stream)
        {
            StringBuilder lineBuffer = new StringBuilder();
            string pun = null;
            string[] splitBuffer = null;
            IniSection sectionBuffer = null;
            char[] buffer = new char[2];
            char[] spli = new char[] { '=' };
            //weird ..... but we don't need to new char[] for each read buffer
            while (Stream.ReadBlock(buffer, 0, 1) > 0)
                switch (buffer[0])
                {
                    case ControlChars.Lf:
                        pun = lineBuffer.ToString();
                        if (!string.IsNullOrWhiteSpace(pun))
                        {
                            // move this line here because no need to check for every read char
                            if (pun.StartsWith("[") && pun.EndsWith("]"))
                            {
                                sectionBuffer = new IniSection(false);
                                this.o_Sections.Add(pun.Substring(1, lineBuffer.Length - 2), sectionBuffer);
                                pun = string.Empty;
                                lineBuffer.Clear();
                            }
                            else if (pun.IndexOf("=") > -1)
                            {
                                splitBuffer = pun.Split(spli, 2);
                                // make sure it split just one time
                                sectionBuffer.IniKeyValues.Add(splitBuffer[0].Trim(), new IniKeyValue(splitBuffer[1].Trim()));
                                pun = string.Empty;
                                lineBuffer.Clear();
                            }
                        }
                        break;
                    case ControlChars.Cr:
                        break;
                    case ControlChars.NullChar:
                        break;
                    default:
                        lineBuffer.Append(buffer[0]);
                        break;
                }
            pun = lineBuffer.ToString();
            if (!string.IsNullOrWhiteSpace(pun))
            {
                //This will make sure last line without \n will not be discarded
                if (pun.StartsWith("[") && pun.EndsWith("]"))
                {
                    sectionBuffer = new IniSection(false);
                    this.o_Sections.Add(pun.Substring(1, pun.Length - 2), sectionBuffer);
                    pun = string.Empty;
                    lineBuffer.Clear();
                }
                else if (pun.IndexOf("=") > -1)
                {
                    splitBuffer = pun.Split('=');
                    sectionBuffer.IniKeyValues.Add(splitBuffer[0].Trim(), new IniKeyValue(splitBuffer[1].Trim()));
                    pun = string.Empty;
                    lineBuffer.Clear();
                }
            }
            spli = null;
            sectionBuffer = null;
            lineBuffer = null;
            splitBuffer = null;
        }

        private void WriteToStream(System.IO.TextWriter theStream)
        {
            this.tmpStringBuild.Clear();
            foreach (var Section_loopVariable in this.o_Sections)
            {
                if (Section_loopVariable.Value.IsComment == false)
                {
                    this.tmpStringBuild.AppendLine("[" + Section_loopVariable.Key + "]");
                }
                else
                {
                    this.tmpStringBuild.AppendLine(";[" + Section_loopVariable.Key + "]");
                }
                foreach (var KeyValue_loopVariable in Section_loopVariable.Value.IniKeyValues)
                {
                    if (Section_loopVariable.Value.IsComment == false)
                    {
                        this.tmpStringBuild.AppendLine(KeyValue_loopVariable.Key + "=" + KeyValue_loopVariable.Value.Value);
                    }
                    else
                    {
                        this.tmpStringBuild.AppendLine(";" + KeyValue_loopVariable.Key + "=" + KeyValue_loopVariable.Value.Value);
                    }
                }
            }
            theStream.Write(this.tmpStringBuild.ToString());
            theStream.Flush();
        }
        private static T InlineAssignHelper<T>(ref T target, T value)
        {
            target = value;
            return value;
        }
        #endregion
    }
}
