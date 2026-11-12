using GameLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameEditor.Logging
{
    internal static class Logger
    {
        private const string LOGS_FOLDER = "Logs_GameEditor";

        private static readonly string _fileDebug, _fileInfo, _fileWarn, _fileError;

        private static StreamWriter _swDebug, _swInfo, _swWarn, _swError;

        static Logger()
        {
            _fileDebug = $@"{LOGS_FOLDER}\debug.log";
            _fileInfo = $@"{LOGS_FOLDER}\info.log";
            _fileWarn = $@"{LOGS_FOLDER}\warn.log";
            _fileError = $@"{LOGS_FOLDER}\error.log";

            if (!Directory.Exists(LOGS_FOLDER))
                Directory.CreateDirectory(LOGS_FOLDER);

            CreateFile(_fileDebug, GameLibConfig.Global.FlushLogsOnStartup);
            CreateFile(_fileInfo, GameLibConfig.Global.FlushLogsOnStartup);
            CreateFile(_fileWarn, GameLibConfig.Global.FlushLogsOnStartup);
            CreateFile(_fileError, GameLibConfig.Global.FlushLogsOnStartup);

            _swDebug = new StreamWriter(_fileDebug, true, Encoding.UTF8);
            _swInfo = new StreamWriter(_fileInfo, true, Encoding.UTF8);
            _swWarn = new StreamWriter(_fileWarn, true, Encoding.UTF8);
            _swError = new StreamWriter(_fileError, true, Encoding.UTF8);
        }

        private static void CreateFile(string path, bool erase)
        {
            if (erase && File.Exists(path))
                File.Delete(path);

            if (!File.Exists(path))
                File.Create(path);
        }

        private static string CreateLine(string level, string location, string message, Exception? e)
        {
            string outStr = "";

            outStr += $"{level}; ";
            outStr += $"{DateTime.Now:yyyy.MM.dd HH:mm:ss:fff}; ";
            outStr += $"{location}; ";
            outStr += $"{message}; ";

            if (e != null)
                outStr += $"{e}";

            return outStr;
        }

        /// <summary>
        /// Write a log in the debug.log file
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="e"></param>
        internal static void Debug(string location, string message, Exception? e)
        {
            _swDebug.WriteLine(CreateLine("DEBUG", location, message, e));
            _swDebug.Flush();
        }

        /// <summary>
        /// Write a log in the info.log file
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="e"></param>
        internal static void Info(string location, string message, Exception? e)
        {
            _swInfo.WriteLine(CreateLine("INFO", location, message, e));
            _swInfo.Flush();
        }

        /// <summary>
        /// Write a log in the warn.log file
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="e"></param>
        internal static void Warn(string location, string message, Exception? e)
        {
            _swWarn.WriteLine(CreateLine("WARN", location, message, e));
            _swWarn.Flush();
        }

        /// <summary>
        /// Write a log in the error.log file
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="e"></param>
        internal static void Error(string location, string message, Exception? e)
        {
            _swError.WriteLine(CreateLine("ERROR", location, message, e));
            _swError.Flush();
        }
    }
}
