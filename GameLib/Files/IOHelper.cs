using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using GameLib.Logging;

namespace GameLib.Files
{
    public static class IOHelper
    {
        /// <summary>
        /// Create the specified file
        /// </summary>
        /// <param name="filename">Path of the file to create (ex: C:\Users\local\test.txt)</param>
        /// <returns></returns>
        public static IOResult CreateFile(string filename)
        {
            if (File.Exists(filename))
                return IOResult.ALREADY_EXISTS;

            try
            {
                FileStream stream = File.Create(filename);
                stream.Close();

                return IOResult.OK;
            }
            catch(Exception e)
            {
                Logger.Error($"{nameof(IOHelper)}.{nameof(CreateFile)}", $"An error occured during the creation of the file {filename}", e);

                return IOResult.ERROR;
            }
        }

        /// <summary>
        /// Create the specified directory
        /// </summary>
        /// <param name="directory">Path of the directory to create (ex: C:\Users\local\test)</param>
        /// <returns></returns>
        public static IOResult CreateDirectory(string directory)
        {
            if (Directory.Exists(directory))
                return IOResult.ALREADY_EXISTS;

            try
            {
                _ = Directory.CreateDirectory(directory);

                return IOResult.OK;
            }
            catch(Exception e)
            {
                Logger.Error($"{nameof(IOHelper)}.{nameof(CreateDirectory)}", $"An error occured during the creation of the directory {directory}", e);

                return IOResult.ERROR;
            }
        }

        /// <summary>
        /// Write the specified content in the specified file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="datas"></param>
        /// <param name="append"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static IOResult WriteToFile(string filename, string datas, bool append, Encoding encoding)
        {
            if (!File.Exists(filename))
                return IOResult.DONT_EXISTS;

            try
            {
                StreamWriter sw = new(filename, append, encoding);

                sw.Write(datas);
                sw.Flush();
                sw.Close();

                return IOResult.OK;
            }
            catch(Exception e)
            {
                Logger.Error($"{nameof(IOHelper)}.{nameof(WriteToFile)}", $"An error occured during the writing process in the file {filename}", e);

                return IOResult.ERROR;
            }
        }

        public static IOResult ReadToEnd(string path, out string result)
        {
            result = String.Empty;

            if (!File.Exists(path))
                return IOResult.DONT_EXISTS;

            try
            {
                result = File.ReadAllText(path);

                return IOResult.OK;
            }
            catch(Exception e)
            {
                Logger.Error($"{nameof(IOHelper)}.{nameof(ReadToEnd)}", $"An error occured during the reading of the file {path}", e);

                result = String.Empty;
                return IOResult.ERROR;
            }
        }
    }

    public enum IOResult
    {
        OK,
        ERROR,
        ALREADY_EXISTS,
        DONT_EXISTS,
    }
}
