using System;
using System.IO;
using System.Collections.Generic;

namespace BuddyApp.Weather
{
	public class CSVReader
	{
		private StreamReader streamReader;
		private char separator = ';';

		/**
		 * Basic constructor. 
		 * 
		 * 
		 */
		public CSVReader ()
		{
		}

		/**
		 * Init the reading of csv file. 
		 * 
		 * 
		 */
		public void Read(string filepath, char separator = ';') {
			FileStream file = File.OpenRead (filepath);
			this.streamReader = new StreamReader (file);
			this.separator = separator;
		}

		/**
		 * Allow you to give the streamReader to this reader. 
		 * 
		 * 
		 */
		public void SetStreamReader(StreamReader streamReader, char separator = ';') {
			this.streamReader = streamReader;
			this.separator = separator;
		}

		/**
		 * 
		 * 
		 */
		public string[] Next() {
			if (this.streamReader.EndOfStream)
				return null;
			string line = this.streamReader.ReadLine();
			if (line != null)
				return line.Split (this.separator);
			else
				return null;
		}

		/**
		 * Close file
		 */
		public void Close() {
			this.streamReader.Close();
		}
	}
}

