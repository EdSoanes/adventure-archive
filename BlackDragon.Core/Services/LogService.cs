using System;

namespace BlackDragon.Core
{
	public class LogService : ILogService
    {
		private object _logLock = new object();
		private bool _enabled = false;
        public LogService()
        {
        }

		#region ILogService implementation

		public void Enable(bool enable)
		{
			lock (_logLock)
				_enabled = enable;
		}

		public void Info(string msg)
		{
			lock (_logLock)
			{
				if (_enabled)
					Console.WriteLine("INFO: " + msg);
			}
		}

		public void Warn(string msg)
		{
			lock (_logLock)
			{
				if (_enabled)
					Console.WriteLine("WARN: " + msg);
			}
		}

		public void Error(string msg)
		{
			lock (_logLock)
			{
				if (_enabled)
					Console.WriteLine("ERR: " + msg);
			}
		}

		public void Error(string msg, Exception ex)
		{
			lock (_logLock)
			{
				if (_enabled)
					Console.WriteLine("ERR: " + msg + ". " + ex.ToString());
			}
		}

		public void Error(Exception ex)
		{
			lock (_logLock)
			{
				if (_enabled)
					Console.WriteLine("ERR: " + ex.ToString());
			}
		}

		#endregion
    }
}

