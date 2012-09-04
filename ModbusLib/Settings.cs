using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		public List<DBInfo> Databases;

		private SortedList<string,DBInfo> dbInfoList;
		[System.Xml.Serialization.XmlIgnore]
		public SortedList<string, DBInfo> DBInfoList {
			get { return dbInfoList; }
			set { dbInfoList = value; }
		}

		public static void init() {
			Settings settings=XMLSer<Settings>.fromXML("Data\\Settings.xml");
			Settings.settings = settings;
			settings.DBInfoList = new SortedList<string, DBInfo>();
			foreach (DBInfo dbInfo in settings.Databases) {
				settings.DBInfoList.Add(dbInfo.ID, dbInfo);
			}
		}
	}
}
