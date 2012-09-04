using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusLib
{
	public class ModbusInitData
	{
		private string id;
		[System.Xml.Serialization.XmlAttribute]
		public string ID {
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


		private bool writeToDB;
		public bool WriteToDB {
			get { return writeToDB; }
			set { writeToDB = value; }
		}

		private int parNumber;
		public int ParNumber {
			get { return parNumber; }
			set { parNumber = value; }
		}
		
	}

	public class ModbusInitDataArray
	{
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

		public void processData() {
			FullData = new SortedList<int, ModbusInitData>();
			MaxAddr = 0;
			foreach (ModbusInitData init in Data) {
				FullData.Add(init.Addr, init);
				if (MaxAddr < init.Addr) {
					MaxAddr = init.Addr;
				}
			}
			
		}

	}

}
