using System;
using System.Collections.Generic;
using System.Text;

namespace mastodonte_bot
{
	public class User
	{
		public SteamKit2.SteamID userID;

		public Reminder? reminder;
	}


	public struct Reminder
	{
		public ulong remindAt;
		public string message;
	}
}
