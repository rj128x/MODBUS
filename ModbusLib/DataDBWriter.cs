using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ModbusLib
{
	public class DataDBRecord
	{
		private int header;
		public int Header {
			get { return header; }
			set { header = value; }
		}

		private double min;
		public double Min {
			get { return min; }
			set { min = value; }
		}

		private double max;
		public double Max {
			get { return max; }
			set { max = value; }
		}

		private double avg;
		public double Avg {
			get { return avg; }
			set { avg = value; }
		}

		private double count;
		public double Count {
			get { return count; }
			set { count = value; }
		}

		public DataDBRecord(int header) {
			this.Header = header;
			Min = 10e10;
			Max = -10e10;
			Avg = 0;
			Count = 0;
		}
	}

	public class DataDBWriter
	{
		private string fileName;
		public string FileName {
			get { return fileName; }
			set { fileName = value; }
		}

		private TextReader reader;
		public TextReader Reader {
			get { return reader; }
			set { reader = value; }
		}

		private List<int> headers;
		public List<int> Headers {
			get { return headers; }
			set { headers = value; }
		}

		private SortedList<int,DataDBRecord> data;
		public SortedList<int, DataDBRecord> Data {
			get { return data; }
			set { data = value; }
		}

		private List<DateTime> dates;
		public List<DateTime> Dates {
			get { return dates; }
			set { dates = value; }
		}

		private DateTime date;
		public DateTime Date {
			get { return date; }
			set { date = value; }
		}

		private ModbusInitDataArray initArray;
		public ModbusInitDataArray InitArray {
			get { return initArray; }
			set { initArray = value; }
		}


		public DataDBWriter(string fileName, ModbusInitDataArray initArray) {
			FileName = fileName;			
			Headers = new List<int>();
			Dates = new List<DateTime>();
			Data = new SortedList<int, DataDBRecord>();
			InitArray = initArray;
		}

		public void ReadAll() {
			reader = new StreamReader(fileName);
			readHeader();
			readData();
			Reader.Close();
			foreach (DataDBRecord rec in Data.Values) {
				rec.Avg = rec.Avg / rec.Count;
			}

		}

		protected void readHeader() {
			string headerStr=Reader.ReadLine();
			string[] headersArr=headerStr.Split(';');
			bool isFirst=true;
			foreach (string header in headersArr) {
				if (!isFirst) {
					int val=Convert.ToInt32(header);
					Headers.Add(val);
					Data.Add(val, new DataDBRecord(val));
				} else {
					Date = DateTime.Parse(header);
				}
				isFirst = false;
			}
		}

		protected void readData() {
			string valsStr;
			while ((valsStr=Reader.ReadLine())!=null){
				string[]valsArr=valsStr.Split(';');
				bool isFirst=false;
				int index=0;
				foreach (string valStr in valsArr) {				
					if (!isFirst) {
						double val=Convert.ToDouble(valStr);
						int header=Headers[index - 1];
						Data[header].Avg += val;
						if (Data[header].Min > val) {
							Data[header].Min = val;
						}
						if (Data[header].Max < val) {
							Data[header].Max = val;
						}
						Data[header].Count++;
						index++;
					} else {
						Dates.Add(DateTime.Parse(valStr));
					}
					isFirst = false;
				}
			}
			
		}

		protected void writeData() {
			SortedList<string,List<string>> inserts=new SortedList<string, List<string>>();
			string insertIntoHeader="INSERT INTO Data (parnumber,object,item,value0,objtype,data_date,rcvstamp)";
			foreach (DataDBRecord rec in Data.Values) {
				if (InitArray.FullData[rec.Header].WriteToDB) {
					ModbusInitData init=InitArray.FullData[rec.Header];
					string insert=String.Format("SELECT {0}, {1}, {2}, {3}, {4}, {5}, {6}", init.ParNumber, init.Obj, init.Item, rec.Avg, init.ObjType, Date.AddMinutes(30), DateTime.Now);
					if (!inserts.ContainsKey(init.DBName)) {
						inserts.Add(init.DBName, new List<string>());
					}
					inserts[init.DBName].Add(insert);
				}
			}


		}
	}
}
