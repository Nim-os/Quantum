using System;
using System.Collections.Generic;
using System.Text;
using SteamKit2;
using System.IO;

namespace steam_reminder_bot
{
	struct LoginInfo
	{
		public string user;
		public string pass;
	}

	static class FileManager
	{
		private const string secretPath = "secret.txt";
		private const string adminListPath = "adminlist.txt";


		public static LoginInfo? GetSecret()
		{
			if(!File.Exists(secretPath))
			{
				return null;
			}

			LoginInfo info = new LoginInfo();

			var reader = File.ReadLines(secretPath).GetEnumerator();

			if (reader.MoveNext())
			{
				info.user = reader.Current;

				if (reader.MoveNext())
				{
					info.pass = reader.Current;
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}

			return info;
		}


		public static List<SteamID> GetAdmins()
		{
			List<SteamID> ret = new List<SteamID>();

			var reader = File.ReadLines(adminListPath).GetEnumerator();

			while (reader.MoveNext())
			{
				ret.Add(new SteamID
				{
					AccountID = uint.Parse(reader.Current)
				});
			}

			return ret;
		}

		//public static void UpdateEntry(SteamID user, Reminder reminder)
		//Run through each line

		//public static bool RemoveEntry(SteamID user)
	}

}
