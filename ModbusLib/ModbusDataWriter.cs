using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ModbusLib
{
	public class ModbusDataWriter
	{
		private DateTime currentDate;
		public DateTime CurrentDate {
			get { return currentDate; }
			set { currentDate = value; }
		}

		private TextWriter currentWriter;
		public TextWriter CurrentWriter {
			get { return currentWriter; }
			set { currentWriter = value; }
		}

		private ModbusInitDataArray initArray;
		public ModbusInitDataArray InitArray {
			get { return initArray; }
			set { initArray = value; }
		}

		private List<int> headers;
		public List<int> Headers {
			get { return headers; }
			set { headers = value; }
		}

		private string headerStr;
		public string HeaderStr {
			get { return headerStr; }
			set { headerStr = value; }
		}

		private RWModeEnum rwMode;
		public RWModeEnum RWMode {
			get { return rwMode; }
			set { rwMode = value; }
		}

		public static string GetDir(ModbusInitDataArray InitArray, RWModeEnum RWMode, DateTime date) {
			string dirName=String.Format("{0}\\{1}\\{2}\\{3}",Settings.single.DataPath,InitArray.ID,RWMode.ToString(),date.ToString("yyyy_MM_dd"));
			return dirName;
		}

		public static String GetFileName(ModbusInitDataArray InitArray, RWModeEnum RWMode, DateTime date,bool createDir) {
			string dirName=GetDir(InitArray, RWMode, date);
			if (createDir) {
				Directory.CreateDirectory(dirName);
			}
			string fileName=String.Format("{0}\\data_{1}.csv",dirName,date.ToString("hh_mm"));
			return fileName;
		}

		public static DateTime GetFileDate(DateTime date, RWModeEnum RWMode) {
			int min=date.Minute;
			if (RWMode == RWModeEnum.hh) {
				min = min < 30 ? 0 : 30;
			}
			DateTime dt=new DateTime(date.Year, date.Month, date.Day, date.Hour - 2, min, 0);
			return dt;
		}
				
		protected void getWriter(DateTime date) {
			DateTime dt=GetFileDate(DateTime.Now, RWMode);
			if (dt != currentDate) {
				try {					
					currentWriter.Close();
				} catch (Exception e) { }
				currentDate = dt;

				string fileName=GetFileName(InitArray, RWMode, currentDate, true);
				
				HeaderStr = String.Format("{0};{1}", CurrentDate.ToString("dd.MM.yyyy hh:mm:ss"), String.Join(";", Headers));
				bool newFile=!File.Exists(fileName);				
				currentWriter=new StreamWriter(fileName,true);
				if (newFile) {
					currentWriter.WriteLine(HeaderStr);
				}
			}
		}

		public ModbusDataWriter(ModbusInitDataArray arr, RWModeEnum mode = RWModeEnum.hh) {
			InitArray = arr;
			Headers = new List<int>();
			foreach (ModbusInitData data in arr.Data) {
				if (!data.Name.Contains("_FLAG") && !String.IsNullOrEmpty(data.Name)) {
					Headers.Add(data.Addr);
				}
			}
			rwMode = mode;
		}

		public void writeData( SortedList<int, double> ResultData) {
			getWriter(DateTime.Now);
			
			List<double> values=new List<double>();
			foreach (KeyValuePair<int,double> de in ResultData) {
				if (Headers.Contains(de.Key)) {
					values.Add(de.Value);
				}
			}
			string valueStr=String.Format("{0};{1}",DateTime.Now.AddHours(-2).ToString("dd.MM.yyyy hh:mm:ss"), String.Join(";", values)).Replace(',','.');
			CurrentWriter.WriteLine(valueStr);
			CurrentWriter.Flush();
		}
	}
}
