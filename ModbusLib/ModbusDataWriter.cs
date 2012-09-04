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
		
		protected void getWriter(DateTime date) {
			int min=date.Minute;
			min = min < 30 ? 0 : 30;
			DateTime dt=new DateTime(date.Year, date.Month, date.Day, date.Hour - 2, min, 0);
			if (dt != currentDate) {
				try {					
					currentWriter.Close();
				} catch (Exception e) { }
				currentDate = dt;
				string fileName=String.Format("{0}\\data_{1}.csv",Settings.single.DataPath,currentDate.ToString("yyyy_MM_dd_hh_mm"));
				bool newFile=!File.Exists(fileName);				
				currentWriter=new StreamWriter(fileName,true);
				if (newFile) {
					currentWriter.WriteLine(HeaderStr);
				}
			}

		}

		public ModbusDataWriter(ModbusInitDataArray arr) {
			InitArray = arr;
			Headers = new List<int>();
			foreach (ModbusInitData data in arr.Data) {
				if (!data.Name.Contains("_FLAG") && !String.IsNullOrEmpty(data.Name)) {
					Headers.Add(data.Addr);
				}
			}
			HeaderStr = String.Format("date;{0}", String.Join(";", Headers));
		}

		public void writeData( SortedList<int, double> ResultData) {
			getWriter(DateTime.Now);
			
			List<double> values=new List<double>();
			foreach (KeyValuePair<int,double> de in ResultData) {
				if (Headers.Contains(de.Key)) {
					values.Add(de.Value);
				}
			}
			string valueStr=String.Format("{0};{1}",DateTime.Now.AddHours(-2).ToString("dd.MM.yyyy hh:mm:ss"), String.Join(";", values));
			CurrentWriter.WriteLine(valueStr);
			CurrentWriter.Flush();
		}
	}
}
