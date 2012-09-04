using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusLib
{
	public class ModbusServers
	{
		private static ModbusServers single;
		
		protected ModbusServers() {
			servers = new SortedList<string, ModbusServer>(); 
		}
		protected SortedList<string,ModbusServer> servers;

		protected ModbusServer get(string ip, ushort port) {
			string full=ip + ":" + port;
			if (servers.ContainsKey(full)) {
				ModbusServer server=servers[full];
				return server;
			} else {
				ModbusServer server = new ModbusServer(ip, port);
				servers.Add(full, server);
				return server;
			}
		}

		public static ModbusServer Get(string ip, ushort port) {
			if (single == null) {
				single = new ModbusServers();
			}
			return single.get(ip, port);
		}

	}

	public class ModbusServer
	{
		private string ip;
		public string IP {
			get { return ip; }
			protected set { ip = value; }
		}

		private ushort port;
		public ushort Port {
			get { return port; }
			protected set { port = value; }
		}

		private Master modbusMaster;
		public Master ModbusMaster {
			get { 
				if (!modbusMaster.connected){
					modbusMaster.connect(ip, port);
				}
				return modbusMaster;
			}
			protected set { modbusMaster = value; }
		}

		public ModbusServer(string ip, ushort port) {
			this.ip = ip;
			this.port = port;
			this.modbusMaster = new Master(ip, port);
		}		
		
	}

	public delegate void FinishEvent(SortedList<int, double> ResultData);

	public class ModbusDataReader
	{
		public event FinishEvent OnFinish;

		private SortedList<int,int> data;
		public SortedList<int, int> Data {
			get { return data; }
			protected set { data = value; }
		}

		private int countData;
		public int CountData {
			get { return countData; }
			protected set { countData = value; }
		}


		private ushort stepData=50;
		public ushort StepData {
			get { return stepData; }
			protected set { stepData = value; }
		}

		private ModbusServer server;
		public ModbusServer Server {
			get { return server; }
			set { server = value; }
		}

		private ModbusInitDataArray initArr;
		public ModbusInitDataArray InitArr {
			get { return initArr; }
			set { initArr = value; }
		}

		public ModbusDataReader(ModbusServer server, ModbusInitDataArray initArr) {
			this.server = server;
			this.countData = initArr.MaxAddr;
			this.InitArr = initArr;
			data = new SortedList<int, int>(countData);
			server.ModbusMaster.OnResponseData += new Master.ResponseData(ModbusMaster_OnResponseData);
		}

		protected ushort startAddr;
		protected bool finished;

		public void readData() {
			startAddr=0;
			finished = false;
			Data.Clear();
			continueRead();
		}

		protected void continueRead() {
			server.ModbusMaster.ReadInputRegister(startAddr, startAddr, (ushort)(stepData * 2));
			startAddr += (ushort)(stepData * 2);
			finished = (startAddr > countData * 2);
		}

		void ModbusMaster_OnResponseData(ushort id, byte function, byte[] data) {

			int[] word=new int[data.Length / 2];
			for (int i=0; i < data.Length; i = i + 2) {
				word[i / 2] = data[i] * 256 + data[i + 1];
			}

			ushort startAddr=id;
			foreach (int w in word) {
				if (Data.ContainsKey(startAddr)) {
					Data[startAddr] = w;
				} else {
					Data.Add(startAddr, w);
				}
				startAddr++;
			}

			if (!finished) {
				continueRead();
			} else {
				Logger.Info("Finish reading");
				SortedList<int, double> ResultData=getResultData();
				if (OnFinish != null) {
					OnFinish(ResultData);
				}
			}
		}

		public SortedList<int, double> getResultData() {
			SortedList<int, double> ResultData=new SortedList<int,double>(CountData);
			foreach (ModbusInitData initData in InitArr.Data) {
				if (Data.ContainsKey(initData.Addr)) {
					ResultData.Add(initData.Addr, Data[initData.Addr] * initData.Scale);
				}
			}
			return ResultData;
		}

	}

}
