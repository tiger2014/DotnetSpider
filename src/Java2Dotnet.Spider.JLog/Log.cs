﻿using System;
using System.IO;
using System.Text;
using System.Net;

namespace Java2Dotnet.Spider.JLog
{
    internal class LogInfo
    {
        public string Type { get; set; }
        public string Time { get; set; }
        public string Message { get; set; }
        public string Machine = Log.Machine;
        public string UserId = Log.UserId;
        public string TaskId = Log.TaskId;
        
        public override string ToString()
        {
            return $"[{Type}] {Time} {Machine}-{UserId}-{TaskId} {Message}";    
        }
    }
    
	public class Log : ILog
	{
		private static readonly object WriteToConsoleLocker = new object();
		private static readonly object WriteToLogFileLocker = new object();
		private static readonly string LogFile;
        public static string UserId { get; set; }
        public static string TaskId { get; set; }
        public static string Machine;
		public string Name { get; }

		static Log()
		{
            Machine = Dns.GetHostName();
			LogFile = Path.Combine(AppContext.BaseDirectory, DateTime.Now.ToString("yyyy-MM-dd") + ".log");
            string mongodbConn=ConfigurationManager.Get("mongodbConn");
		}

		public Log(string name)
		{
			Name = name;
		}

		public void Warn(string message, Exception e, bool showToConsole)
		{
            var log = CreateLogInfo("WARNING", message, e);
			WriteToLogFile(log);
			if (showToConsole)
			{
				WriteToConsole(log);
			}
		}

		public void Warn(string message, bool showToConsole)
		{
			Warn(message, null, showToConsole);
		}

		public void Info(string message, Exception e, bool showToConsole)
		{
            var log = CreateLogInfo("INFO", message, e);
			WriteToLogFile(log);
			if (showToConsole)
			{
				WriteToConsole(log);
			}
		}

		public void Info(string message, bool showToConsole)
		{
			Info(message, null, showToConsole);
		}

		public void Error(string message, Exception e, bool showToConsole)
		{
            var log = CreateLogInfo("ERROR", message, e);
			WriteToLogFile(log);
			if (showToConsole)
			{
				WriteToConsole(log);
			}
		}

		public void Error(string message, bool showToConsole)
		{
			Error(message, null, showToConsole);
		}

		private static void WriteToConsole(LogInfo log)
		{
			lock (WriteToConsoleLocker)
			{
				switch (log.Type)
				{
					case "ERROR":
						{
							Console.ForegroundColor = ConsoleColor.Red;
							break;
						}
					case "INFO":
						{
                            Console.ForegroundColor = ConsoleColor.Magenta;
							break;
						}
					case "WARNING":
						{
							Console.ForegroundColor = ConsoleColor.Yellow;
							break;
						}
				}

				Console.WriteLine(log.ToString());
				Console.ForegroundColor = ConsoleColor.White;
			}
		}

        private LogInfo CreateLogInfo(string type, string message, Exception e)
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            var log=new LogInfo()
            {
                Type=type,
                Time=time,
                Message = message + Environment.NewLine + e
            };
            return log;
        }
        
		private static void WriteToLogFile(LogInfo log)
		{           
			lock (WriteToLogFileLocker)
			{
				File.AppendAllText(LogFile, log.ToString(),Encoding.UTF8);
			}
		}
	}
}
