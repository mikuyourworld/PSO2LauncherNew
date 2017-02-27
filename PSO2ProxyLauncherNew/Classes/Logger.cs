using System;
using System.IO;
using System.Collections.Generic;
//using System.Linq;
using System.ComponentModel;
using System.Collections.Concurrent;
using PSO2ProxyLauncherNew.Classes.Infos;

namespace PSO2ProxyLauncherNew.Classes.Log
{
    internal sealed partial class LogManager
    {
        private static LogManager defaultInstance = new LogManager();

        public static LogManager Default
        { get { return defaultInstance; } }

        private Dictionary<string, Logger> dict_Log;
        public LogManager()
        {
            this.dict_Log = new Dictionary<string, Logger>();
            this.dict_Log.Add(string.Empty, new Logger(new FileInfo(Path.Combine(MyApp.AssemblyInfo.DirectoryPath, "log.txt")).FullName, true));
        }

        public static Logger GeneralLog
        { get { return Default.dict_Log[string.Empty]; } }

        public static Logger GetLog(string path, string seperator)
        {
            return GetLog(new FileInfo(path), seperator, true);
        }

        public static Logger GetLog(string path, string seperator, bool appendExisting)
        {
            return GetLog(new FileInfo(path), seperator, appendExisting);
        }

        public static Logger GetLogDefaultPath(string path, string seperator)
        {
            return GetLog(new FileInfo(Path.Combine(DefaultValues.MyInfo.Directory.LogFolder, path)), seperator, true);
        }

        public static Logger GetLogDefaultPath(string path, string seperator, bool appendExisting)
        {
            return GetLog(new FileInfo(Path.Combine(DefaultValues.MyInfo.Directory.LogFolder, path)), seperator, appendExisting);
        }

        public static Logger GetLog(string path)
        {
            return GetLog(new FileInfo(path), "=", true);
        }

        public static Logger GetLog(string path, bool appendExisting)
        {
            return GetLog(new FileInfo(path), "=", appendExisting);
        }

        public static Logger GetLogDefaultPath(string path)
        {
            return GetLog(new FileInfo(Path.Combine(DefaultValues.MyInfo.Directory.LogFolder, path)), "=", true);
        }

        public static Logger GetLogDefaultPath(string path, bool appendExisting)
        {
            return GetLog(new FileInfo(Path.Combine(DefaultValues.MyInfo.Directory.LogFolder,path)), "=", appendExisting);
        }

        public static Logger GetLog(FileInfo path, string seperator, bool appendExisting)
        {
            if (!Default.dict_Log.ContainsKey(path.FullName))
                Default.dict_Log.Add(path.FullName, new Logger(path, seperator, appendExisting));
            return Default.dict_Log[path.FullName];
        }

        public void Clear()
        {
            foreach (Logger obj in Default.dict_Log.Values)
                obj.Dispose();
            this.dict_Log.Clear();
        }

        public void Dispose()
        {
            this.Clear();
        }
    }

    public class Logger
    {
        public enum LogLevel : short
        {
            Info,
            Error,
            Critical
        }
        internal class LogLine
        {
            public string Message { get; }
            public LogLevel Level { get; }
            public LogLine(string msg, LogLevel _level)
            {
                this.Message = msg;
                this.Level = _level;
            }
        }
        private ConcurrentQueue<LogLine> myQueue;
        private BackgroundWorker myWorker;

        public FileInfo LogPath
        { get; private set; }
        public string SeparatorChar
        { get; set; }

        public Logger(FileInfo pathInfo, string separator, bool appendExisting)
        {
            this.LogPath = pathInfo;
            this.SeparatorChar = separator;
            if (!appendExisting)
                pathInfo.Delete();
            this.myQueue = new ConcurrentQueue<LogLine>();
            this.myWorker = new BackgroundWorker();
            this.myWorker.WorkerReportsProgress = false;
            this.myWorker.WorkerSupportsCancellation = false;
            this.myWorker.DoWork += MyWorker_DoWork;
            this.myWorker.RunWorkerCompleted += MyWorker_RunWorkerCompleted;
        }

        public Logger(FileInfo pathInfo, bool appendExisting) : this(pathInfo, "=", appendExisting) { }

        public Logger(FileInfo pathInfo) : this(pathInfo, "=", true) { }

        public Logger(string path, string separator, bool appendExisting) : this(new FileInfo(path), separator, appendExisting) { }

        public Logger(string path, bool appendExisting) : this(new FileInfo(path), "=", appendExisting) { }

        public Logger(string path) : this(new FileInfo(path), "=", true) { }

        private void StartLog()
        {
            if (!this.myWorker.IsBusy && !this.myQueue.IsEmpty)
                this.myWorker.RunWorkerAsync();
        }

        private void MyWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null) { }
            else
                this.StartLog();
        }

        private void MyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            LogLine _LogLine;
            if (this.myQueue.TryDequeue(out _LogLine))
                if (!string.IsNullOrWhiteSpace(_LogLine.Message))
                {
                    System.Text.StringBuilder myBuilder = new System.Text.StringBuilder();
                    myBuilder.AppendLine(_LogLine.Message);
                    if (!string.IsNullOrWhiteSpace(this.SeparatorChar))
                    {
                        for (int i = 0; i < 15; i++)
                            myBuilder.Append(SeparatorChar);
                        myBuilder.AppendLine(SeparatorChar);
                    }
                    Directory.CreateDirectory(this.LogPath.DirectoryName);
                    using (StreamWriter sr = new StreamWriter(this.LogPath.FullName, true))
                    {
                        sr.Write(myBuilder.ToString());
                        sr.Flush();
                    }
                    myBuilder.Clear();
                }
        }

        public void Print(string msg, LogLevel _level)
        {
            this.myQueue.Enqueue(new LogLine(msg, _level));
            this.StartLog();
        }

        public void Print(string msg)
        {
            this.myQueue.Enqueue(new LogLine(msg,  LogLevel.Info));
            this.StartLog();
        }

        public void Print(Exception ex)
        {
            this.Print(ex.Message + "\r\n" + ex.StackTrace, LogLevel.Error);
        }

        public void Dispose()
        {
            this.myWorker.Dispose();
        }
    }
}
