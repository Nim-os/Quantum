using System;
using System.Collections.Generic;
using System.Text;
using SteamKit2;
using System.IO;

namespace steam_reminder_bot
{
	static class FileManager
	{
		private const string adminListPath = "adminlist.txt";


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
	}
}
