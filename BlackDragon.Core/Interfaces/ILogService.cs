using System;

namespace BlackDragon.Core
{
	public interface ILogService
    {
		void Enable(bool enable);
		void Info(string msg);
		void Warn(string msg);
		void Error(string msg);
		void Error(string msg, Exception ex);
		void Error(Exception ex);
    }
}

