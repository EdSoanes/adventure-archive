using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackDragon.Core
{
	public class FileCacheEntry
	{
		private object _data;

		public string Url
		{
			get;
			private set;
		}

		public bool Persisted
		{
			get;
			set;
		}

		public FileCacheTypes FileType
		{
			get;
			private set;
		}

		public DateTime AddedTime
		{
			get;
			private set;
		}

		public DateTime LastAccessedTime
		{
			get;
			private set;
		}

		public FileCacheEntry(string url, FileCacheTypes type, object data)
		{
			Url = url;
			FileType = type;
			_data = data;
		}

		public T GetData<T>() where T : class
		{
			return _data as T;
		}

		public void SetData(object data)
		{
			_data = data;
		}

		public void SetAddedTime()
		{
			if (AddedTime != DateTime.MinValue)
				AddedTime = DateTime.Now;

			SetLastAccessedTime();
		}

		public void SetLastAccessedTime()
		{
			LastAccessedTime = DateTime.Now;
		}

		public override string ToString()
		{
			return string.Format("[FileCacheEntry: Key={0}, FileType={1}, AddedTime={2}, LastAccessedTime={3}]", Url, FileType, AddedTime, LastAccessedTime);
		}
	}
}
