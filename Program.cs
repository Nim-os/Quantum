using System;

namespace steam_reminder_bot
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.ForegroundColor = ConsoleColor.White;

			Console.Title = "Reminder Bot";

			var bot = new Bot();

			Console.WriteLine("Booting up bot...");

			bot.StartUp();
		}
	}
}
