using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModbusLib;

namespace DBConsole
{
	class Program
	{
		static ModbusInitDataArray arrHH;


		static int step=0;
		static ModbusDataReader mbDRHH;

		static void Main(string[] args) {

			try {
				Settings.init();
				Logger.init(Logger.createFileLogger(Settings.single.LogPath, "log", new Logger()));
				arrHH = XMLSer<ModbusInitDataArray>.fromXML("Data\\MBDataFull.xml");
				arrHH.processData();
				Logger.Info(String.Format("Чтение настроек ModBus: {0} записей", arrHH.MaxAddr));

				DateTime DateEnd=DateTime.Now;
				DateEnd = ModbusDataWriter.GetFileDate(DateEnd, RWModeEnum.hh);
				DateTime DateStart=DateEnd.AddHours(-2);

				DateTime date=DateStart.AddHours(0);
				while (date <= DateEnd) {
					DataDBWriter writer=new DataDBWriter(ModbusDataWriter.GetFileName(arrHH, RWModeEnum.hh, date, false),arrHH);
					writer.ReadAll();
					Console.WriteLine("read " + date);
				}

				Console.ReadLine();
			} catch (Exception e) {
				Logger.Error(e.ToString());
			}
		}
	}
}
