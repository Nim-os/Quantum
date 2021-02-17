using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Quantum
{
	// Can later change to Action<callback, options, arguments) if it makes a difference
	public delegate void Command(SteamKit2.SteamFriends.FriendMsgCallback callback, string options, string[] arguments);

	public static class CommandList
	{
		public static Dictionary<string, Command> RegularList = RegularCommands();
		public static Dictionary<string, Command> AdminList = AdminCommands();

		/* Command template
		 * 
		commands.Add("!.", (callback, options, arguments) =>
		{

		});
		*/

		private static Dictionary<string, Command> RegularCommands()
		{
			Dictionary<string, Command> commands = new Dictionary<string, Command>();

			#region Commands

			commands.Add("!help", (callback, options, arguments) =>
			{
				Bot.SendChat(callback.Sender, "Available commands:\n" + // Optional parts of a command potentially? Best way to indicate command use?
					"!reminder -\\-\\ <time> <-minutes or -hours> <message>");
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
				if (arguments.Length < 2)
				{
					Bot.SendChat(callback.Sender, "Not enough arguments for !reminder.\n" +
						"Make sure to format your command as such: !reminder <time> <-minutes or -hours> <message>");
					return;
				}

				float time = 0;
				string unit = "";
				double totalMinutes = 0;

				try
				{
					time = float.Parse(arguments[0], System.Globalization.CultureInfo.InvariantCulture);
				}
				catch
				{
					Bot.SendChat(callback.Sender, "Number not correctly formatted in command.\n" +
						"Be sure it is the first value after !reminder and is not connected to anything!");
					return;
				}

				if (time < 1 || time > 120)
				{
					Bot.SendChat(callback.Sender, "Time cannot be less than 1 or greater than 120!");
					return;
				}

				if (options.Contains('m'))
				{
					unit = "minutes";
					totalMinutes = time;
				}
				else if (options.Contains('h'))
				{
					unit = "hours";
					totalMinutes = time * 60;
				}
				else
				{
					Bot.SendChat(callback.Sender, "Improper unit of time for reminder set!\n" +
						"Make sure to include -minutes or -hours");
					return;
				}

				if (time == 1)
				{
					unit = unit.Substring(0, unit.Length - 1);
				}


				var str = new StringBuilder();

				for (int i = 1; i < arguments.Length; i++)
				{
					str.Append($"{arguments[i]} ");
				}

				if (string.IsNullOrWhiteSpace(str.ToString()))
				{
					str.Append("No message given.");
				}


				var user = new User
				{
					userID = callback.Sender,
					timer = new Timer(TimeSpan.FromMinutes(totalMinutes).TotalMilliseconds),
					reminder = new Reminder
					{
						message = str.ToString(),
					}
				};

				user.timer.Elapsed += (sender, e) => Bot.HandleReminder(user);
				user.timer.AutoReset = false;
				user.timer.Enabled = true;


				Bot.SendChat(callback.Sender, $"Reminder successfully set. See you in {time} {unit}!");
			});

			#endregion

			return commands;
		}

		/*public static Dictionary<string, Command> ElevatedCommands()
		{
			Dictionary<string, Command> commands = new Dictionary<string, Command>();

			return commands;
		}*/

		private static Dictionary<string, Command> AdminCommands()
		{
			Dictionary<string, Command> commands = new Dictionary<string, Command>();

			#region Commands

			commands.Add(".", (callback, options, arguments) =>
			{
				Bot.SendChat(callback.Sender, $"You are an admin! Welcome back {callback.Sender.AccountID}.");
			});

			commands.Add(".help", (callback, options, arguments) =>
			{
				Bot.SendChat(callback.Sender, "Admin commands:\n" +
					".shutdown -\\-\\ Shutdown the bot.\n" +
					".restart -\\-\\ Restart the bot.\n" +
					".log -\\-\\ Logs a message to the bot's console.\n" +
					".echo -\\-\\ Echos back a message.\n");
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
				Console.WriteLine($"{callback.Sender} at {System.DateTime.Now}: {Utility.CompressStrings(arguments)}");
				Bot.SendChat(callback.Sender, "Message logged.");
			});
			
			commands.Add(".echo", (callback, options, arguments) =>
			{
				Bot.SendChat(callback.Sender, Utility.CompressStrings(arguments));
			});

			commands.Add(".debug", (callback, options, arguments) =>
			{
				var args = new StringBuilder();
				foreach (string arg in arguments)
				{
					args.Append($"{arg}\n");
				}
				Bot.SendChat(callback.Sender, $"Options:\n{options}\n\nArguments:\n{args}");
			});

			#endregion

			return commands;
		}
	}
}
