using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace ModbusLib
{
	public class DBInfo
	{
		private string id;
		public string ID {
			get { return id; }
			set { id = value; }
		}

		private string address;
		public string Address {
			get { return address; }
			set { address = value; }
		}

		private string dbName;
		public string DBName {
			get { return dbName; }
			set { dbName = value; }
		}

		private string user;
		public string User {
			get { return user; }
			set { user = value; }
		}

		private string password;
		public string Password {
			get { return password; }
			set { password = value; }
		}
	}

	public class Settings
	{
		protected static Settings settings;
		private string logPath;
		public string LogPath {
			get { return logPath; }
			set { logPath = value; }
		}

		private string dataPath;
		public string DataPath {
			get { return dataPath; }
			set { dataPath = value; }
		}
		
		public static Settings single {
			get {
				return settings;
			}
		}

		private int hoursDiff;
		public int HoursDiff {
			get { return hoursDiff; }
			set { hoursDiff = value; }
		}

		public List<DBInfo> Databases;

		public List<String> InitFiles;

		private SortedList<string,DBInfo> dbInfoList;
		[System.Xml.Serialization.XmlIgnore]
		public SortedList<string, DBInfo> DBInfoList {
			get { return dbInfoList; }
			set { dbInfoList = value; }
		}

		static Settings() {			
			NFIPoint = new CultureInfo("ru-RU").NumberFormat;
			NFIPoint.NumberDecimalSeparator = ".";
		}
		public static NumberFormatInfo NFIPoint;

		public static void init() {
			System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-GB");
			ci.NumberFormat.NumberDecimalSeparator = ".";
			ci.NumberFormat.NumberGroupSeparator = "";

			System.Threading.Thread.CurrentThread.CurrentCulture = ci;
			System.Threading.Thread.CurrentThread.CurrentUICulture = ci;	
			

			Settings settings=XMLSer<Settings>.fromXML("Data\\Settings.xml");
			Settings.settings = settings;
			settings.DBInfoList = new SortedList<string, DBInfo>();
			foreach (DBInfo dbInfo in settings.Databases) {
				settings.DBInfoList.Add(dbInfo.ID, dbInfo);
			}
		}
	}
}
