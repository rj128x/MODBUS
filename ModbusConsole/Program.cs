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

		static void Main(string[] args) {
			Settings.init();
			/*Settings.single.InitFiles = new List<string>();
			Settings.single.InitFiles.Add("fsd");
			XMLSer<Settings>.toXML(Settings.single,"c:\\out1.xml");*/
			Console.WriteLine(Settings.single.InitFiles.Count);
			Logger.init(Logger.createFileLogger(Settings.single.LogPath, "logR", new Logger()));

			MasterModbusReader reader=new MasterModbusReader(5000);
			reader.Read();


			Console.ReadLine();
		}

	}
}

