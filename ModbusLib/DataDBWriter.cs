using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ModbusLib.Piramida;
using System.Data.SqlClient;

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
			while ((valsStr = Reader.ReadLine()) != null) {
				string[]valsArr=valsStr.Split(';');
				bool isFirst=true;
				int index=0;
				foreach (string valStr in valsArr) {
					if (!isFirst) {
						double val=Convert.ToDouble(valStr);
						int header=Headers[index];
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

		public void writeData() {
			SortedList<string,List<string>> inserts=new SortedList<string, List<string>>();
			SortedList<string,List<string>> deletes=new SortedList<string, List<string>>();
			string insertIntoHeader="INSERT INTO Data (parnumber,object,item,value0,objtype,data_date,rcvstamp,season)";
			string frmt="SELECT {0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}";
			string frmDel="(parnumber={0} and object={1} and objType={2} and item={3} and data_date='{4}')";
			foreach (DataDBRecord rec in Data.Values) {
				ModbusInitData init=InitArray.FullData[rec.Header];
				if (init.WriteToDBHH || init.WriteToDBMin) {
					if (init.WriteToDBMin) {
						string insert=String.Format(frmt, init.ParNumberMin, init.Obj, init.Item, rec.Avg, init.ObjType, Date.AddMinutes(30), DateTime.Now, 0);
						string delete=String.Format(frmDel, init.ParNumberMin, init.Obj, init.ObjType, init.Item, Date.AddMinutes(30));
						if (!inserts.ContainsKey(init.DBNameMin)) {
							inserts.Add(init.DBNameMin, new List<string>());
							deletes.Add(init.DBNameMin, new List<string>());
						}

						inserts[init.DBNameMin].Add(insert);
						deletes[init.DBNameMin].Add(delete);
					}

					if (init.WriteToDBHH) {
						string insert=String.Format(frmt, init.ParNumberHH, init.Obj, init.Item, rec.Avg, init.ObjType, Date.AddMinutes(1), DateTime.Now, 0);
						string delete=String.Format(frmDel, init.ParNumberHH, init.Obj, init.ObjType, init.Item, Date.AddMinutes(1));
						if (!inserts.ContainsKey(init.DBNameHH)) {
							inserts.Add(init.DBNameHH, new List<string>());
							deletes.Add(init.DBNameHH, new List<string>());
						}
						inserts[init.DBNameHH].Add(insert);
						deletes[init.DBNameHH].Add(delete);
					}
				}
			}

			foreach (KeyValuePair<string,List<string>> de in deletes) {
				foreach (string del in de.Value) {
					string delSQL=String.Format("DELETE from DATA where {0}", del);
					SqlCommand command=null;
					command = PiramidaAccess.getConnection(de.Key).CreateCommand();
					command.CommandText = delSQL;
					Logger.Info(delSQL);
					command.ExecuteNonQuery();
				}
			}
			
			foreach (KeyValuePair<string,List<string>> de in inserts) {
				List<string> qInserts=new List<string>();
				for (int i=0; i < de.Value.Count; i++) {					
					qInserts.Add(de.Value[i]);
					if ((i+1)%20==0 ||i==de.Value.Count-1){
						string insertsSQL=String.Join("\nUNION ALL\n", qInserts);
						string insertSQL=String.Format("{0}\n{1}", insertIntoHeader, insertsSQL);
						SqlCommand command=null;
						command = PiramidaAccess.getConnection(de.Key).CreateCommand();
						command.CommandText = insertSQL;
						Logger.Info(insertSQL);
						command.ExecuteNonQuery();
						qInserts=new List<string>();
					}
				}
			}
		}
	}
}
