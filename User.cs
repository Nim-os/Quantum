using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace mastodonte_bot
{
	public class User : IDisposable
	{
		public SteamKit2.SteamID userID;

		public Timer timer;

		public Reminder? reminder;

		public void Dispose()
		{
			timer.Dispose();
		}
	}


	public struct Reminder
	{
		public string message;
	}
}
