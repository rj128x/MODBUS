using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Layout;
using log4net.Appender;
using log4net.Config;

namespace ModbusLib
{
	public class Logger
	{
		public log4net.ILog logger;

		protected static Logger context;

		public Logger() {

		}

		public static Logger createFileLogger(string path, string name, Logger newLogger) {
			string fileName=String.Format("{0}/{1}_{2}.txt", path, name, DateTime.Now.ToString("dd_MM_yyyy"));
			PatternLayout layout = new PatternLayout(@"[%d] %-10p %m%n");
			FileAppender appender=new FileAppender();
			appender.Layout = layout;
			appender.File = fileName;
			appender.AppendToFile = true;
			BasicConfigurator.Configure(appender);
			appender.ActivateOptions();
			newLogger.logger = LogManager.GetLogger(name);
			return newLogger;
		}

		public static void init(Logger context) {
			Logger.context = context;
		}

		protected virtual string createMessage(string message) {
			return String.Format("{0}",  message);
		}

		protected virtual void info(string str) {
			logger.Info(createMessage(str));
		}

		protected virtual void error(string str) {
			logger.Error(createMessage(str));
		}

		protected virtual void debug(string str) {
			logger.Debug(createMessage(str));
		}


		public static void Info(string str) {
			context.info(str);
		}

		public static void Error(string str) {
			context.info(str);
		}

		public static void Debug(string str) {
			context.info(str);
		}

	}
}
