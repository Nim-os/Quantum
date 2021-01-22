using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace steam_reminder_bot
{
	
	public delegate void Command(SteamKit2.SteamFriends.FriendMsgCallback callback, string[] options, string[] arguments);

	public static class CommandList
	{
		/* Command template
		 * 
		commands.Add("!.", (callback, options, arguments) =>
		{

		});
		*/
		public static Dictionary<string, Command> RegularCommands()
		{
			Dictionary<string, Command> commands = new Dictionary<string, Command>();

			

			commands.Add("!help", (callback, options, arguments) =>
			{
				Bot.SendChat(callback.Sender, "Available commands:\n" +
					"!ping, !reminder");
			});

			commands.Add("!hello", (callback, options, arguments) =>
			{
				Bot.SendChat(callback.Sender, "Hello!");
			});

			commands.Add("!ping", (callback, options, arguments) =>
			{
				Bot.SendChat(callback.Sender, "Pong!");
			});

			commands.Add("!reminder", (callback, options, arguments) =>
			{
				Bot.SendChat(callback.Sender, "Unfortunately that service is not set up yet:( Check back later!");
			}); 
						

			return commands;
		}

		/*public static Dictionary<string, Command> ElevatedCommands()
		{
			Dictionary<string, Command> commands = new Dictionary<string, Command>();

			return commands;
		}*/

		public static Dictionary<string, Command> AdminCommands()
		{
			Dictionary<string, Command> commands = new Dictionary<string, Command>();

			commands.Add(".help", (callback, options, arguments) =>
			{
				Bot.SendChat(callback.Sender, "Admin commands:\n" +
					".shutdown -\\-\\ Shutdown the bot.\n" +
					".restart -\\-\\ Restart the bot.\n" +
					".log -\\-\\ Logs a message to the bot's console.\n" +
					".echo -\\-\\ Echos back a message.\n");
				return;
			});

			commands.Add(".shutdown", (callback, options, arguments) =>
			{
				Bot.SendChat(callback.Sender, "Goodnight...");
				Bot.steamUser.LogOff();
			});

			commands.Add(".restart", (callback, options, arguments) =>
			{
				Bot.SendChat(callback.Sender, "Attempting to restart bot. <Command not yet implemented>");
			});
			
			commands.Add(".log", (callback, options, arguments) =>
			{
				Console.WriteLine($"{callback.Sender} at {System.DateTime.Now}: {Bot.CompressStrings(arguments)}");
				Bot.SendChat(callback.Sender, "Message logged.");
			});
			
			commands.Add(".echo", (callback, options, arguments) =>
			{
				Bot.SendChat(callback.Sender, Bot.CompressStrings(arguments));
			});

			commands.Add(".debug", (callback, options, arguments) =>
			{
				var args = new StringBuilder();
				foreach (string arg in arguments)
				{
					args.Append($"{arg}\n");
				}
				Bot.SendChat(callback.Sender, $"Options:\nTODO\n\nArguments:\n{args}");
			});

			return commands;
		}
	}
}
