using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModbusLib;
using System.Net;
using System.IO;
using System.Threading;

namespace ModbusConsole
{
	class Program
	{
		static ModbusInitDataArray arr;
		static ModbusDataWriter mbDW;
		static int step=0;
		static ModbusDataReader mbDR;

		static void Main(string[] args) {
			
			try {
				Settings.init();
				Logger.init(Logger.createFileLogger(Settings.single.LogPath,"log", new Logger()));
				arr=XMLSer<ModbusInitDataArray>.fromXML("Data\\MBData.xml");				
				arr.processData();
				Logger.Info(String.Format("Чтение настроек ModBus: {0} записей", arr.MaxAddr));
				
				Logger.Info("start");

				mbDW=new ModbusDataWriter(arr);

				ModbusServer sv=new ModbusServer(arr.IP,(ushort)arr.Port);
				mbDR = new ModbusDataReader(sv, arr);
				mbDR.OnFinish += new FinishEvent(mbDR_OnFinish);
				mbDR.readData();
				Console.ReadLine();
			} catch (Exception e) {
				Logger.Error(e.ToString());
			}
		}

		static void mbDR_OnFinish(SortedList<int, double> ResultData) {
			step++;
			Console.WriteLine(step);
			mbDW.writeData(ResultData);
			Thread.Sleep(5000);
			mbDR.readData();
		}

	}
}
