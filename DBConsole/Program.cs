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

		static void Main(string[] args) {

			try {
				Settings.init();
				Logger.init(Logger.createFileLogger(Settings.single.LogPath, "logW", new Logger()));
				arrHH = XMLSer<ModbusInitDataArray>.fromXML("Data\\MBDataFullDB.xml");
				arrHH.processData();
				Logger.Info(String.Format("Чтение настроек ModBus: {0} записей", arrHH.MaxAddr));

				//DateTime DateEnd=DateTime.Now;
				DateTime DateEnd=new DateTime(2012, 9, 5, 13, 05, 0);
				DateEnd = ModbusDataWriter.GetFileDate(DateEnd, RWModeEnum.hh).AddMinutes(-30);
				DateTime DateStart=DateEnd.AddHours(-2);

				DateTime date=DateStart.AddHours(0);
				while (date <= DateEnd) {
					try {
						DataDBWriter writer=new DataDBWriter(ModbusDataWriter.GetFileName(arrHH, RWModeEnum.hh, date, false), arrHH);
						writer.ReadAll();
						Console.WriteLine("read " + date);
						writer.writeData();
						Console.WriteLine("write " + date);						
					} catch (Exception e) {
						Logger.Info(e.ToString());
					}finally{
						date = date.AddMinutes(30);
					}
				}

				/*DateTime DateEnd=new DateTime(2012, 9, 5, 10, 20, 0);
				DateEnd = ModbusDataWriter.GetFileDate(DateEnd, RWModeEnum.min).AddMinutes(-1);
				DateTime DateStart=DateEnd.AddMinutes(-4);

				DateTime date=DateStart.AddHours(0);
				while (date <= DateEnd) {
					DataDBWriter writer=new DataDBWriter(ModbusDataWriter.GetFileName(arrHH, RWModeEnum.min, date, false), arrHH);
					writer.ReadAll();
					Console.WriteLine("read " + date);
					writer.writeData();
					Console.WriteLine("write " + date);
					date=date.AddMinutes(1);
				}*/

				Console.ReadLine();
			} catch (Exception e) {
				Logger.Error(e.ToString());
			}
		}
	}
}
