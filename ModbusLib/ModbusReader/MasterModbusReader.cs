using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ModbusLib
{
	public class MasterModbusReader
	{
		private int sleepTime;
		public int SleepTime {
			get { return sleepTime; }
			set { sleepTime = value; }
		}

		private SortedList<string,ModbusInitDataArray> initArrays;
		public SortedList<string, ModbusInitDataArray> InitArrays {
			get { return initArrays; }
			set { initArrays = value; }
		}

		private SortedList<string,ModbusDataReader> readers;
		public SortedList<string, ModbusDataReader> Readers {
			get { return readers; }
			set { readers = value; }
		}

		private SortedList<string,ModbusDataWriter>writersHH;
		public SortedList<string, ModbusDataWriter> WritersHH {
			get { return writersHH; }
			set { writersHH = value; }
		}

		private SortedList<string,ModbusDataWriter>writersMin;
		public SortedList<string, ModbusDataWriter> WritersMin {
			get { return writersMin; }
			set { writersMin = value; }
		}

		private SortedList<string,bool>finishReading;
		public SortedList<string, bool> FinishReading {
			get { return finishReading; }
			set { finishReading = value; }
		}
		
		public MasterModbusReader(int sleepTime) {
			SleepTime = sleepTime;
			InitArrays = new SortedList<string, ModbusInitDataArray>();
			Readers = new SortedList<string, ModbusDataReader>();
			WritersHH = new SortedList<string, ModbusDataWriter>();
			WritersMin = new SortedList<string, ModbusDataWriter>();
			FinishReading = new SortedList<string, bool>();
			foreach (string fileName in Settings.single.InitFiles) {
				try {
					Logger.Info(String.Format("Чтение настроек modbus из файла '{0}'", fileName));
					ModbusInitDataArray arr = XMLSer<ModbusInitDataArray>.fromXML(fileName);
					arr.processData();
					InitArrays.Add(arr.ID, arr);
					String.Format("===Считано {0} записей", arr.FullData.Count);

					Logger.Info(String.Format("Создание объекта чтения данных"));
					ModbusServer sv=new ModbusServer(arr.IP, (ushort)arr.Port);
					ModbusDataReader reader=new ModbusDataReader(sv, arr);
					reader.OnFinish += new FinishEvent(reader_OnFinish);
					readers.Add(arr.ID, reader);
					String.Format("===Объект создан");

					if (arr.WriteMin) {
						Logger.Info(String.Format("Создание объекта записи данных в файл (минуты)"));
						ModbusDataWriter writer=new ModbusDataWriter(arr, RWModeEnum.min);
						writersMin.Add(arr.ID, writer);
						String.Format("===Объект создан");
					}

					if (arr.WriteHH) {
						Logger.Info(String.Format("Создание объекта записи данных в файл (получасовки)"));
						ModbusDataWriter writer=new ModbusDataWriter(arr, RWModeEnum.hh);
						writersHH.Add(arr.ID, writer);
						String.Format("===Объект создан");
					}

					FinishReading.Add(arr.ID, false);

				} catch (Exception e) {
					String.Format("===Ошибка при чтении настроек");
					Logger.Error(e.ToString());
				}
			}
		}

		public void Read() {
			foreach (string key in InitArrays.Keys) {
				FinishReading[key] = false;
			}
			foreach (KeyValuePair<string,ModbusDataReader> de in Readers) {
				de.Value.readData();
			}			
		}
		
		public void reader_OnFinish(string InitArrayID, SortedList<int, double> ResultData) {
			Console.WriteLine(InitArrayID + "  read");
			ModbusInitDataArray init=initArrays[InitArrayID];
			if (init.WriteMin) {
				WritersMin[InitArrayID].writeData(ResultData);
			}
			if (init.WriteHH) {
				WritersHH[InitArrayID].writeData(ResultData);
			}
			FinishReading[InitArrayID] = true;
			if (!FinishReading.Values.Contains(false)) {
				Thread.Sleep(SleepTime);
				Read();
			}
		}


	}
}
