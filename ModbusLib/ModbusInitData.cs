using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusLib
{
	public enum RWModeEnum { hh, min }
	
	public class ModbusInitData
	{
		private int id;
		[System.Xml.Serialization.XmlAttribute]
		public int ID {
			get { return id; }
			set { id = value; }
		}

		private string name;
		[System.Xml.Serialization.XmlAttribute]
		public string Name {
			get { return name; }
			set { name = value; }
		}

		private int addr;
		[System.Xml.Serialization.XmlAttribute]
		public int Addr {
			get { return addr; }
			set { addr = value; }
		}

		private double scale;
		[System.Xml.Serialization.XmlAttribute]
		public double Scale {
			get { return scale; }
			set { scale = value; }
		}


		private bool writeToDBMin;
		[System.Xml.Serialization.XmlAttribute]
		public bool WriteToDBMin {
			get { return writeToDBMin; }
			set { writeToDBMin = value; }
		}

		private bool writeToDBHH;
		[System.Xml.Serialization.XmlAttribute]
		public bool WriteToDBHH {
			get { return writeToDBHH; }
			set { writeToDBHH = value; }
		}

		private int parNumberMin=-1;
		[System.Xml.Serialization.XmlAttribute]
		public int ParNumberMin {
			get { return parNumberMin; }
			set { parNumberMin = value; }
		}

		private int parNumberHH=-1;
		[System.Xml.Serialization.XmlAttribute]
		public int ParNumberHH {
			get { return parNumberHH; }
			set { parNumberHH = value; }
		}

		private int obj=-1;
		[System.Xml.Serialization.XmlAttribute]
		public int Obj {
			get { return obj; }
			set { obj = value; }
		}

		private int objType=-1;
		[System.Xml.Serialization.XmlAttribute]
		public int ObjType {
			get { return objType; }
			set { objType = value; }
		}

		private string dbNameMin=null;
		[System.Xml.Serialization.XmlAttribute]
		public string DBNameMin {
			get { return dbNameMin; }
			set { dbNameMin = value; }
		}

		private string dbNameHH=null;
		[System.Xml.Serialization.XmlAttribute]
		public string DBNameHH {
			get { return dbNameHH; }
			set { dbNameHH = value; }
		}

		private int item=-1;
		[System.Xml.Serialization.XmlAttribute]
		public int Item {
			get { return item; }
			set { item = value; }
		}
		
	}

	public class ModbusInitDataArray
	{
		private string id;
		public string ID {
			get { return id; }
			set { id = value; }
		}

		private string ip;
		public string IP {
			get { return ip; }
			set { ip = value; }
		}

		private int port;
		public int Port {
			get { return port; }
			set { port = value; }
		}

		private List<ModbusInitData> data;
		public List<ModbusInitData> Data {
			get { return data; }
			set { data = value; }
		}

		private bool writeMin;
		public bool WriteMin {
			get { return writeMin; }
			set { writeMin = value; }
		}

		private bool writeHH;
		public bool WriteHH {
			get { return writeHH; }
			set { writeHH = value; }
		}

		private SortedList<int,ModbusInitData> fullData;
		[System.Xml.Serialization.XmlIgnore]
		public SortedList<int, ModbusInitData> FullData {
			get { return fullData; }
			set { fullData = value; }
		}

		private int maxAddr;
		[System.Xml.Serialization.XmlIgnore]
		public int MaxAddr {
			get { return maxAddr; }
			set { maxAddr = value; }
		}

		private int parNumberMin;
		public int ParNumberMin {
			get { return parNumberMin; }
			set { parNumberMin = value; }
		}

		private int parNumberHH;
		public int ParNumberHH {
			get { return parNumberHH; }
			set { parNumberHH = value; }
		}

		private int obj;
		public int Obj {
			get { return obj; }
			set { obj = value; }
		}

		private int objType;
		public int ObjType {
			get { return objType; }
			set { objType = value; }
		}

		private string dbNameMin;
		public string DBNameMin {
			get { return dbNameMin; }
			set { dbNameMin = value; }
		}

		private string dbNameHH;
		public string DBNameHH {
			get { return dbNameHH; }
			set { dbNameHH = value; }
		}
		
		public void processData() {
			FullData = new SortedList<int, ModbusInitData>();
			MaxAddr = 0;
			foreach (ModbusInitData init in Data) {
				init.ParNumberHH=init.ParNumberHH==-1?ParNumberHH:init.ParNumberHH;
				init.ParNumberMin = init.ParNumberMin == -1 ? ParNumberMin : init.ParNumberMin;
				init.Obj = init.Obj == -1 ? Obj : init.Obj;
				init.ObjType = init.ObjType == -1 ? ObjType : init.ObjType;
				init.Item = init.Item == -1 ? init.ID : init.Item;
				init.DBNameHH = init.DBNameHH == null ? DBNameHH : init.DBNameHH;
				init.DBNameMin = init.DBNameMin == null ? DBNameMin : init.DBNameMin;
				
				FullData.Add(init.Addr, init);
				if (MaxAddr < init.Addr) {
					MaxAddr = init.Addr;
				}
			}
			
		}

	}

}
