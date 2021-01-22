using System;
using System.Collections.Generic;
using System.Text;
using SteamKit2;

namespace steam_reminder_bot
{
	public class Bot
	{
		#region Variables

		#region Consts

		const string UnrecognisedMessage = "Unrecognised message, check that your command starts with ! and is typed correctly.\nType !help for a full list of commands!";

		#endregion

		public static Bot instance = null;

		public static SteamClient steamClient = new SteamClient();
		public static CallbackManager manager;
		public static SteamUser steamUser;
		public static SteamFriends steamFriends;

		private string username, password;

		private bool isRunning;

		private List<SteamID> admins;


		#endregion


		#region Setup and Teardown

		public void StartUp()
		{
			if (instance == null)
			{
				instance = this;
			}
			else
			{
				Console.WriteLine($"ERROR: Bot already running.");
				return;
			}

			// Info

			LoginInfo? loginInfo = FileManager.GetSecret();

			if (loginInfo.HasValue)
			{
				Console.WriteLine("Logging in via secret.txt.");

				username = loginInfo.Value.user;
				password = loginInfo.Value.pass;
			}
			else
			{
				Console.WriteLine("Unable to load secret.txt.");

				Console.Write("Username: ");

				username = Console.ReadLine();

				Console.Write("Password: ");

				password = GetHiddenConsoleInput();

				Console.WriteLine();
			}


			// File

			admins = FileManager.GetAdmins();


			// Connection

			manager = new CallbackManager(steamClient);

			steamUser = steamClient.GetHandler<SteamUser>();

			steamFriends = steamClient.GetHandler<SteamFriends>();

			#region Callbacks

			manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
			manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);

			manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
			manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);
			manager.Subscribe<SteamUser.AccountInfoCallback>(OnAccountInfo);

			manager.Subscribe<SteamFriends.FriendsListCallback>(OnFriendsList);
			manager.Subscribe<SteamFriends.FriendMsgCallback>(OnFriendMsg);

			#endregion

			Console.WriteLine("Connecting to Steam...");

			steamClient.Connect();

			isRunning = true;

			while(isRunning)
			{
				manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
			}

			Console.WriteLine("Bot ended successfully.");
		}

		void OnConnected(SteamClient.ConnectedCallback callback)
		{
			Console.WriteLine($"Connected to Steam.\nLogging in {username}");

			steamUser.LogOn(new SteamUser.LogOnDetails
			{
				Username = username,
				Password = password
			});
		}

		void OnDisconnected(SteamClient.DisconnectedCallback callback)
		{
			Console.WriteLine($"Disconnected from Steam.");

			isRunning = false;
		}

		void OnLoggedOn(SteamUser.LoggedOnCallback callback)
		{
			if(callback.Result != EResult.OK)
			{
				switch(callback.Result)
				{
					case EResult.AccountLogonDenied:
						Console.WriteLine("ERROR: Logon denied, SteamGuard required.");
						break;
					default:
						Console.WriteLine($"ERROR: Could not logon to Steam.\n{callback.Result}\n{callback.ExtendedResult}");
						break;
				}

				isRunning = false;
				return;
			}

			Console.WriteLine("Successfully logged on.");
		}

		void OnLoggedOff(SteamUser.LoggedOffCallback callback)
		{
			Console.WriteLine($"Successfully logged off of Steam. {callback.Result}");
		}

		#endregion

		void OnAccountInfo(SteamUser.AccountInfoCallback callback)
		{
			Console.WriteLine($"AccountInfo recieved. {callback.PersonaName} is active.");

			//steamFriends.SetPersonaState(EPersonaState.Online);
		}

		void OnFriendsList(SteamFriends.FriendsListCallback callback)
		{
			foreach (var friend in callback.FriendList)
			{
				Console.WriteLine($"Friend: {friend.SteamID}");

				if (friend.Relationship == EFriendRelationship.RequestRecipient)
				{
					steamFriends.AddFriend(friend.SteamID);
				}
			}
		}

		void OnFriendMsg(SteamFriends.FriendMsgCallback callback)
		{
			SteamID sender = callback.Sender;

			if (callback.EntryType == EChatEntryType.ChatMsg)
			{

				SplitCommand(out string commandName, out string[] options, out string[] arguments, callback.Message);

				commandName = commandName.ToLower();

				#region Message Handling


				Command command;

				if (commandName.StartsWith("!") && CommandList.RegularList.TryGetValue(commandName, out command))
				{
					command(callback, options, arguments);
				}
				else if (commandName.StartsWith("."))
				{
					if(admins.Find(id => id.AccountID == sender.AccountID) != null)
					{
						if (CommandList.AdminList.TryGetValue(commandName, out command))
						{
							command(callback, options, arguments);
						}
						else
						{
							SendChat(sender, "Invalid command.");
						}
					}
					else
					{
						SendChat(sender, $"You do not have sufficient permissions to access this.");
					}
				}
				else
				{
					SendChat(sender, UnrecognisedMessage);
				}
				

				#endregion
			}

		}

		#region Actions

		public static void SendChat(SteamID sender, string message)
		{
			steamFriends.SendChatMessage(sender, EChatEntryType.ChatMsg, message);
		}

		#endregion

		#region Helper Functions

		public static string GetHiddenConsoleInput()
		{
			StringBuilder input = new StringBuilder();
			while (true)
			{
				var key = Console.ReadKey(true);
				if (key.Key == ConsoleKey.Enter)
				{
					break;
				}
				else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
				{
					input.Remove(input.Length - 1, 1);
				}
				else if (key.Key != ConsoleKey.Backspace)
				{
					input.Append(key.KeyChar);
				}
			}
			return input.ToString();
		}

		public static void SplitCommand(out string command, out string[] options, out string[] arguments, string message)
		{
			int index = 0;

			foreach (char c in message)
			{
				if (c == ' ')
				{
					break;
				}

				index += 1;
			}
			
			command = message.Substring(0, index);

			List<string> ops = new List<string>();
			List<string> args = new List<string>();

			var str = new StringBuilder();
			
			bool invertedCommas = false;
			
			index += 1;

			for (; index < message.Length; index++)
			{
				if (message[index] == '\"')
				{
					invertedCommas = !invertedCommas;

					if (!invertedCommas) // End of phrase
					{
						args.Add(str.ToString());

						str.Clear();

						index += 1;
					}

					continue;
				}
				else if (message[index] == ' ' && !invertedCommas)
				{
					args.Add(str.ToString());

					str.Clear();

					continue;
				}

				str.Append(message[index]);
			}

			args.Add(str.ToString());

			options = ops.ToArray(); // TODO

			arguments = args.ToArray();
		}

		public static string CompressStrings(params string[] strings)
		{
			var str = new StringBuilder();

			foreach (string arg in strings)
			{
				str.Append($"{arg} ");
			}

			return str.ToString();
		}

		#endregion
	}
}
