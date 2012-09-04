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
		static ModbusInitDataArray arrHH;

		static ModbusDataWriter mbDWHH;
		static ModbusDataWriter mbDWMin;

		static int step=0;
		static ModbusDataReader mbDRHH;

		static void Main(string[] args) {
			
			try {
				Settings.init();
				Logger.init(Logger.createFileLogger(Settings.single.LogPath,"log", new Logger()));
				arrHH = XMLSer<ModbusInitDataArray>.fromXML("Data\\MBDataFull.xml");
				arrHH.processData();
				Logger.Info(String.Format("Чтение настроек ModBus: {0} записей", arrHH.MaxAddr));
				
				mbDWHH = new ModbusDataWriter(arrHH);
				mbDWMin = new ModbusDataWriter(arrHH,RWModeEnum.min);

				ModbusServer svHH=new ModbusServer(arrHH.IP,(ushort)arrHH.Port);
				mbDRHH = new ModbusDataReader(svHH, arrHH);
				mbDRHH.OnFinish += new FinishEvent(mbDRHH_OnFinish);
				mbDRHH.readData();

				Console.ReadLine();
			} catch (Exception e) {
				Logger.Error(e.ToString());
			}
		}

		static void mbDRHH_OnFinish(SortedList<int, double> ResultData) {
			step++;
			Console.WriteLine(step);
			
			mbDWHH.writeData(ResultData);
			mbDWMin.writeData(ResultData);

			Thread.Sleep(5000);
			mbDRHH.readData();
		}

		
	}
}

